using AOT;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine.Rendering;
using UnityEngine.Serialization;
using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.ARFoundation
{
    /// <summary>
    /// <para>Add this component to a <c>Camera</c> to copy the color camera's texture onto the background.</para>
    /// <para>If you are using the Universal Render Pipeline (version 7.0.0 or later), you must also add the
    /// <see cref="ARBackgroundRendererFeature"/> to the list of render features for the scriptable renderer.</para>
    /// </summary>
    /// <remarks>
    /// <para>
    /// To add the <see cref="ARBackgroundRendererFeature"/> to the list of render features for the scriptable
    /// renderer:
    /// <list type="bullet">
    /// <item><description>In Project Settings &gt; Graphics, select the render pipeline asset
    /// (<c>UniversalRenderPipelineAsset</c>) that is in the Scriptable Render Pipeline Settings
    /// field.</description></item>
    /// <item><description>In the render pipeline asset's Inspector window, make sure that the Render Type is set
    /// to Custom.</description></item>
    /// <item><description>In render pipeline asset's Inspector window, select the Render Type &gt; Data
    /// asset which would be of type <c>ForwardRendererData</c>.</description></item>
    /// <item><description>In forward renderer data's Inspector window, ensure the Render Features list
    /// contains a <see cref="ARBackgroundRendererFeature"/>.</description></item>
    /// </list>
    /// </para>
    /// <para>To customize background rendering with the legacy render pipeline, you can override the
    /// <see cref="legacyCameraEvents"/> property and the
    /// <see cref="ConfigureLegacyCommandBuffer(CommandBuffer)"/> method to modify the given
    /// <c>CommandBuffer</c> with rendering commands and to inject the given <c>CommandBuffer</c> into the Camera's
    /// rendering.</para>
    /// <para>To customize background rendering with a scriptable render pipeline, create a
    /// <c>ScriptableRendererFeature</c> with the background rendering commands, and insert the
    /// <c>ScriptableRendererFeature</c> into the list of render features for the scriptable renderer.</para>
    /// </remarks>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Camera))]
    [RequireComponent(typeof(ARCameraManager))]
    [HelpURL(typeof(ARCameraBackground))]
    public class ARCameraBackground : MonoBehaviour
    {
        /// <summary>
        /// Name for the custom rendering command buffer.
        /// </summary>
        const string k_CustomRenderPassName = "AR Background Pass (LegacyRP)";

        /// <summary>
        /// Name of the main texture parameter for the material.
        /// </summary>
        const string k_MainTexName = "_MainTex";

        /// <summary>
        /// Name of the shader parameter for the display transform matrix.
        /// </summary>
        const string k_DisplayTransformName = "_UnityDisplayTransform";

        /// <summary>
        /// Property ID for the shader parameter for the main texture.
        /// </summary>
        static readonly int k_MainTexId = Shader.PropertyToID(k_MainTexName);

        /// <summary>
        /// Property ID for the shader parameter for the display transform matrix.
        /// </summary>
        static readonly int k_DisplayTransformId = Shader.PropertyToID(k_DisplayTransformName);

        /// <summary>
        /// The Property ID for the shader parameter for the forward vector's scaled length.
        /// </summary>
        static readonly int k_CameraForwardScaleId = Shader.PropertyToID("_UnityCameraForwardScale");

        /// <summary>
        /// The camera to which the projection matrix is set on each frame event.
        /// </summary>
        Camera m_Camera;

        /// <summary>
        /// The camera manager from which frame information is pulled.
        /// </summary>
        ARCameraManager m_CameraManager;

        /// <summary>
        /// The occlusion manager, which might not exist, from which occlusion information is pulled.
        /// </summary>
        AROcclusionManager m_OcclusionManager;

        /// <summary>
        /// Command buffer for any custom rendering commands.
        /// </summary>
        CommandBuffer m_CommandBuffer;

        /// <summary>
        /// Whether to use the custom material for rendering the background.
        /// </summary>
        [SerializeField, FormerlySerializedAs("m_OverrideMaterial")]
        bool m_UseCustomMaterial;

        /// <summary>
        /// A custom material for rendering the background.
        /// </summary>
        [SerializeField, FormerlySerializedAs("m_Material")]
        Material m_CustomMaterial;

        /// <summary>
        /// The default material for rendering the background.
        /// </summary>
        Material m_DefaultMaterial;

        /// <summary>
        /// The previous clear flags for the camera, if any.
        /// </summary>
        CameraClearFlags? m_PreviousCameraClearFlags;

        /// <summary>
        /// The original field of view of the camera, before enabling background rendering.
        /// </summary>
        float? m_PreviousCameraFieldOfView;

        /// <summary>
        /// The original depth mode for the camera.
        /// </summary>
        DepthTextureMode m_PreviousCameraDepthMode;

        /// <summary>
        /// True if background rendering is enabled, false otherwise.
        /// </summary>
        bool m_BackgroundRenderingEnabled;

        /// <summary>
        /// The camera to which the projection matrix is set on each frame event.
        /// </summary>
        /// <value>
        /// The camera to which the projection matrix is set on each frame event.
        /// </value>
#if UNITY_EDITOR
        protected new Camera camera => m_Camera;
#else // UNITY_EDITOR
        protected Camera camera => m_Camera;
#endif // UNITY_EDITOR

        /// <summary>
        /// The camera manager from which frame information is pulled.
        /// </summary>
        /// <value>
        /// The camera manager from which frame information is pulled.
        /// </value>
        protected ARCameraManager cameraManager => m_CameraManager;

        /// <summary>
        /// The occlusion manager, which might not exist, from which occlusion information is pulled.
        /// </summary>
        protected AROcclusionManager occlusionManager => m_OcclusionManager;

        /// <summary>
        /// The current <c>Material</c> used for background rendering.
        /// </summary>
        public Material material => (useCustomMaterial && (customMaterial != null)) ? customMaterial : defaultMaterial;

        /// <summary>
        /// Whether to use the custom material for rendering the background.
        /// </summary>
        /// <value>
        /// <c>true</c> if the custom material should be used for rendering the camera background. Otherwise,
        /// <c>false</c>.
        /// </value>
        public bool useCustomMaterial
        {
            get => m_UseCustomMaterial;
            set => m_UseCustomMaterial = value;
        }

        /// <summary>
        /// A custom material for rendering the background.
        /// </summary>
        /// <value>
        /// A custom material for rendering the background.
        /// </value>
        public Material customMaterial
        {
            get => m_CustomMaterial;
            set => m_CustomMaterial = value;
        }

        /// <summary>
        /// Whether background rendering is enabled.
        /// </summary>
        /// <value>
        /// <c>true</c> if background rendering is enabled and if at least one camera frame has been received.
        /// Otherwise, <c>false</c>.
        /// </value>
        public bool backgroundRenderingEnabled => m_BackgroundRenderingEnabled;

        /// <summary>
        /// The default material for rendering the background.
        /// </summary>
        /// <value>
        /// The default material for rendering the background.
        /// </value>
        Material defaultMaterial => cameraManager.cameraMaterial;

        /// <summary>
        /// Stores the previous culling state (XRCameraSubsystem.invertCulling).
        /// If the requested culling state changes, the command buffer must be rebuilt.
        /// </summary>
        bool m_CommandBufferCullingState;

        XRCameraBackgroundRenderingMode m_CommandBufferRenderOrderState = XRCameraBackgroundRenderingMode.None;

        /// <summary>
        /// A function that can be invoked by
        /// [CommandBuffer.IssuePluginEvent](https://docs.unity3d.com/ScriptReference/Rendering.CommandBuffer.IssuePluginEvent.html).
        /// This function calls the XRCameraSubsystem method that should be called immediately before background rendering.
        /// </summary>
        /// <param name="eventId">The id of the event</param>
        [MonoPInvokeCallback(typeof(Action<int>))]
        static void BeforeBackgroundRenderHandler(int eventId)
        {
            s_CameraSubsystem?.OnBeforeBackgroundRender(eventId);
        }

        /// <summary>
        /// A delegate representation of <see cref="BeforeBackgroundRenderHandler(int)"/>. This maintains a strong
        /// reference to the delegate, which is converted to an IntPtr by <see cref="s_BeforeBackgroundRenderHandlerFuncPtr"/>.
        /// </summary>
        /// <seealso cref="AddBeforeBackgroundRenderHandler(CommandBuffer)"/>
        static Action<int> s_BeforeBackgroundRenderHandler = BeforeBackgroundRenderHandler;

        /// <summary>
        /// A delegate for capturing when the <see cref="currentRenderingMode"/> has changed. Use to change make any changes
        /// to the parameters of the <see cref="ARCameraBackground"/> (IE. changing custom materials out) before configuring
        /// the command buffer for background rendering.
        /// </summary>
        public static Action<XRCameraBackgroundRenderingMode> OnCameraRenderingModeChanged;

        /// <summary>
        /// A pointer to a method to be called immediately before rendering that is implemented in the XRCameraSubsystem implementation.
        /// It is called via [CommandBuffer.IssuePluginEvent](https://docs.unity3d.com/ScriptReference/Rendering.CommandBuffer.IssuePluginEvent.html).
        /// </summary>
        static readonly IntPtr s_BeforeBackgroundRenderHandlerFuncPtr = Marshal.GetFunctionPointerForDelegate(s_BeforeBackgroundRenderHandler);

        /// <summary>
        /// Static reference to the active XRCameraSubsystem. Necessary here for access from a static delegate.
        /// </summary>
        static XRCameraSubsystem s_CameraSubsystem;

        /// <summary>
        /// Whether culling should be inverted. Used during command buffer configuration,
        /// see [CommandBuffer.SetInvertCulling](https://docs.unity3d.com/ScriptReference/Rendering.CommandBuffer.SetInvertCulling.html).
        /// </summary>
        /// <seealso cref="ConfigureLegacyCommandBuffer(CommandBuffer)"/>
        protected bool shouldInvertCulling
        {
            get
            {
                // Use Unity == operator overload for comparing UnityEngine.Object with null
                if (m_CameraManager == null || m_CameraManager.subsystem == null)
                    return false;

                return m_CameraManager.subsystem.invertCulling;
            }
        }

        /// <summary>
        /// The current <see cref="XRCameraBackgroundRenderingMode"/>. Determines which render order to use.
        /// </summary>
        public XRCameraBackgroundRenderingMode currentRenderingMode
        {
            get
            {
                // Use Unity == operator overload for comparing UnityEngine.Object with null
                return m_CameraManager == null ? XRCameraBackgroundRenderingMode.None : m_CameraManager.currentRenderingMode;
            }
        }

        void Awake()
        {
            m_Camera = GetComponent<Camera>();
            m_CameraManager = GetComponent<ARCameraManager>();
            m_OcclusionManager = GetComponent<AROcclusionManager>();
        }

        void OnEnable()
        {
            // Ensure that background rendering is disabled until the first camera frame is received.
            m_BackgroundRenderingEnabled = false;
            cameraManager.frameReceived += OnCameraFrameReceived;
            if (occlusionManager != null)
            {
                occlusionManager.frameReceived += OnOcclusionFrameReceived;
            }

            m_PreviousCameraDepthMode = camera.depthTextureMode;
        }

        void OnDisable()
        {
            if (occlusionManager != null)
            {
                occlusionManager.frameReceived -= OnOcclusionFrameReceived;
            }

            cameraManager.frameReceived -= OnCameraFrameReceived;
            DisableBackgroundRendering();

            // We are no longer setting the projection matrix so tell the camera to resume its normal projection matrix
            // calculations.
            camera.ResetProjectionMatrix();
        }

        /// <summary>
        /// Enable background rendering by disabling the camera's clear flags, and enabling the legacy RP background
        /// rendering if your application is in legacy RP mode.
        /// </summary>
        void EnableBackgroundRendering()
        {
            m_BackgroundRenderingEnabled = true;

            // We must hold a static reference to the camera subsystem so that it is accessible to the
            // static callback needed for calling OnBeforeBackgroundRender() from the render thread
            s_CameraSubsystem = m_CameraManager ? m_CameraManager.subsystem : null;

            DisableBackgroundClearFlags();

            m_PreviousCameraFieldOfView = m_Camera.fieldOfView;

            if (ARRenderingUtils.useLegacyRenderPipeline && defaultMaterial != null)
                EnableLegacyRenderPipelineBackgroundRendering();
        }

        /// <summary>
        /// Disable background rendering by disabling the legacy RP background rendering if your application is  in legacy RP mode
        /// and restoring the camera's clear flags.
        /// </summary>
        void DisableBackgroundRendering()
        {
            m_BackgroundRenderingEnabled = false;

            DisableLegacyRenderPipelineBackgroundRendering();

            RestoreBackgroundClearFlags();

            if (m_PreviousCameraFieldOfView.HasValue)
            {
                m_Camera.fieldOfView = m_PreviousCameraFieldOfView.Value;
                m_PreviousCameraFieldOfView = null;
            }

            s_CameraSubsystem = null;
        }

        /// <summary>
        /// Set the camera's clear flags to do nothing while preserving the previous camera clear flags.
        /// </summary>
        void DisableBackgroundClearFlags()
        {
            m_PreviousCameraClearFlags = m_Camera.clearFlags;
            m_Camera.clearFlags = currentRenderingMode == XRCameraBackgroundRenderingMode.AfterOpaques ? CameraClearFlags.Depth : CameraClearFlags.Nothing;
        }

        /// <summary>
        /// Restore the previous camera's clear flags, if any.
        /// </summary>
        void RestoreBackgroundClearFlags()
        {
            if (m_PreviousCameraClearFlags != null)
            {
                m_Camera.clearFlags = m_PreviousCameraClearFlags.Value;
            }
        }

        /// <summary>
        /// The list of [CameraEvent](https://docs.unity3d.com/ScriptReference/Rendering.CameraEvent.html)s
        /// to add to the [CommandBuffer](https://docs.unity3d.com/ScriptReference/Rendering.CommandBuffer.html)
        /// when rendering before opaques.
        /// </summary>
        static readonly CameraEvent[] s_DefaultBeforeOpaqueCameraEvents = {
            CameraEvent.BeforeForwardOpaque,
            CameraEvent.BeforeGBuffer
        };

        /// <summary>
        /// The list of [CameraEvent](https://docs.unity3d.com/ScriptReference/Rendering.CameraEvent.html)s
        /// to add to the [CommandBuffer](https://docs.unity3d.com/ScriptReference/Rendering.CommandBuffer.html)
        /// when rendering after opaques.
        /// </summary>
        static readonly CameraEvent[] s_DefaultAfterOpaqueCameraEvents = {
            CameraEvent.AfterForwardOpaque,
            CameraEvent.AfterGBuffer
        };

        /// <summary>
        /// The list of [CameraEvent](https://docs.unity3d.com/ScriptReference/Rendering.CameraEvent.html)s
        /// to add to the [CommandBuffer](https://docs.unity3d.com/ScriptReference/Rendering.CommandBuffer.html).
        /// By default, it will select either <see cref="s_DefaultBeforeOpaqueCameraEvents"/> or <see cref="s_DefaultAfterOpaqueCameraEvents"/>
        /// depending on the value of <see cref="currentRenderingMode"/>.
        ///
        /// In the case where Before Opaques rendering has been selected it will return:
        ///
        /// [BeforeForwardOpaque](https://docs.unity3d.com/ScriptReference/Rendering.CameraEvent.BeforeForwardOpaque.html)
        /// and
        /// [BeforeGBuffer](https://docs.unity3d.com/ScriptReference/Rendering.CameraEvent.BeforeGBuffer.html)}.
        ///
        /// In the case where After Opaques rendering has been selected it will return:
        ///
        /// [AfterForwardOpaque](https://docs.unity3d.com/ScriptReference/Rendering.CameraEvent.AfterForwardOpaque.html)
        /// and
        /// [AfterGBuffer](https://docs.unity3d.com/ScriptReference/Rendering.CameraEvent.AfterGBuffer.html)}.
        ///
        /// Override to use different camera events.
        /// </summary>
        protected virtual IEnumerable<CameraEvent> legacyCameraEvents
        {
            get
            {
                return m_CommandBufferRenderOrderState switch
                {
                    XRCameraBackgroundRenderingMode.BeforeOpaques => s_DefaultBeforeOpaqueCameraEvents,
                    XRCameraBackgroundRenderingMode.AfterOpaques => s_DefaultAfterOpaqueCameraEvents,
                    _ => default(IEnumerable<CameraEvent>)
                };
            }
        }

        /// <summary>
        /// Configures the <paramref name="commandBuffer"/> by first clearing it, then adding necessary render commands.
        /// </summary>
        /// <param name="commandBuffer">The command buffer to configure.</param>
        protected virtual void ConfigureLegacyCommandBuffer(CommandBuffer commandBuffer)
        {
            Texture texture = !material.HasProperty(k_MainTexName) ? null : material.GetTexture(k_MainTexName);

            commandBuffer.Clear();
            AddBeforeBackgroundRenderHandler(commandBuffer);
            m_CommandBufferCullingState = shouldInvertCulling;

            commandBuffer.SetInvertCulling(m_CommandBufferCullingState);
            m_CommandBufferRenderOrderState = currentRenderingMode;

            switch (m_CommandBufferRenderOrderState)
            {
                case XRCameraBackgroundRenderingMode.AfterOpaques:
                    commandBuffer.SetViewProjectionMatrices(Matrix4x4.identity, ARCameraBackgroundRenderingUtils.afterOpaquesOrthoProjection);
                    commandBuffer.DrawMesh(ARCameraBackgroundRenderingUtils.fullScreenFarClipMesh, Matrix4x4.identity, material);
                    commandBuffer.SetViewProjectionMatrices(camera.worldToCameraMatrix, camera.projectionMatrix);
                    break;

                case XRCameraBackgroundRenderingMode.BeforeOpaques:
                    commandBuffer.ClearRenderTarget(true, false, Color.clear);
                    commandBuffer.Blit(texture, BuiltinRenderTextureType.CameraTarget, material);
                    break;

                case XRCameraBackgroundRenderingMode.None:
                default:
                    throw new ArgumentOutOfRangeException(nameof(m_CommandBufferRenderOrderState));
            }
        }

        /// <summary>
        /// Enable background rendering getting a command buffer, and configure it for rendering the background.
        /// </summary>
        void EnableLegacyRenderPipelineBackgroundRendering()
        {
            if (m_CommandBuffer == null)
            {
                m_CommandBuffer = new CommandBuffer();
                m_CommandBuffer.name = k_CustomRenderPassName;

                ConfigureLegacyCommandBuffer(m_CommandBuffer);
                AddCommandBufferToCameraEvent();
            }
        }

        /// <summary>
        /// Disable background rendering by removing the command buffer from the camera.
        /// </summary>
        void DisableLegacyRenderPipelineBackgroundRendering()
        {
            if (m_CommandBuffer != null)
            {
                RemoveCommandBufferFromCameraEvents();
                m_CommandBuffer = null;
            }
        }

        /// <summary>
        /// Adds the AR Camera Background <see cref="CommandBuffer"/> to the <see cref="legacyCameraEvents"/>.
        /// </summary>
        /// <exception cref="NullReferenceException">
        /// If the AR Camera Background <see cref="CommandBuffer"/> is <see langword="null"/>.
        /// </exception>
        void AddCommandBufferToCameraEvent()
        {
            if (m_CommandBuffer == null)
                throw new NullReferenceException();

            foreach (var cameraEvent in legacyCameraEvents)
            {
                camera.AddCommandBuffer(cameraEvent, m_CommandBuffer);
            }
        }

        /// <summary>
        /// Removes the AR Camera Background <see cref="CommandBuffer"/> from the camera rendering events.
        /// </summary>
        void RemoveCommandBufferFromCameraEvents()
        {
            foreach (var cameraEvent in legacyCameraEvents)
            {
                camera.RemoveCommandBuffer(cameraEvent, m_CommandBuffer);
            }
        }

        /// <summary>
        /// This adds a command to the <paramref name="commandBuffer"/> to make call from the render thread
        /// to a callback on the `XRCameraSubsystem` implementation. The callback handles any implementation-specific
        /// functionality needed immediately before the background is rendered.
        /// </summary>
        /// <param name="commandBuffer">The [CommandBuffer](https://docs.unity3d.com/ScriptReference/Rendering.CommandBuffer.html)
        /// to add the command to.</param>
        internal static void AddBeforeBackgroundRenderHandler(CommandBuffer commandBuffer)
        {
            commandBuffer.IssuePluginEvent(s_BeforeBackgroundRenderHandlerFuncPtr, 0);
        }

        /// <summary>
        /// Callback for the camera frame event.
        /// </summary>
        /// <param name="eventArgs">The camera event arguments.</param>
        void OnCameraFrameReceived(ARCameraFrameEventArgs eventArgs)
        {
            var activeRenderingMode = currentRenderingMode;

            // Enable background rendering when first frame is received.
            if (m_BackgroundRenderingEnabled)
            {
                if (eventArgs.textures.Count == 0 || activeRenderingMode == XRCameraBackgroundRenderingMode.None)
                {
                    DisableBackgroundRendering();
                    return;
                }

                if (m_CommandBuffer != null)
                {
                    if (m_CommandBufferRenderOrderState != activeRenderingMode)
                    {
                        RemoveCommandBufferFromCameraEvents();
                        RestoreBackgroundClearFlags();

                        OnCameraRenderingModeChanged?.Invoke(activeRenderingMode);
                        SetCameraDepthTextureMode(activeRenderingMode);
                        ConfigureLegacyCommandBuffer(m_CommandBuffer);

                        DisableBackgroundClearFlags();
                        AddCommandBufferToCameraEvent();

                    }
                    else if (m_CommandBufferCullingState != shouldInvertCulling)
                    {
                        ConfigureLegacyCommandBuffer(m_CommandBuffer);
                    }
                }
            }
            else if (eventArgs.textures.Count > 0 && activeRenderingMode != XRCameraBackgroundRenderingMode.None)
            {
                EnableBackgroundRendering();
            }

            Material material = this.material;
            if (material != null)
            {
                var count = eventArgs.textures.Count;
                for (int i = 0; i < count; ++i)
                {
                    material.SetTexture(eventArgs.propertyNameIds[i], eventArgs.textures[i]);
                }

                if (eventArgs.displayMatrix.HasValue)
                {
                    material.SetMatrix(k_DisplayTransformId, eventArgs.displayMatrix.Value);
                }

                SetMaterialKeywords(material, eventArgs.enabledMaterialKeywords, eventArgs.disabledMaterialKeywords);
            }

            if (eventArgs.projectionMatrix.HasValue)
            {
                camera.projectionMatrix = eventArgs.projectionMatrix.Value;
                const float twiceRad2Deg = 2 * Mathf.Rad2Deg;
                var halfHeightOverNear = 1 / camera.projectionMatrix[1, 1];
                camera.fieldOfView = Mathf.Atan(halfHeightOverNear) * twiceRad2Deg;
            }
        }

        /// <summary>
        /// Ensure the camera generates a depth texture after opaques for use when comparing environment depth when rendering
        /// after opaques.
        /// </summary>
        void SetCameraDepthTextureMode(XRCameraBackgroundRenderingMode mode)
        {
            switch (mode)
            {
                case XRCameraBackgroundRenderingMode.AfterOpaques:
                {
                    if (occlusionManager.enabled)
                    {
                        m_PreviousCameraDepthMode = camera.depthTextureMode;
                        camera.depthTextureMode = DepthTextureMode.Depth;
                    }
                    break;
                }

                case XRCameraBackgroundRenderingMode.None:
                case XRCameraBackgroundRenderingMode.BeforeOpaques:
                default:
                    camera.depthTextureMode = m_PreviousCameraDepthMode;
                    break;
            }
        }

        /// <summary>
        /// Callback for the occlusion frame event.
        /// </summary>
        /// <param name="eventArgs">The occlusion frame event arguments.</param>
        void OnOcclusionFrameReceived(AROcclusionFrameEventArgs eventArgs)
        {
            Material material = this.material;
            if (material != null)
            {
                var count = eventArgs.textures.Count;
                for (int i = 0; i < count; ++i)
                {
                    material.SetTexture(eventArgs.propertyNameIds[i], eventArgs.textures[i]);
                }

                SetMaterialKeywords(material, eventArgs.enabledMaterialKeywords, eventArgs.disabledMaterialKeywords);

                // Set scale: this computes the affect the camera's localToWorld has on the the length of the
                // forward vector, i.e., how much farther from the camera are things than with unit scale.
                var forward = transform.localToWorldMatrix.GetColumn(2);
                var scale = forward.magnitude;
                material.SetFloat(k_CameraForwardScaleId, scale);
            }
        }

        static void SetMaterialKeywords(Material material, List<string> enabledMaterialKeywords,
            List<string> disabledMaterialKeywords)
        {
            if (enabledMaterialKeywords != null)
            {
                foreach (var materialKeyword in enabledMaterialKeywords)
                {
                    if (!material.IsKeywordEnabled(materialKeyword))
                    {
                        material.EnableKeyword(materialKeyword);
                    }
                }
            }

            if (disabledMaterialKeywords != null)
            {
                foreach (var materialKeyword in disabledMaterialKeywords)
                {
                    if (material.IsKeywordEnabled(materialKeyword))
                    {
                        material.DisableKeyword(materialKeyword);
                    }
                }
            }
        }
    }
}
