---
uid: arfoundation-occlusion-platform-support
---
# Occlusion platform support

Occlusion is supported on the ARCore, and ARKit platforms, as shown in the table below:

| Provider plug-in | Occlusion supported | Provider documentation |
| :--------------- | :---------: | :--------------------- |
| Google ARCore XR Plug-in | Yes | [Occlusion](xref:arcore-occlusion) (ARCore) |
| Apple ARKit XR Plug-in | Yes | [Occlusion](xref:arkit-occlusion) (ARKit) |
| Apple visionOS XR Plug-in | | |
| Microsoft HoloLens | | |
| Unity OpenXR: Meta | | |
| XR Simulation | | |

## Check for occlusion support

Your app can check at runtime whether a provider plug-in supports occlusion on the user's device.

Use the example code below to check whether the device supports occlusion:

[!code-cs[CheckIfOcclusionLoaded](../../../Tests/CodeSamples/LoaderUtilitySamples.cs#CheckIfOcclusionLoaded)]

[!include[](../../snippets/initialization.md)]

## Optional features

The following table describes the optional features of the [XROcclusionSubsystem](xref:UnityEngine.XR.ARSubsystems.XROcclusionSubsystem). The **Descriptor Property** column provides the relevant property of the [XROcclusionSubsystemDescriptor](xref:UnityEngine.XR.ARSubsystems.XROcclusionSubsystemDescriptor). The **Description** column defines the property. Refer to [AR Occlusion Manager component](xref:arfoundation-occlusion-manager) for more information about the types of depth image described.

| Feature | Descriptor Property | Description  |
| :------ | :---------- | :------------ |
| **Environment Depth Image** | [environmentDepthImageSupported](xref:UnityEngine.XR.ARSubsystems.XROcclusionSubsystemDescriptor.environmentDepthImageSupported) | Whether the subsystem supports environment depth image. |
| **Environment Depth Confidence Image** | [environmentDepthConfidenceImageSupported](xref:UnityEngine.XR.ARSubsystems.XROcclusionSubsystemDescriptor.environmentDepthConfidenceImageSupported) | Whether the subsystem supports environment depth confidence image. |
| **Environment Depth Temporal Smoothing** | [environmentDepthTemporalSmoothingSupported](xref:UnityEngine.XR.ARSubsystems.XROcclusionSubsystemDescriptor.environmentDepthTemporalSmoothingSupported) | Whether temporal smoothing of the environment image is supported. |
| **Human Segmentation Stencil Image** | [humanSegmentationStencilImageSupported](xref:UnityEngine.XR.ARSubsystems.XROcclusionSubsystemDescriptor.humanSegmentationStencilImageSupported) | Whether a subsystem supports human segmentation stencil image. |
| **Human Segmentation Depth Image** | [humanSegmentationDepthImageSupported](xref:UnityEngine.XR.ARSubsystems.XROcclusionSubsystemDescriptor.humanSegmentationDepthImageSupported) | Whether a subsystem supports human segmentation depth image. |

### Optional feature platform support

Occlusion providers may choose whether to implement any of the optional features of AR Foundation's [XROcclusionSubsystem](xref:UnityEngine.XR.ARSubsystems.XROcclusionSubsystem), as indicated in the following table:

| Feature | ARCore | ARKit |
| :------ | :----: | :---: |
| **Environment Depth Image** | Yes | Yes |
| **Environment Depth Confidence Image** | Yes | Yes |
| **Environment Depth Temporal Smoothing** | Yes | Yes |
| **Human Segmentation Stencil Image** | | Yes |
| **Human Segmentation Depth Image** | | Yes |

<a id="check-feature-support"></a>

### Check for optional feature support

Your app can check at runtime whether an occlusion provider supports any optional features on the user's device. The [XROcclusionSubsystemDescriptor](xref:UnityEngine.XR.ARSubsystems.XROcclusionSubsystemDescriptor) contains properties for each optional feature that tell you whether they are supported.

Refer to the following example code to learn how to check for optional feature support:

[!code-cs[DescriptorChecks](../../../Tests/CodeSamples/AROcclusionManagerSamples.cs#DescriptorChecks)]

[!include[](../../snippets/apple-arkit-trademark.md)]
