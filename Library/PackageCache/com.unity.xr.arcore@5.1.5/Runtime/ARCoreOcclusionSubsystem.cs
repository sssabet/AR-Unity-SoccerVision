using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.Collections;
using UnityEngine.Scripting;
using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.ARCore
{
    /// <summary>
    /// The ARCore implementation of the
    /// [XROcclusionSubsystem](xref:UnityEngine.XR.ARSubsystems.XROcclusionSubsystem).
    /// Do not create this directly. Use the
    /// [SubsystemManager](xref:UnityEngine.SubsystemManager)
    /// instead.
    /// </summary>
    [Preserve]
    class ARCoreOcclusionSubsystem : XROcclusionSubsystem
    {
        /// <summary>
        /// Registers the ARCore occlusion subsystem if iOS and not the editor.
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void Register()
        {
            if (!Api.platformAndroid || !Api.loaderPresent)
                return;

            const string k_SubsystemId = "ARCore-Occlusion";

            var occlusionSubsystemCinfo = new XROcclusionSubsystemCinfo()
            {
                id = k_SubsystemId,
                providerType = typeof(ARCoreOcclusionSubsystem.ARCoreProvider),
                subsystemTypeOverride = typeof(ARCoreOcclusionSubsystem),
                environmentDepthImageSupportedDelegate = NativeApi.UnityARCore_OcclusionProvider_DoesSupportEnvironmentDepth,
                // Confidence and smoothing is implicitly supported if environment depth is supported.
                environmentDepthConfidenceImageSupportedDelegate = NativeApi.UnityARCore_OcclusionProvider_DoesSupportEnvironmentDepth,
                environmentDepthTemporalSmoothingSupportedDelegate = NativeApi.UnityARCore_OcclusionProvider_DoesSupportEnvironmentDepth,
            };

            if (!XROcclusionSubsystem.Register(occlusionSubsystemCinfo))
            {
                Debug.Log($"Cannot register the {k_SubsystemId} subsystem");
            }
        }

        /// <summary>
        /// The implementation provider class.
        /// </summary>
        class ARCoreProvider : Provider
        {
            /// <summary>
            /// The shader property name for the environment depth texture.
            /// </summary>
            /// <value>
            /// The shader property name for the environment depth texture.
            /// </value>
            const string k_TextureEnvironmentDepthPropertyName = "_EnvironmentDepth";

            /// <summary>
            /// The shader keyword for enabling environment depth rendering.
            /// </summary>
            /// <value>
            /// The shader keyword for enabling environment depth rendering.
            /// </value>
            const string k_EnvironmentDepthEnabledMaterialKeyword = "ARCORE_ENVIRONMENT_DEPTH_ENABLED";

            /// <summary>
            /// The shader property name identifier for the environment depth texture.
            /// </summary>
            /// <value>
            /// The shader property name identifier for the environment depth texture.
            /// </value>
            static readonly int k_TextureEnvironmentDepthPropertyId = Shader.PropertyToID(k_TextureEnvironmentDepthPropertyName);

            /// <summary>
            /// The shader keywords for enabling environment depth rendering.
            /// </summary>
            /// <value>
            /// The shader keywords for enabling environment depth rendering.
            /// </value>
            static readonly List<string> m_EnvironmentDepthEnabledMaterialKeywords = new List<string>() {k_EnvironmentDepthEnabledMaterialKeyword};

            /// <summary>
            /// The occlusion preference mode for when rendering the background.
            /// </summary>
            OcclusionPreferenceMode m_OcclusionPreferenceMode;

            /// <summary>
            /// Constructs the implementation provider.
            /// </summary>
            public ARCoreProvider()
            {
                bool supportsR16 = SystemInfo.SupportsTextureFormat(TextureFormat.R16);
                bool supportsRHalf = SystemInfo.SupportsTextureFormat(TextureFormat.RHalf);
                bool supportsRenderTextureRHalf = SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.RHalf);
                bool useAdvancedRendering = supportsR16 && supportsRHalf && supportsRenderTextureRHalf;
                NativeApi.UnityARCore_OcclusionProvider_Construct(k_TextureEnvironmentDepthPropertyId, useAdvancedRendering);
            }

            /// <summary>
            /// Starts the provider.
            /// </summary>
            public override void Start() => NativeApi.UnityARCore_OcclusionProvider_Start();

            /// <summary>
            /// Stops the provider.
            /// </summary>
            public override void Stop() => NativeApi.UnityARCore_OcclusionProvider_Stop();

            /// <summary>
            /// Destroys the provider.
            /// </summary>
            public override void Destroy() => NativeApi.UnityARCore_OcclusionProvider_Destruct();

            /// <summary>
            /// The requested environment depth mode.
            /// </summary>
            /// <value>
            /// The requested environment depth mode.
            /// </value>
            public override EnvironmentDepthMode requestedEnvironmentDepthMode
            {
                get => NativeApi.UnityARCore_OcclusionProvider_GetRequestedEnvironmentDepthMode();
                set
                {
                    NativeApi.UnityARCore_OcclusionProvider_SetRequestedEnvironmentDepthMode(value);
                    Api.SetFeatureRequested(Feature.EnvironmentDepth, value.Enabled());
                }
            }

            /// <summary>
            /// The current environment depth mode.
            /// </summary>
            /// <value>Describes the resolution category of the depth image and whether depth is enabled.</value>
            public override EnvironmentDepthMode currentEnvironmentDepthMode
                => NativeApi.UnityARCore_OcclusionProvider_GetCurrentEnvironmentDepthMode();

            public override bool environmentDepthTemporalSmoothingEnabled =>
                NativeApi.UnityARCore_OcclusionProvider_GetEnvironmentDepthTemporalSmoothingEnabled();

            public override bool environmentDepthTemporalSmoothingRequested
            {
                get => Api.GetRequestedFeatures().Any(Feature.EnvironmentDepthTemporalSmoothing);
                set => Api.SetFeatureRequested(Feature.EnvironmentDepthTemporalSmoothing, value);
            }

            /// <summary>
            /// Specifies the requested occlusion preference mode.
            /// </summary>
            /// <value>
            /// The requested occlusion preference mode.
            /// </value>
            public override OcclusionPreferenceMode requestedOcclusionPreferenceMode
            {
                get => m_OcclusionPreferenceMode;
                set => m_OcclusionPreferenceMode = value;
            }

            /// <summary>
            /// Get the occlusion preference mode currently in use by the provider.
            /// </summary>
            public override OcclusionPreferenceMode currentOcclusionPreferenceMode => m_OcclusionPreferenceMode;

            /// <summary>
            /// Gets the environment texture descriptor.
            /// </summary>
            /// <param name="environmentDepthDescriptor">The environment depth texture descriptor to be populated, if
            /// available.</param>
            /// <returns>
            /// <c>true</c> if the environment depth texture descriptor is available and is returned. Otherwise,
            /// <c>false</c>.
            /// </returns>
            public override bool TryGetEnvironmentDepth(out XRTextureDescriptor environmentDepthDescriptor)
                => NativeApi.UnityARCore_OcclusionProvider_TryGetEnvironmentDepth(out environmentDepthDescriptor);

            /// <summary>
            /// Gets the CPU construction information for a environment depth image.
            /// </summary>
            /// <param name="cinfo">The CPU image construction information, on success.</param>
            /// <returns>
            /// <c>true</c> if the environment depth texture is available and its CPU image construction information is
            /// returned. Otherwise, <c>false</c>.
            /// </returns>
            /// <remarks>
            /// If  <see cref='environmentDepthTemporalSmoothingEnabled'/> is <c>true</c> then the CPU image construction information
            /// will be for the temporally smoothed environmental depth image otherwise it will be for the raw environmental depth image.
            /// </remarks>
            public override bool TryAcquireEnvironmentDepthCpuImage(out XRCpuImage.Cinfo cinfo)
            {
                return environmentDepthTemporalSmoothingEnabled
                    ? ARCoreCpuImageApi.TryAcquireLatestImage(ARCoreCpuImageApi.ImageType.EnvironmentDepth, out cinfo)
                    : ARCoreCpuImageApi.TryAcquireLatestImage(ARCoreCpuImageApi.ImageType.RawEnvironmentDepth, out cinfo);
            }

            public override bool TryAcquireRawEnvironmentDepthCpuImage(out XRCpuImage.Cinfo cinfo) =>
                ARCoreCpuImageApi.TryAcquireLatestImage(ARCoreCpuImageApi.ImageType.RawEnvironmentDepth, out cinfo);

            public override bool TryAcquireSmoothedEnvironmentDepthCpuImage(out XRCpuImage.Cinfo cinfo) =>
                ARCoreCpuImageApi.TryAcquireLatestImage(ARCoreCpuImageApi.ImageType.EnvironmentDepth, out cinfo);

            /// <summary>
            /// The CPU image API for interacting with the environment depth image.
            /// </summary>
            public override XRCpuImage.Api environmentDepthCpuImageApi => ARCoreCpuImageApi.instance;

            /// <summary>
            /// Get the environment depth confidence texture descriptor.
            /// </summary>
            /// <param name="environmentDepthConfidenceDescriptor">The environment depth texture descriptor to be
            /// populated, if available.</param>
            /// <returns>
            /// <c>true</c> if the environment depth confidence texture descriptor is available and is returned.
            /// Otherwise, <c>false</c>.
            /// </returns>
            public override bool TryGetEnvironmentDepthConfidence(out XRTextureDescriptor environmentDepthConfidenceDescriptor)
                => NativeApi.UnityARCore_OcclusionProvider_TryGetEnvironmentDepthConfidence(out environmentDepthConfidenceDescriptor);

            /// <summary>
            /// Gets the CPU construction information for a environment depth confidence image.
            /// </summary>
            /// <param name="cinfo">The CPU image construction information, on success.</param>
            /// <returns>
            /// <c>true</c> if the environment depth texture confidence is available and its CPU image construction information is
            /// returned. Otherwise, <c>false</c>.
            /// </returns>
            public override bool TryAcquireEnvironmentDepthConfidenceCpuImage(out XRCpuImage.Cinfo cinfo)
                => ARCoreCpuImageApi.TryAcquireLatestImage(ARCoreCpuImageApi.ImageType.RawEnvironmentDepthConfidence,
                    out cinfo);

            /// <summary>
            /// The CPU image API for interacting with the environment depth confidence image.
            /// </summary>
            public override XRCpuImage.Api environmentDepthConfidenceCpuImageApi => ARCoreCpuImageApi.instance;

            /// <summary>
            /// Gets the occlusion texture descriptors associated with the current AR frame.
            /// </summary>
            /// <param name="defaultDescriptor">The default descriptor value.</param>
            /// <param name="allocator">The allocator to use when creating the returned <c>NativeArray</c>.</param>
            /// <returns>The occlusion texture descriptors.</returns>
            public override unsafe NativeArray<XRTextureDescriptor> GetTextureDescriptors(XRTextureDescriptor defaultDescriptor,
                                                                                          Allocator allocator)
            {
                var textureDescriptors = NativeApi.UnityARCore_OcclusionProvider_AcquireTextureDescriptors(out int length,
                                                                                                           out int elementSize);

                try
                {
                    return NativeCopyUtility.PtrToNativeArrayWithDefault(defaultDescriptor, textureDescriptors,
                                                                         elementSize, length, allocator);
                }
                finally
                {
                    NativeApi.UnityARCore_OcclusionProvider_ReleaseTextureDescriptors(textureDescriptors);
                }
            }

            /// <summary>
            /// Gets the enabled and disabled shader keywords for the material.
            /// </summary>
            /// <param name="enabledKeywords">The keywords to enable for the material.</param>
            /// <param name="disabledKeywords">The keywords to disable for the material.</param>
            public override void GetMaterialKeywords(out List<string> enabledKeywords, out List<string> disabledKeywords)
            {
                bool isEnvDepthEnabled = NativeApi.UnityARCore_OcclusionProvider_IsEnvironmentDepthEnabled();

                if ((m_OcclusionPreferenceMode == OcclusionPreferenceMode.NoOcclusion) || !isEnvDepthEnabled)
                {
                    enabledKeywords = null;
                    disabledKeywords = m_EnvironmentDepthEnabledMaterialKeywords;
                }
                else
                {
                    enabledKeywords = m_EnvironmentDepthEnabledMaterialKeywords;
                    disabledKeywords = null;
                }
            }
        }

        /// <summary>
        /// Container to wrap the native ARCore human body APIs.
        /// </summary>
        static class NativeApi
        {
            [DllImport("UnityARCore")]
            public static extern Supported UnityARCore_OcclusionProvider_DoesSupportEnvironmentDepth();

            [DllImport("UnityARCore")]
            public static extern void UnityARCore_OcclusionProvider_Construct(int textureEnvDepthPropertyId, bool useAdvancedRendering);

            [DllImport("UnityARCore")]
            public static extern void UnityARCore_OcclusionProvider_Start();

            [DllImport("UnityARCore")]
            public static extern void UnityARCore_OcclusionProvider_Stop();

            [DllImport("UnityARCore")]
            public static extern void UnityARCore_OcclusionProvider_Destruct();

            [DllImport("UnityARCore")]
            public static extern EnvironmentDepthMode UnityARCore_OcclusionProvider_GetRequestedEnvironmentDepthMode();

            [DllImport("UnityARCore")]
            public static extern void UnityARCore_OcclusionProvider_SetRequestedEnvironmentDepthMode(EnvironmentDepthMode environmentDepthMode);

            [DllImport("UnityARCore")]
            public static extern EnvironmentDepthMode UnityARCore_OcclusionProvider_GetCurrentEnvironmentDepthMode();

            [DllImport("UnityARCore")]
            public static unsafe extern bool UnityARCore_OcclusionProvider_TryGetEnvironmentDepth(out XRTextureDescriptor envDepthDescriptor);

            [DllImport("UnityARCore")]
            public static unsafe extern void* UnityARCore_OcclusionProvider_AcquireTextureDescriptors(out int length, out int elementSize);

            [DllImport("UnityARCore")]
            public static extern unsafe void UnityARCore_OcclusionProvider_ReleaseTextureDescriptors(void* descriptors);

            [DllImport("UnityARCore")]
            public static extern bool UnityARCore_OcclusionProvider_IsEnvironmentDepthEnabled();

            [DllImport("UnityARCore")]
            public static extern bool UnityARCore_OcclusionProvider_GetEnvironmentDepthTemporalSmoothingEnabled();

            [DllImport("UnityARCore")]
            public static extern bool UnityARCore_OcclusionProvider_TryGetEnvironmentDepthConfidence(out XRTextureDescriptor environmentDepthConfidenceDescriptor);
        }
    }
}
