---
uid: arfoundation-vs-node-on-participants-changed
---
# On Participants Changed

Triggers when AR Participants have changed. AR Participants can be added, updated, and/or removed every frame if there is an enabled AR Participant Manager in the scene.

![On Participants Changed](../../images/visual-scripting/vs-on-participants-changed.png)<br/>*On Participants Changed node, shown with Get Variable*

## Input Data Ports

| Port | Data type | Description |
| :--- | :-------- | :---------- |
| **Target** | [GameObject](xref:UnityEngine.GameObject) | Target GameObject should have an enabled [ARParticipantManager](xref:arfoundation-participant-tracking#ar-participant-manager-component) component. If you do not connect this port, this node searches for an enabled AR Participant Manager component in the scene instead, and throws an exception if none is found. |

[!include[](snippets/get-variable-tip.md)]

## Output Data Ports

| Port | Data type | Description |
| :--- | :-------- | :---------- |
| **Added** | [List](xref:System.Collections.Generic.List`1) of [ARParticipant](xref:UnityEngine.XR.ARFoundation.ARParticipant) | List of AR Participants that have been added. |
| **Updated** | [List](xref:System.Collections.Generic.List`1) of [ARParticipant](xref:UnityEngine.XR.ARFoundation.ARParticipant) | List of AR Participants that have been updated. |
| **Removed** | [List](xref:System.Collections.Generic.List`1) of [ARParticipant](xref:UnityEngine.XR.ARFoundation.ARParticipant) | List of AR Participants that have been removed. |
