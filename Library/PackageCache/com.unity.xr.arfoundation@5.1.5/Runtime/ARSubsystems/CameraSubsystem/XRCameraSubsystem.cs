using System;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine.SubsystemsImplementation;

namespace UnityEngine.XR.ARSubsystems
{
    /// <summary>
    /// Provides access to a device's camera.
    /// </summary>
    /// <remarks>
    /// The <c>XRCameraSubsystem</c> links a Unity <c>Camera</c> to a device camera for video overlay (pass-thru
    /// rendering). It also allows developers to query for environmental light estimation if available.
    /// </remarks>
    public class XRCameraSubsystem : SubsystemWithProvider<XRCameraSubsystem, XRCameraSubsystemDescriptor, XRCameraSubsystem.Provider>
    {
        /// <summary>
        /// Interface for providing camera functionality for the implementation.
        /// </summary>
        public class Provider : SubsystemProvider<XRCameraSubsystem>
        {
            /// <summary>
            /// An instance of the <see cref="XRCpuImage.Api"/> used to operate on <see cref="XRCpuImage"/> objects.
            /// </summary>
            public virtual XRCpuImage.Api cpuImageApi => null;

            /// <summary>
            /// Get the Material used by this <see cref="XRCameraSubsystem"/> to render the camera texture.
            /// </summary>
            /// <value>The Material to render the camera texture.</value>
            public virtual Material cameraMaterial => null;

            /// <summary>
            /// Get whether camera permission has been granted.
            /// </summary>
            /// <value><see langword="true"/> if camera permission has been granted. Otherwise, <see langword="false"/>.</value>
            public virtual bool permissionGranted => false;

            /// <summary>
            /// Get whether culling should be inverted during rendering. Some front-facing camera modes might
            /// require this.
            /// </summary>
            /// <value><see langword="true"/> if culling should be inverted during rendering. Otherwise, <see langword="false"/>.</value>
            public virtual bool invertCulling => false;

            /// <summary>
            /// Get the actual camera facing direction.
            /// </summary>
            /// <value>The current camera facing direction.</value>
            /// <seealso cref="requestedCamera"/>
            public virtual Feature currentCamera => Feature.None;

            /// <summary>
            /// Get or set the requested camera facing direction, that is, the <see cref="Feature.AnyCamera"/> bits.
            /// </summary>
            /// <value>The requested camera facing direction</value>.
            public virtual Feature requestedCamera
            {
                get => Feature.None;
                set { }
            }

            /// <summary>
            /// Get whether auto focus is enabled.
            /// </summary>
            /// <value><see langword="true"/> if auto focus is enabled. Otherwise, <see langword="false"/>.</value>
            /// <seealso cref="autoFocusRequested"/>
            public virtual bool autoFocusEnabled => false;

            /// <summary>
            /// Get or set whether auto focus is requested.
            /// </summary>
            /// <value><see langword="true"/> if auto focus is requested. Otherwise, <see langword="false"/>.</value>
            public virtual bool autoFocusRequested
            {
                get => false;
                set { }
            }

            /// <summary>
            /// Get the current light estimation mode in use by the subsystem.
            /// </summary>
            /// <value>The current light estimation mode.</value>
            /// <seealso cref="requestedLightEstimation"/>
            public virtual Feature currentLightEstimation => Feature.None;

            /// <summary>
            /// Get or set the requested light estimation mode.
            /// </summary>
            /// <value>The requested light estimation mode.</value>
            public virtual Feature requestedLightEstimation
            {
                get => Feature.None;
                set { }
            }

            /// <summary>
            /// Property to be implemented by the provider to query or set the current camera configuration.
            /// </summary>
            /// <value>The current camera configuration, if it exists. Otherwise, <see langword="null"/>.</value>
            /// <exception cref="System.NotSupportedException">Thrown when setting the current configuration if the
            /// implementation does not support camera configurations.</exception>
            /// <exception cref="System.ArgumentException">Thrown when setting the current configuration if the given
            /// configuration is not a valid, supported camera configuration.</exception>
            /// <exception cref="System.InvalidOperationException">Thrown when setting the current configuration if the
            /// implementation is unable to set the current camera configuration.</exception>
            public virtual XRCameraConfiguration? currentConfiguration
            {
                get => null;
                set => throw new NotSupportedException("setting current camera configuration is not supported by this implementation");
            }

            /// <summary>
            /// Get the current <see cref="XRCameraBackgroundRenderingMode"/>.
            /// </summary>
            /// <value>The current <see cref="XRCameraBackgroundRenderingMode"/>.</value>
            /// <seealso cref="requestedBackgroundRenderingMode"/>
            public virtual XRCameraBackgroundRenderingMode currentBackgroundRenderingMode => XRCameraBackgroundRenderingMode.None;

            /// <summary>
            /// Get or set the requested <see cref="XRCameraBackgroundRenderingMode"/>.
            /// </summary>
            /// <value>The requested background rendering mode.</value>
            public virtual XRSupportedCameraBackgroundRenderingMode requestedBackgroundRenderingMode
            {
                get => XRSupportedCameraBackgroundRenderingMode.Any;
                set { }
            }

            /// <summary>
            /// Get the supported <see cref="XRCameraBackgroundRenderingMode"/>s defined as <see cref="XRSupportedCameraBackgroundRenderingMode"/>s.
            /// </summary>
            /// <value>The supported background rendering modes.</value>
            public virtual XRSupportedCameraBackgroundRenderingMode supportedBackgroundRenderingMode => XRSupportedCameraBackgroundRenderingMode.None;

            /// <summary>
            /// Start the camera for the subsystem.
            /// </summary>
            public override void Start() { }

            /// <summary>
            /// Stop the camera for the subsystem.
            /// </summary>
            public override void Stop() { }

            /// <summary>
            /// Destroy the camera for the subsystem.
            /// </summary>
            public override void Destroy() { }

            /// <summary>
            /// Get the camera frame for the subsystem.
            /// </summary>
            /// <param name="cameraParams">The current Unity <c>Camera</c> parameters.</param>
            /// <param name="cameraFrame">The current camera frame returned by the method.</param>
            /// <returns><see langword="true"/> if the method successfully got a frame. Otherwise, <see langword="false"/>.</returns>
            public virtual bool TryGetFrame(
                XRCameraParams cameraParams,
                out XRCameraFrame cameraFrame)
            {
                cameraFrame = default;
                return false;
            }

            /// <summary>
            /// Get the camera intrinsics information.
            /// </summary>
            /// <param name="cameraIntrinsics">The camera intrinsics information returned from the method.</param>
            /// <returns><see langword="true"/> if the method successfully gets the camera intrinsics information.
            /// Otherwise, <see langword="false"/>.</returns>
            public virtual bool TryGetIntrinsics(
                out XRCameraIntrinsics cameraIntrinsics)
            {
                cameraIntrinsics = default(XRCameraIntrinsics);
                return false;
            }

            /// <summary>
            /// Get the supported camera configurations.
            /// </summary>
            /// <param name="defaultCameraConfiguration">A default value used to fill the returned array before copying in
            /// real values. This ensures future additions to this <see langword="struct"/> are backwards compatible.</param>
            /// <param name="allocator">The allocation strategy to use for the returned data.</param>
            /// <returns>The supported camera configurations.</returns>
            public virtual NativeArray<XRCameraConfiguration> GetConfigurations(XRCameraConfiguration defaultCameraConfiguration,
                                                                                Allocator allocator)
            {
                return new NativeArray<XRCameraConfiguration>(0, allocator);
            }

            /// <summary>
            /// Get the <see cref="XRTextureDescriptor"/>s associated with the current <see cref="XRCameraFrame"/>.
            /// </summary>
            /// <param name="defaultDescriptor">A default value used to fill the returned array before copying in real
            /// values. This ensures future additions to this <see langword="struct"/> are backwards compatible.</param>
            /// <param name="allocator">The allocation strategy to use for the returned data..</param>
            /// <returns>The current texture descriptors.</returns>
            public virtual NativeArray<XRTextureDescriptor> GetTextureDescriptors(
                XRTextureDescriptor defaultDescriptor,
                Allocator allocator)
            {
                return new NativeArray<XRTextureDescriptor>(0, allocator);
            }

            /// <summary>
            /// Get the enabled and disabled shader keywords for the Material.
            /// </summary>
            /// <param name="enabledKeywords">The keywords to enable for the Material.</param>
            /// <param name="disabledKeywords">The keywords to disable for the Material.</param>
            public virtual void GetMaterialKeywords(out List<string> enabledKeywords, out List<string> disabledKeywords)
            {
                enabledKeywords = null;
                disabledKeywords = null;
            }

            /// <summary>
            /// Get the latest native camera image.
            /// </summary>
            /// <param name="cameraImageCinfo">The metadata required to construct a <see cref="XRCpuImage"/>.</param>
            /// <returns><see langword="true"/> if the camera image is acquired. Otherwise, <see langword="false"/>.</returns>
            /// <exception cref="System.NotSupportedException">Thrown if the implementation does not support camera image.</exception>
            public virtual bool TryAcquireLatestCpuImage(out XRCpuImage.Cinfo cameraImageCinfo)
            {
                throw new NotSupportedException("getting camera image is not supported by this implementation");
            }

            /// <summary>
            /// Create the camera material from the given camera shader name.
            /// </summary>
            /// <param name="cameraShaderName">The name of the camera shader.</param>
            /// <returns>The created camera material shader.</returns>
            /// <exception cref="System.InvalidOperationException">Thrown if the shader cannot be found or if a material
            /// cannot be created for the shader.</exception>
            protected Material CreateCameraMaterial(string cameraShaderName)
            {
                var shader = Shader.Find(cameraShaderName);
                if (shader == null)
                {
                    throw new InvalidOperationException(
                        $"Could not find shader named '{cameraShaderName}' required "
                        + $"for video overlay on camera subsystem.");
                }

                var material = new Material(shader);
                if (material == null)
                {
                    throw new InvalidOperationException(
                        $"Could not create a material for shader named '{cameraShaderName}' required for video"
                        + " overlay on camera subsystem.");
                }

                return material;
            }

            /// <summary>
            /// Method to be implemented by the provider to handle any required platform-specific functionality
            /// immediately before rendering the camera background. This method will always be called on the render
            /// thread and should only be called by the code responsible for executing background rendering on
            /// mobile AR platforms.
            /// </summary>
            /// <param name="id">Platform-specific identifier.</param>
            public virtual void OnBeforeBackgroundRender(int id) {}
        }

        /// <summary>
        /// Get the camera currently in use.
        /// </summary>
        /// <value>The current camera.</value>
        public Feature currentCamera => provider.currentCamera.Cameras();

        /// <summary>
        /// Get or set the requested camera (that is, the <see cref="Feature.AnyCamera"/> bits).
        /// </summary>
        /// <value>The requested camera.</value>
        public Feature requestedCamera
        {
            get => provider.requestedCamera;
            set => provider.requestedCamera = value.Cameras();
        }

        /// <summary>
        /// Get the current <see cref="XRCameraBackgroundRenderingMode"/>.
        /// </summary>
        /// <value>The current camera background rendering mode.</value>
        /// <seealso cref="requestedCameraBackgroundRenderingMode"/>
        public XRCameraBackgroundRenderingMode currentCameraBackgroundRenderingMode => permissionGranted
            ? provider.currentBackgroundRenderingMode
            : XRCameraBackgroundRenderingMode.None;

        /// <summary>
        /// Get or set the requested <see cref="XRSupportedCameraBackgroundRenderingMode"/>.
        /// </summary>
        /// <value>The requested camera background rendering mode.</value>
        public XRSupportedCameraBackgroundRenderingMode requestedCameraBackgroundRenderingMode
        {
            get => provider.requestedBackgroundRenderingMode;
            set => provider.requestedBackgroundRenderingMode = value;
        }

        /// <summary>
        /// Get the supported <see cref="XRSupportedCameraBackgroundRenderingMode"/>s.
        /// Indicates which <see cref="XRCameraBackgroundRenderingMode"/>s are supported.
        /// </summary>
        /// <value>The supported camera background rendering modes.</value>
        public XRSupportedCameraBackgroundRenderingMode supportedCameraBackgroundRenderingMode =>
            provider.supportedBackgroundRenderingMode;

        /// <summary>
        /// Indicates whether auto focus is enabled.
        /// </summary>
        /// <value><see langword="true"/> if auto focus is enabled. Otherwise, <see langword="false"/>.</value>
        /// <seealso cref="autoFocusRequested"/>
        public bool autoFocusEnabled => provider.autoFocusEnabled;

        /// <summary>
        /// Get or set whether autofocus is requested.
        /// </summary>
        /// <value><see langword="true"/> if autofocus is requested. Otherwise, <see langword="false"/>.</value>
        public bool autoFocusRequested
        {
            get => provider.autoFocusRequested;
            set => provider.autoFocusRequested = value;
        }

        /// <summary>
        /// Get the current light estimation mode in use by the provider.
        /// </summary>
        /// <value>The current light estimation mode.</value>
        /// <seealso cref="requestedLightEstimation"/>
        public Feature currentLightEstimation => provider.currentLightEstimation.LightEstimation();

        /// <summary>
        /// Get or set the requested light estimation mode.
        /// </summary>
        /// <value>The requested light estimation mode.</value>
        public Feature requestedLightEstimation
        {
            get => provider.requestedLightEstimation.LightEstimation();
            set => provider.requestedLightEstimation = value.LightEstimation();
        }

        /// <summary>
        /// Get the Material used by <see cref="XRCameraSubsystem"/> to render the camera texture.
        /// </summary>
        /// <value>The Material to render the camera texture.</value>
        public Material cameraMaterial => provider.cameraMaterial;

        /// <summary>
        /// Indicates whether camera permission has been granted.
        /// </summary>
        /// <value><see langword="true"/> if camera permission has been granted. Otherwise, <see langword="false"/>.</value>
        public bool permissionGranted => provider.permissionGranted;

        /// <summary>
        /// Set this to <see langword="true"/> to invert the culling mode during rendering. Some front-facing
        /// camera modes might require this.
        /// </summary>
        public bool invertCulling => provider.invertCulling;

        /// <summary>
        /// The current camera configuration.
        /// </summary>
        /// <value>The current camera configuration, if it exists. Otherwise, <see langword="null"/>.</value>
        /// <exception cref="System.NotSupportedException">Thrown when setting the current configuration if the
        /// implementation does not support camera configurations.</exception>
        /// <exception cref="System.ArgumentNullException">Thrown when setting the current configuration if the given
        /// configuration is <see langword="null"/>.</exception>
        /// <exception cref="System.ArgumentException">Thrown when setting the current configuration if the given
        /// configuration is not a supported camera configuration.</exception>
        /// <exception cref="System.InvalidOperationException">Thrown when setting the current configuration if the
        /// implementation is unable to set the current camera configuration.
        /// </exception>
        public virtual XRCameraConfiguration? currentConfiguration
        {
            get => provider.currentConfiguration;
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value), "cannot set the camera configuration to null");
                }

                provider.currentConfiguration = value;
            }
        }

        /// <summary>
        /// Construct the <see cref="XRCameraSubsystem"/>.
        /// </summary>
        public XRCameraSubsystem() { }

        /// <summary>
        /// Gets the <see cref="XRTextureDescriptor"/>s associated with the current frame. The caller owns the returned
        /// <c>NativeArray</c> and is responsible for calling <c>Dispose</c> on it.
        /// </summary>
        /// <returns>An array of texture descriptors.</returns>
        /// <param name="allocator">The allocator to use when creating the returned <c>NativeArray</c>.</param>
        public NativeArray<XRTextureDescriptor> GetTextureDescriptors(
            Allocator allocator)
        {
            return provider.GetTextureDescriptors(
                default(XRTextureDescriptor),
                allocator);
        }

        /// <summary>
        /// Method to be called on the render thread to handle any required platform-specific functionality
        /// immediately before rendering the camera background. This method will always be called on the render
        /// thread and should only be called by the code responsible for executing background rendering on
        /// mobile AR platforms.
        /// </summary>
        /// <param name="id">Platform-specific identifier.</param>
        public void OnBeforeBackgroundRender(int id)
        {
            provider.OnBeforeBackgroundRender(id);
        }

        /// <summary>
        /// Get the camera intrinsics information.
        /// </summary>
        /// <param name="cameraIntrinsics">The returned camera intrinsics information.</param>
        /// <returns><see langword="true"/> if the method successfully gets the camera intrinsics information.
        /// Otherwise, <see langword="false"/>.</returns>
        /// <remarks>
        /// > [!NOTE]
        /// > The intrinsics may change each frame. You should call this each frame that you need intrinsics
        /// > in order to ensure you are using the intrinsics for the current frame.
        /// </remarks>
        public bool TryGetIntrinsics(out XRCameraIntrinsics cameraIntrinsics)
        {
            return provider.TryGetIntrinsics(out cameraIntrinsics);
        }

        /// <summary>
        /// Get the supported camera configurations.
        /// </summary>
        /// <param name="allocator">The allocation strategy to use for the returned data.</param>
        /// <returns>The supported camera configurations.</returns>
        public NativeArray<XRCameraConfiguration> GetConfigurations(Allocator allocator)
        {
            return provider.GetConfigurations(default, allocator);
        }

        /// <summary>
        /// Get the latest frame from the provider.
        /// </summary>
        /// <param name="cameraParams">The Unity <c>Camera</c> parameters.</param>
        /// <param name="frame">The camera frame to be populated if the subsystem is running and successfully provides
        /// the latest camera frame.</param>
        /// <returns><see langword="true"/> if the camera frame is successfully returned. Otherwise, <see langword="false"/>.</returns>
        public bool TryGetLatestFrame(XRCameraParams cameraParams, out XRCameraFrame frame)
        {
            if (running && provider.TryGetFrame(cameraParams, out frame))
            {
                return true;
            }

            frame = default;
            return false;
        }

        /// <summary>
        /// Get the enabled and disabled shader keywords for the material.
        /// </summary>
        /// <param name="enabledKeywords">The keywords to enable for the material.</param>
        /// <param name="disabledKeywords">The keywords to disable for the material.</param>
        public void GetMaterialKeywords(out List<string> enabledKeywords, out List<string> disabledKeywords)
            => provider.GetMaterialKeywords(out enabledKeywords, out disabledKeywords);

        /// <summary>
        /// Attempts to acquire the latest camera image. This provides direct access to the raw pixel data, as well as
        /// to utilities to convert to RGB and Grayscale formats. This method is obsolete. Use
        /// <see cref="TryAcquireLatestCpuImage"/> instead.
        /// </summary>
        /// <param name="cpuImage">A valid <see cref="XRCpuImage"/> if this method returns <see langword="true"/>.</param>
        /// <returns><see langword="true"/> if the image was acquired. Otherwise, <see langword="false"/>.</returns>
        /// <exception cref="System.NotSupportedException">Thrown if the implementation does not support camera image.</exception>
        /// <remarks>
        /// The returned <see cref="XRCpuImage"/> must be disposed to avoid resource leaks.
        /// </remarks>
        [Obsolete("Use TryAcquireLatestCpuImage instead. (2020-05-19)")]
        public bool TryGetLatestImage(out XRCpuImage cpuImage) => TryAcquireLatestCpuImage(out cpuImage);

        /// <summary>
        /// Attempts to acquire the latest camera image. This provides direct access to the raw pixel data, as well as
        /// to utilities to convert to RGB and Grayscale formats.
        /// </summary>
        /// <param name="cpuImage">A valid <see cref="XRCpuImage"/> if this method returns <see langword="true"/>.</param>
        /// <returns><see langword="true"/> if the image was acquired. Otherwise, <see langword="false"/>.</returns>
        /// <exception cref="System.NotSupportedException">Thrown if the implementation does not support camera image.</exception>
        /// <remarks>The returned <see cref="XRCpuImage"/> must be disposed to avoid resource leaks.</remarks>
        public bool TryAcquireLatestCpuImage(out XRCpuImage cpuImage)
        {
            if (provider.cpuImageApi != null && provider.TryAcquireLatestCpuImage(out var cinfo))
            {
                cpuImage = new XRCpuImage(provider.cpuImageApi, cinfo);
                return true;
            }

            cpuImage = default;
            return false;
        }

        /// <summary>
        /// Registers a camera subsystem implementation based on the given subsystem parameters.
        /// </summary>
        /// <param name="cameraSubsystemParams">The parameters defining the camera subsystem functionality implemented
        /// by the subsystem provider.</param>
        /// <returns><see langword="true"/> if the subsystem implementation is registered. Otherwise, <see langword="false"/>.</returns>
        /// <exception cref="System.ArgumentException">Thrown when the values specified in the <see cref="XRCameraSubsystemCinfo"/> parameter are invalid.
        /// Typically, this will occur:
        /// <list type="bullet">
        /// <item>
        /// <description>if <see cref="XRCameraSubsystemCinfo.id"/> is <see langword="null"/> or empty</description>
        /// </item>
        /// <item>
        /// <description>if <see cref="XRCameraSubsystemCinfo.implementationType"/> is <see langword="null"/></description>
        /// </item>
        /// <item>
        /// <description>if <see cref="XRCameraSubsystemCinfo.implementationType"/> does not derive from
        /// <see cref="XRCameraSubsystem"/>
        /// </description>
        /// </item>
        /// </list>
        /// </exception>
        public static bool Register(XRCameraSubsystemCinfo cameraSubsystemParams)
        {
            var cameraSubsystemDescriptor = XRCameraSubsystemDescriptor.Create(cameraSubsystemParams);
            SubsystemDescriptorStore.RegisterDescriptor(cameraSubsystemDescriptor);
            return true;
        }
    }
}
