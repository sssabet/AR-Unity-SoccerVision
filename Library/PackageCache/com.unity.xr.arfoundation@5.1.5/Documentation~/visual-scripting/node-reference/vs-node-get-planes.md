---
uid: arfoundation-vs-node-get-planes
---
# Get Planes

Save all AR Planes to the input List.

![Get Planes](../../images/visual-scripting/vs-get-planes.png)<br/>*Get Planes node, shown with Get Variable*

## Input Data Ports

| Port | Data type | Description |
| :--- | :-------- | :---------- |
| **Manager** | [ARPlaneManager](xref:UnityEngine.XR.ARFoundation.ARPlaneManager) | An active and enabled `ARPlaneManager`. If you do not connect this port, this node searches for an enabled AR Plane Manager component in the scene instead, and throws an exception if none is found. |
| **List** | [List](xref:System.Collections.Generic.List`1) of [ARPlane](xref:UnityEngine.XR.ARFoundation.ARPlane) | Where to save the AR Planes. This node clears the list, then adds the planes. If you do not connect this port, this node allocates a new list instead. |

[!include[](snippets/get-variable-tip.md)]

## Output Data Ports

| Port | Data type | Description |
| :--- | :-------- | :---------- |
| **List** | [List](xref:System.Collections.Generic.List`1) of [ARPlane](xref:UnityEngine.XR.ARFoundation.ARPlane) | The same List you connected to the Input port, now containing all AR Planes. |
