---
uid: arfoundation-vs-node-on-tracked-images-changed
---
# On Tracked Images Changed

Triggers when AR Tracked Images have changed. AR Tracked Images can be added, updated, and/or removed every frame if there is an enabled AR Tracked Image Manager in the scene.

![On Tracked Images Changed](../../images/visual-scripting/vs-on-tracked-images-changed.png)<br/>*On Tracked Images Changed node, shown with Get Variable*

## Input Data Ports

| Port | Data type | Description |
| :--- | :-------- | :---------- |
| **Target** | [GameObject](xref:UnityEngine.GameObject) | Target GameObject should have an enabled [ARTrackedImageManager](xref:arfoundation-image-tracking#ar-tracked-image-manager-component) component. If you do not connect this port, this node searches for an enabled AR Tracked Image Manager component in the scene instead, and throws an exception if none is found. |

[!include[](snippets/get-variable-tip.md)]

## Output Data Ports

| Port | Data type | Description |
| :--- | :-------- | :---------- |
| **Added** | [List](xref:System.Collections.Generic.List`1) of [ARTrackedImage](xref:UnityEngine.XR.ARFoundation.ARTrackedImage) | List of AR Tracked Images that have been added. |
| **Updated** | [List](xref:System.Collections.Generic.List`1) of [ARTrackedImage](xref:UnityEngine.XR.ARFoundation.ARTrackedImage) | List of AR Tracked Images that have been updated. |
| **Removed** | [List](xref:System.Collections.Generic.List`1) of [ARTrackedImage](xref:UnityEngine.XR.ARFoundation.ARTrackedImage) | List of AR Tracked Images that have been removed. |
