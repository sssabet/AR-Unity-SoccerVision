---
uid: arcore-camera
---
# Camera

This page is a supplement to the AR Foundation [Camera](xref:arfoundation-camera) manual. The following sections only contain information about APIs where ARCore exhibits unique platform-specific behavior.

[!include[](../snippets/arf-docs-tip.md)]

## Optional feature support

ARCore implements the following optional features of AR Foundation's [XRCameraSubsystem](xref:UnityEngine.XR.ARSubsystems.XRCameraSubsystem):

| Feature | Descriptor Property | Supported |
| :------ | :--------------- | :--------: |
| **Brightness** | [supportsAverageBrightness](xref:UnityEngine.XR.ARSubsystems.XRCameraSubsystemDescriptor.supportsAverageBrightness) | Yes |
| **Color temperature** | [supportsAverageColorTemperature](xref:UnityEngine.XR.ARSubsystems.XRCameraSubsystemDescriptor.supportsAverageColorTemperature) | |
| **Color correction** | [supportsColorCorrection](xref:UnityEngine.XR.ARSubsystems.XRCameraSubsystemDescriptor.supportsColorCorrection) | Yes |
| **Display matrix** | [supportsDisplayMatrix](xref:UnityEngine.XR.ARSubsystems.XRCameraSubsystemDescriptor.supportsDisplayMatrix) | Yes |
| **Projection matrix** | [supportsProjectionMatrix](xref:UnityEngine.XR.ARSubsystems.XRCameraSubsystemDescriptor.supportsProjectionMatrix) | Yes |
| **Timestamp** | [supportsTimestamp](xref:UnityEngine.XR.ARSubsystems.XRCameraSubsystemDescriptor.supportsTimestamp) | Yes |
| **Camera configurations** | [supportsCameraConfigurations](xref:UnityEngine.XR.ARSubsystems.XRCameraSubsystemDescriptor.supportsCameraConfigurations) | Yes |
| **Camera image** | [supportsCameraImage](xref:UnityEngine.XR.ARSubsystems.XRCameraSubsystemDescriptor.supportsCameraImage) | Yes |
| **Average intensity in lumens** | [supportsAverageIntensityInLumens](xref:UnityEngine.XR.ARSubsystems.XRCameraSubsystemDescriptor.supportsAverageIntensityInLumens) | |
| **Focus modes** | [supportsFocusModes](xref:UnityEngine.XR.ARSubsystems.XRCameraSubsystemDescriptor.supportsFocusModes) | Yes |
| **Face tracking ambient intensity light estimation** | [supportsFaceTrackingAmbientIntensityLightEstimation](xref:UnityEngine.XR.ARSubsystems.XRCameraSubsystemDescriptor.supportsFaceTrackingAmbientIntensityLightEstimation) | Yes |
| **Face tracking HDR light estimation** | [supportsFaceTrackingHDRLightEstimation](xref:UnityEngine.XR.ARSubsystems.XRCameraSubsystemDescriptor.supportsFaceTrackingHDRLightEstimation) | |
| **World tracking ambient intensity light estimation** | [supportsWorldTrackingAmbientIntensityLightEstimation](xref:UnityEngine.XR.ARSubsystems.XRCameraSubsystemDescriptor.supportsWorldTrackingAmbientIntensityLightEstimation) | Yes |
| **World tracking HDR light estimation** | [supportsWorldTrackingHDRLightEstimation](xref:UnityEngine.XR.ARSubsystems.XRCameraSubsystemDescriptor.supportsWorldTrackingHDRLightEstimation) | Yes |
| **Camera grain** | [supportsCameraGrain](xref:UnityEngine.XR.ARSubsystems.XRCameraSubsystemDescriptor.supportsCameraGrain) | |
| **Exif data** | [supportsExifData](xref:UnityEngine.XR.ARSubsystems.XRCameraSubsystemDescriptor.supportsExifData) | |

> [!NOTE]
> Refer to AR Foundation [Camera platform support](xref:arfoundation-camera-platform-support) for more information 
> on the optional features of the camera subsystem.

## Camera configuration

[XRCameraConfiguration](xref:UnityEngine.XR.ARSubsystems.XRCameraConfiguration) contains an `IntPtr` field `nativeConfigurationHandle`, which is a platform-specific handle. For ARCore, this handle is the pointer to the `ArCameraConfiguration`. The native object is managed by Unity. Do not manually destroy it.

## EXIF data

This package implements AR Foundation's [EXIF data](xref:arfoundation-exif-data) API using ARCore's [ArImageMetadata](https://developers.google.com/ar/reference/c/group/ar-image-metadata#arimagemetadata). Refer to the following table to understand which tags ARCore supports:

| EXIF tag                | Supported |
| :---------------------- | :-------: |
| ApertureValue           | Yes |
| BrightnessValue         |     |
| ColorSpace              |     |
| ExposureBiasValue       |     |
| ExposureTime            | Yes |
| FNumber                 | Yes |
| Flash                   | Yes |
| FocalLength             | Yes |
| PhotographicSensitivity | Yes |
| MeteringMode            |     |
| ShutterSpeedValue       | Yes |
