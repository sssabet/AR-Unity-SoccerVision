using System;
using System.Runtime.InteropServices;
using System.Text;

namespace UnityEngine.XR.ARSubsystems
{
    /// <summary>
    /// The color space, as defined by the EXIF specification.
    /// <seealso href="https://web.archive.org/web/20190624045241if_/http://www.cipa.jp:80/std/documents/e/DC-008-Translation-2019-E.pdf"/>
    /// </summary>
    public enum XRCameraFrameExifDataColorSpace : ushort
    {
        /// <summary>
        /// sRGB color space.
        /// </summary>
        sRGB = 1,

        /// <summary>
        /// A color space other than sRGB is used.
        /// </summary>
        Uncalibrated = 65535,
    }

    /// <summary>
    /// The metering mode, as defined by the EXIF specification.
    /// <seealso href="https://web.archive.org/web/20190624045241if_/http://www.cipa.jp:80/std/documents/e/DC-008-Translation-2019-E.pdf"/>
    /// </summary>
    public enum XRCameraFrameExifDataMeteringMode : ushort
    {
        /// <summary>
        /// Unknown.
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// Average metering.
        /// </summary>
        Average = 1,
        
        /// <summary>
        /// Center-weighted metering.
        /// </summary>
        CenterWeightedAverage = 2,
        
        /// <summary>
        /// Spot metering.
        /// </summary>
        Spot = 3,
        
        /// <summary>
        /// MultiSpot metering.
        /// </summary>
        MultiSpot = 4,
        
        /// <summary>
        /// Pattern metering.
        /// </summary>
        Pattern = 5,
        
        /// <summary>
        /// Partial metering.
        /// </summary>
        Partial = 6,
        
        /// <summary>
        /// Other.
        /// </summary>
        Other = 255,
    }

    /// <summary>
    /// Represents the properties included in a camera frame's EXIF data.
    /// </summary>
    [Flags]
    public enum XRCameraFrameExifDataProperties
    {
        /// <summary>
        /// The camera frame EXIF data is disabled or unsupported.
        /// </summary>
        None = 0,

        /// <summary>
        /// The lens aperture of the frame is included.
        /// </summary>
        ApertureValue = 1 << 0,

        /// <summary>
        /// The brightness of the frame is included.
        /// </summary>
        BrightnessValue = 1 << 1,

        /// <summary>
        /// The exposure time of the frame is included.
        /// </summary>
        ExposureTime = 1 << 2,

        /// <summary>
        /// The shutter speed of the frame is included.
        /// </summary>
        ShutterSpeedValue = 1 << 3,

        /// <summary>
        /// The exposure bias of the frame is included.
        /// </summary>
        ExposureBiasValue = 1 << 4,

        /// <summary>
        /// The F number of the frame is included.
        /// </summary>
        FNumber = 1 << 5,

        /// <summary>
        /// The lens focal length of the frame is included.
        /// </summary>
        FocalLength = 1 << 6,

        /// <summary>
        /// The flash status of the frame is included.
        /// </summary>
        Flash = 1 << 7,

        /// <summary>
        /// The color space of the frame is included.
        /// </summary>
        ColorSpace = 1 << 8,

        /// <summary>
        /// The photographic sensitivity of the frame is included.
        /// </summary>
        PhotographicSensitivity = 1 << 9,

        /// <summary>
        /// The metering mode of the frame is included.
        /// </summary>
        MeteringMode = 1 << 10,
    }

    /// <summary>
    /// Represents EXIF data from the frame captured by the device camera.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct XRCameraFrameExifData : IEquatable<XRCameraFrameExifData>
    {
        IntPtr m_NativePtr;
        double m_ApertureValue;
        double m_BrightnessValue;
        double m_ExposureTime;
        double m_ShutterSpeedValue;
        float m_ExposureBiasValue;
        float m_FNumber;
        float m_FocalLength;
        short m_Flash;
        XRCameraFrameExifDataColorSpace m_ColorSpace;
        short m_PhotographicSensitivity;
        XRCameraFrameExifDataMeteringMode m_MeteringMode;
        XRCameraFrameExifDataProperties m_Properties;

        bool hasApertureValue => (m_Properties & XRCameraFrameExifDataProperties.ApertureValue) != 0;
        bool hasBrightnessValue => (m_Properties & XRCameraFrameExifDataProperties.BrightnessValue) != 0;
        bool hasExposureTime => (m_Properties & XRCameraFrameExifDataProperties.ExposureTime) != 0;
        bool hasShutterSpeedValue => (m_Properties & XRCameraFrameExifDataProperties.ShutterSpeedValue) != 0;
        bool hasExposureBiasValue => (m_Properties & XRCameraFrameExifDataProperties.ExposureBiasValue) != 0;
        bool hasFNumber => (m_Properties & XRCameraFrameExifDataProperties.FNumber) != 0;
        bool hasFocalLength => (m_Properties & XRCameraFrameExifDataProperties.FocalLength) != 0;
        bool hasFlash => (m_Properties & XRCameraFrameExifDataProperties.Flash) != 0;
        bool hasColorSpace => (m_Properties & XRCameraFrameExifDataProperties.ColorSpace) != 0;
        bool hasPhotographicSensitivity => (m_Properties & XRCameraFrameExifDataProperties.PhotographicSensitivity) != 0;
        bool hasMeteringMode => (m_Properties & XRCameraFrameExifDataProperties.MeteringMode) != 0;

        /// <summary>
        /// Points to a provider-specific data structure in unmanaged memory that you can use to access additional EXIF properties.
        /// Refer to your provider's documentation to learn how to use this pointer.
        /// </summary>
        public IntPtr nativePtr => m_NativePtr;

        /// <summary>
        /// Creates a <see cref="XRCameraFrameExifData"/>.
        /// </summary>
        /// <param name="nativePtr">The native pointer.</param>
        /// <param name="apertureValue">The lens aperture of the frame.</param>
        /// <param name="brightnessValue">The brightness of the frame.</param>
        /// <param name="exposureTime">The exposure Time of the frame.</param>
        /// <param name="shutterSpeedValue">The shutter speed of the frame.</param>
        /// <param name="exposureBiasValue">The exposure bias of the frame.</param>
        /// <param name="fNumber">The F number of the frame.</param>
        /// <param name="focalLength">The lens focal length of the frame.</param>
        /// <param name="flash">The flash status of the frame.</param>
        /// <param name="colorSpace">The color space of the frame.</param>
        /// <param name="photographicSensitivity">The photographicSensitivity of the frame.</param>
        /// <param name="meteringMode">The metering mode of the frame.</param>
        /// <param name="properties">The set of flags that indicates which properties are included in the EXIF data of the frame.</param>
        public XRCameraFrameExifData(
            IntPtr nativePtr,
            double apertureValue,
            double brightnessValue,
            double exposureTime,
            double shutterSpeedValue,
            float exposureBiasValue,
            float fNumber,
            float focalLength,
            short flash,
            XRCameraFrameExifDataColorSpace colorSpace,
            short photographicSensitivity,
            XRCameraFrameExifDataMeteringMode meteringMode,
            XRCameraFrameExifDataProperties properties)
        {
            m_NativePtr = nativePtr;
            m_ApertureValue = apertureValue;
            m_BrightnessValue = brightnessValue;
            m_ExposureTime = exposureTime;
            m_ShutterSpeedValue = shutterSpeedValue;
            m_ExposureBiasValue = exposureBiasValue;
            m_FNumber = fNumber;
            m_FocalLength = focalLength;
            m_Flash = flash;
            m_ColorSpace = colorSpace;
            m_PhotographicSensitivity = photographicSensitivity;
            m_MeteringMode = meteringMode;
            m_Properties = properties;
        }

        /// <summary>
        /// Get the lens aperture of the frame if possible.
        /// </summary>
        /// <param name="apertureValue">The lens aperture of the camera frame.</param>
        /// <returns><see langword="true"/> if the EXIF data contains the lens aperture of the frame. Otherwise, <see langword="false"/>.
        /// Equal to <see cref="hasApertureValue"/>.</returns>
        public bool TryGetApertureValue(out double apertureValue)
        {
            apertureValue = m_ApertureValue;
            return hasApertureValue;
        }

        /// <summary>
        /// Get the brightness of the frame if possible.
        /// </summary>
        /// <param name="brightnessValue">The brightness of the camera frame.</param>
        /// <returns><see langword="true"/> if the EXIF data contains the brightness of the frame. Otherwise, <see langword="false"/>.
        /// Equal to <see cref="hasBrightnessValue"/>.</returns>
        public bool TryGetBrightnessValue(out double brightnessValue)
        {
            brightnessValue = m_BrightnessValue;
            return hasBrightnessValue;
        }

        /// <summary>
        /// Get the exposure time of the frame if possible.
        /// </summary>
        /// <param name="exposureTime">The exposure time of the camera frame.</param>
        /// <returns><see langword="true"/> if the EXIF data contains the exposure time of the frame. Otherwise, <see langword="false"/>.
        /// Equal to <see cref="hasExposureTime"/>.</returns>
        public bool TryGetExposureTime(out double exposureTime)
        {
            exposureTime = m_ExposureTime;
            return hasExposureTime;
        }

        /// <summary>
        /// Get the shutter speed of the frame if possible.
        /// </summary>
        /// <param name="shutterSpeedValue">The shutter speed of the camera frame.</param>
        /// <returns><see langword="true"/> if the EXIF data contains the shutter speed of the frame. Otherwise, <see langword="false"/>.
        /// Equal to <see cref="hasShutterSpeedValue"/>.</returns>
        public bool TryGetShutterSpeedValue(out double shutterSpeedValue)
        {
            shutterSpeedValue = m_ShutterSpeedValue;
            return hasShutterSpeedValue;
        }

        /// <summary>
        /// Get the exposure bias of the frame if possible.
        /// </summary>
        /// <param name="exposureBiasValue">The exposure bias of the camera frame.</param>
        /// <returns><see langword="true"/> if the EXIF data contains the exposure bias of the frame. Otherwise, <see langword="false"/>.
        /// Equal to <see cref="hasExposureBiasValue"/>.</returns>
        public bool TryGetExposureBiasValue(out float exposureBiasValue)
        {
            exposureBiasValue = m_ExposureBiasValue;
            return hasExposureBiasValue;
        }

        /// <summary>
        /// Get the F number of the frame if possible.
        /// </summary>
        /// <param name="fNumber">The F number of the camera frame.</param>
        /// <returns><see langword="true"/> if the EXIF data contains the F number of the frame. Otherwise, <see langword="false"/>.
        /// Equal to <see cref="hasFNumber"/>.</returns>
        public bool TryGetFNumber(out float fNumber)
        {
            fNumber = m_FNumber;
            return hasFNumber;
        }

        /// <summary>
        /// Get the lens focal length of the frame if possible.
        /// </summary>
        /// <param name="focalLength">The lens focal length of the camera frame.</param>
        /// <returns><see langword="true"/> if the EXIF data contains the lens focal length of the frame. Otherwise, <see langword="false"/>.
        /// Equal to <see cref="hasFocalLength"/>.</returns>
        public bool TryGetFocalLength(out float focalLength)
        {
            focalLength = m_FocalLength;
            return hasFocalLength;
        }

        /// <summary>
        /// Get the flash status of the frame if possible.
        /// </summary>
        /// <param name="flash">The flash status of the camera frame.</param>
        /// <returns><see langword="true"/> if the EXIF data contains the flash status of the frame. Otherwise, <see langword="false"/>.
        /// Equal to <see cref="hasFlash"/>.</returns>
        public bool TryGetFlash(out short flash)
        {
            flash = m_Flash;
            return hasFlash;
        }

        /// <summary>
        /// Get the color space of the frame if possible.
        /// </summary>
        /// <param name="colorSpace">The color space of the camera frame.</param>
        /// <returns><see langword="true"/> if the EXIF data contains the color space of the frame. Otherwise, <see langword="false"/>.
        /// Equal to <see cref="hasColorSpace"/>.</returns>
        public bool TryGetColorSpace(out XRCameraFrameExifDataColorSpace colorSpace)
        {
            colorSpace = m_ColorSpace;
            return hasColorSpace;
        }

        /// <summary>
        /// Get the photographic sensitivity of the frame if possible.
        /// </summary>
        /// <param name="photographicSensitivity">The photographic sensitivity of the camera frame.</param>
        /// <returns><see langword="true"/> if the EXIF data contains the photographic sensitivity of the frame. Otherwise, <see langword="false"/>.
        /// Equal to <see cref="hasPhotographicSensitivity"/>.</returns>
        public bool TryGetPhotographicSensitivity(out short photographicSensitivity)
        {
            photographicSensitivity = m_PhotographicSensitivity;
            return hasPhotographicSensitivity;
        }

        /// <summary>
        /// Get the metering mode of the frame if possible.
        /// </summary>
        /// <param name="meteringMode">The metering mode of the camera frame.</param>
        /// <returns><see langword="true"/> if the EXIF data contains the metering mode of the frame. Otherwise, <see langword="false"/>.
        /// Equal to <see cref="hasMeteringMode"/>.</returns>
        public bool TryGetMeteringMode(out XRCameraFrameExifDataMeteringMode meteringMode)
        {
            meteringMode = m_MeteringMode;
            return hasMeteringMode;
        }

        /// <summary>
        /// Indicates whether any property was assigned a value.
        /// </summary>
        /// <value><see langword="true"/> if any property was assigned a value. Otherwise, <see langword="false"/>.</value>
        public bool hasAnyProperties => (m_Properties != XRCameraFrameExifDataProperties.None);

        /// <summary>
        /// Compares for equality.
        /// </summary>
        /// <param name="other">The other <see cref="XRCameraFrameExifData"/> to compare against.</param>
        /// <returns><see langword="true"/> if the <see cref="XRCameraFrameExifData"/> represents the same object.
        /// Otherwise, <see langword="false"/>.</returns>
        public bool Equals(XRCameraFrameExifData other)
        {
            return m_NativePtr.Equals(other.m_NativePtr) &&
                m_ApertureValue.Equals(other.m_ApertureValue) && 
                m_BrightnessValue.Equals(other.m_BrightnessValue) && 
                m_ExposureTime.Equals(other.m_ExposureTime) &&
                m_ShutterSpeedValue.Equals(other.m_ShutterSpeedValue) && 
                m_ExposureBiasValue.Equals(other.m_ExposureBiasValue) && 
                m_FNumber.Equals(other.m_FNumber) && 
                m_FocalLength.Equals(other.m_FocalLength) && 
                m_Flash.Equals(other.m_Flash) && 
                m_ColorSpace.Equals(other.m_ColorSpace) && 
                m_PhotographicSensitivity.Equals(other.m_PhotographicSensitivity) && 
                m_MeteringMode.Equals(other.m_MeteringMode) && 
                m_Properties == other.m_Properties;
        }

        /// <summary>
        /// Compares for equality.
        /// </summary>
        /// <param name="obj">An <c>object</c> to compare against.</param>
        /// <returns><see langword="true"/> if <paramref name="obj"/> is an <see cref="XRCameraFrameExifData"/> and
        /// <see cref="Equals(XRCameraFrameExifData)"/> is also <see langword="true"/>. Otherwise, <see langword="false"/>.</returns>
        public override bool Equals(System.Object obj)
        {
            return obj is XRCameraFrameExifData data && Equals(data);
        }

        /// <summary>
        /// Compares <paramref name="lhs"/> and <paramref name="rhs"/> for equality using <see cref="Equals(XRCameraFrameExifData)"/>.
        /// </summary>
        /// <param name="lhs">The left-hand-side <see cref="XRCameraFrameExifData"/> of the comparison.</param>
        /// <param name="rhs">The right-hand-side <see cref="XRCameraFrameExifData"/> of the comparison.</param>
        /// <returns><see langword="true"/> if <paramref name="lhs"/> compares equal to <paramref name="rhs"/>.
        /// Otherwise, <see langword="false"/>.</returns>
        public static bool operator ==(XRCameraFrameExifData lhs, XRCameraFrameExifData rhs)
        {
            return lhs.Equals(rhs);
        }

        /// <summary>
        /// Compares <paramref name="lhs"/> and <paramref name="rhs"/> for inequality using <see cref="Equals(XRCameraFrameExifData)"/>.
        /// </summary>
        /// <param name="lhs">The left-hand-side <see cref="XRCameraFrameExifData"/> of the comparison.</param>
        /// <param name="rhs">The right-hand-side <see cref="XRCameraFrameExifData"/> of the comparison.</param>
        /// <returns><see langword="false"/> if <paramref name="lhs"/> compares equal to <paramref name="rhs"/>.
        /// Otherwise, <see langword="true"/>.</returns>
        public static bool operator !=(XRCameraFrameExifData lhs, XRCameraFrameExifData rhs)
        {
            return !lhs.Equals(rhs);
        }

        /// <summary>
        /// Generates a hash code suitable for use in <c>HashSet</c> and <c>Dictionary</c>.
        /// </summary>
        /// <returns>A hash of this <see cref="XRCameraFrameExifData"/>.</returns>
        public override int GetHashCode()
        {
            int hashCode = 486187739;
            unchecked
            {
                hashCode = (hashCode * 486187739) + m_NativePtr.GetHashCode();
                hashCode = (hashCode * 486187739) + m_ApertureValue.GetHashCode();
                hashCode = (hashCode * 486187739) + m_BrightnessValue.GetHashCode();
                hashCode = (hashCode * 486187739) + m_ExposureTime.GetHashCode();
                hashCode = (hashCode * 486187739) + m_ShutterSpeedValue.GetHashCode();
                hashCode = (hashCode * 486187739) + m_ExposureBiasValue.GetHashCode();
                hashCode = (hashCode * 486187739) + m_FNumber.GetHashCode();
                hashCode = (hashCode * 486187739) + m_FocalLength.GetHashCode();
                hashCode = (hashCode * 486187739) + m_Flash.GetHashCode();
                hashCode = (hashCode * 486187739) + m_ColorSpace.GetHashCode();
                hashCode = (hashCode * 486187739) + m_PhotographicSensitivity.GetHashCode();
                hashCode = (hashCode * 486187739) + m_MeteringMode.GetHashCode();
                hashCode = (hashCode * 486187739) + ((int)m_Properties).GetHashCode();
            }
            return hashCode;
        }

        /// <summary>
        /// Generates a string representation of this <see cref="XRCameraFrameExifData"/> suitable for debugging purposes.
        /// </summary>
        /// <returns>A string representation of this <see cref="XRCameraFrameExifData"/>.</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine("{");
            sb.AppendLine($"  ApertureValue: {m_ApertureValue}");
            sb.AppendLine($"  BrightnessValue: {m_BrightnessValue}");
            sb.AppendLine($"  ExposureTime: {m_ExposureTime}");
            sb.AppendLine($"  ShutterSpeed: {m_ShutterSpeedValue}");
            sb.AppendLine($"  ExposureBiasValue: {m_ExposureBiasValue}");
            sb.AppendLine($"  FNumber: {m_FNumber}");
            sb.AppendLine($"  FocalLength : {m_FocalLength}");
            sb.AppendLine($"  Flash: {m_Flash}");
            sb.AppendLine($"  ColorSpace: {m_ColorSpace}");
            sb.AppendLine($"  PhotographicSensitivity: {m_PhotographicSensitivity}");
            sb.AppendLine($"  MeteringMode: {m_MeteringMode}");
            sb.AppendLine("}");
            return sb.ToString();
        }
    }
}
