---
uid: arcore-occlusion
---
# Occlusion

ARCore provides support for occlusion based on environment depth images it generates every frame. These depth images provide a distance in meters from the device to the environment.

This page is a supplement to the AR Foundation [Occlusion](xref:arfoundation-occlusion) manual. The following sections only contain information about APIs where ARCore exhibits unique platform-specific behavior.

[!include[](../snippets/arf-docs-tip.md)]

## Check if occlusion is supported

* The [XROcclusionSubsystemDescriptor](xref:UnityEngine.XR.ARSubsystems.XROcclusionSubsystemDescriptor) properties [supportsEnvironmentDepthImage](xref:UnityEngine.XR.ARSubsystems.XROcclusionSubsystemDescriptor.supportsEnvironmentDepthImage) and [supportsEnvironmentDepthConfidenceImage](xref:UnityEngine.XR.ARSubsystems.XROcclusionSubsystemDescriptor.supportsEnvironmentDepthConfidenceImage) require that you start an AR session before support can be determined. If there is no AR session, these properties return `false`. They might return `true` later when a session has been started.

## Optional feature support

ARCore implements the following optional features of AR Foundation's [XROcclusionSubsystem](xref:UnityEngine.XR.ARSubsystems.XROcclusionSubsystem). The availability of features depends on device hardware and software. Refer to [Requirements](#occlusion-requirements) for more information.

| Feature | Descriptor Property | Supported |
| :------ | :--------------- | :----------: |
| **Environment Depth Image** | [environmentDepthImageSupported](xref:UnityEngine.XR.ARSubsystems.XROcclusionSubsystemDescriptor.environmentDepthImageSupported) | Yes |
| **Environment Depth Confidence Image** | [environmentDepthConfidenceImageSupported](xref:UnityEngine.XR.ARSubsystems.XROcclusionSubsystemDescriptor.environmentDepthConfidenceImageSupported) | Yes |
| **Environment Depth Temporal Smoothing** | [environmentDepthImageSupported](xref:UnityEngine.XR.ARSubsystems.XROcclusionSubsystemDescriptor.environmentDepthImageSupported) | Yes |
| **Human Segmentation Stencil Image** | [humanSegmentationStencilImageSupported](xref:UnityEngine.XR.ARSubsystems.XROcclusionSubsystemDescriptor.humanSegmentationStencilImageSupported) | |
| **Human Segmentation Depth Image** | [humanSegmentationDepthImageSupported](xref:UnityEngine.XR.ARSubsystems.XROcclusionSubsystemDescriptor.humanSegmentationDepthImageSupported) | |

> [!NOTE]
> Refer to AR Foundation [Occlusion platform support](xref:arfoundation-occlusion-platform-support) for more information 
> on the optional features of the occlusion subsystem.

## Supported textures

The ARCore implementation of AR Foundation [Occlusion](xref:arfoundation-occlusion) supports [AROcclusionManager.environmentDepthTexture](xref:UnityEngine.XR.ARFoundation.AROcclusionManager.environmentDepthTexture) but does not support the other Textures related to human segmentation.

## Environment depth

The [XROcclusionSubsystem](xref:UnityEngine.XR.ARSubsystems.XROcclusionSubsystem) provides access to two types of environment depth: [raw](xref:UnityEngine.XR.ARSubsystems.XROcclusionSubsystem.TryAcquireRawEnvironmentDepthCpuImage(UnityEngine.XR.ARSubsystems.XRCpuImage@)) and [smoothed](xref:UnityEngine.XR.ARSubsystems.XROcclusionSubsystem.TryAcquireSmoothedEnvironmentDepthCpuImage(UnityEngine.XR.ARSubsystems.XRCpuImage@)).

- **Raw**: The raw, unsmoothed depth data. This is useful for custom processing where raw data is necessary and corresponds to ARCore's [ArFrame_acquireRawDepthImage16Bits](https://developers.google.com/ar/reference/c/group/ar-frame#arframe_acquirerawdepthimage16bits) function.
- **Smoothed**: Depth data with smoothing applied. This is useful when performing occlusion and corresponds to ARCore's [ArFrame_acquireDepthImage16Bits](https://developers.google.com/ar/reference/c/group/ar-frame#arframe_acquiredepthimage16bits) function.

> [!NOTE]
> Both raw and smoothed are always available if depth is supported, regardless of the value of [environmentDepthTemporalSmoothingEnabled](xref:UnityEngine.XR.ARSubsystems.XROcclusionSubsystem.environmentDepthTemporalSmoothingEnabled).

<a id="occlusion-requirements"></a>

## Requirements

Environment depth requires a device with depth support. Refer to [ARCore supported devices](https://developers.google.com/ar/devices#google_play_devices) for a list of devices that support depth.
