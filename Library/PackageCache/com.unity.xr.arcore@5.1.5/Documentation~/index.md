---
uid: arcore-manual
---
# Google ARCore XR Plug-in

Use the Google ARCore XR Plug-in package to enable ARCore support in your [AR Foundation](https://docs.unity3d.com/Packages/com.unity.xr.arfoundation@latest) project. This package implements the following AR Foundation features using ARCore 1.31:

| Feature | Description |
| :------ | :---------- |
| [Session](xref:arcore-session) | Enable, disable, and configure AR on the target platform. |
| Device tracking | Track the device's position and rotation in physical space. |
| [Camera](xref:arcore-camera) | Render images from device cameras and perform light estimation. |
| [Plane detection](xref:arcore-plane-detection) | Detect and track surfaces. |
| [Image tracking](xref:arcore-image-tracking) | Detect and track 2D images. |
| [Face tracking](xref:arcore-face-tracking) | Detect and track human faces. |
| [Point clouds](xref:arcore-point-clouds) | Detect and track feature points. |
| Raycasts | Cast rays against tracked items. |
| Anchors | Track arbitrary points in space. |
| Environment probes | Generate cubemaps of the environment. |
| [Occlusion](xref:arcore-occlusion) | Occlude AR content with physical objects and perform human segmentation. |

## Unsupported features

This package does not implement the following AR Foundation features as they are not supported by ARCore 1.31:

| Feature | Description |
| :------ | :---------- |
| [Object tracking](xref:arfoundation-object-tracking) | Detect and track 3D objects. |
| [Body tracking](xref:arfoundation-body-tracking) | Detect and track a human body. |
| [Meshing](xref:arfoundation-meshing) | Generate meshes of the environment. |
| [Participants](xref:arfoundation-participant-tracking) | Track other devices in a shared AR session. |

# Install the Google ARCore XR Plug-in

When you enable the Google ARCore XR Plug-in in **Project Settings** > **XR Plug-in Management**, Unity automatically installs this package if necessary. Refer to [Enable the ARCore plug-in](xref:arcore-project-config#enable-arcore) for instructions.

You can also install and uninstall this package using the [Package Manager](https://learn.unity.com/tutorial/the-package-manager). Installing through the Package Manager does not automatically enable the plug-in. In this case you must enable it in **Project Settings** > **XR Plug-in Management**.

# Requirements

Version 5.1 of the Google ARCore XR Plug-in is compatible with the following versions of the Unity Editor:

* 2021.3
* 2022.3
* 2023.2
