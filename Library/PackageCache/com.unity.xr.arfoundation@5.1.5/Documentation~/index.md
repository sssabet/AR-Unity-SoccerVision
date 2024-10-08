---
uid: arfoundation-manual
---
# AR Foundation

AR Foundation enables you to create multi-platform augmented reality (AR) apps with Unity. In an AR Foundation project, you choose which AR features to enable by adding the corresponding manager components to your scene. When you build and run your app on an AR device, AR Foundation enables these features using the platform's native AR SDK, so you can create once and deploy to the world's leading AR platforms.

![A screenshot from a mobile device shows an interior office environment. Yellow polygons indicate that planes have been detected on the floor, seats, and other surfaces. The user has placed a magenta cube on one of the planes via raycasting.](images/sample-simple-ar.png)<br/>*The [Simple AR sample](https://github.com/Unity-Technologies/arfoundation-samples#simple-ar) scene shows you how to get started with plane detection and raycasting*

# Required packages

The AR Foundation package contains interfaces for AR features, but doesn't implement any features itself. To use AR Foundation on a target platform, you also need a separate *provider plug-in* package for that platform.

Unity officially supports the following provider plug-ins:

* [Google ARCore XR Plug-in](https://docs.unity3d.com/Packages/com.unity.xr.arcore@5.1/manual/index.html) on Android
* [Apple ARKit XR Plug-in](https://docs.unity3d.com/Packages/com.unity.xr.arkit@5.1/manual/index.html) on iOS
* [Apple visionOS XR Plug-in](https://docs.unity3d.com/Packages/com.unity.xr.visionos@0.6/manual/index.html) on visionOS
* [OpenXR Plug-in](https://docs.unity3d.com/Packages/com.unity.xr.openxr@1.5/manual/index.html) on HoloLens 2
* [Unity OpenXR: Meta](https://docs.unity3d.com/Packages/com.unity.xr.meta-openxr@1.0/manual/index.html) on Meta Quest

> [!NOTE]
> AR Foundation will not work on a target platform unless you also install the provider plug-in package for that platform. See [Install AR Foundation](xref:arfoundation-install) for detailed setup instructions.

# Features

AR Foundation supports the following features:

| Feature | Description |
| :------ | :---------- |
| [Session](xref:arfoundation-session) | Enable, disable, and configure AR on the target platform. |
| [Device tracking](xref:arfoundation-device-tracking) | Track the device's position and rotation in physical space. |
| [Camera](xref:arfoundation-camera) | Render images from device cameras and perform light estimation. |
| [Plane detection](xref:arfoundation-plane-detection) | Detect and track flat surfaces. |
| [Image tracking](xref:arfoundation-image-tracking) | Detect and track 2D images. |
| [Object tracking](xref:arfoundation-object-tracking) | Detect and track 3D objects. |
| [Face tracking](xref:arfoundation-face-tracking) | Detect and track human faces. |
| [Body tracking](xref:UnityEngine.XR.ARFoundation.ARHumanBodyManager) | Detect and track a human body. |
| [Point clouds](xref:arfoundation-point-clouds) | Detect and track feature points. |
| [Raycasts](xref:arfoundation-raycasts) | Cast rays against tracked items. |
| [Anchors](xref:arfoundation-anchors) | Track arbitrary points in space. |
| [Meshing](xref:arfoundation-meshing) | Generate meshes of the environment. |
| [Environment probes](xref:arfoundation-environment-probes) | Generate cubemaps of the environment. |
| [Occlusion](xref:arfoundation-occlusion) | Occlude AR content with physical objects and perform human segmentation. |
| [Participants](xref:arfoundation-participant-tracking) | Track other devices in a shared AR session. |

## Platform support

AR Foundation provider plug-ins rely on platform implementations of AR features, such as Google's ARCore on Android and Apple's ARKit on iOS. Not all features are available on all platforms.

The table below lists the available features in each Unity-supported provider plug-in:

<table>
  <tr>
  	<td rowspan="2" style="vertical-align: bottom; background-color: #ffffff;"><strong>Feature</strong></td>
  	<td style="text-align: center">ARCore</td>
  	<td colspan="2" style="text-align: center">ARKit</td>
  	<td colspan="2" style="text-align: center">OpenXR</td>
  </tr>
  <tr style="border-bottom: 2px solid #dddddd">
  	<th style="text-align: center">Android</th>
  	<th style="text-align: center">iOS</th>
  	<th style="text-align: center">visionOS</th>
  	<th style="text-align: center">HoloLens</th>
  	<th style="text-align: center">Meta Quest</th>
  </tr>
  <tr>
  	<td><a href="features/session.md">Session</a></td>
  	<td style="text-align: center">Yes</td> <!-- Android -->
  	<td style="text-align: center">Yes</td> <!-- iOS -->
  	<td style="text-align: center">Yes</td> <!-- visionOS -->
  	<td style="text-align: center">Yes</td> <!-- HoloLens -->
  	<td style="text-align: center">Yes</td> <!-- Meta Quest -->
  </tr>
  <tr>
  	<td><a href="features/device-tracking.md">Device tracking</a></td>
  	<td style="text-align: center">Yes</td> <!-- Android -->
  	<td style="text-align: center">Yes</td> <!-- iOS -->
  	<td style="text-align: center">Yes</td> <!-- visionOS -->
  	<td style="text-align: center">Yes</td> <!-- HoloLens -->
  	<td style="text-align: center">Yes</td> <!-- Meta Quest -->
  </tr>
  <tr>
  	<td><a href="features/Camera/camera.md">Camera</a></td>
  	<td style="text-align: center">Yes</td> <!-- Android -->
  	<td style="text-align: center">Yes</td> <!-- iOS -->
  	<td style="text-align: center"></td>    <!-- visionOS -->
  	<td style="text-align: center"></td>    <!-- HoloLens -->
  	<td style="text-align: center">Yes</td> <!-- Meta Quest -->
  </tr>
  <tr>
  	<td><a href="features/plane-detection/plane-detection.md">Plane detection</a></td>
  	<td style="text-align: center">Yes</td> <!-- Android -->
  	<td style="text-align: center">Yes</td> <!-- iOS -->
  	<td style="text-align: center">Yes</td> <!-- visionOS -->
  	<td style="text-align: center">Yes</td> <!-- HoloLens -->
  	<td style="text-align: center">Yes</td> <!-- Meta Quest -->
  </tr>
  <tr>
  	<td><a href="features/image-tracking.md">Image tracking</a></td>
  	<td style="text-align: center">Yes</td> <!-- Android -->
  	<td style="text-align: center">Yes</td> <!-- iOS -->
  	<td style="text-align: center">Yes</td> <!-- visionOS -->
  	<td style="text-align: center"></td>    <!-- HoloLens -->
  	<td style="text-align: center"></td>    <!-- Meta Quest -->
  </tr>
  <tr>
  	<td><a href="features/object-tracking.md">Object tracking</a></td>
  	<td style="text-align: center"></td>    <!-- Android -->
  	<td style="text-align: center">Yes</td> <!-- iOS -->
  	<td style="text-align: center"></td>    <!-- visionOS -->
  	<td style="text-align: center"></td>    <!-- HoloLens -->
  	<td style="text-align: center"></td>    <!-- Meta Quest -->
  </tr>
  <tr>
  	<td><a href="features/face-tracking.md">Face tracking</a></td>
  	<td style="text-align: center">Yes</td> <!-- Android -->
  	<td style="text-align: center">Yes</td> <!-- iOS -->
  	<td style="text-align: center"></td>    <!-- visionOS -->
  	<td style="text-align: center"></td>    <!-- HoloLens -->
  	<td style="text-align: center"></td>    <!-- Meta Quest -->
  </tr>
  <tr>
  	<td><a href="features/body-tracking.md">Body tracking</a></td>
  	<td style="text-align: center"></td>    <!-- Android -->
  	<td style="text-align: center">Yes</td> <!-- iOS -->
  	<td style="text-align: center"></td>    <!-- visionOS -->
  	<td style="text-align: center"></td>    <!-- HoloLens -->
  	<td style="text-align: center"></td>    <!-- Meta Quest -->
  </tr>
  <tr>
  	<td><a href="features/point-clouds.md">Point clouds</a></td>
  	<td style="text-align: center">Yes</td> <!-- Android -->
  	<td style="text-align: center">Yes</td> <!-- iOS -->
  	<td style="text-align: center"></td>    <!-- visionOS -->
  	<td style="text-align: center"></td>    <!-- HoloLens -->
  	<td style="text-align: center"></td>    <!-- Meta Quest -->
  </tr>
  <tr>
  	<td><a href="features/raycasts.md">Raycasts</a></td>
  	<td style="text-align: center">Yes</td> <!-- Android -->
  	<td style="text-align: center">Yes</td> <!-- iOS -->
  	<td style="text-align: center"></td>    <!-- visionOS -->
  	<td style="text-align: center">Yes</td> <!-- HoloLens -->
  	<td style="text-align: center">Yes</td> <!-- Meta Quest -->
  </tr>
  <tr>
  	<td><a href="features/anchors.md">Anchors</a></td>
  	<td style="text-align: center">Yes</td> <!-- Android -->
  	<td style="text-align: center">Yes</td> <!-- iOS -->
  	<td style="text-align: center">Yes</td> <!-- visionOS -->
  	<td style="text-align: center">Yes</td> <!-- HoloLens -->
  	<td style="text-align: center">Yes</td> <!-- Meta Quest -->
  </tr>
  <tr>
  	<td><a href="features/meshing.md">Meshing</a></td>
  	<td style="text-align: center"></td>    <!-- Android -->
  	<td style="text-align: center">Yes</td> <!-- iOS -->
  	<td style="text-align: center">Yes</td> <!-- visionOS -->
  	<td style="text-align: center">Yes</td> <!-- HoloLens -->
  	<td style="text-align: center"></td>    <!-- Meta Quest -->
  </tr>
  <tr>
  	<td><a href="features/environment-probes.md">Environment probes</a></td>
  	<td style="text-align: center">Yes</td> <!-- Android -->
  	<td style="text-align: center">Yes</td> <!-- iOS -->
  	<td style="text-align: center"></td>    <!-- visionOS -->
  	<td style="text-align: center"></td>    <!-- HoloLens -->
  	<td style="text-align: center"></td>    <!-- Meta Quest -->
  </tr>
  <tr>
  	<td><a href="features/occlusion.md">Occlusion</a></td>
  	<td style="text-align: center">Yes</td> <!-- Android -->
  	<td style="text-align: center">Yes</td> <!-- iOS -->
  	<td style="text-align: center"></td>    <!-- visionOS -->
  	<td style="text-align: center"></td>    <!-- HoloLens -->
  	<td style="text-align: center"></td>    <!-- Meta Quest -->
  </tr>
  <tr>
  	<td><a href="features/participant-tracking.md">Participants</a></td>
  	<td style="text-align: center"></td>    <!-- Android -->
  	<td style="text-align: center">Yes</td> <!-- iOS -->
  	<td style="text-align: center"></td>    <!-- visionOS -->
  	<td style="text-align: center"></td>    <!-- HoloLens -->
  	<td style="text-align: center"></td>    <!-- Meta Quest -->
  </tr>
</table>

[!include[](snippets/arf-docs-tip.md)]

# Samples

For pre-configured sample scenes that demonstrate how to use each feature, see the [AR Foundation Samples](https://github.com/Unity-Technologies/arfoundation-samples) GitHub repository.

[!include[](snippets/apple-arkit-trademark.md)]
