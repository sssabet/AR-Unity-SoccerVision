---
uid: arfoundation-vs-node-on-point-clouds-changed
---
# On Point Clouds Changed

Triggers when AR Point Clouds have changed. AR Point Clouds can be added, updated, and/or removed every frame if there is an enabled AR Point Cloud Manager in the scene.

![On Point Clouds Changed](../../images/visual-scripting/vs-on-point-clouds-changed.png)<br/>*On Point Clouds Changed node, shown with Get Variable*

## Input Data Ports

| Port | Data type | Description |
| :--- | :-------- | :---------- |
| **Target** | [GameObject](xref:UnityEngine.GameObject) | Target GameObject should have an enabled [ARPointCloudManager](xref:arfoundation-point-clouds#ar-point-cloud-manager-component) component. If you do not connect this port, this node searches for an enabled AR Point Cloud Manager component in the scene instead, and throws an exception if none is found. |

[!include[](snippets/get-variable-tip.md)]

## Output Data Ports

| Port | Data type | Description |
| :--- | :-------- | :---------- |
| **Added** | [List](xref:System.Collections.Generic.List`1) of [ARPointCloud](xref:UnityEngine.XR.ARFoundation.ARPointCloud) | List of AR Point Clouds that have been added. |
| **Updated** | [List](xref:System.Collections.Generic.List`1) of [ARPointCloud](xref:UnityEngine.XR.ARFoundation.ARPointCloud) | List of AR Point Clouds that have been updated. |
| **Removed** | [List](xref:System.Collections.Generic.List`1) of [ARPointCloud](xref:UnityEngine.XR.ARFoundation.ARPointCloud) | List of AR Point Clouds that have been removed. |
