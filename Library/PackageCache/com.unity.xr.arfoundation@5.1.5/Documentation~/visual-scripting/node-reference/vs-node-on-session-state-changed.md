---
uid: arfoundation-vs-node-on-session-state-changed
---
# On Session State Changed

Triggers when the AR [session state](xref:arfoundation-session#session-state) changes.

![On Session State Changed](../../images/visual-scripting/vs-on-session-state-changed.png)<br/>*On Session State Changed node, shown with Session State Switch*

## Output Data Ports

| Port | Data type | Description |
| :--- | :-------- | :---------- |
| **AR Session State** | [ARSessionState](xref:UnityEngine.XR.ARFoundation.ARSessionState) | The new `ARSessionState`. You can connect this to a Session State Switch node to take different actions based on the state. |

# Session State Switch

Trigger one of the output flows based on the input `ARSessionState`.

## Input Data Ports

| Port | Data type | Description |
| :--- | :-------- | :---------- |
| **AR Session State** | [ARSessionState](xref:UnityEngine.XR.ARFoundation.ARSessionState) | An `ARSessionState` object. You can get this via the On AR Session State Changed node. |

## Output Control Ports

| Port | Description |
| :--- | :---------- |
| **None** | Triggers if the session state is [None](xref:UnityEngine.XR.ARFoundation.ARSessionState.None). AR has not been initialized and availability is unknown. |
| **Unsupported** | Triggers if the session state is [Unsupported](xref:UnityEngine.XR.ARFoundation.ARSessionState.Unsupported). The device does not support AR. |
| **Checking Availability** | Triggers if the session state is [CheckingAvailability](xref:UnityEngine.XR.ARFoundation.ARSessionState.CheckingAvailability). The session subsystem is currently checking availability of AR on the device. |
| **Needs Install** | Triggers if the session state is [NeedsInstall](xref:UnityEngine.XR.ARFoundation.ARSessionState.NeedsInstall). The device supports AR, but requires additional software to be installed. |
| **Installing** | Triggers if the session state is [Installing](xref:UnityEngine.XR.ARFoundation.ARSessionState.Installing). AR software is currently installing. |
| **Ready** | Triggers if the session state is [Ready](xref:UnityEngine.XR.ARFoundation.ARSessionState.Ready). The device supports AR, and any necessary software is installed. |
| **Session Initializing** | Triggers if the session state is [SessionInitializing](xref:UnityEngine.XR.ARFoundation.ARSessionState.SessionInitializing). This usually means AR is running, but not yet tracking successfully. |
| **Session Tracking** | Triggers if the session state is [SessionTracking](xref:UnityEngine.XR.ARFoundation.ARSessionState.SessionTracking). The AR Session is running and tracking successfully. The device is able to determine its position and orientation in the world. |
