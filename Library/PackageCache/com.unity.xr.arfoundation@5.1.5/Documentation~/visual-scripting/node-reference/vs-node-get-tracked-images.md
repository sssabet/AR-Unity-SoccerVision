---
uid: arfoundation-vs-node-get-tracked-images
---
# Get Tracked Images

Save all AR Tracked Images to the input List.

![Get Tracked Images](../../images/visual-scripting/vs-get-tracked-images.png)<br/>*Get Tracked Images node, shown with Get Variable*

## Input Data Ports

| Port | Data type | Description |
| :--- | :-------- | :---------- |
| **Manager** | [ARTrackedImageManager](xref:UnityEngine.XR.ARFoundation.ARTrackedImageManager) | An active and enabled `ARTrackedImageManager`. If you do not connect this port, this node searches for an enabled AR Tracked Image Manager component in the scene instead, and throws an exception if none is found. |
| **List** | [List](xref:System.Collections.Generic.List`1) of [ARTrackedImage](xref:UnityEngine.XR.ARFoundation.ARTrackedImage) | Where to save the AR Tracked Images. This node clears the list, then adds the tracked images. If you do not connect this port, this node allocates a new list instead. |

[!include[](snippets/get-variable-tip.md)]

## Output Data Ports

| Port | Data type | Description |
| :--- | :-------- | :---------- |
| **List** | [List](xref:System.Collections.Generic.List`1) of [ARTrackedImage](xref:UnityEngine.XR.ARFoundation.ARTrackedImage) | The same List you connected to the Input port, now containing all AR Tracked Images. |
