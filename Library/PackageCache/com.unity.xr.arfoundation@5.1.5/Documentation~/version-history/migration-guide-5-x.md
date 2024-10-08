---
uid: arfoundation-migration-guide-5-x
---
# Migration guide

This guide covers the differences between AR Foundation 4.x and 5.x.

## `ARSubsystems` package is merged into `ARFoundation`

Until now, the interfaces for AR-related subsystem were provided by [AR Subsystems package](https://docs.unity3d.com/Packages/com.unity.xr.arsubsystems@4.2?subfolder=/manual/). These interfaces have been migrated to AR Foundation package. However, all these AR-related subsystems are still using the same namespace `Unity.XR.ARSubsystems`.

### Adapting an existing project

The breaking change here is mostly package dependency. In previous versions, `ARFoundation` package was dependent on `ARSubsytems` package. This dependency is now removed. This means previous apps which have AR Subsystems package as an explicit dependency can now replace it with AR Foundation package.

To make the transition easier, we are still publishing an empty `ARSubsystems` package with dependency on `ARFoundation`. This will ensure that the project which has an explicit dependency on `ARSubsystems` package continues to work as expected.

## Texture Importer

`TextureImporterInternals.GetSourceTextureDimensions` has been removed. Use `TextureImporter.GetSourceTextureWidthAndHeight` instead.

## XRSubsystem

The [XRSubsystem](xref:UnityEngine.XR.ARSubsystems.XRSubsystem%601) is deprecated. Use [SubsystemWithProvider](xref:UnityEngine.SubsystemsImplementation.SubsystemWithProvider) instead. This is the new Subsystem base class in Unity core and it requires an implementation of [SubsystemDescriptorWithProvider](xref:UnityEngine.SubsystemsImplementation.SubsystemDescriptorWithProvider) and [SubsystemProvider](xref:UnityEngine.SubsystemsImplementation.SubsystemProvider).

- Implementing a subsystem using deprecated APIs:

```c#
public class TestSubsystemDescriptor : SubsystemDescriptor<TestSubsystem>
{ }

public class TestSubsystem : XRSubsystem<TestSubsystemDescriptor>
{
    protected override void OnStart() { }

    protected override void OnStop() { }

    protected override void OnDestroyed() { }
}
```

- Implementing a subsystem using the new APIs:

```c#
public class TestSubsystemDescriptor : SubsystemDescriptorWithProvider<TestSubsystem, TestSubsystemProvider>
{ }

public class TestSubsystem : SubsystemWithProvider<TestSubsystem, TestSubsystemDescriptor, TestSubsystemProvider>
{ }

public class TestSubsystemProvider : SubsystemProvider<TestSubsystem>
{
    public override void Start() { }

    public override void Stop() { }

    public override void Destroy() { }
}
```

## XR Origin

`ARSessionOrigin` is now deprecated and will be replaced with `XROrigin`. In order to prepare your projects for the eventual removal of `ARSessionOrigin`, make sure to follow these steps:

- Replace all references in custom scripts from `ARSessionOrigin` to `XROrigin`.
- Once the upgrade is made to `XROrigin`, change all references to properties in `ARSessionOrigin` (now `XROrigin`) from camelCase to PascalCase.
- Remove all `ARSessionOrigin` components in the project and replace them with `XROrigin`.
- If you want to convert an existing object with `ARSessionOrigin` attached, make sure that you parent the camera under the camera offset object.

![XR Origin Hierarchy](../images/xr-origin-hierarchy.png)

For more about XR Origin, see the [XR Core Utilities Package documentation](xref:xr-core-utils-xr-origin).

### Removal of [`ARSessionOrigin.MakeContentAppearAt`](https://docs.unity3d.com/Packages/com.unity.xr.arfoundation@4.2/api/UnityEngine.XR.ARFoundation.ARSessionOrigin.html#UnityEngine_XR_ARFoundation_ARSessionOrigin_MakeContentAppearAt_UnityEngine_Transform_UnityEngine_Quaternion_)
To ensure compatibility across Unity XR packages such as [XR Interaction Toolkit](https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit@latest), [`ARSessionOrigin`](xref:UnityEngine.XR.ARFoundation.ARSessionOrigin) has been reimplemented as well as deprecated, and is now derived from [`XROrigin`](xref:Unity.XR.CoreUtils.XROrigin).

[`XROrigin`](xref:Unity.XR.CoreUtils.XROrigin) does not contain equivalent methods to [`ARSessionOrigin.MakeContentAppearAt`](https://docs.unity3d.com/Packages/com.unity.xr.arfoundation@4.2/api/UnityEngine.XR.ARFoundation.ARSessionOrigin.html#UnityEngine_XR_ARFoundation_ARSessionOrigin_MakeContentAppearAt_UnityEngine_Transform_UnityEngine_Quaternion_) which appeared in older versions of AR Foundation. If you are migrating a project which uses [`MakeContentAppearAt`](https://docs.unity3d.com/Packages/com.unity.xr.arfoundation@4.2/api/UnityEngine.XR.ARFoundation.ARSessionOrigin.html#UnityEngine_XR_ARFoundation_ARSessionOrigin_MakeContentAppearAt_UnityEngine_Transform_UnityEngine_Quaternion_), you can add extension methods to your project to continue supporting this API with [`XROrigin`](xref:Unity.XR.CoreUtils.XROrigin). Example extension method implementations are provided in the [AR Foundation Samples](https://github.com/Unity-Technologies/arfoundation-samples) project at [`arfoundation-samples/Assets/Scripts/XROriginExtensions.cs`](https://github.com/Unity-Technologies/arfoundation-samples/blob/5.1/Assets/Scripts/Runtime/XROriginExtensions.cs).

## `XRDepthSubsystem` is now `XRPointCloudSubsystem`

The depth information is represented by features points which can be correlated between multiple frames. A point cloud is a set of these feature points. The `XRDepthSubsystem` was the [tracking subsystem]((xref:arsubsystems-manual#tracking-subsystems)) interface to access this data using `XRPointCloud` as its trackable.

To more accurately communicate the purpose of this subsystem, the `XRDepthSubsystem` has been renamed to [XRPointCloudSubsystem](xref:UnityEngine.XR.ARSubsystems.XRPointCloudSubsystem). The `XRDepthSubsystem` has been deprecated and will be removed in the future releases.

### Changes

* `XRDepthSubsystem` renamed to `XRPointCloudSubsystem`.
* `XRDepthSubsystemDescriptor` renamed to `XRPointCloudSubsystemDescriptor`.

## `ARCameraBackground`, `ARCameraManager`, and `XRCameraBackgroundRenderingMode`

Before, the `ARCameraBackground` and `ARCameraManager` would assume the background must be rendered before all opaque geometry. This led to unnecesary overdraw when rendering opaques over the camera background. Now, the rendering order is selectable using `ARCameraManager.requestedBackgroundRenderingMode` and `ARCameraManager.currentRenderingMode`. This enables selection between `BeforeOpaques` and `AfterOpaques` for platform provided materials.

Now you can request `Invalid` (No camera rendering/camera rendering unsupported), `Any` (let the platform decide), `BeforeOpaques` (default behavior), and `AfterOpaques` (render after opaques have finished) rendering modes and if the platform supports your requested mode then it will return the proper material to use when rendering that mode. If a platform only supports a specific mode then the platform should always have the `XRCameraSubsystem.currentRenderingMode` property return either the supported rendering mode, or `Invalid` if rendering the background is not supported at all or has been disabled internally.

`AfterOpaques` has slightly different rendering behavior beyond its render order. `AfterOpaques` causes the `ARCameraBackground` to render a fullscreen quad at the *farClipPlane* to ensure that depth testing properly culls fragments that are behind opaque geometry.

### Custom Shaders and Materials with `ARCameraBackground`

If you are using a custom material with the `ARCameraBackground` component then it is your responsibility to properly handle any material swapping when changing the render mode. To do this, subscribe to the `ARCameraBackground.OnCameraRenderingModeChanged`. The callback will happen prior to any [Command Buffer](https://docs.unity3d.com/ScriptReference/Rendering.CommandBuffer.html) configurations.

Custom material shaders will need to properly handle depth testing with [ZTest](https://docs.unity3d.com/Manual/SL-ZTest.html) when rendering in `AfterOpaques` mode so as to not improperly occlude geometry when rendering the background. `ZTest LEqual` would be proper for this use case.

### Custom Shaders and Materials with Occlusion

When handling *Occlusion* in your custom shader, you should consider that the depth buffer has already been written to by the Opaque Geometry and won't produce the correct results when rendering color and depth using `ZTest LEqual`. Your shader will need to handle sampling the occlusion texture and properly discarding fragments that fail a depth test between the current depth buffer and the occlusion texture.

You may accomplish occlusion in `AfterOpaque` rendering mode by comparing the [current camera depth texture](https://docs.unity3d.com/Manual/SL-CameraDepthTexture.html) and the occlusion depth texture values, discarding any fragments that have the current depth being "in front of" the occlusion depth. It may also be beneficial to separate this out into a different pass so that the shader branching is limited to just the fragments that have opaque geometry. Below is a psuedo-code example:

```hlsl
SubShader
{
    Pass
    {
        Name "Custom Camera Background Pass"
        ZWrite On
        // Only process fragments that are behind geometry.
        ZTest LEqual

        // ...

        // In Fragment Shader
        fragmentOut fragment(v2f in)
        {
            // ...

            fragmentOut.depth = occlusionDepth;
            fragmentOut.color = backgroundColor;
        }
    }

    Pass
    {
        Name "Custom Camera Background Occlusion Pass"
        ZWrite On
        // Only process fragments where geometry is in front of the background.
        // This limits branching to just the fragments that need it.
        ZTest Greater

        // ...

        // Inserts camera depth texture when available.
        // See https://docs.unity3d.com/Manual/SL-CameraDepthTexture.html for more information.
        UNITY_DECLARE_DEPTH_TEXTURE(_CameraDepthTexture);

        // In Fragment Shader
        fragmentOut fragment(v2f in)
        {
            // ...

            float cameraDepth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, in.uvCoords);

            // ...

            // Discard fragments that are considered behind opaque geometry.
            // Otherwise write color and depth.
            if (cameraDepth > occlusionDepth)
            {
                discard;
            }

            fragmentOut.depth = occlusionDepth;
            fragmentOut.color = backgroundColor;
        }
    }
}
```

### URP Version 14.0.2 Incompatibility

When using URP version 14.0.2 there is a bug that causes URP to not respect `RendererFeature` input requests. This means that even though the `ARCameraBackground` might request for a Camera Depth Texture, URP will not respect this request and the camera background will overwrite geometry.

To workaround this, make sure you are using URP version 14.0.3 or greater.
