using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine.Rendering;

namespace UnityEngine.XR.ARSubsystems
{
    /// <summary>
    /// Represents the properties included in the camera frame.
    /// </summary>
    [Flags]
    public enum XRCameraFrameProperties
    {
        /// <summary>
        /// The timestamp of the frame is included.
        /// </summary>
        [Description("Timestamp")]
        Timestamp = (1 << 0),

        /// <summary>
        /// The average brightness of the frame is included.
        /// </summary>
        [Description("AverageBrightness")]
        AverageBrightness = (1 << 1),

        /// <summary>
        /// The average color temperature of the frame is included.
        /// </summary>
        [Description("AverageColorTemperature")]
        AverageColorTemperature = (1 << 2),

        /// <summary>
        /// The color correction value of the frame is included.
        /// </summary>
        [Description("ColorCorrection")]
        ColorCorrection = (1 << 3),

        /// <summary>
        /// The projection matrix for the frame is included.
        /// </summary>
        [Description("ProjectionMatrix")]
        ProjectionMatrix = (1 << 4),

        /// <summary>
        /// The display matrix for the frame is included.
        /// </summary>
        [Description("DisplayMatrix")]
        DisplayMatrix = (1 << 5),

        /// <summary>
        /// The average intensity in lumens is included.
        /// </summary>
        [Description("AverageIntensityInLumens")]
        AverageIntensityInLumens = (1 << 6),

        /// <summary>
        /// The camera exposure duration is included.
        /// </summary>
        [Description("ExposureDuration")]
        ExposureDuration = (1 << 7),

        /// <summary>
        /// The camera exposure offset is included.
        /// </summary>
        [Description("ExposureOffset")]
        ExposureOffset = (1 << 8),

        /// <summary>
        /// The estimated scene main light direction is included.
        /// </summary>
        [Description("MainLightDirection")]
        MainLightDirection = (1 << 9),

        /// <summary>
        /// The estimated scene main light color is included.
        /// </summary>
        [Description("MainLightColor")]
        MainLightColor = (1 << 10),

        /// <summary>
        /// The estimated scene main light intensity in lumens is included.
        /// </summary>
        [Description("MainLightIntensityLumens")]
        MainLightIntensityLumens = (1 << 11),

        /// <summary>
        /// Ambient spherical harmonics are included.
        /// </summary>
        [Description("AmbientSphericalHarmonics")]
        AmbientSphericalHarmonics = (1 << 12),

        /// <summary>
        /// The camera grain texture is included.
        /// </summary>
        [Description("CameraGrain")]
        CameraGrain = (1 << 13),

        /// <summary>
        /// The camera grain noise intensity is included.
        /// </summary>
        [Description("NoiseIntensity")]
        NoiseIntensity = (1 << 14),
        
        /// <summary>
        /// EXIF data for the frame is included.
        /// </summary>
        [Description("ExifData")]
        ExifData = (1 << 15),
    }

    /// <summary>
    /// Represents a frame captured by the device camera with included metadata.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct XRCameraFrame : IEquatable<XRCameraFrame>
    {
        /// <summary>
        /// The timestamp of the frame, in nanoseconds.
        /// </summary>
        /// <value>The timestamp.</value>
        public long timestampNs => m_TimestampNs;
        long m_TimestampNs;

        /// <summary>
        /// The average pixel intensity of the frame in gamma color space, used to match the intensity of light in the
        /// real-world environment. Values are in the range [0.0, 1.0] with zero being black and one being white.
        /// </summary>
        /// <value>The average pixel intensity of the frame.</value>
        /// <seealso cref="averageIntensityInLumens"/>
        public float averageBrightness => m_AverageBrightness;
        float m_AverageBrightness;

        /// <summary>
        /// The estimated color temperature of ambient light in the frame, in degrees Kelvin.
        /// </summary>
        /// <value>The estimated color temperature.</value>
        /// <remarks>
        /// A value of 6500 represents neutral (pure white) lighting; lower values indicate a "warmer" yellow or
        /// orange tint, and higher values indicate a "cooler" blue tint.
        /// </remarks>
        public float averageColorTemperature => m_AverageColorTemperature;
        float m_AverageColorTemperature;

        /// <summary>
        /// The estimated color correction value of the frame.
        /// </summary>
        /// <value>The color correction value.</value>
        /// <remarks>
        /// The RGB scale factors are not intended to brighten nor dim the scene. They are only to shift the color
        /// of virtual objects towards the color of the light; not intensity of the light.
        /// </remarks>
        public Color colorCorrection => m_ColorCorrection;
        Color m_ColorCorrection;

        /// <summary>
        /// The 4x4 projection matrix for the frame.
        /// </summary>
        /// <value>The projection matrix.</value>
        public Matrix4x4 projectionMatrix => m_ProjectionMatrix;
        Matrix4x4 m_ProjectionMatrix;

        /// <summary>
        /// The 4x4 display matrix for the frame. Defines how to render the frame to the screen.
        /// </summary>
        /// <value>The display matrix.</value>
        public Matrix4x4 displayMatrix => m_DisplayMatrix;
        Matrix4x4 m_DisplayMatrix;

        /// <summary>
        /// The camera's <see cref="TrackingState"/> when this frame was captured.
        /// </summary>
        /// <value>The tracking state.</value>
        public TrackingState trackingState => m_TrackingState;
        TrackingState m_TrackingState;

        /// <summary>
        /// The native pointer associated with this frame.
        /// The data pointed to by this pointer is specific to provider implementation.
        /// </summary>
        /// <value>The native pointer.</value>
        public IntPtr nativePtr => m_NativePtr;
        IntPtr m_NativePtr;

        /// <summary>
        /// The set of flags that indicates which properties are included in the frame.
        /// </summary>
        /// <value>The included camera frame properties.</value>
        public XRCameraFrameProperties properties => m_Properties;
        XRCameraFrameProperties m_Properties;

        /// <summary>
        /// The estimated intensity of the real-world environment, in lumens.
        /// Represents an average of directional light sources.
        /// </summary>
        /// <value>The estimated intensity.</value>
        /// <seealso cref="averageBrightness"/>
        public float averageIntensityInLumens => m_AverageIntensityInLumens;
        float m_AverageIntensityInLumens;

        /// <summary>
        /// The camera exposure duration of the frame, in seconds with sub-millisecond precision.
        /// </summary>
        /// <value>The camera exposure duration.</value>
        public double exposureDuration => m_ExposureDuration;
        double m_ExposureDuration;

        /// <summary>
        /// The camera exposure offset of the frame for lighting scaling.
        /// </summary>
        /// <value>The camera exposure offset.</value>
        public float exposureOffset => m_ExposureOffset;
        float m_ExposureOffset;

        /// <summary>
        /// The estimated intensity in lumens of the strongest directional light source in the real-world environment.
        /// </summary>
        /// <value>The estimated intensity of the main light.</value>
        public float mainLightIntensityLumens => m_MainLightIntensityLumens;
        float m_MainLightIntensityLumens;

        /// <summary>
        /// The estimated color of the strongest directional light source in the real-world environment.
        /// </summary>
        /// <value>The estimated color of the main light.</value>
        public Color mainLightColor => m_MainLightColor;
        Color m_MainLightColor;

        /// <summary>
        /// The estimated direction of the strongest directional light source in the real-world environment.
        /// </summary>
        /// <value>The estimated direction of the main light.</value>
        public Vector3 mainLightDirection => m_MainLightDirection;
        Vector3 m_MainLightDirection;

        /// <summary>
        /// The ambient spherical harmonic coefficients that represent the real-world lighting.
        /// </summary>
        /// <value>The ambient spherical harmonic coefficients.</value>
        /// <remarks>
        /// See <see href="https://docs.unity3d.com/ScriptReference/Rendering.SphericalHarmonicsL2.html">Rendering.SphericalHarmonicsL2</see>
        /// for further details.
        /// </remarks>
        public SphericalHarmonicsL2 ambientSphericalHarmonics => m_AmbientSphericalHarmonics;
        SphericalHarmonicsL2 m_AmbientSphericalHarmonics;

        /// <summary>
        /// A texture that simulates the camera's noise.
        /// </summary>
        /// <value>The camera grain texture.</value>
        public XRTextureDescriptor cameraGrain => m_CameraGrain;
        XRTextureDescriptor m_CameraGrain;

        /// <summary>
        /// The level of intensity of camera grain noise in a scene.
        /// </summary>
        /// <value>The noise intensity.</value>
        public float noiseIntensity => m_NoiseIntensity;
        float m_NoiseIntensity;
        
        /// <summary>
        /// The frame's EXIF data.
        /// </summary>
        /// <value>The EXIF data.</value>
        XRCameraFrameExifData m_ExifData;

        /// <summary>
        /// Indicates whether <see cref="timestampNs"/> was assigned a value.
        /// </summary>
        /// <value><see langword="true"/> if the frame has a timestamp. Otherwise, <see langword="false"/>.</value>
        public bool hasTimestamp => (m_Properties & XRCameraFrameProperties.Timestamp) != 0;

        /// <summary>
        /// Indicates whether <see cref="averageBrightness"/> was assigned a value.
        /// </summary>
        /// <value><see langword="true"/> if the frame has an average brightness value. Otherwise, <see langword="false"/>.</value>
        public bool hasAverageBrightness => (m_Properties & XRCameraFrameProperties.AverageBrightness) != 0;

        /// <summary>
        /// Indicates whether <see cref="averageColorTemperature"/> was assigned a value.
        /// </summary>
        /// <value><see langword="true"/> if the frame has an average color temperature value. Otherwise, <see langword="false"/>.</value>
        public bool hasAverageColorTemperature => (m_Properties & XRCameraFrameProperties.AverageColorTemperature) != 0;

        /// <summary>
        /// Indicates whether <see cref="colorCorrection"/> was assigned a value.
        /// </summary>
        /// <value><see langword="true"/> if the frame has a color correction value. Otherwise, <see langword="false"/>.</value>
        public bool hasColorCorrection => (m_Properties & XRCameraFrameProperties.ColorCorrection) != 0;

        /// <summary>
        /// Indicates whether <see cref="projectionMatrix"/> was assigned a value.
        /// </summary>
        /// <value><see langword="true"/> if the frame has a projection matrix. Otherwise, <see langword="false"/>.</value>
        public bool hasProjectionMatrix => (m_Properties & XRCameraFrameProperties.ProjectionMatrix) != 0;

        /// <summary>
        /// Indicates whether <see cref="displayMatrix"/> was assigned a value.
        /// </summary>
        /// <value><see langword="true"/> if the frame has a display matrix. Otherwise, <see langword="false"/>.</value>
        public bool hasDisplayMatrix => (m_Properties & XRCameraFrameProperties.DisplayMatrix) != 0;

        /// <summary>
        /// Indicates whether <see cref="averageIntensityInLumens"/> was assigned a value.
        /// </summary>
        /// <value><see langword="true"/> if the frame has an average intensity value in lumens. Otherwise, <see langword="false"/>.</value>
        public bool hasAverageIntensityInLumens => (m_Properties & XRCameraFrameProperties.AverageIntensityInLumens) != 0;

        /// <summary>
        /// Indicates whether <see cref="exposureDuration"/> was assigned a value.
        /// </summary>
        /// <value><see langword="true"/> if the frame has an exposure duration value. Otherwise, <see langword="false"/>.</value>
        public bool hasExposureDuration => (m_Properties & XRCameraFrameProperties.ExposureDuration) != 0;

        /// <summary>
        /// Indicates whether <see cref="exposureOffset"/> was assigned a value.
        /// </summary>
        /// <value><see langword="true"/> if the frame has an exposure offset value. Otherwise, <see langword="false"/>.</value>
        public bool hasExposureOffset => (m_Properties & XRCameraFrameProperties.ExposureOffset) != 0;

        /// <summary>
        /// Indicates whether <see cref="mainLightIntensityLumens"/> was assigned a value.
        /// </summary>
        /// <value><see langword="true"/> if the frame has an estimated main light intensity value.
        /// Otherwise, <see langword="false"/>.</value>
        public bool hasMainLightIntensityLumens => (m_Properties & XRCameraFrameProperties.MainLightIntensityLumens) != 0;

        /// <summary>
        /// Indicates whether <see cref="mainLightColor"/> was assigned a value.
        /// </summary>
        /// <value><see langword="true"/> if the frame has an estimated main light color value. Otherwise, <see langword="false"/>.</value>
        public bool hasMainLightColor => (m_Properties & XRCameraFrameProperties.MainLightColor) != 0;

        /// <summary>
        /// Indicates whether <see cref="mainLightDirection"/> was assigned a value.
        /// </summary>
        /// <value><see langword="true"/> if the frame has an estimated main light direction value. Otherwise, <see langword="false"/>.</value>
        public bool hasMainLightDirection => (m_Properties & XRCameraFrameProperties.MainLightDirection) != 0;

        /// <summary>
        /// Indicates whether <see cref="ambientSphericalHarmonics"/> was assigned a value.
        /// </summary>
        /// <value><see langword="true"/> if the frame has values for ambient spherical harmonics coefficients.
        /// Otherwise, <see langword="false"/>.</value>
        public bool hasAmbientSphericalHarmonics => (m_Properties & XRCameraFrameProperties.AmbientSphericalHarmonics) != 0;

        /// <summary>
        /// Indicates whether <see cref="cameraGrain"/> was assigned a value.
        /// </summary>
        /// <value><see langword="true"/> if the frame has a camera grain texture. Otherwise, <see langword="false"/>.</value>
        public bool hasCameraGrain => (m_Properties & XRCameraFrameProperties.CameraGrain) != 0;

        /// <summary>
        /// Indicates whether <see cref="noiseIntensity"/> was assigned a value.
        /// </summary>
        /// <value><see langword="true"/> if the frame has a camera grain noise intensity value. Otherwise, <see langword="false"/>.</value>
        public bool hasNoiseIntensity => (m_Properties & XRCameraFrameProperties.NoiseIntensity) != 0;

        /// <summary>
        /// Indicates whether <see cref="exifData"/> was assigned a value.
        /// </summary>
        /// <value><see langword="true"/> if the frame has EXIF data. Otherwise, <see langword="false"/>.</value>
        bool hasExifData => (m_Properties & XRCameraFrameProperties.ExifData) != 0;
        
        /// <summary>
        /// Creates a <see cref="XRCameraFrame"/>.
        /// </summary>
        /// <param name="timestamp">The timestamp of the frame, in nanoseconds.</param>
        /// <param name="averageBrightness">The estimated intensity of the frame, in gamma color space.</param>
        /// <param name="averageColorTemperature">The estimated color temperature of the frame.</param>
        /// <param name="colorCorrection">The estimated color correction value of the frame.</param>
        /// <param name="projectionMatrix">The 4x4 projection matrix for the frame.</param>
        /// <param name="displayMatrix">The 4x4 display matrix for the frame.</param>
        /// <param name="trackingState">The camera's <see cref="TrackingState"/> when the frame was captured.</param>
        /// <param name="nativePtr">The native pointer associated with the frame.</param>
        /// <param name="properties">The set of flags that indicates which properties are included in the frame..</param>
        /// <param name="averageIntensityInLumens">The estimated intensity of the real-world environment, in lumens.</param>
        /// <param name="exposureDuration">The camera exposure duration of the frame, in seconds with sub-millisecond precision.</param>
        /// <param name="exposureOffset">The camera exposure offset of the frame for lighting scaling.</param>
        /// <param name="mainLightIntensityInLumens">The estimated intensity in lumens of strongest real-world directional light source.</param>
        /// <param name="mainLightColor">The estimated color of the strongest real-world directional light source.</param>
        /// <param name="mainLightDirection">The estimated direction of the strongest real-world directional light source.</param>
        /// <param name="ambientSphericalHarmonics">The ambient spherical harmonic coefficients that represent the real-world lighting.</param>
        /// <param name="cameraGrain">A texture that simulates the camera's noise.</param>
        /// <param name="noiseIntensity">The level of intensity of camera grain noise in a scene.</param>
        public XRCameraFrame(
            long timestamp,
            float averageBrightness,
            float averageColorTemperature,
            Color colorCorrection,
            Matrix4x4 projectionMatrix,
            Matrix4x4 displayMatrix,
            TrackingState trackingState,
            IntPtr nativePtr,
            XRCameraFrameProperties properties,
            float averageIntensityInLumens,
            double exposureDuration,
            float exposureOffset,
            float mainLightIntensityInLumens,
            Color mainLightColor,
            Vector3 mainLightDirection,
            SphericalHarmonicsL2 ambientSphericalHarmonics,
            XRTextureDescriptor cameraGrain,
            float noiseIntensity)
        {
            m_TimestampNs = timestamp;
            m_AverageBrightness = averageBrightness;
            m_AverageColorTemperature = averageColorTemperature;
            m_ColorCorrection = colorCorrection;
            m_ProjectionMatrix = projectionMatrix;
            m_DisplayMatrix = displayMatrix;
            m_TrackingState = trackingState;
            m_NativePtr = nativePtr;
            m_Properties = properties;
            m_AverageIntensityInLumens = averageIntensityInLumens;
            m_ExposureDuration = exposureDuration;
            m_ExposureOffset = exposureOffset;
            m_MainLightIntensityLumens = mainLightIntensityInLumens;
            m_MainLightColor = mainLightColor;
            m_MainLightDirection = mainLightDirection;
            m_AmbientSphericalHarmonics = ambientSphericalHarmonics;
            m_CameraGrain = cameraGrain;
            m_NoiseIntensity = noiseIntensity;
            m_ExifData = default;
        }
        
        /// <summary>
        /// Creates a <see cref="XRCameraFrame"/> with EXIF data.
        /// </summary>
        /// <param name="timestamp">The timestamp of the frame, in nanoseconds.</param>
        /// <param name="averageBrightness">The estimated intensity of the frame, in gamma color space.</param>
        /// <param name="averageColorTemperature">The estimated color temperature of the frame.</param>
        /// <param name="colorCorrection">The estimated color correction value of the frame.</param>
        /// <param name="projectionMatrix">The 4x4 projection matrix for the frame.</param>
        /// <param name="displayMatrix">The 4x4 display matrix for the frame.</param>
        /// <param name="trackingState">The camera's <see cref="TrackingState"/> when the frame was captured.</param>
        /// <param name="nativePtr">The native pointer associated with the frame.</param>
        /// <param name="properties">The set of flags that indicates which properties are included in the frame.</param>
        /// <param name="averageIntensityInLumens">The estimated intensity of the real-world environment, in lumens.</param>
        /// <param name="exposureDuration">The camera exposure duration of the frame, in seconds with sub-millisecond precision.</param>
        /// <param name="exposureOffset">The camera exposure offset of the frame for lighting scaling.</param>
        /// <param name="mainLightIntensityInLumens">The estimated intensity in lumens of strongest real-world directional light source.</param>
        /// <param name="mainLightColor">The estimated color of the strongest real-world directional light source.</param>
        /// <param name="mainLightDirection">The estimated direction of the strongest real-world directional light source.</param>
        /// <param name="ambientSphericalHarmonics">The ambient spherical harmonic coefficients that represent the real-world lighting.</param>
        /// <param name="cameraGrain">A texture that simulates the camera's noise.</param>
        /// <param name="noiseIntensity">The level of intensity of camera grain noise in a scene.</param>
        /// <param name="exifData">The EXIF data.</param>
        public XRCameraFrame(
            long timestamp,
            float averageBrightness,
            float averageColorTemperature,
            Color colorCorrection,
            Matrix4x4 projectionMatrix,
            Matrix4x4 displayMatrix,
            TrackingState trackingState,
            IntPtr nativePtr,
            XRCameraFrameProperties properties,
            float averageIntensityInLumens,
            double exposureDuration,
            float exposureOffset,
            float mainLightIntensityInLumens,
            Color mainLightColor,
            Vector3 mainLightDirection,
            SphericalHarmonicsL2 ambientSphericalHarmonics,
            XRTextureDescriptor cameraGrain,
            float noiseIntensity,
            XRCameraFrameExifData exifData) 
        : this(timestamp, averageBrightness, averageColorTemperature, 
            colorCorrection, projectionMatrix, displayMatrix, 
            trackingState, nativePtr, properties, 
            averageIntensityInLumens, exposureDuration, exposureOffset, 
            mainLightIntensityInLumens, mainLightColor, mainLightDirection,
            ambientSphericalHarmonics, cameraGrain, noiseIntensity)
        {
            m_ExifData = exifData;
        }

        /// <summary>
        /// Get the timestamp of the frame if possible.
        /// </summary>
        /// <param name="timestampNs">The timestamp of the camera frame, equal to <see cref="timestampNs"/>.</param>
        /// <returns><see langword="true"/> if the frame has a timestamp. Otherwise, <see langword="false"/>.
        ///   Equal to <see cref="hasTimestamp"/>.</returns>
        public bool TryGetTimestamp(out long timestampNs)
        {
            timestampNs = this.timestampNs;
            return hasTimestamp;
        }

        /// <summary>
        /// Get the average brightness of the frame if possible.
        /// </summary>
        /// <param name="averageBrightness">The average pixel intensity of the frame, equal to <see cref="averageBrightness"/>.</param>
        /// <returns><see langword="true"/> if the frame has an average brightness value. Otherwise, <see langword="false"/>.
        ///   Equal to <see cref="hasAverageBrightness"/>.</returns>
        public bool TryGetAverageBrightness(out float averageBrightness)
        {
            averageBrightness = this.averageBrightness;
            return hasAverageBrightness;
        }

        /// <summary>
        /// Get the estimated color temperature of the frame if possible.
        /// </summary>
        /// <param name="averageColorTemperature">The estimated color temperature of the frame, in degrees Kelvin.
        ///   Equal to <see cref="averageColorTemperature"/>.</param>
        /// <returns><see langword="true"/> if the frame has an estimated color temperature value. Otherwise, <see langword="false"/>.
        ///   Equal to <see cref="hasAverageColorTemperature"/>.</returns>
        public bool TryGetAverageColorTemperature(out float averageColorTemperature)
        {
            averageColorTemperature = this.averageColorTemperature;
            return hasAverageColorTemperature;
        }

        /// <summary>
        /// Get the projection matrix for the frame if possible.
        /// </summary>
        /// <param name="projectionMatrix">The projection matrix. Equal to <see cref="projectionMatrix"/>.</param>
        /// <returns><see langword="true"/> if the frame has a projection matrix. Otherwise, <see langword="false"/>.
        ///   Equal to <see cref="hasProjectionMatrix"/>.</returns>
        public bool TryGetProjectionMatrix(out Matrix4x4 projectionMatrix)
        {
            projectionMatrix = this.projectionMatrix;
            return hasProjectionMatrix;
        }

        /// <summary>
        /// Get the display matrix for the frame if possible.
        /// </summary>
        /// <param name="displayMatrix">The display matrix. Equal to <see cref="displayMatrix"/>.</param>
        /// <returns><see langword="true"/> if the frame has a display matrix. Otherwise, <see langword="false"/>.
        ///   Equal to <see cref="hasDisplayMatrix"/>.</returns>
        public bool TryGetDisplayMatrix(out Matrix4x4 displayMatrix)
        {
            displayMatrix = this.displayMatrix;
            return hasDisplayMatrix;
        }

        /// <summary>
        /// Get the estimated intensity in lumens of the real-world environment, if possible.
        /// </summary>
        /// <param name="averageIntensityInLumens">The estimated intensity. Equal to <see cref="averageIntensityInLumens"/>.</param>
        /// <returns><see langword="true"/> if the frame has an estimated intensity value in lumens.
        ///   Otherwise, returns <see langword="false"/>. Equal to <see cref="hasAverageIntensityInLumens"/>.</returns>
        public bool TryGetAverageIntensityInLumens(out float averageIntensityInLumens)
        {
            averageIntensityInLumens = this.averageIntensityInLumens;
            return hasAverageIntensityInLumens;
        }
        
        /// <summary>
        /// Get the frame's EXIF data, if possible.
        /// </summary>
        /// <param name="exifData">The EXIF data.</param>
        /// <returns><see langword="true"/> if the frame has EXIF data.
        ///   Otherwise, returns <see langword="false"/>.</returns>
        public bool TryGetExifData(out XRCameraFrameExifData exifData)
        {
            exifData = m_ExifData;
            return hasExifData;
        }

        /// <summary>
        /// Compares for equality.
        /// </summary>
        /// <param name="other">The other <see cref="XRCameraFrame"/> to compare against.</param>
        /// <returns><see langword="true"/> if the <see cref="XRCameraFrame"/> represents the same object.
        /// Otherwise, <see langword="false"/>.</returns>
        public bool Equals(XRCameraFrame other)
        {
            return (m_TimestampNs.Equals(other.m_TimestampNs) && m_AverageBrightness.Equals(other.m_AverageBrightness)
                    && m_AverageColorTemperature.Equals(other.m_AverageColorTemperature)
                    && m_ProjectionMatrix.Equals(other.m_ProjectionMatrix)
                    && m_DisplayMatrix.Equals(other.m_DisplayMatrix)
                    && m_AverageIntensityInLumens.Equals(other.m_AverageIntensityInLumens)
                    && m_ExposureDuration.Equals(other.m_ExposureDuration)
                    && m_ExposureOffset.Equals(other.m_ExposureOffset)
                    && m_MainLightDirection.Equals(other.m_MainLightDirection)
                    && m_MainLightIntensityLumens.Equals(other.m_MainLightIntensityLumens)
                    && m_MainLightColor.Equals(other.m_MainLightColor)
                    && m_AmbientSphericalHarmonics.Equals(other.m_AmbientSphericalHarmonics)
                    && m_CameraGrain.Equals(other.m_CameraGrain)
                    && m_NoiseIntensity.Equals(other.m_NoiseIntensity)
                    && m_ExifData.Equals(other.m_ExifData)
                    && (m_Properties == other.m_Properties));
        }

        /// <summary>
        /// Compares for equality.
        /// </summary>
        /// <param name="obj">An <c>object</c> to compare against.</param>
        /// <returns><see langword="true"/> if <paramref name="obj"/> is an <see cref="XRCameraFrame"/> and
        /// <see cref="Equals(XRCameraFrame)"/> is also <see langword="true"/>. Otherwise, <see langword="false"/>.</returns>
        public override bool Equals(System.Object obj)
        {
            return ((obj is XRCameraFrame) && Equals((XRCameraFrame)obj));
        }

        /// <summary>
        /// Compares <paramref name="lhs"/> and <paramref name="rhs"/> for equality using <see cref="Equals(XRCameraFrame)"/>.
        /// </summary>
        /// <param name="lhs">The left-hand-side <see cref="XRCameraFrame"/> of the comparison.</param>
        /// <param name="rhs">The right-hand-side <see cref="XRCameraFrame"/> of the comparison.</param>
        /// <returns><see langword="true"/> if <paramref name="lhs"/> compares equal to <paramref name="rhs"/>.
        /// Otherwise, <see langword="false"/>.</returns>
        public static bool operator ==(XRCameraFrame lhs, XRCameraFrame rhs)
        {
            return lhs.Equals(rhs);
        }

        /// <summary>
        /// Compares <paramref name="lhs"/> and <paramref name="rhs"/> for inequality using <see cref="Equals(XRCameraFrame)"/>.
        /// </summary>
        /// <param name="lhs">The left-hand-side <see cref="XRCameraFrame"/> of the comparison.</param>
        /// <param name="rhs">The right-hand-side <see cref="XRCameraFrame"/> of the comparison.</param>
        /// <returns><see langword="false"/> if <paramref name="lhs"/> compares equal to <paramref name="rhs"/>.
        /// Otherwise, <see langword="true"/>.</returns>
        public static bool operator !=(XRCameraFrame lhs, XRCameraFrame rhs)
        {
            return !lhs.Equals(rhs);
        }

        /// <summary>
        /// Generates a hash code suitable for use in <c>HashSet</c> and <c>Dictionary</c>.
        /// </summary>
        /// <returns>A hash of this <see cref="XRCameraFrame"/>.</returns>
        public override int GetHashCode()
        {
            int hashCode = 486187739;
            unchecked
            {
                hashCode = (hashCode * 486187739) + m_TimestampNs.GetHashCode();
                hashCode = (hashCode * 486187739) + m_AverageBrightness.GetHashCode();
                hashCode = (hashCode * 486187739) + m_AverageColorTemperature.GetHashCode();
                hashCode = (hashCode * 486187739) + m_ColorCorrection.GetHashCode();
                hashCode = (hashCode * 486187739) + m_ProjectionMatrix.GetHashCode();
                hashCode = (hashCode * 486187739) + m_DisplayMatrix.GetHashCode();
                hashCode = (hashCode * 486187739) + m_AverageIntensityInLumens.GetHashCode();
                hashCode = (hashCode * 486187739) + m_ExposureDuration.GetHashCode();
                hashCode = (hashCode * 486187739) + m_ExposureOffset.GetHashCode();
                hashCode = (hashCode * 486187739) + m_MainLightDirection.GetHashCode();
                hashCode = (hashCode * 486187739) + m_MainLightColor.GetHashCode();
                hashCode = (hashCode * 486187739) + m_AmbientSphericalHarmonics.GetHashCode();
                hashCode = (hashCode * 486187739) + m_MainLightIntensityLumens.GetHashCode();
                hashCode = (hashCode * 486187739) + m_CameraGrain.GetHashCode();
                hashCode = (hashCode * 486187739) + m_NoiseIntensity.GetHashCode();
                hashCode = (hashCode * 486187739) + m_ExifData.GetHashCode();
                hashCode = (hashCode * 486187739) + m_NativePtr.GetHashCode();
                hashCode = (hashCode * 486187739) + ((int)m_Properties).GetHashCode();
            }
            return hashCode;
        }

        /// <summary>
        /// Generates a string representation of this <see cref="XRCameraFrame"/> suitable for debugging purposes.
        /// </summary>
        /// <returns>A string representation of this <see cref="XRCameraFrame"/>.</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine("{");
            sb.AppendLine($"  TimestampNs: {m_TimestampNs}");
            sb.AppendLine($"  AverageBrightness: {m_AverageBrightness:0.000}");
            sb.AppendLine($"  AverageColorTemperature: {m_AverageColorTemperature:0.000}");
            sb.AppendLine($"  ColorCorrection: {m_ColorCorrection}");
            sb.AppendLine($"  ProjectionMatrix: {m_ProjectionMatrix:0.000}");
            sb.AppendLine($"  DisplayMatrix: {m_DisplayMatrix:0.000}");
            sb.AppendLine($"  ExposureDuration: {m_ExposureDuration:0.000}");
            sb.AppendLine($"  ExposureOffset: {m_ExposureOffset}");
            sb.AppendLine($"  MainLightDirection: {m_MainLightDirection:0.000}");
            sb.AppendLine($"  MainLightIntensityLumens: {m_MainLightIntensityLumens:0.000}");
            sb.AppendLine($"  MainLightColor: {m_MainLightColor:0.000}");
            sb.AppendLine($"  AmbientSphericalHarmonics: {m_AmbientSphericalHarmonics}");
            sb.AppendLine($"  NoiseIntensity: {m_NoiseIntensity:0.000}");
            sb.AppendLine($"  NativePtr: {m_NativePtr.ToString("X16")}");
            sb.AppendLine($"  ExifData: {m_ExifData.ToString()}");
            sb.AppendLine("}");
            return sb.ToString();
        }
    }
}
