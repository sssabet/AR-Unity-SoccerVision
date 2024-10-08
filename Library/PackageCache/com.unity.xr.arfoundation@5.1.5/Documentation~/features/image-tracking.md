---
uid: arfoundation-image-tracking
---
# AR Tracked Image Manager component

The tracked image manager is a type of [trackable manager](xref:arfoundation-managers#trackables-and-trackable-managers) and performs 2D image tracking.

![AR Tracked Image Manager component](../images/ar-tracked-image-manager.png)<br/>*AR Tracked Image Manager component*

The tracked image manager creates [GameObjects](xref:UnityEngine.GameObject) for each detected image in the environment. Before an image can be detected, the manager must be instructed to look for a set of reference images compiled into a reference image library. It only detects images in this library.

## Terminology

| Term | Description |
| :--- | :---------- |
| **Reference image** | A reference image is an image that the `XRImageTrackingSubsystem` attempts to find in the real world. The subsystem associates detected images with the reference image used to detect them. Each detected image has a pose in the world.|
| **Reference image library** | A set of reference images. When you start an image tracking subsystem, you must first provide it with a library of reference images so it knows what to search for.|

## Create a reference image library

You create reference image libraries in the Unity Editor. From Unity's main menu, go to **Assets** &gt; **Create** &gt; **XR** &gt; **Reference Image Library**.

This creates a new reference image library asset in your project. To add images, select this Asset, then click **Add Image** and complete the fields that appear:

![Editing a reference image library](../images/create-reference-image-library.gif)

You can specify the following information:

|**Field**|**Description**|
|------|-------|
|**Name**|A string name that is available at runtime. This name isn't used by the subsystem, but it can be useful to identify which reference image has been detected. There is no check for duplicate name conflicts.|
|**Specify Size**|If enabled, you can specify the physical size you expect the image to be in the real world. This field is mandatory for some providers and optional for others. For more information, see your provider's documentation. If you specify this field, the dimensions must be greater than zero. Editing one dimension (for example, width) causes the other dimension (height) to change automatically based on the image's aspect ratio.|
|**Keep Texture at Runtime**|If enabled, `XRReferenceImage.texture` contains a reference to the source texture. This can be useful if you need access to the source texture at runtime. By default, this is unchecked to reduce the built Player size. When unchecked, `XRReferenceImage.texture` is null.|

>[!TIP]
> You can include an `XRReferenceImageLibrary` in an AssetBundle and use it in an app that was not built with that reference image library present.

## Reference image library

The reference image library can be set at runtime, but as long as the tracked image manager component is enabled, the reference image library must be non-null. You can set it via script with:

```csharp
ARTrackedImageManager manager = ...;
manager.referenceLibrary = myReferenceImageLibrary;
```

You can set the reference image library to be either an [XRReferenceImageLibrary](xref:UnityEngine.XR.ARSubsystems.XRReferenceImageLibrary) or a [RuntimeReferenceImageLibrary](xref:UnityEngine.XR.ARSubsystems.RuntimeReferenceImageLibrary). You can only create an `XRReferenceImageLibrary` in the Editor, and you can't modify it at runtime. A `RuntimeReferenceImageLibrary` is the runtime representation of an `XRReferenceImageLibrary`. When you set the library to be an `XRReferenceImageLibrary`, the image tracking subsystem automatically converts it to a `RuntimeReferenceImageLibrary` for consumption.

The actual image library data is provider-specific; see your provider's documentation for details.

You can create a `RuntimeReferenceImageLibrary` from an `XRReferenceImageLibrary` with the [`ARTrackedImageManager.CreateRuntimeLibrary`](xref:UnityEngine.XR.ARFoundation.ARTrackedImageManager.CreateRuntimeLibrary(UnityEngine.XR.ARSubsystems.XRReferenceImageLibrary)) method:

```csharp
XRReferenceImageLibrary serializedLibrary = ...
RuntimeReferenceImageLibrary runtimeLibrary = trackedImageManager.CreateRuntimeLibrary(serializedLibrary);
```

> [!NOTE]
> The ordering of the [XRReferenceImage](xref:UnityEngine.XR.ARSubsystems.XRReferenceImage)s in the `RuntimeReferenceImageLibrary` is undefined; that is, it might not match the order in which the images appeared in the source `XRReferenceImageLibrary`. Each reference image does have a string name that you assign it, and a randomly assigned [Guid](xref:System.Guid). The `Guid` are the same between the source `XRReferenceImageLibrary` and its corresponding `RuntimeReferenceImageLibrary`.

## Use reference image libraries with AssetBundles

To include an `XRReferenceImageLibrary` in an [AssetBundle](https://docs.unity3d.com/Manual/AssetBundlesIntro.html), AR Foundation requires that you first call [ARBuildProcessor.PreprocessBuild](xref:UnityEditor.XR.ARSubsystems.ARBuildProcessor.PreprocessBuild*) before you build the AssetBundles. Platform-specific data is attached to `XRReferenceImageLibrary` assets during this preprocessor step.

Refer to the code sample below to understand how to build AssetBundles that contain reference image libraries:

[!code-cs[export_assetbundles_function](../../Tests/Editor/CodeSamples/AssetBundlesSamples.cs#export_assetbundles_function)]

## Respond to detected images

Subscribe to the ARTrackedImageManager's [trackedImagesChanged](xref:UnityEngine.XR.ARFoundation.ARTrackedImageManager.trackedImagesChanged) event to be notified whenever an image is added (that is, first detected), updated, or removed:

[!code-cs[trackedimage_subscribe_to_events](../../Tests/CodeSamples/TrackedImageSamples.cs#trackedimage_subscribe_to_events)]

Note that images also have a [tracking state](#tracking-state) which can provide additional information about the tracking quality. An image that goes out of view, for example, might not be "removed", but its tracking state likely changes.

You can also get all the currently tracked images with the ARTrackedImageManager's [trackables](xref:UnityEngine.XR.ARFoundation.ARTrackableManager`5.trackables) property. This acts like an [IEnumerable](xref:System.Collections.IEnumerable) collection, so you can use it in a `foreach` statement:

[!code-cs[trackedimage_enumerate_trackables](../../Tests/CodeSamples/TrackedImageSamples.cs#trackedimage_enumerate_trackables)]

Or access a specific image by its [TrackableId](xref:UnityEngine.XR.ARSubsystems.TrackableId):

[!code-cs[trackedimage_get_by_trackableId](../../Tests/CodeSamples/TrackedImageSamples.cs#trackedimage_get_by_trackableId)]

## Tracked Image Prefab

The [ARTrackedImageManager](xref:UnityEngine.XR.ARFoundation.ARTrackedImageManager) has a ["Tracked Image Prefab"](xref:UnityEngine.XR.ARFoundation.ARTrackedImageManager.trackedImagePrefab) field; however, this is not intended for content. When an image is detected, ARFoundation creates a new [GameObject](xref:UnityEngine.GameObject) to represent it.

If "Tracked Image Prefab" is `null`, then AR Foundation creates a GameObject with an [ARTrackedImage](xref:UnityEngine.XR.ARFoundation.ARTrackedImage) component on it. However, if you want every tracked image to also include additional components, you can provide a Prefab for AR Foundation to instantiate for each detected image. In other words, the purpose of the Prefab field is to extend the default behavior of tracked images; it is not the recommended way to place content in the world.

If you would like to [instantiate](xref:UnityEngine.Object.Instantiate(UnityEngine.Object)) content at the pose of the detected image and have its pose updated automatically, then you should parent your content to the `ARTrackedImage`.

## Add new reference images at runtime

Some subsystems might support image libraries that are modifiable at runtime. In this case, the subsystem produces a `RuntimeReferenceImageLibrary` that is a `MutableRuntimeReferenceImageLibrary`. To use it, you need to cast the `RuntimeReferenceImageLibrary` to a `MutableRuntimeReferenceImageLibrary`:

[!code-cs[trackedimage_ScheduleAddImageJob](../../Tests/CodeSamples/TrackedImageSamples.cs#trackedimage_ScheduleAddImageJob)]

To create an empty library that you can add images to later, you can call `CreateRuntimeLibrary` without arguments:

[!code-cs[trackedimage_CreateRuntimeLibrary](../../Tests/CodeSamples/TrackedImageSamples.cs#trackedimage_CreateRuntimeLibrary)]

You can check whether a particular tracked image manager supports mutable libraries with its descriptor:

[!code-cs[trackedimage_supportsMutableLibrary](../../Tests/CodeSamples/TrackedImageSamples.cs#trackedimage_supportsMutableLibrary)]

You can add images to mutable libraries allow images at any time. Adding an image can be computationally resource-intensive, and might take a few frames to complete. The [Unity Job System](https://docs.unity3d.com/Manual/JobSystem.html) is used to process images asynchronously.

To add an image to a [MutableRuntimeReferenceImageLibrary](xref:UnityEngine.XR.ARSubsystems.MutableRuntimeReferenceImageLibrary), use the [ScheduleAddImageJob](xref:UnityEngine.XR.ARFoundation.MutableRuntimeReferenceImageLibraryExtensions.ScheduleAddImageJob(UnityEngine.XR.ARSubsystems.MutableRuntimeReferenceImageLibrary,UnityEngine.Texture2D,System.String,System.Nullable{System.Single},Unity.Jobs.JobHandle)) method. This returns a [JobHandle](xref:Unity.Jobs.JobHandle) that you can use to determine when the job is complete. You can safely discard this handle if you don't need to do this.

If you use the [extension method](xref:UnityEngine.XR.ARFoundation.MutableRuntimeReferenceImageLibraryExtensions.ScheduleAddImageJob(UnityEngine.XR.ARSubsystems.MutableRuntimeReferenceImageLibrary,UnityEngine.Texture2D,System.String,System.Nullable{System.Single},Unity.Jobs.JobHandle)) which accepts a [Texture2D](xref:UnityEngine.Texture2D), you do not need to manage any memory.

If you use the version of [ScheduleAddImageJob](xref:UnityEngine.XR.ARSubsystems.MutableRuntimeReferenceImageLibrary.ScheduleAddImageJob(Unity.Collections.NativeSlice{System.Byte},UnityEngine.Vector2Int,UnityEngine.TextureFormat,UnityEngine.XR.ARSubsystems.XRReferenceImage,Unity.Jobs.JobHandle)) that accepts a [NativeSlice](xref:Unity.Collections.NativeSlice`1) or pointer, you are responsible for managing the memory, that is, freeing it when the job completes. You can do this by scheduling a dependent job that frees the memory:

[!code-cs[trackedimage_DeallocateOnJobCompletion](../../Tests/CodeSamples/TrackedImageSamples.cs#trackedimage_DeallocateOnJobCompletion)]

Multiple add image jobs can be processed concurrently. Whether or not `MutableRuntimeReferenceImageLibrary` is currently in use for image tracking has no effect on this.

### Reference images and texture import settings

Image detection algorithms rely on accurate knowledge of a reference image's aspect ratio; however, Unity's default texture import setting adjusts texture resolutions to the nearest powers of 2 (PoT). This can have a negative effect on tracking capabilities when tracking non-power of 2 (NPoT) images.

When a NPoT image is imported with default settings and added to a `MutableRuntimeReferenceImageLibrary` during runtime, it may display squashing, stretching, or scaling artifacts.

To ensure images maintain their aspect ratio when imported, select the image in your project assets and change the **Non-Power of 2** texture import setting to **None**.

>[!NOTE]
> For textures with power-of 2 aspect ratios, **Non-Power of 2** setting will not be selectable as no adjustments are necessary.

![Non-Power of Two Reference Image Settings](../images/ar-tracked-image-NPoT-texture-settings.png)

## Create a manager at runtime

When you add a component to an active `GameObject` at runtime, Unity immediately invokes its `OnEnable` method. However, the `ARTrackedImageManager` requires a non-null reference image library. If the reference image library is null when the `ARTrackedImageManager` is enabled, it automatically disables itself.

To add an `ARTrackedImageManager` at runtime, set its reference image library and then re-enable it:

```csharp
var manager = gameObject.AddComponent<ARTrackedImageManager>();
manager.referenceLibrary = myLibrary;
manager.enabled = true;
```

## Maximum number of moving images

Some providers can track moving images. This typically requires more CPU resources, so you can specify the number of moving images to track simultaneously. Check for support via the `SubsystemDescriptor` (`ARTrackedImageManager.descriptor`).

## Tracked Image Prefab

This Prefab is instantiated whenever an image from the reference image library is detected. The manager ensures the instantiated `GameObject` includes an `ARTrackedImage` component. You can get the reference image that was used to detect the `ARTrackedImage` with the `ARTrackedImage.referenceImage` property.

## Tracking State

There are three possible tracking states for `ARTrackedImages`:

| TrackingState                                                     | Description                   |
|:---------------:                                                  |:-------------                 |
| [None](xref:UnityEngine.XR.ARSubsystems.TrackingState.None)       | The image is not being tracked. Note that this may be the initial state when the image is first detected. |
| [Limited](xref:UnityEngine.XR.ARSubsystems.TrackingState.Limited) | The image is being tracked, but not as effectively. The situations in which an image is considered `Limited` instead of `Tracking` depend on the underlying AR framework. Examples that could cause `Limited` tracking include:  <ul><li>Obscuring the image so that it is not visible to the camera.</li><li>The image is not tracked as a moving image. This can happen, for example, if the `maxNumberOfMovingImages` is exceeded.</li></ul>
| [Tracking](xref:UnityEngine.XR.ARSubsystems.TrackingState.Tracking) | The underlying AR SDK reports that it is actively tracking the image. |

### Determining when an image is visible

There is no API to determine the visibility of an image. Generally, if the tracking state is [Tracking](xref:UnityEngine.XR.ARSubsystems.TrackingState.Tracking), it likely changes to `Limited` when the image is not visible. However, there are other situations in which the tracking state can be in states other than `Tracked`.

If this information is important to your application, considering comparing the `ARTrackedImage`'s transform with the camera's view frustum.
