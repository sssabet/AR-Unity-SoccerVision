---
uid: arfoundation-device-tracking
---
# Device tracking

After following the AR Foundation [scene setup](xref:arfoundation-scene-setup) instructions, your scene contains a preconfigured GameObject named "XR Origin". The XR Origin GameObject automatically handles device tracking and transforming trackables into Unity's coordinate system via its [XROrigin](xref:Unity.XR.CoreUtils.XROrigin) component and GameObject hierarchy with a Camera and [TrackedPoseDriver](xref:UnityEngine.InputSystem.XR.TrackedPoseDriver), as detailed below.

# XR Origin component

The [XROrigin](xref:Unity.XR.CoreUtils.XROrigin) component transforms trackable features, such as planar surfaces and feature points, into their final position, rotation, and scale in a scene.

![XR Origin component](../images/xr-origin.png)<br/>*XR Origin component*

## Session space vs world space

The [XROrigin](xref:Unity.XR.CoreUtils.XROrigin) component transforms trackables from an AR device's "session space", which is an unscaled space relative to the beginning of the AR session, into Unity world space. For example, session space (0, 0, 0) refers to the position at which the AR session was created, and corresponds to the position of the XR Origin in Unity world space.

This concept is similar to the difference between "model" or "local" space and world space when working with other Assets in Unity. For instance, if you import a house asset from a DCC tool, the door's position is relative to the modeler's origin. This is commonly called "model space" or "local space". When Unity instantiates it, it also has a world space relative to Unity's origin.

## GameObject hierarchy

Note that the XR Origin is created with two child GameObjects named "Camera Offset" and "Main Camera". Each of these GameObjects and their Transforms are significant.

When your application runs on device, the position and rotation of the XR Origin reflect the initial position and rotation of the user's device at application start time. The XR Origin does not move during an AR session, but the [TrackedPoseDriver](xref:UnityEngine.InputSystem.XR.TrackedPoseDriver) component on its Main Camera GameObject will automatically update the Main Camera's position and rotation to match those of the AR device as it moves through the physical environment.

The [XROrigin](xref:Unity.XR.CoreUtils.XROrigin) component uses the Camera Offset GameObject to apply the **Camera Y Offset** value when its **Tracking Origin Mode** is set to **Device**. You can apply additional camera offsets by inserting another GameObject between XR Origin and Camera Offset.

At runtime, the [XROrigin](xref:Unity.XR.CoreUtils.XROrigin) component creates another child GameObject named "Trackables". When any [managers](xref:arfoundation-managers) detect and add new trackables to a scene, they will instantiate them as children of the Trackables GameObject. As child GameObjects, instantiated trackables and their positions, rotations, and scales are defined relative to the XR Origin.

## Scale

[XROrigin](xref:Unity.XR.CoreUtils.XROrigin) also allows you to scale virtual content. To apply scale to the XR Origin, set the scale of its Transform component using either the Inspector or the [transform](xref:UnityEngine.Component.transform) property. This has the effect of scaling all the data coming from the device, including the Main Camera's position and any detected trackables. Larger values make AR content appear smaller, and vice versa. For example, a scale of 10 would make your content appear 10 times smaller, while 0.1 would make your content appear 10 times larger.

## Device targets

[XROrigin](xref:Unity.XR.CoreUtils.XROrigin) is a shared class between AR Foundation and the [XR Interaction Toolkit](https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit@2.1/manual/index.html) package. XR Interaction Toolkit is an optional additional package that can used to build AR experiences with Unity.

The default [XROrigin](xref:Unity.XR.CoreUtils.XROrigin) in AR Foundation is pre-configured for mobile AR experiences; however, if you wish to target AR HMD's with handheld controller inputs, you can [install XR Interaction Toolkit](https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit@2.1/manual/installation.html) for additional [XROrigin](xref:Unity.XR.CoreUtils.XROrigin) configurations. For more information on [XROrigin](xref:Unity.XR.CoreUtils.XROrigin) configurations, see the full [XR Origin component documentation](https://docs.unity3d.com/Packages/com.unity.xr.core-utils@2.1/manual/xr-origin.html).
