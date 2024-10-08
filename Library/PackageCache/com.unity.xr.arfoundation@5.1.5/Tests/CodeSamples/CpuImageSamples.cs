using System;
using System.Collections;
using NUnit.Framework;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.Rendering;
using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.ARFoundation
{
    [TestFixture]
    class CpuImageSamples
    {
        ARCameraManager m_CameraManager;
        ARCameraBackground m_ARCameraBackground;
        RenderTexture m_RenderTexture;

        void CommandBufferBlit()
        {
            #region GPU_Blit
            // Create a new command buffer
            var commandBuffer = new CommandBuffer();
            commandBuffer.name = "AR Camera Background Blit Pass";

            // Get a reference to the AR Camera Background's main texture
            // We will copy this texture into our chosen render texture
            var texture = !m_ARCameraBackground.material.HasProperty("_MainTex") ?
                null : m_ARCameraBackground.material.GetTexture("_MainTex");

            // Save references to the active render target before we overwrite it
            var colorBuffer = Graphics.activeColorBuffer;
            var depthBuffer = Graphics.activeDepthBuffer;

            // Set Unity's render target to our render texture
            Graphics.SetRenderTarget(m_RenderTexture);

            // Clear the render target before we render new pixels into it
            commandBuffer.ClearRenderTarget(true, false, Color.clear);

            // Blit the AR Camera Background into the render target
            commandBuffer.Blit(
                texture,
                BuiltinRenderTextureType.CurrentActive,
                m_ARCameraBackground.material);

            // Execute the command buffer
            Graphics.ExecuteCommandBuffer(commandBuffer);

            // Set Unity's render target back to its previous value
            Graphics.SetRenderTarget(colorBuffer, depthBuffer);
            #endregion
        }

        void SynchronousConversion()
        {
            #region Synchronous_Conversion
            // Acquire an XRCpuImage
            if (!m_CameraManager.TryAcquireLatestCpuImage(out XRCpuImage image))
                return;

            // Set up our conversion params
            var conversionParams = new XRCpuImage.ConversionParams
            {
                // Convert the entire image
                inputRect = new RectInt(0, 0, image.width, image.height),

                // Output at full resolution
                outputDimensions = new Vector2Int(image.width, image.height),

                // Convert to RGBA format
                outputFormat = TextureFormat.RGBA32,

                // Flip across the vertical axis (mirror image)
                transformation = XRCpuImage.Transformation.MirrorY
            };

            // Create a Texture2D to store the converted image
            var texture = new Texture2D(image.width, image.height, TextureFormat.RGBA32, false);

            // Texture2D allows us write directly to the raw texture data as an optimization
            var rawTextureData = texture.GetRawTextureData<byte>();
            try
            {
                unsafe
                {
                    // Synchronously convert to the desired TextureFormat
                    image.Convert(
                        conversionParams,
                        new IntPtr(rawTextureData.GetUnsafePtr()),
                        rawTextureData.Length);
                }
            }
            finally
            {
                // Dispose the XRCpuImage after we're finished to prevent any memory leaks
                image.Dispose();
            }

            // Apply the converted pixel data to our texture
            texture.Apply();
            #endregion
        }

        #region Asynchronous_Conversion
        void AsynchronousConversion()
        {
            // Acquire an XRCpuImage
            if (m_CameraManager.TryAcquireLatestCpuImage(out XRCpuImage image))
            {
                // If successful, launch an asynchronous conversion coroutine
                StartCoroutine(ConvertImageAsync(image));

                // It is safe to dispose the image before the async operation completes
                image.Dispose();
            }
        }

        IEnumerator ConvertImageAsync(XRCpuImage image)
        {
            // Create the async conversion request
            var request = image.ConvertAsync(new XRCpuImage.ConversionParams
            {
                // Use the full image
                inputRect = new RectInt(0, 0, image.width, image.height),

                // Optionally downsample by 2
                outputDimensions = new Vector2Int(image.width / 2, image.height / 2),

                // Output an RGB color image format
                outputFormat = TextureFormat.RGB24,

                // Flip across the Y axis
                transformation = XRCpuImage.Transformation.MirrorY
            });

            // Wait for the conversion to complete
            while (!request.status.IsDone())
                yield return null;

            // Check status to see if the conversion completed successfully
            if (request.status != XRCpuImage.AsyncConversionStatus.Ready)
            {
                // Something went wrong
                Debug.LogErrorFormat("Request failed with status {0}", request.status);

                // Dispose even if there is an error
                request.Dispose();
                yield break;
            }

            // Image data is ready. Let's apply it to a Texture2D
            var rawData = request.GetData<byte>();

            // Create a texture
            var texture = new Texture2D(
                request.conversionParams.outputDimensions.x,
                request.conversionParams.outputDimensions.y,
                request.conversionParams.outputFormat,
                false);

            // Copy the image data into the texture
            texture.LoadRawTextureData(rawData);
            texture.Apply();

            // Dispose the request including raw data
            request.Dispose();
        }
        #endregion

        #region Asynchronous_Conversion_With_Delegate
        public void GetImageAsync()
        {
            // Acquire an XRCpuImage
            if (m_CameraManager.TryAcquireLatestCpuImage(out XRCpuImage image))
            {
                // Perform async conversion
                image.ConvertAsync(new XRCpuImage.ConversionParams
                {
                    // Get the full image
                    inputRect = new RectInt(0, 0, image.width, image.height),

                    // Downsample by 2
                    outputDimensions = new Vector2Int(image.width / 2, image.height / 2),

                    // Color image format
                    outputFormat = TextureFormat.RGB24,

                    // Flip across the Y axis
                    transformation = XRCpuImage.Transformation.MirrorY

                    // Call ProcessImage when the async operation completes
                }, ProcessImage);

                // It is safe to dispose the image before the async operation completes
                image.Dispose();
            }
        }

        void ProcessImage(
            XRCpuImage.AsyncConversionStatus status,
            XRCpuImage.ConversionParams conversionParams,
            NativeArray<byte> data)
        {
            if (status != XRCpuImage.AsyncConversionStatus.Ready)
            {
                Debug.LogErrorFormat("Async request failed with status {0}", status);
                return;
            }

            // Copy to a Texture2D, pass to a computer vision algorithm, etc
            DoSomethingWithImageData(data);

            // Data is destroyed upon return. No need to dispose
        }
        #endregion

        static void StartCoroutine(IEnumerator coroutine)
        {
            // In a future version of this test we could set up an XR Origin and AR Session, and change this method to
            // start the given coroutine on some GameObject.

            // For now the purpose of these tests is simply to ensure that sample code in docs successfully compiles.
        }

        static void DoSomethingWithImageData(NativeArray<byte> data)
        {
        }
    }
}
