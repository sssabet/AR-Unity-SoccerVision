using UnityEngine;
using UnityEngine.Rendering;
#if MODULE_URP_ENABLED
using System;
using UnityEngine.Rendering.Universal;
using UnityEngine.XR.ARSubsystems;
#else
using ScriptableRendererFeature = UnityEngine.ScriptableObject;
#endif

namespace UnityEngine.XR.ARFoundation
{
    /// <summary>
    /// A render feature for rendering the camera background for AR devices.
    /// </summary>
    public class ARBackgroundRendererFeature : ScriptableRendererFeature
    {
#if MODULE_URP_ENABLED
        /// <summary>
        /// The scriptable render pass to be added to the renderer when the camera background is to be rendered.
        /// </summary>
        ARCameraBeforeOpaquesRenderPass beforeOpaquesScriptablePass => m_BeforeOpaquesScriptablePass ??= new ARCameraBeforeOpaquesRenderPass();
        ARCameraBeforeOpaquesRenderPass m_BeforeOpaquesScriptablePass;

        /// <summary>
        /// The scriptable render pass to be added to the renderer when the camera background is to be rendered.
        /// </summary>
        ARCameraAfterOpaquesRenderPass afterOpaquesScriptablePass => m_AfterOpaquesScriptablePass ??= new ARCameraAfterOpaquesRenderPass();
        ARCameraAfterOpaquesRenderPass m_AfterOpaquesScriptablePass;

        /// <summary>
        /// Create the scriptable render pass.
        /// </summary>
        public override void Create() {}

        /// <summary>
        /// Add the background rendering pass when rendering a game camera with an enabled AR camera background component.
        /// </summary>
        /// <param name="renderer">The scriptable renderer in which to enqueue the render pass.</param>
        /// <param name="renderingData">Additional rendering data about the current state of rendering.</param>
        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            var currentCamera = renderingData.cameraData.camera;
            if ((currentCamera != null) && (currentCamera.cameraType == CameraType.Game))
            {
                ARCameraBackground cameraBackground = currentCamera.gameObject.GetComponent<ARCameraBackground>();
                if ((cameraBackground != null) && cameraBackground.backgroundRenderingEnabled
                    && (cameraBackground.material != null)
                    && TrySelectRenderPassForBackgroundRenderMode(cameraBackground.currentRenderingMode, out var renderPass))
                {
                    var invertCulling = cameraBackground.GetComponent<ARCameraManager>()?.subsystem?.invertCulling ?? false;
                    renderPass.Setup(cameraBackground, invertCulling);
                    renderer.EnqueuePass(renderPass);
                }
            }
        }

        /// <summary>
        /// Selects the render pass for a given <see cref="UnityEngine.XR.ARSubsystems.XRCameraBackgroundRenderingMode"/>
        /// </summary>
        /// <param name="renderingMode">The <see cref="UnityEngine.XR.ARSubsystems.XRCameraBackgroundRenderingMode"/>
        /// that indicates which render pass to use.
        /// </param>
        /// <param name="renderPass">The <see cref="ARCameraBackgroundRenderPass"/> that corresponds
        /// to the given <paramref name="renderingMode">.
        /// </param>
        /// <returns>
        /// <c>true</c> if <paramref name="renderPass"/> was populated. Otherwise, <c>false</c>.
        /// </returns>
        bool TrySelectRenderPassForBackgroundRenderMode(XRCameraBackgroundRenderingMode renderingMode, out ARCameraBackgroundRenderPass renderPass)
        {
            switch (renderingMode)
            {
                case XRCameraBackgroundRenderingMode.AfterOpaques:
                    renderPass = afterOpaquesScriptablePass;
                    return true;

                case XRCameraBackgroundRenderingMode.BeforeOpaques:
                    renderPass = beforeOpaquesScriptablePass;
                    return true;

                case XRCameraBackgroundRenderingMode.None:
                default:
                    renderPass = null;
                    return false;
            }
        }

        /// <summary>
        /// An abstract <see cref="ScriptableRenderPass"/> that provides common utilities for rendering an AR Camera Background.
        /// </summary>
        abstract class ARCameraBackgroundRenderPass : ScriptableRenderPass
        {
            /// <summary>
            /// The name for the custom render pass which will display in graphics debugging tools.
            /// </summary>
            const string k_CustomRenderPassName = "AR Background Pass (URP)";

            /// <summary>
            /// The mesh for rendering the background material.
            /// </summary>
            protected Mesh m_BackgroundMesh;

            /// <summary>
            /// The material used for rendering the device background using the camera video texture and potentially
            /// other device-specific properties and textures.
            /// </summary>
            Material m_BackgroundMaterial;

            /// <summary>
            /// Whether the culling mode should be inverted.
            /// ([CommandBuffer.SetInvertCulling](https://docs.unity3d.com/ScriptReference/Rendering.CommandBuffer.SetInvertCulling.html)).
            /// </summary>
            bool m_InvertCulling;

            /// <summary>
            /// The projection matrix used to render the <see cref="mesh"/>.
            /// </summary>
            protected abstract Matrix4x4 projectionMatrix { get; }

            /// <summary>
            /// The <see cref="Mesh"/> used in this custom render pass.
            /// </summary>
            protected abstract Mesh mesh { get; }

            /// <summary>
            /// Set up the background render pass.
            /// </summary>
            /// <param name="cameraBackground">The <see cref="ARCameraBackground"/> component that provides the <see cref="Material"/>
            /// and any additional rendering information required by the render pass.</param>
            /// <param name="invertCulling">Whether the culling mode should be inverted.</param>
            public void Setup(ARCameraBackground cameraBackground, bool invertCulling)
            {
                SetupInternal(cameraBackground);
                m_BackgroundMaterial = cameraBackground.material;
                m_InvertCulling = invertCulling;
            }

            /// <summary>
            /// Provides inheritors an opportunity to perform any specialized setup during <see cref="ScriptableRenderPass.Setup"/>.
            /// </summary>
            /// <param name="cameraBackground">The <see cref="ARCameraBackground"/> component that provides the <see cref="Material"/>
            /// and any additional rendering information required by the render pass.</param>
            protected virtual void SetupInternal(ARCameraBackground cameraBackground) {}

            /// <summary>
            /// Execute the commands to render the camera background.
            /// </summary>
            /// <param name="context">The render context for executing the render commands.</param>
            /// <param name="renderingData">Additional rendering data about the current state of rendering.</param>
            public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
            {
                var cmd = CommandBufferPool.Get(k_CustomRenderPassName);
                cmd.BeginSample(k_CustomRenderPassName);

                ARCameraBackground.AddBeforeBackgroundRenderHandler(cmd);

                cmd.SetInvertCulling(m_InvertCulling);

                cmd.SetViewProjectionMatrices(Matrix4x4.identity, projectionMatrix);

                cmd.DrawMesh(mesh, Matrix4x4.identity, m_BackgroundMaterial);

                cmd.SetViewProjectionMatrices(renderingData.cameraData.camera.worldToCameraMatrix,
                                              renderingData.cameraData.camera.projectionMatrix);

                cmd.EndSample(k_CustomRenderPassName);
                context.ExecuteCommandBuffer(cmd);

                CommandBufferPool.Release(cmd);
            }

            /// <summary>
            /// Clean up any resources for the render pass.
            /// </summary>
            /// <param name="commandBuffer">The command buffer for frame cleanup.</param>
            public override void FrameCleanup(CommandBuffer commandBuffer)
            {
            }
        }

        /// <summary>
        /// The custom render pass to render the camera background before rendering opaques.
        /// </summary>
        class ARCameraBeforeOpaquesRenderPass : ARCameraBackgroundRenderPass
        {
            /// <summary>
            /// Constructs the background render pass.
            /// </summary>
            public ARCameraBeforeOpaquesRenderPass()
            {
                renderPassEvent = RenderPassEvent.BeforeRenderingOpaques;
            }

            /// <summary>
            /// Configure the render pass by setting the render target and clear values.
            /// </summary>
            /// <param name="commandBuffer">The command buffer for configuration.</param>
            /// <param name="renderTextureDescriptor">The descriptor of the target render texture.</param>
            public override void Configure(CommandBuffer commandBuffer, RenderTextureDescriptor renderTextureDescriptor)
            {
                ConfigureClear(ClearFlag.Depth, Color.clear);
            }

            /// <inheritdoc />
            protected override Matrix4x4 projectionMatrix => ARCameraBackgroundRenderingUtils.beforeOpaquesOrthoProjection;

            /// <inheritdoc />
            protected override Mesh mesh => ARCameraBackgroundRenderingUtils.fullScreenNearClipMesh;
        }

        /// <summary>
        /// The custom render pass to render the camera background after rendering opaques.
        /// </summary>
        class ARCameraAfterOpaquesRenderPass : ARCameraBackgroundRenderPass
        {
            /// <summary>
            /// Constructs the background render pass.
            /// </summary>
            public ARCameraAfterOpaquesRenderPass()
            {
                renderPassEvent = RenderPassEvent.AfterRenderingOpaques;
            }

            /// <summary>
            /// Configure the render pass by setting the render target and clear values.
            /// </summary>
            /// <param name="commandBuffer">The command buffer for configuration.</param>
            /// <param name="renderTextureDescriptor">The descriptor of the target render texture.</param>
            public override void Configure(CommandBuffer commandBuffer, RenderTextureDescriptor renderTextureDescriptor)
            {
                ConfigureClear(ClearFlag.None, Color.clear);
            }

            /// <inheritdoc />
            protected override void SetupInternal(ARCameraBackground cameraBackground)
            {
                if (cameraBackground.GetComponent<AROcclusionManager>()?.enabled ?? false)
                {
                    // If an occlusion texture is being provided, rendering will need
                    // to compare it against the depth texture created by the camera.
                    ConfigureInput(ScriptableRenderPassInput.Depth);
                }
            }

            /// <inheritdoc />
            protected override Matrix4x4 projectionMatrix => ARCameraBackgroundRenderingUtils.afterOpaquesOrthoProjection;

            /// <inheritdoc />
            protected override Mesh mesh => ARCameraBackgroundRenderingUtils.fullScreenFarClipMesh;
        }
#endif // MODULE_URP_ENABLED
    }
}
