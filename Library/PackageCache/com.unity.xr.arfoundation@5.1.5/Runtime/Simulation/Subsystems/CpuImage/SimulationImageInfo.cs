using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.XR.CoreUtils;
using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.Simulation
{
    class SimulationImageInfo : IDisposable
    {
        [StructLayout(LayoutKind.Sequential)]
        public readonly struct Cinfo
        {
            public Cinfo(SimulationImageInfo inputImageData)
            {
                width = inputImageData.textureDimensions.x;
                height = inputImageData.textureDimensions.y;
                planeCount = 1;
                format = inputImageData.textureFormat.ToXRCpuImageFormat();
                unsafe
                {
                    info0 = new XRCpuImage.Plane.Cinfo(
                        new IntPtr(inputImageData.ImageData.GetUnsafeReadOnlyPtr()),
                        inputImageData.textureDataLength,
                        GetRowStride(width, format),
                        GetPixelStride(format));
                }
                info1 = default;
                info2 = default;
            }

            public override string ToString()
            {
                var stringBuilder = new System.Text.StringBuilder();
                stringBuilder.AppendLine($"width: {width}");
                stringBuilder.AppendLine($"height: {height}");
                stringBuilder.AppendLine($"planeCount: {planeCount}");
                stringBuilder.AppendLine($"format: {format}");
                stringBuilder.AppendLine($"info0: {info0}");
                stringBuilder.AppendLine($"info1: {info1}");
                stringBuilder.AppendLine($"info2: {info2}");
                return stringBuilder.ToString();
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            static int GetPixelStride(XRCpuImage.Format format)
            {
                switch (format)
                {
                    case XRCpuImage.Format.OneComponent8:
                        return 1;
                    case XRCpuImage.Format.DepthUint16:
                        return 2;
                    case XRCpuImage.Format.RGB24:
                        return 3;
                    case XRCpuImage.Format.OneComponent32:
                    case XRCpuImage.Format.ARGB32:
                    case XRCpuImage.Format.RGBA32:
                    case XRCpuImage.Format.BGRA32:
                    case XRCpuImage.Format.DepthFloat32:
                        return 4;
                    case XRCpuImage.Format.AndroidYuv420_888:
                    case XRCpuImage.Format.IosYpCbCr420_8BiPlanarFullRange:
                    case XRCpuImage.Format.Unknown:
                    default:
                        throw new ArgumentException($"Unsupported format {format}");
                }
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            static int GetRowStride(int width, XRCpuImage.Format format) => width * GetPixelStride(format);

            public readonly int width;
            public readonly int height;
            public readonly int planeCount;
            public readonly XRCpuImage.Format format;
            public readonly XRCpuImage.Plane.Cinfo info0;
            public readonly XRCpuImage.Plane.Cinfo info1;
            public readonly XRCpuImage.Plane.Cinfo info2;
        }

        NativeArray<byte> m_ImageData;
        public NativeArray<byte>.ReadOnly ImageData => m_ImageData.AsReadOnly();

        public int textureDataLength { get; private set; }

        public Vector2Int textureDimensions { get; private set; }

        public TextureFormat textureFormat { get; private set; }

        public long timestampNs { get; private set; }

        public SimulationImageInfo(ref CameraTextureProvider.TextureReadbackEventArgs args)
        {
            UpdateInfo(ref args);
        }

        public void UpdateInfo(ref CameraTextureProvider.TextureReadbackEventArgs args)
        {
            textureDataLength = args.textureData.Length;
            NativeArrayUtils.EnsureCapacity(ref m_ImageData, args.textureData.Length, Allocator.Persistent);
            m_ImageData.Slice(0, textureDataLength).CopyFrom(args.textureData);

            textureDimensions = args.textureDimensions;
            textureFormat = args.textureFormat;
            timestampNs = args.timestampNs;
        }

        public void Dispose()
        {
            if (m_ImageData.IsCreated)
            {
                m_ImageData.Dispose();
            }
        }

        public Cinfo GetCinfo()
        {
            return new Cinfo(this);
        }
    }
}
