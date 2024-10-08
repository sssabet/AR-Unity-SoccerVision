---
uid: arfoundation-face-tracking-face-manager
---
# AR Face Manager component

The [ARFaceManager](xref:UnityEngine.XR.ARFoundation.ARFaceManager) component is a type of [trackable manager](xref:arfoundation-managers#trackables-and-trackable-managers) that detects and tracks human faces in the physical environment. As a trackable manager, the AR Face Manager creates GameObjects in your scene for each detected face.

![AR Face Manager component](../../images/ar-face-manager.png)<br/>*AR Face Manager component*

| Property               | Description |
| :--------------------- | :---------- |
| **trackables Changed** | Invoked when trackables have changed (been added, updated, or removed). |
| **Face Prefab**        | If not `null`, this prefab is instantiated for each detected face. If the prefab does not contain an [AR Face component](xref:arfoundation-face-tracking-arface), `ARFaceManager` will add one. |
| **Maximum Face Count** | The maximum number of faces to track simultaneously. |

## Get started

Add an AR Face Manager component to your XR Origin GameObject to enable face tracking in your app. If your scene doesn't contain an XR Origin GameObject, first follow the [Scene setup](xref:arfoundation-scene-setup) instructions.

Whenever your app doesn't need face tracking functionality, disable the AR Face Manager component to disable face tracking, which can improve app performance. If the user's device doesn't [support](xref:arfoundation-face-tracking-platform-support) face tracking, the AR Face Manager component will disable itself during `OnEnable`.

## Respond to detected faces

While enabled, the AR Face Manager component will get changes reported by the [XRFaceSubsystem](xref:UnityEngine.XR.ARSubsystems.XRFaceSubsystem) every frame. If any faces were added, updated, or removed, the [facesChanged](xref:UnityEngine.XR.ARFoundation.ARFaceManager.facesChanged) event is invoked with the relevant information. 

Subscribe to the `facesChanged` event by following these instructions:

1. Create a public method on a `MonoBehavior` or `ScriptableObject` with a single parameter of type [ARFacesChangedEventArgs](xref:UnityEngine.XR.ARFoundation.ARFacesChangedEventArgs), as shown in the following example code:

    [!code-cs[FacesChanged](../../../Tests/CodeSamples/ARFaceManagerSamples.cs#FacesChanged)]

2. Use the following example code to subscribe to the `facesChanged` event:

    [!code-cs[FacesSubscribe](../../../Tests/CodeSamples/ARFaceManagerSamples.cs#FacesSubscribe)]
