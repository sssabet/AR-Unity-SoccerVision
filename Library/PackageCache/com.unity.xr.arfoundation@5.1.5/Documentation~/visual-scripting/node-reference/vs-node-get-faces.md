---
uid: arfoundation-vs-node-get-faces
---
# Get Faces

Save all AR Faces to the input List.

![Get Faces](../../images/visual-scripting/vs-get-faces.png)<br/>*Get Faces node, shown with Get Variable*

## Input Data Ports

| Port | Data type | Description |
| :--- | :-------- | :---------- |
| **Manager** | [ARFaceManager](xref:UnityEngine.XR.ARFoundation.ARFaceManager) | An active and enabled `ARFaceManager`. If you do not connect this port, this node searches for an enabled AR Face Manager component in the scene instead, and throws an exception if none is found. |
| **List** | [List](xref:System.Collections.Generic.List`1) of [ARFace](xref:UnityEngine.XR.ARFoundation.ARFace) | Where to save the AR Faces. This node clears the list, then adds the faces. If you do not connect this port, this node allocates a new list instead. |

[!include[](snippets/get-variable-tip.md)]

## Output Data Ports

| Port | Data type | Description |
| :--- | :-------- | :---------- |
| **List** | [List](xref:System.Collections.Generic.List`1) of [ARFace](xref:UnityEngine.XR.ARFoundation.ARFace) | The same List you connected to the Input port, now containing all AR Faces. |
