---
uid: arfoundation-implement-a-provider
---
# Implement a provider

To implement a provider for one or more of the subsystems in this package (for example, say you are a hardware manufacturer for a new AR device), Unity recommends that your implementation inherit from that subsystem's base class. These base class types follow a naming convention of **XR<Feature>Subsystem**, and they are found in the `UnityEngine.XR.ARSubsystems` namespace. Each subsystem base class has a nested abstract class called `Provider`, which is the primary interface you must implement for each subsystem you plan to support.

Subsystem implementations should be independent from each other. For example, your implementation of the [XRPlaneSubsystem](xref:UnityEngine.XR.ARSubsystems.XRPlaneSubsystem) should have the same behavior whether or not your [XRPointCloudSubsystem](xref:UnityEngine.XR.ARSubsystems.XRPointCloudSubsystem) implementation is also active in a user's scene.

## Register a subsystem descriptor

Each subsystem type has a corresponding subsystem descriptor type. Your provider should create and register a subsystem descriptor instance with Unity's [SubsystemManager](https://docs.unity3d.com/ScriptReference/SubsystemManager.html) to enable runtime discovery and activation of subsystems. To register your subsystem descriptor, include a static void method with the `[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]` attribute as shown in the example below, and in it call the type-appropriate registration method for the type of subsystem descriptor you are registering.

```csharp
// This class defines a Raycast subsystem provider
class MyRaycastSubsystem : XRRaycastSubsystem
{
    class MyProvider : Provider
    {
        // ...
        // XRRaycastSubsystem.Provider is a nested abstract class for you 
        // to implement
        // ... 
    }

    // This method registers the subsystem descriptor with the SubsystemManager
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void RegisterDescriptor()
    {
        // In this case XRRaycastSubsystemDescriptor provides a registration 
        // helper method. See each subsystem descriptor's API documentation 
        // for more information.
        XRRaycastSubsystemDescriptor.RegisterDescriptor(new XRImageTrackingSubsystemDescriptor.Cinfo
        {
            providerType = typeof(MyProvider),
            subsystemTypeOverride = typeof(MyRaycastSubsystem),
            // ...
            // You populate all required fields based on the details of 
            // your provider implementation
            // ...
        });
    }
}
```

### Native plug-ins

Some XR subsystems, notably including the mesh subsystem, are not defined in the `ARSubsystems` namespace. These subsystems conform to Unity's **native plug-in interface**, and their descriptors cannot be registered via C#. For more information about native plug-ins, see the [Unity XR SDK documentation](https://docs.unity3d.com/Manual/xr-sdk.html).

## Implement a tracking subsystem

Each tracking subsystem defines a method called [GetChanges](xref:UnityEngine.XR.ARSubsystems.TrackingSubsystem`4.GetChanges(Unity.Collections.Allocator)), which reports all added, updated, and removed trackables since the previous call to [GetChanges](xref:UnityEngine.XR.ARSubsystems.TrackingSubsystem`4.GetChanges(Unity.Collections.Allocator)). You are required to implement the [GetChanges](xref:UnityEngine.XR.ARSubsystems.TrackingSubsystem`4.GetChanges(Unity.Collections.Allocator)) method and should expect it to be called once per frame. Your provider must not update or remove a trackable without adding it first, nor update a trackable after it has been removed.

## Implement an XR Loader

An `XRLoader` is responsible for creating and destroying subsystem instances based on the settings found in **Project Settings** > **XR Plug-in Management**. All provider plug-ins must implement an `XRLoader`. For more information on authoring an `XRLoader`, see the [XR Plug-in Management provider documentation](https://docs.unity3d.com/Packages/com.unity.xr.management@latest?subfolder=/manual/Provider.html).

Example `XRLoader` implementations can be found in existing provider plug-ins, such as the [ARCoreLoader](xref:UnityEngine.XR.ARCore.ARCoreLoader) and [ARKitLoader](xref:UnityEngine.XR.ARKit.ARKitLoader). Install these packages to view their source code.
