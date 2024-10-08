---
uid: arfoundation-vs-node-on-environment-probes-changed
---
# On Environment Probes Changed

Triggers when AR Environment Probes have changed. AR Environment Probes can be added, updated, and/or removed every frame if there is an enabled AR Environment Probe Manager in the scene.

![On Environment Probes Changed](../../images/visual-scripting/vs-on-environment-probes-changed.png)<br/>*On Environment Probes Changed node, shown with Get Variable*

## Input Data Ports

| Port | Data type | Description |
| :--- | :-------- | :---------- |
| **Target** | [GameObject](xref:UnityEngine.GameObject) | Target GameObject should have an enabled [AREnvironmentProbeManager](xref:arfoundation-environment-probes#ar-environment-probe-manager-component) component. If you do not connect this port, this node searches for an enabled AR Environment Probe Manager component in the scene instead, and throws an exception if none is found. |

[!include[](snippets/get-variable-tip.md)]

## Output Data Ports

| Port | Data type | Description |
| :--- | :-------- | :---------- |
| **Added** | [List](xref:System.Collections.Generic.List`1) of [AREnvironmentProbe](xref:UnityEngine.XR.ARFoundation.AREnvironmentProbe) | List of AR Environment Probes that have been added. |
| **Updated** | [List](xref:System.Collections.Generic.List`1) of [AREnvironmentProbe](xref:UnityEngine.XR.ARFoundation.AREnvironmentProbe) | List of AR Environment Probes that have been updated. |
| **Removed** | [List](xref:System.Collections.Generic.List`1) of [AREnvironmentProbe](xref:UnityEngine.XR.ARFoundation.AREnvironmentProbe) | List of AR Environment Probes that have been removed. |
