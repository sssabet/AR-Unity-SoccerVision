using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.PlayerLoop;
using UnityEngine.UIElements;
using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.Simulation
{
    class SimulationXRCpuImageApi : XRCpuImage.Api
    {
        static readonly HashSet<TextureFormat> k_SingleChannelInputSupportedConversions = new()
        {
            TextureFormat.Alpha8,
            TextureFormat.R8,
            TextureFormat.R16,
            TextureFormat.RFloat,
        };

        static readonly HashSet<TextureFormat> k_MultiChannelColorInputSupportedConversions = new()
        {
            TextureFormat.RGBA32,
            TextureFormat.ARGB32,
            TextureFormat.BGRA32,
            TextureFormat.RGB24,
        };

        static readonly Dictionary<XRCpuImage.Format, HashSet<TextureFormat>> k_SupportedFormatConversionMap = new()
        {
            { XRCpuImage.Format.DepthFloat32, k_SingleChannelInputSupportedConversions },
            { XRCpuImage.Format.DepthUint16, k_SingleChannelInputSupportedConversions },
            { XRCpuImage.Format.OneComponent32, k_SingleChannelInputSupportedConversions },
            { XRCpuImage.Format.ARGB32, k_MultiChannelColorInputSupportedConversions },
            { XRCpuImage.Format.RGBA32, k_MultiChannelColorInputSupportedConversions },
            { XRCpuImage.Format.BGRA32, k_MultiChannelColorInputSupportedConversions },
            { XRCpuImage.Format.RGB24, k_MultiChannelColorInputSupportedConversions }
        };

        /// <summary>
        /// The type of image to acquire. Used by <see cref="SimulationXRCpuImageApi.TryAcquireLatestImage"/>.
        /// </summary>
        public enum ImageType
        {
            Camera = 0,
        }

        /// <summary>
        /// The shared API instance.
        /// </summary>
        public static SimulationXRCpuImageApi instance { get; } = new();

        Dictionary<ImageType, SimulationImageInfo> m_LatestImageCache = new();
        Dictionary<IntPtr, SimulationImageInfo> m_RetainedImageData = new();

        SimulationXRCpuImageApi() => RegisterPlatformInterface();

        ~SimulationXRCpuImageApi()
        {
            foreach (var kvp in m_RetainedImageData)
            {
                kvp.Value.Dispose();
            }
            m_RetainedImageData.Clear();

            foreach (var kvp in m_LatestImageCache)
            {
                kvp.Value.Dispose();
            }
            m_LatestImageCache.Clear();
        }

        /// <summary>
        /// Tries to acquire the latest image of type <paramref name="imageType"/>.
        /// </summary>
        /// <param name="imageType">The type of image to acquire.</param>
        /// <param name="cinfo">On success, populated with construction information for an
        ///     <see cref="XRCpuImage"/>.</param>
        /// <returns>Returns <see langword="true"/> if the latest image of type <paramref name="imageType"/> was successfully acquired.
        /// Otherwise, returns <see langword="false"/>.</returns>
        public static bool TryAcquireLatestImage(ImageType imageType, out XRCpuImage.Cinfo cinfo)
        {
            return NativeApi.TryAcquireLatestImageOfType(imageType, out cinfo);
        }

        /// <summary>
        /// Get the status of an existing asynchronous conversion request.
        /// </summary>
        /// <param name="requestId">The unique identifier associated with a request.</param>
        /// <returns>The state of the request.</returns>
        /// <seealso cref="ConvertAsync(int, XRCpuImage.ConversionParams)"/>
        public override XRCpuImage.AsyncConversionStatus GetAsyncRequestStatus(int requestId)
        {
            return NativeApi.GetAsyncRequestStatus(requestId);
        }

        /// <summary>
        /// Dispose an existing native image identified by <paramref name="nativeHandle"/>.
        /// </summary>
        /// <param name="nativeHandle">A unique identifier for this camera image.</param>
        /// <seealso cref="Provider.TryAcquireLatestCpuImage"/>
        public override void DisposeImage(int nativeHandle)
        {
            NativeApi.DisposeImage(nativeHandle);
        }

        /// <summary>
        /// Dispose an existing async conversion request.
        /// </summary>
        /// <param name="requestId">A unique identifier for the request.</param>
        /// <seealso cref="ConvertAsync(int, XRCpuImage.ConversionParams)"/>
        public override void DisposeAsyncRequest(int requestId)
        {
            NativeApi.DisposeAsyncRequest(requestId);
        }

        /// <summary>
        /// Get information about an image plane from a native image handle by index.
        /// </summary>
        /// <param name="nativeHandle">A unique identifier for this camera image.</param>
        /// <param name="planeIndex">The index of the plane to get.</param>
        /// <param name="planeCinfo">The returned camera plane information if successful.</param>
        /// <returns>
        /// <see langword="true"/> if the image plane was acquired. Otherwise, <c>false</c>.
        /// </returns>
        /// <seealso cref="Provider.TryAcquireLatestCpuImage"/>
        public override bool TryGetPlane(
            int nativeHandle,
            int planeIndex,
            out XRCpuImage.Plane.Cinfo planeCinfo)
        {
            return NativeApi.TryGetPlane(nativeHandle, planeIndex, out planeCinfo);
        }

        /// <summary>
        /// Determine whether a native image handle returned by <see cref="Provider.TryAcquireLatestCpuImage"/> is currently
        /// valid. An image may become invalid if it has been disposed.
        /// </summary>
        /// <remarks>
        /// If a handle is valid, <see cref="TryConvert"/> and <see cref="TryGetConvertedDataSize"/> should not fail.
        /// </remarks>
        /// <param name="nativeHandle">A unique identifier for the camera image in question.</param>
        /// <returns><see langword="true"/> if <paramref name="nativeHandle"/> is a valid handle. Otherwise, <see langword="false"/>.</returns>
        /// <seealso cref="DisposeImage"/>
        public override bool NativeHandleValid(int nativeHandle)
        {
            return NativeApi.HandleValid(nativeHandle);
        }

        /// <summary>
        /// Get the number of bytes required to store an image with the given dimensions and <see cref="UnityEngine.TextureFormat"/>.
        /// </summary>
        /// <param name="nativeHandle">A unique identifier for the camera image to convert.</param>
        /// <param name="dimensions">The dimensions of the output image.</param>
        /// <param name="format">The <see cref="UnityEngine.TextureFormat"/> for the image.</param>
        /// <param name="size">The number of bytes required to store the converted image.</param>
        /// <returns><see langword="true"/> if the output <paramref name="size"/> was set. Otherwise, <see langword="false"/>.</returns>
        public override bool TryGetConvertedDataSize(
            int nativeHandle,
            Vector2Int dimensions,
            TextureFormat format,
            out int size)
        {
            return NativeApi.TryGetConvertedDataSize(nativeHandle, dimensions, format, out size);
        }

        /// <summary>
        /// Convert the image with handle <paramref name="nativeHandle"/> using the provided
        /// <paramref cref="conversionParams"/>.
        /// </summary>
        /// <param name="nativeHandle">A unique identifier for the camera image to convert.</param>
        /// <param name="conversionParams">The parameters to use during the conversion.</param>
        /// <param name="destinationBuffer">A buffer to write the converted image to.</param>
        /// <param name="bufferLength">The number of bytes available in the buffer.</param>
        /// <returns>
        /// <see langword="true"/> if the image was converted and stored in <paramref name="destinationBuffer"/>. Otherwise, <see langword="false"/>.
        /// </returns>
        public override bool TryConvert(int nativeHandle, XRCpuImage.ConversionParams conversionParams,
            IntPtr destinationBuffer, int bufferLength)
        {
            return NativeApi.TryConvert(nativeHandle, conversionParams, destinationBuffer, bufferLength);
        }

        /// <summary>
        /// Create an asynchronous request to convert a camera image, similar to <see cref="TryConvert"/> except
        /// the conversion should happen on a thread other than the calling (main) thread.
        /// </summary>
        /// <param name="nativeHandle">A unique identifier for the camera image to convert.</param>
        /// <param name="conversionParams">The parameters to use during the conversion.</param>
        /// <returns>A unique identifier for this request.</returns>
        public override int ConvertAsync(int nativeHandle, XRCpuImage.ConversionParams conversionParams)
        {
            return NativeApi.CreateAsyncConversionRequest(nativeHandle, conversionParams);
        }

        /// <summary>
        /// Get a pointer to the image data from a completed asynchronous request. This method should only succeed
        /// if <see cref="GetAsyncRequestStatus"/> returns <see cref="XRCpuImage.AsyncConversionStatus.Ready"/>.
        /// </summary>
        /// <param name="requestId">The unique identifier associated with a request.</param>
        /// <param name="dataPtr">A pointer to the native buffer containing the data.</param>
        /// <param name="dataLength">The number of bytes in <paramref name="dataPtr"/>.</param>
        /// <returns><see langword="true"/> if <paramref name="dataPtr"/> and <paramref name="dataLength"/> were set and point
        ///  to the image data. Otherwise, <see langword="false"/>.</returns>
        public override bool TryGetAsyncRequestData(int requestId, out IntPtr dataPtr, out int dataLength)
        {
            return NativeApi.TryGetAsyncRequestData(requestId, out dataPtr, out dataLength);
        }

        /// <summary>
        /// Similar to <see cref="ConvertAsync(int, XRCpuImage.ConversionParams)"/> but takes a delegate to
        /// invoke when the request is complete, rather than returning a request id.
        /// </summary>
        /// <remarks>
        /// If the first parameter to <paramref name="callback"/> is
        /// <see cref="XRCpuImage.AsyncConversionStatus.Ready"/> then the <c>dataPtr</c> parameter must be valid
        /// for the duration of the invocation. The data may be destroyed immediately upon return. The
        /// <paramref name="context"/> parameter must be passed back to the <paramref name="callback"/>.
        /// </remarks>
        /// <param name="nativeHandle">A unique identifier for the camera image to convert.</param>
        /// <param name="conversionParams">The parameters to use during the conversion.</param>
        /// <param name="callback">A delegate which must be invoked when the request is complete, whether the
        /// conversion was successful or not.</param>
        /// <param name="context">A native pointer which must be passed back unaltered to
        /// <paramref name="callback"/>.</param>
        public override void ConvertAsync(
            int nativeHandle,
            XRCpuImage.ConversionParams conversionParams,
            OnImageRequestCompleteDelegate callback,
            IntPtr context)
        {
            NativeApi.CreateAsyncConversionRequestWithCallback(nativeHandle, conversionParams, callback, context);
        }

        /// <summary>
        /// Determines whether a given
        /// <see cref="UnityEngine.TextureFormat"/> is supported for image
        /// conversion.
        /// </summary>
        /// <param name="image">The <see cref="XRCpuImage"/> to convert.</param>
        /// <param name="format">The <see cref="UnityEngine.TextureFormat"/> to test.</param>
        /// <returns><see langword="true"/> if <paramref name="image"/> can be converted to <paramref name="format"/>.
        /// Otherwise, <see langword="false"/>.</returns>
        public override bool FormatSupported(XRCpuImage image, TextureFormat format)
        {
            return k_SupportedFormatConversionMap.TryGetValue(image.format, out var supportedConversionsFormats)
                && supportedConversionsFormats.Contains(format);
        }

        internal static void OnCameraDataReceived(CameraTextureProvider.TextureReadbackEventArgs textureReadbackEventArgs)
        {
            lock (instance)
            {
                if (instance.m_LatestImageCache.TryGetValue(textureReadbackEventArgs.imageType, out var cachedImageInfo))
                {
                    cachedImageInfo.UpdateInfo(ref textureReadbackEventArgs);
                }
                else
                {
                    cachedImageInfo = new SimulationImageInfo(ref textureReadbackEventArgs);
                    instance.m_LatestImageCache.Add(textureReadbackEventArgs.imageType, cachedImageInfo);
                }
            }
        }

        static bool TryGetImageDataSynchronized(IntPtr nativeHandle, out SimulationImageInfo.Cinfo imageData)
        {
            imageData = default;
            lock(instance)
            {
                if (instance.m_RetainedImageData.TryGetValue(nativeHandle, out var imageDataBuffer))
                {
                    imageData = imageDataBuffer.GetCinfo();
                    return true;
                }
            }

            return false;
        }

        static IntPtr AcquireImageSynchronized(ImageType imageType, out double timestampInSeconds)
        {
            var nativeHandle = IntPtr.Zero;
            timestampInSeconds = 0;
            lock (instance)
            {
                if (instance.m_LatestImageCache.Remove(imageType, out var latestImageInfo))
                {
                    nativeHandle = new IntPtr(latestImageInfo.GetHashCode());
                    instance.m_RetainedImageData.Add(nativeHandle, latestImageInfo);
                    timestampInSeconds = latestImageInfo.timestampNs / 1e9;
                }
            }

            return nativeHandle;
        }

        static void ReleaseImageSynchronized(IntPtr nativeHandle)
        {
            lock (instance)
            {
                if (instance.m_RetainedImageData.Remove(nativeHandle, out var imageDataBuffer))
                {
                    imageDataBuffer.Dispose();
                }
            }
        }

        static void Log(IntPtr messagePtr)
        {
            if (messagePtr != IntPtr.Zero)
            {
                try
                {
                    Debug.Log(Marshal.PtrToStringAnsi(messagePtr));
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
            }
        }

        static int GetRetainedImageCount()
        {
            lock (instance)
            {
                return instance.m_RetainedImageData.Count;
            }
        }

        static void RegisterPlatformInterface()
        {
            SimulationPlatformInterface.Delegates delegates = new()
            {
                TryGetImageData = TryGetImageDataSynchronized,
                AcquireImage = AcquireImageSynchronized,
                ReleaseImage = ReleaseImageSynchronized,
                Log = Log,
                GetAcquiredImageCount = GetRetainedImageCount,
            };
            var platformInterface = new SimulationPlatformInterface(delegates);
            NativeApi.RegisterPlatformInterface(platformInterface);
        }


        static class NativeApi
        {
            [DllImport("XRSimulationSubsystem", EntryPoint = "XRSimulationSubsystem_CpuImage_TryAcquireLatestImage")]
            public static extern bool TryAcquireLatestImageOfType(ImageType imageType, out XRCpuImage.Cinfo cameraImageCinfo);

            [DllImport("XRSimulationSubsystem", EntryPoint = "XRSimulationSubsystem_CpuImage_GetAsyncRequestStatus")]
            public static extern XRCpuImage.AsyncConversionStatus GetAsyncRequestStatus(int requestId);

            [DllImport("XRSimulationSubsystem", EntryPoint = "XRSimulationSubsystem_CpuImage_DisposeImage")]
            public static extern void DisposeImage(int nativeHandle);

            [DllImport("XRSimulationSubsystem", EntryPoint = "XRSimulationSubsystem_CpuImage_DisposeAsyncRequest")]
            public static extern void DisposeAsyncRequest(int requestHandle);

            [DllImport("XRSimulationSubsystem", EntryPoint = "XRSimulationSubsystem_CpuImage_TryGetPlane")]
            public static extern bool TryGetPlane(
                int nativeHandle, int planeIndex,
                out XRCpuImage.Plane.Cinfo planeCinfo);

            [DllImport("XRSimulationSubsystem", EntryPoint = "XRSimulationSubsystem_CpuImage_HandleValid")]
            public static extern bool HandleValid(int nativeHandle);

            [DllImport("XRSimulationSubsystem", EntryPoint = "XRSimulationSubsystem_CpuImage_TryGetConvertedDataSize")]
            public static extern bool TryGetConvertedDataSize(
                int nativeHandle,
                Vector2Int dimensions,
                TextureFormat format,
                out int size);

            [DllImport("XRSimulationSubsystem", EntryPoint = "XRSimulationSubsystem_CpuImage_TryConvert")]
            public static extern bool TryConvert(
                int nativeHandle,
                XRCpuImage.ConversionParams conversionParams,
                IntPtr buffer,
                int bufferLength);

            [DllImport("XRSimulationSubsystem", EntryPoint = "XRSimulationSubsystem_CpuImage_CreateAsyncConversionRequest")]
            public static extern int CreateAsyncConversionRequest(
                int nativeHandle,
                XRCpuImage.ConversionParams conversionParams);

            [DllImport("XRSimulationSubsystem", EntryPoint = "XRSimulationSubsystem_CpuImage_TryGetAsyncRequestData")]
            public static extern bool TryGetAsyncRequestData(
                int requestHandle,
                out IntPtr dataPtr,
                out int dataLength);

            [DllImport("XRSimulationSubsystem", EntryPoint = "XRSimulationSubsystem_CpuImage_CreateAsyncConversionRequestWithCallback")]
            public static extern void CreateAsyncConversionRequestWithCallback(
                int nativeHandle,
                XRCpuImage.ConversionParams conversionParams,
                OnImageRequestCompleteDelegate callback,
                IntPtr context);

            [DllImport("XRSimulationSubsystem", EntryPoint = "XRSimulationSubsystem_CpuImage_RegisterPlatformInterface")]
            public static extern void RegisterPlatformInterface(in SimulationPlatformInterface apiInterface);
        }
    }
}
