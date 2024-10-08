---
uid: arfoundation-vs-node-get-point-clouds
---
# Get Point Clouds

Save all AR Point Clouds to the input List.

![Get Point Clouds](../../images/visual-scripting/vs-get-point-clouds.png)<br/>*Get Point Clouds node, shown with Get Variable*

## Input Data Ports

| Port | Data type | Description |
| :--- | :-------- | :---------- |
| **Manager** | [ARPointCloudManager](xref:UnityEngine.XR.ARFoundation.ARPointCloudManager) | An active and enabled `ARPointCloudManager`. If you do not connect this port, this node searches for an enabled AR Point Cloud Manager component in the scene instead, and throws an exception if none is found. |
| **List** | [List](xref:System.Collections.Generic.List`1) of [ARPointCloud](xref:UnityEngine.XR.ARFoundation.ARPointCloud) | Where to save the AR Point Clouds. This node clears the list, then adds the point clouds. If you do not connect this port, this node allocates a new list instead. |

[!include[](snippets/get-variable-tip.md)]

## Output Data Ports

| Port | Data type | Description |
| :--- | :-------- | :---------- |
| **List** | [List](xref:System.Collections.Generic.List`1) of [ARPointCloud](xref:UnityEngine.XR.ARFoundation.ARPointCloud) | The same List you connected to the Input port, now containing all AR Point Clouds. |
