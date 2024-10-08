---
uid: arcore-image-tracking
---
# Image tracking

This page is a supplement to the AR Foundation [Image tracking](xref:arfoundation-image-tracking) manual. The following sections only contain information about APIs where ARCore exhibits unique platform-specific behavior.

[!include[](../snippets/arf-docs-tip.md)]

## Reference image libraries

When you build an ARCore app for the Android platform, this package creates an `imgdb` file for each reference image library. ARCore creates these files in your project's `StreamingAssets` folder, in a subdirectory called `HiddenARCore`, so Unity can access them at runtime.

## Texture formats

You can use .jpg or .png files as AR reference images in ARCore. If a reference image in the `XRReferenceImageLibrary` isn't a .jpg or .png, a script in this package called the `ARCoreBuildProcessor` will attempt to convert the Texture to a .png so that ARCore can use it.

When you export a  `Texture2D` to .png, it can fail if the Texture's [Texture Import Settings](https://docs.unity3d.com/Manual/class-TextureImporter.html) have **Read/Write Enabled** disabled and **Compression** is set to **None**.

To use the Texture at runtime (not as a source Asset for the reference image), create a separate .jpg or .png copy for the source Asset. This reduces the performance impact of the Texture Import Settings at runtime.

## AssetBundles

Reference image libraries can be stored in AssetBundles and loaded at runtime, but setting up your project to build the AssetBundles correctly requires special instructions. Refer to [Use reference image libraries with AssetBundles](xref:arfoundation-image-tracking#use-reference-image-libraries-with-assetbundles) in AR Foundation for more information.

## Reference image dimensions

To improve image detection in ARCore you can specify the image dimensions. When you specify the dimensions for a reference image, ARCore receives the image's width, and then determines the height from the image's aspect ratio.
