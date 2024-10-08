---
uid: arfoundation-raycasts
---
# AR Raycast Manager component

The [ARRaycastManager](xref:UnityEngine.XR.ARFoundation.ARRaycastManager) is a type of [trackable manager](xref:arfoundation-managers#trackables-and-trackable-managers).

![AR Raycast Manager component](../images/ar-raycast-manager.png)<br/>*AR Raycast Manager component*

## Ray casting

Ray casting (also known as hit testing) allows you to determine where a [ray](xref:UnityEngine.Ray) (defined by an origin and direction) intersects with a [trackable](xref:UnityEngine.XR.ARFoundation.ARTrackable). The ray cast interface is similar to the one in the Unity Physics module, but since AR trackables don't necessarily have a presence in the physics world, AR Foundation provides a separate interface.

The Raycast Manager serves two purposes:
1. Provides an API to perform single raycasts.
1. Allows you to create a persistent [ARRaycast](xref:UnityEngine.XR.ARFoundation.ARRaycast). An `ARRaycast` is a type of trackable and is updated automatically until you remove it. Conceptually, it is similar to repeating the same raycast query each frame, but platforms with direct support for this feature can provide better results.

## Single raycasts

There are two ray casting methods on the [ARRaycastManager](xref:UnityEngine.XR.ARFoundation.ARRaycastManager) that perform single raycasts.

The first method takes a two-dimensional pixel position on the screen.

[!code-cs[ARRaycastManager_Raycast_screenPoint](../../Runtime/ARFoundation/ARRaycastManager.cs#ARRaycastManager_Raycast_screenPoint)]

You can, for example, pass a touch position directly:

[!code-cs[raycast_using_touch](../../Tests/CodeSamples/RaycastSamples.cs#raycast_using_touch)]

The second method takes an arbitrary [Ray](xref:UnityEngine.Ray) (a position and direction):

[!code-cs[ARRaycastManager_Raycast_ray](../../Runtime/ARFoundation/ARRaycastManager.cs#ARRaycastManager_Raycast_ray)]

The following table summarizes the other parameters:

| Parameter | Description |
| :-------- | :---------- |
| `hitResults` | The results for both methods are stored in this `List`, which must not be `null`. This lets you reuse the same `List` object to avoid garbage-collected allocations. |
| `trackableTypeMask` | The type, or types, of trackables to hit test against. This is a flag, so multiple types can be bitwise OR'd together, for example, [TrackableType.PlaneWithinPolygon](xref:UnityEngine.XR.ARSubsystems.TrackableType.PlaneWithinPolygon) &#124; [TrackableType.FeaturePoint](xref:UnityEngine.XR.ARSubsystems.TrackableType.FeaturePoint) |

### Determining what the raycast hit

If the raycast hits something, `hitResults` will be populated with a `List` of [ARRaycastHits](xref:UnityEngine.XR.ARFoundation.ARRaycastHit).

Use the [hitType](xref:UnityEngine.XR.ARFoundation.ARRaycastHit.hitType) to determine what kind of thing the raycast hit. If it hit a [trackable](xref:UnityEngine.XR.ARFoundation.ARTrackable), such as a [plane](xref:UnityEngine.XR.ARFoundation.ARPlane), then the [ARRaycastHit.trackable](xref:UnityEngine.XR.ARFoundation.ARRaycastHit.trackable) property can be cast to that type of trackable:

[!code-cs[raycasthit_trackable](../../Tests/CodeSamples/RaycastSamples.cs#raycasthit_trackable)]

## Persistent raycasts

Persistent raycasts are a type of [trackable](xref:UnityEngine.XR.ARFoundation.ARTrackable). Each [ARRaycast](xref:UnityEngine.XR.ARFoundation.ARRaycast) continues to update automatically until you remove it or disable the [ARRaycastManager](xref:UnityEngine.XR.ARFoundation.ARRaycastManager).

To add or remove a persistent raycast, call [AddRaycast](xref:UnityEngine.XR.ARFoundation.ARRaycastManager.AddRaycast(UnityEngine.Vector2,System.Single)) or [RemoveRaycast](xref:UnityEngine.XR.ARFoundation.ARRaycastManager.RemoveRaycast(UnityEngine.XR.ARFoundation.ARRaycast)) on the [ARRaycastManager](xref:UnityEngine.XR.ARFoundation.ARRaycastManager) component from script code.

Persistent raycasts must be created from a screen point:

[!code-cs[ARRaycastManager_AddRaycast_screenPoint](../../Runtime/ARFoundation/ARRaycastManager.cs#ARRaycastManager_AddRaycast_screenPoint)]

When you create a new [ARRaycast](xref:UnityEngine.XR.ARFoundation.ARRaycast), AR Foundation creates a new GameObject with an AR Raycast component on it. You can optionally provide a Prefab in the **Raycast Prefab** field that is instantiated for each [ARRaycast](xref:UnityEngine.XR.ARFoundation.ARRaycast), which allows you to extend the default behavior of each [ARRaycast](xref:UnityEngine.XR.ARFoundation.ARRaycast).

## Supported trackables

[ARRaycastManager](xref:UnityEngine.XR.ARFoundation.ARRaycastManager) supports ray casting against most [TrackableType](xref:UnityEngine.XR.ARSubsystems.TrackableType)s; however, not all platforms support all trackables. For a list of supported [TrackableType](xref:UnityEngine.XR.ARSubsystems.TrackableType)s per platform, see below.

| **TrackableType**     | **ARCore** | **ARKit** |
|:-                     |:-          |:-         |
| `Depth`               | &check;    |           |
| `Face`                |            |           |
| `FeaturePoint`        | &check;    | &check;   |
| `Image`               | &check;    |           |
| `Planes`              | &check;    | &check;   |
| `PlaneEstimated`      | &check;    | &check;   |
| `PlaneWithinBounds`   | &check;    | &check;   |
| `PlaneWithinInfinity` |            | &check;   |
| `PlaneWithinPolygon`  | &check;    | &check;   |
