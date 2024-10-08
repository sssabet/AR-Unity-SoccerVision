---
uid: arfoundation-occlusion-manager
---
# AR Occlusion Manager component

The occlusion manager exposes per-frame images representing depth or stencil images. Incorporating depth images into the rendering process achieves realistic blending of augmented and real-world content by ensuring physical objects occlude virtual content located behind them in shared AR space.

Add the [AROcclusionManager](xref:UnityEngine.XR.ARFoundation.AROcclusionManager) component to a [Camera](xref:arfoundation-camera) with the [ARCameraBackground](xref:arfoundation-camera-components) component to automatically enable the background rendering pass to incorporate any available depth information when rendering the depth buffer. To add the  `ARCameraBackground` and `AROcclusionManager` to your camera, follow the steps outlined in [Managers](xref:arfoundation-managers).

![AR Occlusion Manager component](../../images/ar-occlusion-manager.png)<br/>*AR Occlusion Manager component*

## Supported depth images

The types of depth images supported are:

| Type of depth image | Description |
| :------- | :---------- |
| **Environment Depth** | Distance from the device to any part of the environment in the camera field of view. |
| **Human Depth** | Distance from the device to any part of a human recognized within the camera field of view. |
| **Human Stencil** | Value designating, for each pixel, whether that pixel contains a recognized human. |

## AR Occlusion Manager properties

The following table describes the properties of the AR Occlusion Manager component:

| Property | Description |
| :------- | :---------- |
| **Environment Depth Mode** | The mode for generating the environment depth texture. There are four options: <ul><li><strong>Disabled:</strong> No environment depth texture produced.</li><li><strong>Fastest:</strong> Minimal rendering quality. Minimal frame computation.</li><li><strong>Medium:</strong> Medium rendering quality. Medium frame computation.</li><li><strong>Best:</strong> Best rendering quality. Increased frame computation.</li></ul> |
| **Temporal Smoothing** | Whether temporal smoothing should be applied to the environment depth image, if supported on the chosen platform. Temporal smoothing reduces noise to provide smoother images and more consistent occlusion. |
| **Human Segmentation Stencil Mode** | The mode for generating human segmentation stencil texture: <ul><li><strong>Disabled:</strong> No human stencil texture produced.</li><li><strong>Fastest:</strong> Minimal rendering quality. Minimal frame computation.</li><li><strong>Medium:</strong> Medium rendering quality. Medium frame computation.</li><li><strong>Best:</strong> Best rendering quality. Increased frame computation.</li></ul> |
| **Human Segmentation Depth Mode** | The mode for generating human segmentation depth texture: <ul><li><strong>Disabled:</strong> No human stencil texture produced.</li><li><strong>Fastest:</strong> Minimal rendering quality. Minimal frame computation.</li><li><strong>Best:</strong> Best rendering quality. Increased frame computation.</li></ul> |
| **Occlusion Preference Mode** | If both environment texture and human stencil & depth textures are available, this mode specifies which should be used for occlusion. There are three options: <ul><li><strong>Prefer Environment Occlusion </li><li><strong>Prefer Human Occlusion</li><li><strong>No Occlusion</strong></li></ul> |

> [!NOTE]
> Use **Occlusion Preference Mode** to choose the type of depth image to use on ARKit. Human stencil depth images are only supported on ARKit. Environment and depth images are never simultaneously enabled.
