---
uid: arfoundation-image-capture
---
# Image capture

Your app can access images captured by the device camera if the following conditions are met:

* Device platform supports camera feature
* User has accepted any required camera permissions
* Camera feature is enabled, for example [ARCameraManager](xref:arfoundation-camera-components#ar-camera-manager-component) is active and enabled

The method you choose to access device camera images depends on how you intend to process the image. There are tradeoffs to either a GPU-based or a CPU-based approach.

## Understand GPU vs CPU

There are two ways to access device camera images:

* **GPU:** GPU offers best performance if you will simply render the image or process it with a shader.
* **CPU:** Use CPU if you will access the image's pixel data in a C# script. This is more resource-intensive, but allows you to perform operations such as save the image to a file or pass it to a computer vision system.

# Access images via GPU

Camera Textures are usually [external Textures](https://docs.unity3d.com/ScriptReference/Texture2D.CreateExternalTexture.html) that do not last beyond a frame boundary. You can copy the Camera image to a [Render Texture](https://docs.unity3d.com/Manual/class-RenderTexture.html) to persist it or process it further.

The following code sets up a [command buffer](https://docs.unity3d.com/ScriptReference/Rendering.CommandBuffer.html) that performs a GPU copy or ["blit"](https://docs.unity3d.com/ScriptReference/Rendering.CommandBuffer.Blit.html) to a Render Texture of your choice immediately. The code clears the the render texture before the copy by calling [ClearRenderTarget](https://docs.unity3d.com/ScriptReference/Rendering.CommandBuffer.ClearRenderTarget.html).

[!code-cs[GPU_Blit](../../../Tests/CodeSamples/CpuImageSamples.cs#GPU_Blit)]

# Access images via CPU

To access the device camera image on the CPU, first call [ARCameraManager.TryAcquireLatestCpuImage](xref:UnityEngine.XR.ARFoundation.ARCameraManager.TryAcquireLatestCpuImage(UnityEngine.XR.ARSubsystems.XRCpuImage@)) to obtain an `XRCpuImage`.

> [!NOTE]
> On iOS 16 or newer, you can also use [ARKitCameraSubsystem.TryAcquireHighResolutionCpuImage](https://docs.unity3d.com/Packages/com.unity.xr.arkit@5.1/api/UnityEngine.XR.ARKit.ARKitCameraSubsystem.html#UnityEngine_XR_ARKit_ARKitCameraSubsystem_TryAcquireHighResolutionCpuImage). See [High resolution CPU image](https://docs.unity3d.com/Packages/com.unity.xr.arkit@5.1/manual/arkit-camera.html#high-resolution-cpu-image) to learn more.

[XRCpuImage](xref:UnityEngine.XR.ARSubsystems.XRCpuImage) is a struct that represents a native pixel array. When your app no longer needs this resource, you must call [XRCpuImage.Dispose](xref:UnityEngine.XR.ARSubsystems.XRCpuImage.Dispose) to release the associated memory back to the AR platform. You should call `Dispose` as soon as possible, as failure to `Dispose` too many `XRCpuImage` instances can cause the AR platform to run out of memory and prevent you from capturing new camera images.

Once you have an `XRCpuImage`, you can convert it to a [Texture2D](xref:UnityEngine.Texture2D) or access the raw image data directly:

- [Synchronous conversion](#synchronous-conversion) to a grayscale or color TextureFormat
- [Asynchronous conversion](#asynchronous-conversion) to grayscale or color
- [Raw image planes](#raw-image-planes)

## Synchronous conversion

To synchronously convert an `XRCpuImage` to a grayscale or color format, call [XRCpuImage.Convert](xref:UnityEngine.XR.ARSubsystems.XRCpuImage.Convert(UnityEngine.XR.ARSubsystems.XRCpuImage.ConversionParams,System.IntPtr,System.Int32)):

```csharp
public void Convert(
    XRCpuImage.ConversionParams conversionParams,
    IntPtr destinationBuffer,
    int bufferLength)
```

This method converts the `XRCpuImage` to the [TextureFormat](xref:UnityEngine.TextureFormat) specified by the [ConversionParams](xref:UnityEngine.XR.ARSubsystems.XRCpuImage.ConversionParams), then writes the data to `destinationBuffer`.

Grayscale image conversions such as `TextureFormat.Alpha8` and `TextureFormat.R8` are typically very fast, while color conversions require more CPU-intensive computations.

Use [XRCpuImage.GetConvertedDataSize](xref:UnityEngine.XR.ARSubsystems.XRCpuImage.GetConvertedDataSize(UnityEngine.Vector2Int,UnityEngine.TextureFormat)) if needed to get the required size for `destinationBuffer`.

### Example

The example code below executes the following steps:

1. Acquire an `XRCpuImage`
2. Synchronously convert to an `RGBA32` color format
3. Apply the converted pixel data to a texture

[!code-cs[Synchronous_Conversion](../../../Tests/CodeSamples/CpuImageSamples.cs#Synchronous_Conversion)]

The [AR Foundation Samples](https://github.com/Unity-Technologies/arfoundation-samples#cpu-images) GitHub repository contains a similar [example](https://github.com/Unity-Technologies/arfoundation-samples/blob/main/Assets/Scripts/CpuImageSample.cs) that you can run on your device.

## Asynchronous conversion

If you do not need to access the converted image immediately, you can convert it asynchronously.

Asynchronous conversion has three steps:

1. Call [XRCpuImage.ConvertAsync(XRCpuImage.ConversionParams)](xref:UnityEngine.XR.ARSubsystems.XRCpuImage.ConvertAsync(UnityEngine.XR.ARSubsystems.XRCpuImage.ConversionParams)). `ConvertAsync` returns an [XRCpuImage.AsyncConversion](xref:UnityEngine.XR.ARSubsystems.XRCpuImage.AsyncConversion) object to track the conversion status.

    > [!NOTE]
    > You can dispose `XRCpuImage` before asynchronous conversion completes. The data contained by the `XRCpuImage.AsyncConversion` is not bound to the `XRCpuImage`.

2. Await the `AsyncConversion` status until conversion is done:

    ```csharp
    while (!conversion.status.IsDone())
        yield return null;
    ```

    After conversion is done, read the [status](xref:UnityEngine.XR.ARSubsystems.XRCpuImage.AsyncConversionStatus) value to determine whether conversion succeeded. `AsyncConversionStatus.Ready` indicates a successful conversion.

3. If successful, call [AsyncConversion.GetData\<T\>](xref:UnityEngine.XR.ARSubsystems.XRCpuImage.AsyncConversion.GetData``1) to retrieve the converted data.

    `GetData<T>` returns a `NativeArray<T>` that is a view into the native pixel array. You don't need to dispose this `NativeArray`, as `AsyncConversion.Dispose` will dispose it.

    > [!IMPORTANT]
    > You must explicitly dispose `XRCpuImage.AsyncConversion`. Failing to dispose an `AsyncConversion` will leak memory until the `XRCameraSubsystem` is destroyed.

Asynchronous requests typically complete within one frame, but can take longer if you queue multiple requests at once. Requests are processed in the order they are received, and there is no limit on the number of requests.

### Examples

[!code-cs[Asynchronous_Conversion](../../../Tests/CodeSamples/CpuImageSamples.cs#Asynchronous_Conversion)]

There is also an overload of `ConvertAsync` that accepts a delegate and does not return an `XRCpuImage.AsyncConversion`, as shown in the example below:

[!code-cs[Asynchronous_Conversion_With_Delegate](../../../Tests/CodeSamples/CpuImageSamples.cs#Asynchronous_Conversion_With_Delegate)]

If you need the data to persist beyond the lifetime of your delegate, make a copy. See [NativeArray\<T\>.CopyFrom](https://docs.unity3d.com/ScriptReference/Unity.Collections.NativeArray_1.CopyFrom.html).

## Raw image planes

> [!NOTE]
> An image "plane", in this context, refers to a channel used in the video format. It is not a planar surface and not related to `ARPlane`.

Most video formats use a YUV encoding variant, where Y is the luminance plane, and the UV plane(s) contain chromaticity information. U and V can be interleaved or separate planes, and there might be additional padding per pixel or per row.

If you need access to the raw, platform-specific YUV data, you can get each image "plane" using the `XRCpuImage.GetPlane` method as shown in the example below:

```csharp
if (!cameraManager.TryAcquireLatestCpuImage(out XRCpuImage image))
    return;

// Consider each image plane
for (int planeIndex = 0; planeIndex < image.planeCount; ++planeIndex)
{
    // Log information about the image plane
    var plane = image.GetPlane(planeIndex);
    Debug.LogFormat("Plane {0}:\n\tsize: {1}\n\trowStride: {2}\n\tpixelStride: {3}",
        planeIndex, plane.data.Length, plane.rowStride, plane.pixelStride);

    // Do something with the data
    MyComputerVisionAlgorithm(plane.data);
}

// Dispose the XRCpuImage to avoid resource leaks
image.Dispose();
```

`XRCpuImage.Plane` provides direct access to a native memory buffer via `NativeArray<byte>`. This represents a view into the native memory — you don't need to dispose the `NativeArray`. You should consider this memory read-only, and its data is valid until the `XRCpuImage` is disposed.
