---
uid: arfoundation-camera-platform-support
---
# Camera platform support

The AR Foundation [XRCameraSubsystem](xref:UnityEngine.XR.ARSubsystems.XRCameraSubsystem) is supported on the ARCore, ARKit, Meta OpenXR and XR Simulation platforms, as shown in the following table:

| Provider plug-in | Camera supported | Provider documentation |
| :--------------- | :-----------------------: | :--------------------- |
| Google ARCore XR Plug-in | Yes | [Camera](xref:arcore-camera) (ARCore) |
| Apple ARKit XR Plug-in | Yes | [Camera](xref:arkit-camera) (ARKit) |
| Apple visionOS XR Plug-in | | |
| Microsoft HoloLens | | |
| Unity OpenXR: Meta | Yes | [Camera](xref:meta-openxr-camera) (Meta OpenXR) |
| XR Simulation | Yes | [Camera](xref:arfoundation-simulation-camera) (XR Simulation) |

## Check for camera support

Your app can check at runtime whether a provider plug-in supports camera components on the user's device. Use the following example code to check whether the device supports camera components:

[!code-cs[CheckIfCameraLoaded](../../../Tests/CodeSamples/LoaderUtilitySamples.cs#CheckIfCameraLoaded)]

[!include[](../../snippets/initialization.md)]

> [!NOTE]
> This code example returns `true` if the provider plug-in implements the camera subsystem. The availability of the camera on a specific device may not be available other reasons, for example user permissions.

## Optional features

The following table lists the optional features of the camera subsystem. Each optional feature is defined by a **Descriptor Property** of the [XRCameraSubsystemDescriptor](xref:UnityEngine.XR.ARSubsystems.XRCameraSubsystemDescriptor), which you can check at runtime to determine whether a feature is supported. The **Description** column lists the optional API that may or may not be implemented on each platform. Refer to [Check for optional feature support](#check-feature-support) for a code example to check whether a feature is supported. 

> [!TIP]
> Check the API documentation to understand how each API behaves when the provider does not support a feature. For example, some properties throw an exception if you try to set the value when the feature is not supported.

| Feature | Descriptor Property | Description |
| :------ | :--------------- | :----------------- |
| **Brightness** | [supportsAverageBrightness](xref:UnityEngine.XR.ARSubsystems.XRCameraSubsystemDescriptor.supportsAverageBrightness) | Indicates whether the provider implementation can provide a value for [XRCameraFrame.averageBrightness](xref:UnityEngine.XR.ARSubsystems.XRCameraFrame.averageBrightness). |
| **Color temperature** | [supportsAverageColorTemperature](xref:UnityEngine.XR.ARSubsystems.XRCameraSubsystemDescriptor.supportsAverageColorTemperature) | Indicates whether the provider implementation can provide a value for [XRCameraFrame.averageColorTemperature](xref:UnityEngine.XR.ARSubsystems.XRCameraFrame.averageColorTemperature). |
| **Color correction** | [supportsColorCorrection](xref:UnityEngine.XR.ARSubsystems.XRCameraSubsystemDescriptor.supportsColorCorrection) | Indicates whether the provider implementation can provide a value for [XRCameraFrame.colorCorrection](xref:UnityEngine.XR.ARSubsystems.XRCameraFrame.colorCorrection). |
| **Display matrix** | [supportsDisplayMatrix](xref:UnityEngine.XR.ARSubsystems.XRCameraSubsystemDescriptor.supportsDisplayMatrix) | Indicates whether the provider implementation can provide a value for [XRCameraFrame.displayMatrix](xref:UnityEngine.XR.ARSubsystems.XRCameraFrame.displayMatrix). |
| **Projection matrix** | [supportsProjectionMatrix](xref:UnityEngine.XR.ARSubsystems.XRCameraSubsystemDescriptor.supportsProjectionMatrix) | Indicates whether the provider implementation can provide a value for [XRCameraFrame.projectionMatrix](xref:UnityEngine.XR.ARSubsystems.XRCameraFrame.projectionMatrix). |
| **Timestamp** | [supportsTimestamp](xref:UnityEngine.XR.ARSubsystems.XRCameraSubsystemDescriptor.supportsTimestamp) | Indicates whether the provider implementation can provide a value for [XRCameraFrame.timestampNs](xref:UnityEngine.XR.ARSubsystems.XRCameraFrame.timestampNs). |
| **Camera configuration** | [supportsCameraConfigurations](xref:UnityEngine.XR.ARSubsystems.XRCameraSubsystemDescriptor.supportsCameraConfigurations) | Indicates whether the provider implementation supports [camera configurations](xref:UnityEngine.XR.ARSubsystems.XRCameraConfiguration). |
| **Camera image** | [supportsCameraImage](xref:UnityEngine.XR.ARSubsystems.XRCameraSubsystemDescriptor.supportsCameraImage) | Indicates whether the provider implementation can provide camera images. |
| **Average intensity in lumens** | [supportsAverageIntensityInLumens](xref:UnityEngine.XR.ARSubsystems.XRCameraSubsystemDescriptor.supportsAverageIntensityInLumens) | Indicates whether the provider implementation can provide a value for [XRCameraFrame.averageIntensityInLumens](xref:UnityEngine.XR.ARSubsystems.XRCameraFrame.averageIntensityInLumens). |
| **Focus modes** | [supportsFocusModes](xref:UnityEngine.XR.ARSubsystems.XRCameraSubsystemDescriptor.supportsFocusModes) | Indicates whether the provider implementation supports the ability to set the camera's [focus mode](xref:UnityEngine.XR.ARSubsystems.CameraFocusMode). |
| **Face tracking ambient intensity light estimation** | [supportsFaceTrackingAmbientIntensityLightEstimation](xref:UnityEngine.XR.ARSubsystems.XRCameraSubsystemDescriptor.supportsFaceTrackingAmbientIntensityLightEstimation) | Indicates whether the provider implementation supports ambient intensity light estimation while face tracking is enabled.  |
| **Face tracking HDR light estimation** | [supportsFaceTrackingHDRLightEstimation](xref:UnityEngine.XR.ARSubsystems.XRCameraSubsystemDescriptor.supportsFaceTrackingHDRLightEstimation) | Indicates whether the provider implementation supports HDR light estimation while face tracking is enabled. |
| **World tracking ambient intensity light estimation** | [supportsWorldTrackingAmbientIntensityLightEstimation](xref:UnityEngine.XR.ARSubsystems.XRCameraSubsystemDescriptor.supportsWorldTrackingAmbientIntensityLightEstimation) | Indicates whether the provider implementation supports ambient intensity light estimation while world tracking.  |
| **World tracking HDR light estimation** | [supportsWorldTrackingHDRLightEstimation](xref:UnityEngine.XR.ARSubsystems.XRCameraSubsystemDescriptor.supportsWorldTrackingHDRLightEstimation) | Indicates whether the provider implementation supports HDR light estimation while world tracking. |
| **Camera grain** | [supportsCameraGrain](xref:UnityEngine.XR.ARSubsystems.XRCameraSubsystemDescriptor.supportsCameraGrain) | Indicates whether the provider implementation can provide a value for [XRCameraFrame.cameraGrain](xref:UnityEngine.XR.ARSubsystems.XRCameraFrame.cameraGrain). |
| **Exif data** | [supportsExifData](xref:UnityEngine.XR.ARSubsystems.XRCameraSubsystemDescriptor.supportsExifData) | Indicates whether the provider implementation supports [EXIF data](xref:UnityEngine.XR.ARSubsystems.XRCameraFrameExifData). |

### Optional feature platform support

The following table lists whether certain XR plug-in providers support each optional feature.

| Feature | ARCore | ARKit | Meta OpenXR | XR Simulation |
| :------ | :----: | :---: | :---------: | :-----------: |
| **Brightness** | Yes | | | |
| **Color temperature** | | Yes | | |
| **Color correction** | Yes | | | |
| **Display matrix** | Yes | Yes | | |
| **Projection matrix** | Yes | Yes | | |
| **Timestamp** | Yes | Yes | | |
| **Camera configurations** | Yes | Yes | | Yes |
| **Camera image** | Yes | Yes | | Yes |
| **Average intensity in lumens** | | Yes | | |
| **Focus modes** | Yes | Yes | | |
| **Face tracking ambient intensity light estimation** | Yes | Yes | | |
| **Face tracking HDR light estimation** | | Yes | | |
| **World tracking ambient intensity light estimation** | Yes | Yes | | |
| **World tracking HDR light estimation** | Yes | | | |
| **Camera grain**  | | iOS 13+ | | |
| **Image stabilization** | | | | |
| **EXIF data** | | iOS 16+ | | |

<a id="check-feature-support"></a>

### Check for optional feature support

Your app can check at runtime whether a camera provider supports any optional features on the user's device. The [XRCameraSubsystemDescriptor](xref:UnityEngine.XR.ARSubsystems.XRCameraSubsystemDescriptor) contains Boolean properties for each optional feature that tell you whether they are supported.

Refer to the following example code to learn how to check for optional feature support:

[!code-cs[DescriptorChecks](../../../Tests/CodeSamples/ARCameraManagerSamples.cs#DescriptorChecks)]

[!include[](../../snippets/apple-arkit-trademark.md)]