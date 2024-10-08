---
uid: arcore-face-tracking
---
# Face tracking

This page is a supplement to the AR Foundation [Face tracking](xref:arfoundation-face-tracking) manual. The following sections only contain information about APIs where ARCore exhibits unique platform-specific behavior.

[!include[](../snippets/arf-docs-tip.md)]

## Optional feature support

ARCore implements the following optional features of AR Foundation's [XRFaceSubsystem](xref:UnityEngine.XR.ARSubsystems.XRFaceSubsystem):

| Feature | Descriptor Property | Supported |
| :------ | :--------------- | :----: |
| **Face pose** | [supportsFacePose](xref:UnityEngine.XR.ARSubsystems.XRFaceSubsystemDescriptor.supportsFacePose) | Yes |
| **Face mesh vertices and indices** | [supportsFaceMeshVerticesAndIndices](xref:UnityEngine.XR.ARSubsystems.XRFaceSubsystemDescriptor.supportsFaceMeshVerticesAndIndices) | Yes |
| **Face mesh UVs** | [supportsFaceMeshUVs](xref:UnityEngine.XR.ARSubsystems.XRFaceSubsystemDescriptor.supportsFaceMeshUVs) | Yes |
| **Face mesh normals** | [supportsFaceMeshNormals](xref:UnityEngine.XR.ARSubsystems.XRFaceSubsystemDescriptor.supportsFaceMeshNormals) | Yes |
| **Eye tracking** |  [supportsEyeTracking](xref:UnityEngine.XR.ARSubsystems.XRFaceSubsystemDescriptor.supportsEyeTracking) | |

> [!NOTE]
> Refer to AR Foundation [Face tracking platform support](xref:arfoundation-face-tracking-platform-support) for more information 
> on the optional features of the face subsystem.

## Session configuration

Face tracking on ARCore requires the use of the user-facing or "selfie" camera. It is the responsibility of your session's [XRSessionSubsystem.configurationChooser](xref:UnityEngine.XR.ARSubsystems.XRSessionSubsystem.configurationChooser) to choose the camera facing direction. You can override the configuration chooser to meet your app's needs. For more information on the [ConfigurationChooser](xref:UnityEngine.XR.ARSubsystems.ConfigurationChooser), refer to the [What’s new in Unity’s AR Foundation | Unite Now 2020](https://www.youtube.com/watch?v=jBRxY2KnrUs&t=677s) video (YouTube). You can access a sample that shows how to use the `ConfigurationChooser` to choose between the user-facing and world-facing camera on the [AR Foundation samples](https://github.com/Unity-Technologies/arfoundation-samples/tree/5.1/Assets/Scenes/Configurations) GitHub repository.

## Face regions

The ARCore face subsystem provides face tracking methods that allow access to "regions". Regions are specific to ARCore. ARCore provides access to the following regions that define features on a face:

- Nose tip
- Forehead left
- Forehead right

Each region has a [Pose](xref:UnityEngine.Pose) associated with it. To access face regions, obtain an instance of the [ARCoreFaceSubsystem](xref:UnityEngine.XR.ARCore.ARCoreFaceSubsystem) using the following script:

```csharp
XRFaceSubsystem faceSubsystem = ...
#if UNITY_ANDROID
var arcoreFaceSubsystem = faceSubsystem as ARCoreFaceSubsystem;
if (arcoreFaceSubsystem != null)
{
    var regionData = new NativeArray<ARCoreFaceRegionData>(0, Allocator.Temp);
    arcoreFaceSubsystem.GetRegionPoses(faceId, Allocator.Temp, ref regionData);
    using (regionData)
    {
        foreach (var data in regionData)
        {
            Debug.LogFormat("Region {0} is at {1}", data.region, data.pose);
        }
    }
}
#endif
```
