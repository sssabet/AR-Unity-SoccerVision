using System;

namespace UnityEngine.XR.ARSubsystems
{
    public partial struct XRCpuImage
    {
        /// <summary>
        /// Formats used by the raw <see cref="XRCpuImage"/> data. See <see cref="XRCpuImage.format"/>.
        /// </summary>
        public enum Format
        {
            /// <summary>
            /// The format is unknown or could not be determined.
            /// </summary>
            Unknown = 0,

            /// <summary>
            /// <para>Three-Plane YUV 420 format commonly used by Android. See
            /// <a href="https://developer.android.com/ndk/reference/group/media#group___media_1gga9c3dace30485a0f28163a882a5d65a19aea9797f9b5db5d26a2055a43d8491890">
            /// AIMAGE_FORMAT_YUV_420_888</a>.</para>
            /// <para>This format consists of three image planes. The first is the Y (luminocity) plane, with 8 bits per
            /// pixel. The second and third are the U and V (chromaticity) planes, respectively. Each 2x2 block of pixels
            /// share the same chromaticity value, so a given (x, y) pixel's chromaticity value is given by
            /// <code>
            /// u = UPlane[(y / 2) * rowStride + (x / 2) * pixelStride];
            /// v = VPlane[(y / 2) * rowStride + (x / 2) * pixelStride];
            /// </code></para>
            /// </summary>
            AndroidYuv420_888 = 1,

            /// <summary>
            /// <para>Bi-Planar Component Y'CbCr 8-bit 4:2:0, full-range (luma=[0,255] chroma=[1,255]) commonly used by
            /// iOS. See
            /// <a href="https://developer.apple.com/documentation/corevideo/1563591-pixel_format_identifiers/kcvpixelformattype_420ypcbcr8biplanarfullrange">
            /// kCVPixelFormatType_420YpCbCr8BiPlanarFullRange</a>.</para>
            /// <para>This format consists of two image planes. The first is the Y (luminosity) plane, with 8 bits per
            /// pixel. The second plane is the UV (chromaticity) plane. The U and V chromaticity values are interleaved
            /// (u0, v0, u1, v1, etc.). Each 2x2 block of pixels share the same chromaticity values, so a given (x, y)
            /// pixel's chromaticity value is given by
            /// <code>
            /// u = UvPlane[(y / 2) * rowStride + (x / 2) * pixelStride];
            /// v = UvPlane[(y / 2) * rowStride + (x / 2) * pixelStride + 1];
            /// </code>
            /// pixelStride is always 2 for this format, so this can be optimized to
            /// <code>
            /// u = UvPlane[(y >> 1) * rowStride + x &amp; ~1];
            /// v = UvPlane[(y >> 1) * rowStride + x | 1];
            /// </code></para>
            /// </summary>
            IosYpCbCr420_8BiPlanarFullRange = 2,

            /// <summary>
            /// A single channel image format with 8 bits per pixel.
            /// </summary>
            OneComponent8 = 3,

            /// <summary>
            /// IEEE754-2008 binary32 float, describing the depth (distance to an object) in meters
            /// </summary>
            DepthFloat32 = 4,

            /// <summary>
            /// 16-bit unsigned integer, describing the depth (distance to an object) in millimeters.
            /// </summary>
            DepthUint16 = 5,

            /// <summary>
            /// Single channel image format with 32 bits per pixel.
            /// </summary>
            OneComponent32 = 6,

            /// <summary>
            /// 4 channel image with 8 bits per channel, describing the color in ARGB order.
            /// </summary>
            ARGB32 = 7,

            /// <summary>
            /// 4 channel image with 8 bits per channel, describing the color in RGBA order.
            /// </summary>
            RGBA32 = 8,

            /// <summary>
            /// 4 channel image with 8 bits per channel, describing the color in BGRA order.
            /// </summary>
            BGRA32 = 9,

            /// <summary>
            /// 3 8-bit unsigned integer channels, describing the color in RGB order.
            /// </summary>
            RGB24 = 10,
        }
    }

    /// <summary>
    /// Extensions to convert between <see cref="UnityEngine.TextureFormat"/> and <see cref="XRCpuImage.Format"/>.
    /// </summary>
    public static class XRCpuImageFormatExtensions
    {
        /// <summary>
        /// Attempts to convert an <see cref="XRCpuImage.Format"/> to a <see cref="UnityEngine.TextureFormat"/>.
        /// </summary>
        /// <param name="this">Defines an extension of <see cref="XRCpuImage.Format"/>.</param>
        /// <returns>
        /// Returns a texture format that matches <paramref name="this"/> if possible. Returns 0 if there
        /// is no matching texture format.
        /// </returns>
        public static TextureFormat AsTextureFormat(this XRCpuImage.Format @this)
        {
            return @this switch
            {
                XRCpuImage.Format.OneComponent8 => TextureFormat.R8,
                XRCpuImage.Format.DepthFloat32 => TextureFormat.RFloat,
                XRCpuImage.Format.DepthUint16 => TextureFormat.RFloat,
                XRCpuImage.Format.RGBA32 => TextureFormat.RGBA32,
                XRCpuImage.Format.BGRA32 => TextureFormat.BGRA32,
                XRCpuImage.Format.RGB24 => TextureFormat.RGB24,
                _ => 0
            };
        }

        /// <summary>
        /// Attempts to convert a <see cref="UnityEngine.TextureFormat"/> to an <see cref="XRCpuImage.Format"/>.
        /// </summary>
        /// <param name="this">Defines an extension of <see cref="UnityEngine.TextureFormat"/>.</param>
        /// <returns>
        /// Returns a <see cref="XRCpuImage.Format"/> that matches <paramref name="this"/> if possible. Returns
        /// <see cref="XRCpuImage.Format.Unknown"/> if there is no matching <see cref="XRCpuImage.Format"/>.
        /// </returns>
        /// <remarks>
        /// For some formats, there may be multiple <see cref="XRCpuImage.Format"/>s that match the format. In this case
        /// this method will return a generalized format. For example, <see cref="UnityEngine.TextureFormat.RFloat"/> will return
        /// <see cref="XRCpuImage.Format.OneComponent32"/> instead of <see cref="XRCpuImage.Format.DepthFloat32"/> as it's
        /// possible that the single channel is not depth. In these cases, you should also check the image type.
        /// </remarks>
        public static XRCpuImage.Format ToXRCpuImageFormat(this TextureFormat @this)
        {
            return @this switch {
                TextureFormat.R8 => XRCpuImage.Format.OneComponent8,
                TextureFormat.RFloat => XRCpuImage.Format.OneComponent32,
                TextureFormat.ARGB32 => XRCpuImage.Format.ARGB32,
                TextureFormat.RGBA32 => XRCpuImage.Format.RGBA32,
                TextureFormat.BGRA32 => XRCpuImage.Format.BGRA32,
                TextureFormat.RGB24 => XRCpuImage.Format.RGB24,
                _ => XRCpuImage.Format.Unknown
            };
        }
    }
}
