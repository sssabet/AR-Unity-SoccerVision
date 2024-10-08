---
uid: arfoundation-vs-node-get-participants
---
# Get Participants

Save all AR Participants to the input List.

![Get Participants](../../images/visual-scripting/vs-get-participants.png)<br/>*Get Participants node, shown with Get Variable*

## Input Data Ports

| Port | Data type | Description |
| :--- | :-------- | :---------- |
| **Manager** | [ARParticipantManager](xref:UnityEngine.XR.ARFoundation.ARParticipantManager) | An active and enabled `ARParticipantManager`. If you do not connect this port, this node searches for an enabled AR Participant Manager component in the scene instead, and throws an exception if none is found. |
| **List** | [List](xref:System.Collections.Generic.List`1) of [ARParticipant](xref:UnityEngine.XR.ARFoundation.ARParticipant) | Where to save the AR Participants. This node clears the list, then adds the participants. If you do not connect this port, this node allocates a new list instead. |

[!include[](snippets/get-variable-tip.md)]

## Output Data Ports

| Port | Data type | Description |
| :--- | :-------- | :---------- |
| **List** | [List](xref:System.Collections.Generic.List`1) of [ARParticipant](xref:UnityEngine.XR.ARFoundation.ARParticipant) | The same List you connected to the Input port, now containing all AR Participants. |
