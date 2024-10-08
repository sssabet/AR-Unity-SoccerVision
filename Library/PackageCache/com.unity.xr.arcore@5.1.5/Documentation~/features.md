---
uid: arcore-features
---
# Features

AR features implement [AR Foundation](xref:arfoundation-manual) interfaces. These features are documented in the AR Foundation package manual, so this manual only includes information regarding APIs where ARCore exhibits unique platform-specific behavior.

This package implements the following AR features:

| Feature | Description |
| :------ | :---------- |
| [Session](xref:arcore-session) | Enable, disable, and configure AR on the target platform. |
| Device tracking | Track the device's position and rotation in physical space. |
| [Camera](xref:arcore-camera) | Render images from device cameras and perform light estimation. |
| [Plane detection](xref:arcore-plane-detection) | Detect and track flat surfaces. |
| [Image tracking](xref:arcore-image-tracking) | Detect and track 2D images. |
| [Face tracking](xref:arcore-face-tracking) | Detect and track human faces. |
| [Point clouds](xref:arcore-point-clouds) | Detect and track feature points. |
| Raycasts | Cast rays against tracked items. |
| Anchors | Track arbitrary points in space. |
| Environment probes | Generate cubemaps of the environment. |
| [Occlusion](xref:arcore-occlusion) | Occlude AR content with physical objects and perform human segmentation. |

> [!TIP]
> If your app does not use a feature, it's best practice to disable that feature. In some cases doing so may improve your app's performance.
