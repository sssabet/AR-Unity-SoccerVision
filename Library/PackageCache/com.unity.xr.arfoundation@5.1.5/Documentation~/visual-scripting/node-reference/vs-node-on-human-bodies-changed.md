---
uid: arfoundation-vs-node-on-human-bodies-changed
---
# On Human Bodies Changed

Triggers when AR Human Bodies have changed. AR Human Bodies can be added, updated, and/or removed every frame if there is an enabled AR Human Body Manager in the scene.

![On Human Bodies Changed](../../images/visual-scripting/vs-on-human-bodies-changed.png)<br/>*On Human Bodies Changed node, shown with Get Variable*

## Input Data Ports

| Port | Data type | Description |
| :--- | :-------- | :---------- |
| **Target** | [GameObject](xref:UnityEngine.GameObject) | Target GameObject should have an enabled [ARHumanBodyManager](xref:arfoundation-body-tracking#ar-human-body-manager-component) component. If you do not connect this port, this node searches for an enabled AR Human Body Manager component in the scene instead, and throws an exception if none is found. |

[!include[](snippets/get-variable-tip.md)]

## Output Data Ports

| Port | Data type | Description |
| :--- | :-------- | :---------- |
| **Added** | [List](xref:System.Collections.Generic.List`1) of [ARHumanBody](xref:UnityEngine.XR.ARFoundation.ARHumanBody) | List of AR Human Bodies that have been added. |
| **Updated** | [List](xref:System.Collections.Generic.List`1) of [ARHumanBody](xref:UnityEngine.XR.ARFoundation.ARHumanBody) | List of AR Human Bodies that have been updated. |
| **Removed** | [List](xref:System.Collections.Generic.List`1) of [ARHumanBody](xref:UnityEngine.XR.ARFoundation.ARHumanBody) | List of AR Human Bodies that have been removed. |
