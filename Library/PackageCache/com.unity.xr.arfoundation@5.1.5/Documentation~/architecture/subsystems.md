---
uid: arfoundation-subsystems
---
# Subsystems

A *subsystem* (shorthand for [SubsystemWithProvider](xref:UnityEngine.SubsystemsImplementation.SubsystemWithProvider)) defines the life cycle and scripting interface of a Unity engine feature. All subsystems share a common subsystem life cycle, but their feature implementations can vary on different platforms, providing a layer of abstraction between your application code and platform-specific SDK's such as Google ARCore or Apple ARKit.

AR Foundation defines its AR features using subsystems. For example, the [XRPlaneSubsystem](xref:UnityEngine.XR.ARSubsystems.XRPlaneSubsystem) defines an interface for plane detection. You use the same application code to interact with a detected plane on iOS and Android — or any other platform with an implementation of the plane subsystem — but AR Foundation itself does not contain subsystem implementations for these platforms.

Subsystem implementations are called *providers*, and are typically made available in separate packages called *provider plug-ins*. For example, the [Google ARCore XR Plug-in](https://docs.unity3d.com/Packages/com.unity.xr.arcore@5.1/manual/index.html) provides subsystem implementations for the Android platform, and the [Apple ARKit XR Plug-in](https://docs.unity3d.com/Packages/com.unity.xr.arkit@5.1/manual/index.html) provides implementations for iOS.

With the exception of [Device tracking](xref:arfoundation-device-tracking), the table below lists AR Foundation's AR features and their corresponding subsystems. (Device tracking uses the [Input System](https://docs.unity3d.com/Packages/com.unity.inputsystem@latest) and is not subsytem-based.)

| Feature | Subsystem |
| :------ | :-------- |
| [Session](xref:arfoundation-session)                       | [XRSessionSubsystem](xref:UnityEngine.XR.ARSubsystems.XRSessionSubsystem) |
| [Camera](xref:arfoundation-camera)                         | [XRCameraSubsystem](xref:UnityEngine.XR.ARSubsystems.XRCameraSubsystem) |
| [Plane detection](xref:arfoundation-plane-detection)       | [XRPlaneSubsystem](xref:UnityEngine.XR.ARSubsystems.XRPlaneSubsystem) |
| [Image tracking](xref:arfoundation-image-tracking)         | [XRImageTrackingSubsystem](xref:UnityEngine.XR.ARSubsystems.XRImageTrackingSubsystem) |
| [Object tracking](xref:arfoundation-object-tracking)       | [XRObjectTrackingSubsystem](xref:UnityEngine.XR.ARSubsystems.XRObjectTrackingSubsystem) |
| [Face tracking](xref:arfoundation-face-tracking)           | [XRFaceSubsystem](xref:UnityEngine.XR.ARSubsystems.XRFaceSubsystem) |
| [Body tracking](xref:arfoundation-body-tracking)           | [XRHumanBodySubsystem](xref:UnityEngine.XR.ARSubsystems.XRHumanBodySubsystem) |
| [Point clouds](xref:arfoundation-point-clouds)             | [XRPointCloudSubsystem](xref:UnityEngine.XR.ARSubsystems.XRPointCloudSubsystem) |
| [Raycasts](xref:arfoundation-raycasts)                     | [XRRaycastSubsystem](xref:UnityEngine.XR.ARSubsystems.XRRaycastSubsystem) |
| [Anchors](xref:arfoundation-anchors)                       | [XRAnchorSubsystem](xref:UnityEngine.XR.ARSubsystems.XRAnchorSubsystem) |
| [Meshing](xref:arfoundation-meshing)                       | [XRMeshSubsystem](xref:UnityEngine.XR.XRMeshSubsystem) |
| [Environment Probes](xref:arfoundation-environment-probes) | [XREnvironmentProbeSubsystem](xref:UnityEngine.XR.ARSubsystems.XREnvironmentProbeSubsystem) |
| [Occlusion](xref:arfoundation-occlusion)                   | [XROcclusionSubsystem](xref:UnityEngine.XR.ARSubsystems.XROcclusionSubsystem) |
| [Participants](xref:arfoundation-participant-tracking)     | [XRParticipantSubsystem](xref:UnityEngine.XR.ARSubsystems.XRParticipantSubsystem) |

## Subsystem life cycle

All subsystems have the same life cycle: they can be created, started, stopped, and destroyed. You don't typically need to create or destroy a subsystem instance yourself, as this is the responsibility of Unity's active `XRLoader`. Each provider plug-in contains an `XRLoader` implementation (or simply, a loader).  Most commonly, a loader creates an instance of all applicable subsystems when your application initializes and destroys them when your application quits, although this behavior is configurable. When a trackable manager goes to `Start` a subsystem, it gets the subsystem instance from the project's active loader based on the settings found in **Project Settings** > **XR Plug-in Management**. For more information about loaders and their configuration, see the [XR Plug-in Management end-user documentation](https://docs.unity3d.com/Packages/com.unity.xr.management@latest?subfolder=/manual/EndUser.html).

In a typical AR Foundation scene, any [managers](xref:arfoundation-managers) present in the scene will `Start` and `Stop` their subsystems when the manager is enabled or disabled, respectively. The exact behavior of `Start` and `Stop` varies by subsystem, but generally corresponds to "start doing work" and "stop doing work", respectively. You can start or stop a subystem at any time based on the needs of your application.

## Subsystem descriptors

Each subsystem has a corresponding [SubsystemDescriptor](xref:UnityEngine.SubsystemsImplementation.SubsystemDescriptorWithProvider) whose properties describe the range of the subystem's capabilities. Providers might assign different values to these properties at runtime based on different platform or device limitations. 

Wherever you use a capability described in a subsystem descriptor, you should check its property value at runtime first to confirm whether that capability is supported on the target device, as shown in the example below:

```csharp
var trackedImageManager = FindObjectOfType(typeof(ARTrackedImageManager));
var imageTrackingSubystem = trackedImageManager.subsystem;

// Query whether the image tracking provider supports runtime modification
// of reference image libraries
if (imageTrackingSubsystem.subsystemDescriptor.supportsMutableLibrary)
{
    // take some action
}

// Equivalently:
if (trackedImageManager.descriptor.supportsMutableLibrary)
{
    // take some action
}
```

## Tracking subsystems

A *tracking subsystem* is a subsystem that detects and tracks one or more objects, called trackables, in the physical environment.

A *trackable* represents something that a tracking subsystem can detect and track. For example, the [XRPlaneSubsystem](xref:UnityEngine.XR.ARSubsystems.XRPlaneSubsystem) detects and tracks [BoundedPlane](xref:UnityEngine.XR.ARSubsystems.BoundedPlane) trackables. Each trackable has a `TrackableId`, which is a 128-bit GUID (Globally Unique Identifier) that can be used to uniquely identify trackables across multiple frames as they are added, updated, or removed.

In code, a trackable is defined as any class which implements [ITrackable](xref:UnityEngine.XR.ARSubsystems.ITrackable). In the `UnityEngine.XR.ARSubsystems` namespace, all trackable implementations (like [BoundedPlane](xref:UnityEngine.XR.ARSubsystems.BoundedPlane)) are structs. In the `UnityEngine.XR.ARFoundation` namespace, all trackable implementations (like [ARPlane](xref:UnityEngine.XR.ARFoundation.ARPlane)) are components which wrap these structs.
