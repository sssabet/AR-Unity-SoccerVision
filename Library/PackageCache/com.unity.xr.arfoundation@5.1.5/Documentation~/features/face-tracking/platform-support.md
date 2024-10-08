---
uid: arfoundation-face-tracking-platform-support
---
# Face tracking platform support

The AR Foundation [XRFaceSubsystem](xref:UnityEngine.XR.ARSubsystems.XRFaceSubsystem) is supported on the ARCore and ARKit platforms, as shown in the following table:

| Provider plug-in | Face tracking supported | Provider documentation |
| :--------------- | :-----------------------: | :--------------------- |
| Google ARCore XR Plug-in | Yes | [Face tracking](xref:arcore-face-tracking) (ARCore) |
| Apple ARKit XR Plug-in | Yes | [Face tracking](xref:arkit-face-tracking) (ARKit) |
| Apple visionOS XR Plug-in | | |
| Microsoft HoloLens | | |
| Unity OpenXR: Meta | | |
| XR Simulation | | |

## Check for face tracking support

Your app can check at runtime whether a provider plug-in supports face tracking on the user's device. Use the following example code to check whether the device supports face components:

[!code-cs[CheckIfFaceLoaded](../../../Tests/CodeSamples/LoaderUtilitySamples.cs#CheckIfFaceLoaded)]

[!include[](../../snippets/initialization.md)]

## Optional features

The following table lists the optional features of the face subsystem. Each optional feature is defined by a **Descriptor Property** of the [XRFaceSubsystemDescriptor](xref:UnityEngine.XR.ARSubsystems.XRFaceSubsystemDescriptor), which you can check at runtime to determine whether a feature is supported. Refer to [Check for optional feature support](#check-feature-support) for a code example to check whether a feature is supported. 

| Feature | Descriptor Property | Description |
| :------ | :--------------- | :----------------- |
| **Face pose** | [supportsFacePose](xref:UnityEngine.XR.ARSubsystems.XRFaceSubsystemDescriptor.supportsFacePose) | Whether the subsystem can produce a `Pose` for each detected face. |
| **Face mesh vertices and indices** | [supportsFaceMeshVerticesAndIndices](xref:UnityEngine.XR.ARSubsystems.XRFaceSubsystemDescriptor.supportsFaceMeshVerticesAndIndices) | Whether the subsystem supports face meshes and can produce vertices and triangle indices that represent a face mesh. |
| **Face mesh UVs** | [supportsFaceMeshUVs](xref:UnityEngine.XR.ARSubsystems.XRFaceSubsystemDescriptor.supportsFaceMeshUVs) | Whether the subsystem supports texture coordinates for each face mesh. |
| **Face mesh normals** | [supportsFaceMeshNormals](xref:UnityEngine.XR.ARSubsystems.XRFaceSubsystemDescriptor.supportsFaceMeshNormals) | Whether the subsystem supports normals for each face mesh. |
| **Eye tracking** |  [supportsEyeTracking](xref:UnityEngine.XR.ARSubsystems.XRFaceSubsystemDescriptor.supportsEyeTracking) | Whether the subsystem supports eye tracking for each detected face. |

### Optional feature platform support

The following table lists whether certain XR plug-in providers support each optional feature:

| Feature | ARCore | ARKit | 
| :------ | :----: | :---: |
| **Face pose** | Yes | Yes |
| **Face mesh vertices and indices** | Yes | Yes |
| **Face mesh UVs** | Yes | Yes |
| **Face mesh normals** | Yes | |
| **Eye tracking** | | Yes |

<a id="check-feature-support"></a>

### Check for optional feature support

Your app can check at runtime whether a face tracking provider supports any optional features on the user's device. The [XRFaceSubsystemDescriptor](xref:UnityEngine.XR.ARSubsystems.XRFaceSubsystemDescriptor) contains Boolean properties for each optional feature that tell you whether they are supported.

Refer to the following example code to learn how to check for optional feature support:

[!code-cs[DescriptorChecks](../../../Tests/CodeSamples/ARFaceManagerSamples.cs#DescriptorChecks)]

[!include[](../../snippets/apple-arkit-trademark.md)]