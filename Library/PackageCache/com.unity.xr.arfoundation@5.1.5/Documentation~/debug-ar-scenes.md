---
uid: arfoundation-debug-menu
---
# AR Foundation Debug menu

You can add the AR Foundation Debug menu to an AR scene to add a menu of tracking information and available configurations. When that scene is active on a target device, you can use the menu to view the following information:

- Stats Info
    - FPS
    - Tracking Mode
- Session Configurations
- Camera Configuration
- Debug Options
    - XR Origin
    - Planes
    - Anchors
    - Point Clouds

The AR Debug menu works with any scene that contains an [`ARSession`](xref:UnityEngine.XR.ARFoundation.ARSession), an [`XROrigin`](xref:Unity.XR.CoreUtils.XROrigin), and a [`Camera`](https://docs.unity3d.com/Manual/class-Camera.html). 

The Debug menu is a Prefab object containing the [`ARDebugMenu`](xref:UnityEngine.XR.ARFoundation.ARDebugMenu) script and a set of GameObjects and components that provide the UI. See [Setting up the menu](#setting-up-the-menu) for instructions to add the Debug menu to a scene.

# Setting up the menu

To create an out-of-the-box fully configured debug menu, right-click anywhere in the Hierarchy window and select **XR** &gt; **AR Debug Menu**.

Right-click in the Hierarchy window       |  Select XR
:-------------------------:|:-------------------------:
![Right-click in the Hierarchy window to display a menu of GameObject creation options](images/ar-debug-menu-xr.png)  |  ![Select XR &gt; AR Debug Menu](images/ar-debug-menu-scene-inspector.png)

The AR Debug menu appears as a toolbar that is anchored either to the left or bottom side of the screen.

# Features

## Stats Info

The Stats Info tab provides general information about a session, including current FPS and [`TrackingMode`](xref:UnityEngine.XR.ARFoundation.TrackingMode).

![Stats Info tab](images/ar-debug-menu-status-info.png)<br/>*Stats Info tab*

## Session Configurations

The Session Configurations tab displays all the available session [`Configurations`](xref:UnityEngine.XR.ARSubsystems.Configuration) for a specific platform and highlights the currently active configuration.

![Session Configurations tab](images/ar-debug-menu-configurations.png)<br/>*Session Configurations tab*

## Camera Configuration

The Camera Configuration tab displays the currently active [`XRCameraConfiguration`](xref:UnityEngine.XR.ARSubsystems.XRCameraConfiguration) and allows the user to select from other camera configurations that are available for the currently active session [`Configuration`](xref:UnityEngine.XR.ARSubsystems.Configuration).

![Camera Configuration tab](images/ar-debug-menu-camera-configurations.png)<br/>*Camera Configuration tab*

## Debug Options

The Debug Options tab allows the user to toggle visualizers for various [ARTrackables](xref:UnityEngine.XR.ARFoundation.ARTrackable) in the scene.

![Debug Options tab](images/ar-debug-menu-debug-options.png)<br/>*Debug Options tab*
