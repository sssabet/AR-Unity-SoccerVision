---
uid: arfoundation-vs-node-get-tracked-objects
---
# Get Tracked Objects

Save all AR Tracked Objects to the input List.

![Get Tracked Objects](../../images/visual-scripting/vs-get-tracked-objects.png)<br/>*Get Tracked Objects node, shown with Get Variable*

## Input Data Ports

| Port | Data type | Description |
| :--- | :-------- | :---------- |
| **Manager** | [ARTrackedObjectManager](xref:UnityEngine.XR.ARFoundation.ARTrackedObjectManager) | An active and enabled `ARTrackedObjectManager`. If you do not connect this port, this node searches for an enabled AR Tracked Object Manager component in the scene instead, and throws an exception if none is found. |
| **List** | [List](xref:System.Collections.Generic.List`1) of [ARTrackedObject](xref:UnityEngine.XR.ARFoundation.ARTrackedObject) | Where to save the AR Tracked Objects. This node clears the list, then adds the tracked objects. If you do not connect this port, this node allocates a new list instead. |

[!include[](snippets/get-variable-tip.md)]

## Output Data Ports

| Port | Data type | Description |
| :--- | :-------- | :---------- |
| **List** | [List](xref:System.Collections.Generic.List`1) of [ARTrackedObject](xref:UnityEngine.XR.ARFoundation.ARTrackedObject) | The same List you connected to the Input port, now containing all AR Tracked Objects. |
