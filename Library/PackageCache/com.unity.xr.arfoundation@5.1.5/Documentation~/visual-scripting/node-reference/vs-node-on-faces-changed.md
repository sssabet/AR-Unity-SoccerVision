---
uid: arfoundation-vs-node-on-faces-changed
---
# On Faces Changed

Triggers when AR Faces have changed. AR Faces can be added, updated, and/or removed every frame if there is an enabled AR Face Manager in the scene.

![On Faces Changed](../../images/visual-scripting/vs-on-faces-changed.png)<br/>*On Faces Changed node, shown with Get Variable*

## Input Data Ports

| Port | Data type | Description |
| :--- | :-------- | :---------- |
| **Target** | [GameObject](xref:UnityEngine.GameObject) | Target GameObject should have an enabled [ARFaceManager](xref:arfoundation-face-tracking#ar-face-manager-component) component. If you do not connect this port, this node searches for an enabled AR Face Manager component in the scene instead, and throws an exception if none is found. |

[!include[](snippets/get-variable-tip.md)]

## Output Data Ports

| Port | Data type | Description |
| :--- | :-------- | :---------- |
| **Added** | [List](xref:System.Collections.Generic.List`1) of [ARFace](xref:UnityEngine.XR.ARFoundation.ARFace) | List of AR Faces that have been added. |
| **Updated** | [List](xref:System.Collections.Generic.List`1) of [ARFace](xref:UnityEngine.XR.ARFoundation.ARFace) | List of AR Faces that have been updated. |
| **Removed** | [List](xref:System.Collections.Generic.List`1) of [ARFace](xref:UnityEngine.XR.ARFoundation.ARFace) | List of AR Faces that have been removed. |
