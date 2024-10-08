---
uid: arfoundation-plane-arplane
---
# AR Plane component

The [ARPlane](xref:UnityEngine.XR.ARFoundation.ARPlane) component is a type of [trackable](xref:arfoundation-managers#trackables-and-trackable-managers) that contains the data associated with a plane.

![AR Plane component](../../images/ar-plane.png)<br/>*AR Plane component*

| Property | Description |
| :------- | :---------- |
| **Destroy On Removal** | If `true`, this component's GameObject is destroyed when this trackable is removed. |
| **Vertex Changed Threshold** | The largest value by which a plane's vertex position may change before invoking the [boundaryChanged](xref:UnityEngine.XR.ARFoundation.ARPlane.boundaryChanged) event. Units are in meters. |

## Plane life cycle

As trackables, AR planes have a life cycle that consists of three phases: added, updated, and removed. Your app can [Respond to detected planes](xref:arfoundation-plane-arplanemanager#respond-to-detected-planes) during your AR session by subscribing to the AR Plane Manger component's `planesChanged` event.

### Added

When a plane is first detected, the plane manager creates a new GameObject with an AR Plane component attached, then invokes the `planesChanged` event, passing you a reference to the new AR Plane component via the [added](xref:UnityEngine.XR.ARFoundation.ARPlanesChangedEventArgs.added) property.

### Updated

Each subsequent frame after a plane is added, the plane manager might update that plane's information. On platforms that continuously scan the environment such as ARCore and ARKit, a plane typically grows over time as more of the surface is detected.

When a plane is updated, it's likely that its boundary vertices have changed. To respond to this, subscribe to the plane's [boundaryChanged](xref:UnityEngine.XR.ARFoundation.ARPlane.boundaryChanged) event. `boundaryChanged` is only invoked if at least one boundary vertex position has changed by at least the **Vertex Changed Threshold**, or if the total number of vertices changes.

When a plane's boundary vertices have changed, it's possible that the position and rotation of its Transform component have also changed to better reflect the updated surface area. If your app is designed to render AR content at a consistent position and rotation relative to a plane, you should create an [Anchor](xref:arfoundation-anchors) at that position, then parent your content GameObjects to the anchor GameObject. This will ensure that your content remains correctly positioned as the plane boundary changes over time.

#### Tracking state

When a plane leaves the device camera's field of view, the plane manager might set its [trackingState](xref:UnityEngine.XR.ARFoundation.ARTrackable`2.trackingState) to **Limited** instead of removing it. A value of **Limited** indicates that the plane manager is aware of a plane but cannot currently track its position.

If your app responds to plane life cycle events, you should check each plane's `trackingState` value whenever the plane is updated.

### Removed

When a plane is no longer detected, the plane manager might remove it. Removed planes can no longer be updated. If a removed plane's **Destroy on Removal** property is set to true, the plane manager will destroy it immediately after invoking the `planesChanged` event.

When a plane is removed, this may not indicate that a surface is no longer present in the environment, but rather that the AR platform's understanding of the environment has changed in a way that invalidates that plane.

Some platforms support the concept of planes *subsuming* each other, or merging together. When one plane subsumes another, its boundary expands to contain that plane's detected surface area, and the other plane is removed. The removed plane's [subsumedBy](xref:UnityEngine.XR.ARFoundation.ARPlane.subsumedBy) property may contain a reference to the plane that subsumed it if the AR platform supports this.

> [!IMPORTANT]
> Do not call `Destroy` on any AR Plane component or its GameObject. AR planes are managed by the AR Plane Manager component, and destroying them yourself can result in errors. Consider disabling the GameObject or not rendering the plane mesh instead.

[!include[](../../snippets/apple-arkit-trademark.md)]
