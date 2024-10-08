---
uid: arcore-changelog
---
# Changelog

All notable changes to this package will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/en/1.0.0/)
and this project adheres to [Semantic Versioning](http://semver.org/spec/v2.0.0.html).

## [5.1.5] - 2024-05-31

### Changed

- Changed the AR Foundation dependency version from 5.1.4 to 5.1.5.

## [5.1.4] - 2024-04-29

### Changed

- Upgraded ARCore SDK version to 1.42 from 1.31.
- Changed the AR Foundation dependency version from 5.1.3 to 5.1.4.

### Fixed

- Fixed an issue where the tvOS build target would have compile errors in `ARCore.InputLayoutLoader`.
- Fixed an issue where the AR background was rendered with incorrect rotation on Pixel Fold devices when the device was in unfolded mode.

## [5.1.3] - 2024-03-12

### Changed

- Changed the AR Foundation dependency version from 5.1.2 to 5.1.3

### Fixed

- Fixed issue [ARCB-21](https://issuetracker.unity3d.com/issues/google-arcore-error-isnt-fixed-when-pressing-the-fix-button-in-project-validation) where 'Fix' button on Project Validation rule requiring OpenGLES3 would not fix the issue if Auto Graphics API was enabled.

## [5.1.2] - 2024-01-22

### Changed

- Changed the image tracking documentation to reference the newly updated [Use reference image libraries with AssetBundles](xref:arfoundation-image-tracking#use-reference-image-libraries-with-assetbundles) manual in AR Foundation.
- Changed the project validation rules to remove a warning if this plug-in was not enabled for the Android build target. There are legitimate reasons to build for Android without enabling the ARCore plug-in, so this warning was not helpful.
- Changed AR Foundation dependency version to 5.1.2.

### Fixed

- Fixed issue [ARFB-322](https://issuetracker.unity3d.com/issues/android-crash-on-android-when-artrackedimagemanager-component-is-added-to-the-ar-session-origin-game-object) where an image library that had an image(s) score too low with the [arcoreimg](https://developers.google.com/ar/develop/augmented-images/arcoreimg) tool, or fail to score at all, would cause a crash at runtime.

## [5.1.1] - 2023-11-14

### Changed

- Updated AR Foundation dependency to version 5.1.1.

## [5.1.0] - 2023-08-30

### Changed

- Updated AR Foundation dependency to version 5.1.0.

## [5.1.0-pre.10] - 2023-07-21

No changes

## [5.1.0-pre.9] - 2023-07-06

### Fixed

- Fixed issue [ARCB-11](https://issuetracker.unity3d.com/issues/ar-foundation-gcalloc-is-using-1-dot-8kb-when-built-on-android) where unnecessary memory was allocated when requesting Camera permissions. This fix requires Unity Editor version 2022.2 or newer.

## [5.1.0-pre.8] - 2023-06-09

### Changed

- Changed the project validation rule to update the minimum Android SDK version to 23 in Unity 2023.2 or newer. 

### Fixed

- Fixed issue [ARFB-283](https://issuetracker.unity3d.com/issues/autofocusrequested-can-not-automatically-focus-after-configuring-camera-settings-on-a-device) where changing [ARCameraManager.currentConfiguration](xref:UnityEngine.XR.ARFoundation.ARCameraManager.currentConfiguration) could disable some ARCore features, including autofocus.

## [5.1.0-pre.6] - 2023-05-23

No changes

## [5.1.0-pre.4] - 2023-04-11

### Changed

- Changed Project Validation message for enabling Google ARCore Plug-in to not assume that you are targeting ARCore.

### Fixed

- Fixed issues [ARFB-251](https://issuetracker.unity3d.com/issues/autofocus-stops-working-on-android-when-arsession-dot-reset-is-triggered) and [ARFB-258](https://issuetracker.unity3d.com/issues/camera-feed-turns-off-when-arsession-dot-reset-is-called) where [ARSession.Reset](xref:UnityEngine.XR.ARFoundation.ARSession.Reset) could have unintended side effects such as disabling the device camera's autofocus.

## [5.1.0-pre.3] - 2023-02-06

### Fixed

- Fixed [Issue #1017](https://github.com/Unity-Technologies/arfoundation-samples/issues/1017) in the `ARCoreBackgroundAfterOpaques` shader where some variables were not scoped to the vertex shader correctly.

## [5.1.0-pre.2] - 2022-11-01

No changes

## [5.1.0-pre.1] - 2022-10-06

No changes

## [5.0.2] - 2022-09-11

### Fixed

- Fixed a bug where the `HandheldARInputDevice` did not appear in the Editor's InputActions UI. Now, if you are manually configuring the **Tracked Pose Driver** component on the camera under XR Origin, you can select **AR Handheld Device > devicePosition** and **AR Handheld Device > deviceRotation** for position and rotation input bindings respectively.

## [5.0.0-pre.13] - 2022-06-28

No changes

## [5.0.0-pre.12] - 2022-05-19

### Added

- Added support for changing the Camera Background rendering order so that the background can be rendered either `BeforeOpaques` or `AfterOpaques` by setting the `ARCoreCameraSubsystem.requestedRenderingMode`. Shader code for rendering `AfterOpaques` can be found in `com.unity.xr.ARCore/Assets/ARCoreBackgroundAfterOpaque.shader`.
- Added Project Validation for assessing project setup correctness. See [Project Validation manual](xref:arcore-project-config) for details.

### Changed

- Updated to ARCore 1.31.
- Updated to use the new 16-bit depth APIs introduced in ARCore SDK 1.31. This increases the depth range in ARCore from 8.191 meters to 65.535 meters.

### Deprecated

- `ARCoreCameraSubsystem.backgroundShaderName` has been marked obsolete. Instead use `ARCoreCameraSubsystem.backgroundShaderNames` to receive an array of available shader names.
- Deprecated the following depth subsystem APIs in favor of point cloud subsystem APIs. Unity's API Updater should automatically convert any deprecated APIs references to the new APIs when the project is loaded into the Editor again.
  - `ARCoreXRDepthSubsystem` has been renamed to `ARCoreXRPointCloudSubsystem`.
  - `ARCoreLoader.depthSubsystem` has been deprecated. Use `ARCoreLoader.pointCloudSubsystem` instead.

### Fixed

- Fixed `TryAddRaycast` api in AR Raycast provider where only the first hit result in the list was processed after performing a raycast. Now all the hit results are processed. This should not have any impact on user facing APIs since ARCore SDK always returns a single `ArHitResult` in the list.

## [5.0.0-pre.9] - 2022-03-01

### Added

- Added support for a new [OcclusionPreferenceMode.NoOcclusion](xref:UnityEngine.XR.ARSubsystems.OcclusionPreferenceMode) mode that, when set, disables occlusion rendering on the camera background when using [ARCameraBackground](xref:UnityEngine.XR.ARFoundation.ARCameraBackground) and [AROcclusionManager](xref:UnityEngine.XR.ARFoundation.AROcclusionManager).

- Added the method `CancelPermissionRequest` in [ARCorePermissionManager](xref:UnityEngine.XR.ARCore.ARCorePermissionManager) for users who want to do permission handling on their own. This addresses [issue 1387507](https://issuetracker.unity3d.com/issues/arfoundation-camera-request-permission-window-is-not-shown-when-app-is-minimized-and-opened-again-on-android-device).

### Fixed

- Removed an errant bit mask that was being applied to the occlusion `XRCpuImage`s.

## [5.0.0-pre.8] - 2022-02-09

### Fixed

- Fixed a bug where the list of configurations per sesssion would not clear even when the [ARCoreSessionSubsystem](xref:UnityEngine.XR.ARCore.ARCoreSessionSubsystem) was deinitialized.
- Fixed a [memory leak](https://issuetracker.unity3d.com/issues/arcore-android-memory-leak-in-xr-application-when-using-arocclusionmanager-component) in the [AROcclusionManager](xref:UnityEngine.XR.ARFoundation.AROcclusionManager).

## [5.0.0-pre.7] - 2021-12-10

### Added

- Added a warning which is shown in the console when an Android app is built with only 32-bit architecture.

## [5.0.0-pre.6] - 2021-11-17

No changes

## [5.0.0-pre.5] - 2021-10-28

### Changed

- The minimum Unity version for this package is now 2021.2.

### Removed

- Removed deprecated APIs: `ARCorePlaneProvider` and `ARCoreImageTrackingProvider`.

### Fixed

- Fixed a missing dependency on built-in [UnityWebRequest](https://docs.unity3d.com/2021.2/Documentation/ScriptReference/Networking.UnityWebRequest.html) module.

## [4.2.0] - 2021-08-11

No changes

## [4.2.0-pre.12] - 2021-07-08

### Added

- Added methods to get the [raw](xref:UnityEngine.XR.ARSubsystems.XROcclusionSubsystem.TryAcquireRawEnvironmentDepthCpuImage(UnityEngine.XR.ARSubsystems.XRCpuImage@)) and [smoothed](xref:UnityEngine.XR.ARSubsystems.XROcclusionSubsystem.TryAcquireSmoothedEnvironmentDepthCpuImage(UnityEngine.XR.ARSubsystems.XRCpuImage@)) depth images independently.
- Added support for the [depthSensorSupported](xref:UnityEngine.XR.ARSubsystems.XRCameraConfiguration.depthSensorSupported) flag on the `XRCameraConfiguration` to indicate whether or not a camera configuration supports using a depth sensor.

### Fixed

- Fixed an issue where Occlusion shaders would fail compilation on certain devices.
- Fixed a crash that would occur when loading images into a `MutableRuntimeReferenceImageLibrary` at runtime. The issue tracker can be found [here](https://issuetracker.unity3d.com/issues/arfoundation-arcore-android-il2cpp-runtime-crash-when-initializing-mutableruntimereferenceimagelibrary).

## [4.2.0-pre.10] - 2021-06-28

No changes

## [4.2.0-pre.9] - 2021-05-27

### Fixed

- Fixed a crash (which could also appear as a hang) when pausing or resuming a session in a development build.

## [4.2.0-pre.8] - 2021-05-20

No changes

## [4.2.0-pre.7] - 2021-05-13

### Added

- Added support for [ARBuildProcessor](xref:UnityEditor.XR.ARSubsystems.ARBuildProcessor).
- Added support for the [TrackableType.Depth](xref:UnityEngine.XR.ARSubsystems.TrackableType) raycast result. This allows you to raycast against the depth map on devices that support depth.
- Added support for ARCore Raw Depth and Raw Depth Confidence images in both texture and [XRCpuImage](xref:UnityEngine.XR.ARSubsystems.XRCpuImage) formats.

### Changed

- Updated to ARCore 1.24.

### Fixed

- Fixed delay with switching to front-facing camera at the start/resuming of an `ARSession`.

## [4.2.0-pre.5] - 2021-04-07

No changes

## [4.2.0-pre.4] - 2021-03-19

### Fixed

- Fixed a regression introduced in 4.2.0-pre.3 which caused the session to fail if it was started in a user-facing camera direction.

## [4.2.0-pre.3] - 2021-03-15

### Changed

- Update to ARCore 1.23.

### Fixed

- Fixed possible crash when converting [CPU camera images](xref:UnityEngine.XR.ARSubsystems.XRCpuImage) to RGB color formats.

## [4.2.0-pre.2] - 2021-03-03

### Added

- When building an Android Player with ARCore enabled, this package will check the Gradle version and warn if it determines the Gradle version is too low. The option to suppress this check and resulting warning notification can now be affected programmatically with a new public API [ARCoreSettings.ignoreGradleVersion](xref:UnityEditor.XR.ARCore.ARCoreSettings.ignoreGradleVersion).
- Added support for [session recording and playback](xref:arcore-session-recording).

### Changed

- The following properties have been deprecated:
  - `ArSession.IsNull` => Compare to `null` instead.
  - `ArSession.Null` => Use [`default`](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/operators/default) instead.
  - `ArConfig.IsNull` => Compare to `null` instead.
  - `ArConfig.Null` => Use [`default`](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/operators/default) instead.
  - `ArCameraConfig.IsNull` => Compare to `null` instead.
  - `ArCameraConfig.Null` => Use [`default`](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/operators/default) instead.
  - `ArCameraConfigFilter.IsNull` => Compare to `null` instead.
  - `ArCameraConfigFilter.Null` => Use [`default`](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/operators/default) instead.
- Updated [XR Plug-in Management](https://docs.unity3d.com/Packages/com.unity.xr.management@4.0) dependency to 4.0.
- Updated [known limitations](xref:arcore-manual#known-limitations) to clarify depth image support behavior.
- The minimum Unity version for this package is now 2020.3.

### Fixed

- Improved how this package handles spaces in path names to fix an issue that caused the build to fail when Unity builds the reference image library.
- You can now use reference images that are located outside the `Assets` folder.
- Fixed an issue where the [plane detection mode](xref:UnityEngine.XR.ARSubsystems.PlaneDetectionMode) reported incorrectly. The [requestedPlaneDetectionMode](xref:UnityEngine.XR.ARSubsystems.XRPlaneSubsystem.requestedPlaneDetectionMode) and [currentPlaneDetectionMode](xref:UnityEngine.XR.ARSubsystems.XRPlaneSubsystem.currentPlaneDetectionMode) incorrectly reported that vertical plane detection is enabled or requested when only horizontal plane detection is enabled or requested.
- Excluded tests from scripting API docs.

## [4.1.3] - 2021-01-05

### Added

- ARCore requires Gradle version 5.6.4. When building the Android Player, this package now detects the version of Gradle in use and, if an incompatible version is detected, presents a dialog indicating that the Gradle version is insufficient. This dialog can be suppressed, and the option to do so is saved in the [ARCore Settings asset](xref:UnityEditor.XR.ARCore.ARCoreSettings) (Edit > Project Settings > XR Plug-in Management > ARCore).

### Changed

- Update ARCore to version 1.22.

### Fixed

- When building for Android, this package would always update the Android manifest with entries necessary for ARCore to function, even when ARCore was not enabled via [XR Plug-in Management](https://docs.unity3d.com/Packages/com.unity.xr.management@3.2). This has been fixed so that the Android manifest is not changed unless ARCore is enabled.
- When any property of an [ARCoreSettings](xref:UnityEditor.XR.ARCore.ARCoreSettings) is changed, the asset is correctly marked dirty.

## [4.1.1] - 2020-11-11

### Changed

- Released package for Unity 2021.1

## [4.1.0-preview.13] - 2020-11-09

### Changed

- Switch to using all 16-bits from the pixels in the ARCore environment depth image, rather just the lower 13 bits.

### Fixed

- Fix crash when performing a synchronous [XRCpuImage conversion](xref:UnityEngine.XR.ARSubsystems.XRCpuImage.Convert(UnityEngine.XR.ARSubsystems.XRCpuImage.ConversionParams,System.IntPtr,System.Int32)) while an [asynchronous conversion](xref:UnityEngine.XR.ARSubsystems.XRCpuImage.ConvertAsync(UnityEngine.XR.ARSubsystems.XRCpuImage.ConversionParams)) is already running.

## [4.1.0-preview.12] - 2020-11-02

### Added

- Support the [NotTrackingReason](xref:UnityEngine.XR.ARSubsystems.NotTrackingReason): [CameraUnavailable](xref:UnityEngine.XR.ARSubsystems.NotTrackingReason.CameraUnavailable). This maps to ARCore's [AR_TRACKING_FAILURE_REASON_CAMERA_UNAVAILABLE](https://developers.google.com/ar/reference/c/group/shared-types#group__shared__types_1gga9e2ef6b4a95c498672e80760a254edbeaeb94796b7b4b0c226744759d570f0934).

## [4.1.0-preview.11] - 2020-10-22

### Added

- Added support for the new method [ScheduleAddImageWithValidationJob](xref:UnityEngine.XR.ARSubsystems.MutableRuntimeReferenceImageLibrary.ScheduleAddImageWithValidationJob(Unity.Collections.NativeSlice{System.Byte},UnityEngine.Vector2Int,UnityEngine.TextureFormat,UnityEngine.XR.ARSubsystems.XRReferenceImage,Unity.Jobs.JobHandle)) on the [MutableRuntimeReferenceImageLibrary](xref:UnityEngine.XR.ARSubsystems.MutableRuntimeReferenceImageLibrary).

### Changed

- The implementation of [XRCpuImage.ConvertAsync](xref:UnityEngine.XR.ARSubsystems.XRCpuImage.ConvertAsync(UnityEngine.XR.ARSubsystems.XRCpuImage.ConversionParams)) is now multithreaded across all hardware cores to produce the result faster. Previously, only the [synchronous version](xref:UnityEngine.XR.ARSubsystems.XRCpuImage.Convert(UnityEngine.XR.ARSubsystems.XRCpuImage.ConversionParams,System.IntPtr,System.Int32)) was multithreaded. However, on newer devices with high camera resolutions, the single threaded asynchronous conversion would often take multiple frames to complete. Now, both synchronous and asynchronous conversions are multithreaded.

## [4.1.0-preview.10] - 2020-10-12

### Added

- Add a new [beforeGetCameraConfiguration event](xref:UnityEngine.XR.ARCore.ARCoreCameraSubsystem.beforeGetCameraConfiguration) to the [ARCoreCameraSubsystem](xref:UnityEngine.XR.ARCore.ARCoreCameraSubsystem) which allows you to manipulate the [ArCameraConfigFilter](https://developers.google.com/ar/reference/c/group/ar-camera-config-filter) before calls to [ArSession_getSupportedCameraConfigsWithFilter](https://developers.google.com/ar/reference/c/group/ar-session#arsession_getsupportedcameraconfigswithfilter).

### Changed

- Move pause to a background thread. Previously, pausing the session was an expensive operation (up to 700ms). Now, pausing the session does not block the main thread.
- Update [XR Plug-in Management](https://docs.unity3d.com/Packages/com.unity.xr.management@3.2) to 3.2.16.

### Fixed

- Fixed a bug which failed to supply an [XRReferenceImage's](xref:UnityEngine.XR.ARSubsystems.XRReferenceImage) physical dimensions to ARCore when the reference image was added at runtime, e.g., with [ScheduleAddImageJob](xref:UnityEngine.XR.ARSubsystems.MutableRuntimeReferenceImageLibrary.ScheduleAddImageJob(Unity.Collections.NativeSlice{System.Byte},UnityEngine.Vector2Int,UnityEngine.TextureFormat,UnityEngine.XR.ARSubsystems.XRReferenceImage,Unity.Jobs.JobHandle)).

## [4.1.0-preview.9] - 2020-09-25

### Added

- Added support for [`ARRaycast`](https://docs.unity3d.com/Packages/com.unity.xr.arfoundation@4.1/api/UnityEngine.XR.ARFoundation.ARRaycast.html) trackables. To learn more about the underlying ARCore API see [Google's ARCore Docs](https://developers.google.com/ar/reference/unity/class/GoogleARCore/InstantPlacementPoint).

### Changed

- Eliminated unnecessary copying of background texture when running with single-threaded rendering.
- Update ARCore to version 1.19.
- Updating dependency on com.unity.xr.management to 3.2.15.

### Fixed

- Fix links in documentation.
- Handle non-unit scale in the background shader when calculating depth for environmental occlusion.
- Fix unhandled exception when processing reference image libraries. This did not prevent the generation of the image libraries, but may have adversely affected error reporting.

## [4.1.0-preview.7] - 2020-08-26

No changes

## [4.1.0-preview.6] - 2020-07-27

### Added

- Added Depth option to [ARCoreSettings](xref:UnityEditor.XR.ARCore.ARCoreSettings). This indicates whether or not ARCore Depth API support is required for a specific application.  See the [ARCore Depth API Docs](https://developers.google.com/ar/develop/java/depth/overview) for more details.

### Fixed

- Fixed a compilation error when the [Input System package](https://docs.unity3d.com/Packages/com.unity.inputsystem@1.0/manual/index.html) is present.
- Fixed an issue where ARCore shaders could incorrectly remain in the [Preloaded Assets](xref:UnityEditor.PlayerSettings.GetPreloadedAssets) array, which could interfere with builds on other platforms.

## [4.1.0-preview.5] - 2020-07-16

### Fixed

- Minor documentation fixes
- Fixed a regression that caused the [beforeSetConfiguration](xref:UnityEngine.XR.ARCore.ARCoreSessionSubsystem.beforeSetConfiguration) event on the [ARCoreSessionSubsystem](xref:UnityEngine.XR.ARCore.ARCoreSessionSubsystem) to not be rasied.

## [4.1.0-preview.3] - 2020-07-09

### Changed

- Update [XR Plug-in Management](https://docs.unity3d.com/Packages/com.unity.xr.management@3.2) to 3.2.13.

### Fixed

- Android builds no longer fail when targeting an Android SDK version below 24, which is relevant when ARCore is "optional".
- Fixed a crash on launch when camera permissions were not already granted to the app.

## [4.1.0-preview.2] - 2020-06-24

### Added

- Add support for ARCore environment depth through the [AROcclusionManager](https://docs.unity3d.com/Packages/com.unity.xr.arfoundation@4.1/api/UnityEngine.XR.ARFoundation.AROcclusionManager.html) and [XROcclusionSubsystem](https://docs.unity3d.com/Packages/com.unity.xr.arsubsystems@4.1/api/UnityEngine.XR.ARSubsystems.XROcclusionSubsystem.html).

### Changed

- Update ARCore to version 1.18.
- The ARCore Settings has been moved from Project Settings > XR to Project Settings > XR Plug-in Management for consistency with other XR platforms. See [ARCoreSettings](https://docs.unity3d.com/Packages/com.unity.xr.arcore@4.0/api/UnityEditor.XR.ARCore.ARCoreSettings.html?q=ARCoreSettings) for more information. ARCore build settings will no longer have to be created manually, installing the package will automatically create ARCore settings.
- Minimum Unity version for this package is now 2019.4.
- Updating the documentation for the new package version, and adding information on the occlusion functionality.

## [4.0.3] - 2020-05-21

### Changed

- Update ARCore to version 1.17
- Updated "camera image" APIs to use the new "CPU image" API. See the [ARFoundation migration guide](https://docs.unity3d.com/Packages/com.unity.xr.arfoundation@4.0/manual/migration-guide-3.html#xrcameraimage-is-now-xrcpuimage) for more details.
- When the [ARCore Loader](https://docs.unity3d.com/Packages/com.unity.xr.arfoundation@4.0/manual/#provider-plugin-setup) in XR Management is diabled the ARCore binaries are not packaged into the build and ARCore build checks are not run.
- AAR files includes in this package are now compatible with Android Gradle Plugin 3.6.3 and older.
- The ARCore Settings has been moved from Project Settings > XR to Project Settings > XR Plug-in Management for consistency with other XR platforms. See [ARCoreSettings](https://docs.unity3d.com/Packages/com.unity.xr.arcore@4.0/api/UnityEditor.XR.ARCore.ARCoreSettings.html?q=ARCoreSettings) for more information. ARCore build settings will no longer have to be created manually, installing the package will automatically create ARCore settings.

### Fixed

- Fixed a bug where horizontal planes would still be reported when the plane detection mode was set to vertical only.
- Fixed a bug which caused a new point cloud to be reported each time the `XRDepthSubsystem` was stopped and restarted without removing the old point cloud, causing multiple point clouds to appear over time. Now, the point cloud's trackable id is unique to the session. Creating a new session, or resetting an existing session, will create a new point cloud and remove the old one (if there was one previously), but starting and stopping the `XRDepthSubsystem` no longer changes the trackable id. There is only ever one point cloud.

## [4.0.0-preview.3] - 2020-05-04

### Changed

- Added callback event `beforeSetConfiguration` in `ARCoreSessionSubsystem` that is triggered when the native [ArConfig](https://developers.google.com/ar/reference/c/group/config#arconfig) object is dirty.
- Added `SetConfigurationDirty()` to `ARCoreSessionSubsystem` which notifies that there is a change in the [ArConfig](https://developers.google.com/ar/reference/c/group/config#arconfig) object and triggers the `beforeSetConfiguration` event.
- Updating dependency on com.unity.xr.management to 3.2.10.

### Fixed

- Fixed all broken or missing scripting API documentation.

## [4.0.0-preview.1] - 2020-02-26

### Changed

- The ARSubsystem implementions have been updated to reflect changes in the ARSubsystems API.
- See the [ARFoundation Migration Guide](https://docs.unity3d.com/Packages/com.unity.xr.arfoundation@4.0/manual/migration-guide-3.html) for more details.

## [3.1.3] - 2020-04-13

### Changed

- Updating the dependency on AR Subsystems to 3.1.3.

## [3.1.0-preview.9] - 2020-03-13

### Fixed

- Setting [XRSessionSubsystem.matchFrameRate](https://docs.unity3d.com/Packages/com.unity.xr.arsubsystems@3.1/api/UnityEngine.XR.ARSubsystems.XRSessionSubsystem.html#UnityEngine_XR_ARSubsystems_XRSessionSubsystem_matchFrameRate) after the session subsystem was started had no effect. This has been fixed.

## [3.1.0-preview.8] - 2020-03-12

No changes

## [3.1.0-preview.7] - 2020-02-27

No changes

## [3.1.0-preview.6] - 2020-02-03

### Added

- Added support for gamma colorspace for the surfaced environment probe cubemap textures.

### Added

- Refactored native ARCore `XRCameraConfig` native handle representation.  Instead of an index into the list of available `ArCameraConfig`s, the `IntPtr` surfaced is the pointer to the actual `ArCameraConfig`.  See the following [ARCore documentation](https://developers.google.com/ar/reference/c/group/cameraconfig#arcameraconfig) for more information.
- Provider now provides the the maximum fps supported by the configuration as the `framerate` field in `XRCameraConfig` instead of the minimum framerate.

### Fixed

- Fixed a crash when attempting to destroy ARSession component.
- Fixed issue where Environment Probe cubemaps were not generating mipmaps.
- Fixed a crash when querying whether the "match frame rate" option was enabled. This could happen when resuming the ARSession.
- Fixed a potential crash when using image tracking or adding an image to an existing library at runtime.
- Fixed an issue where cubemap texture had incorrect Z-faces.
- Fixed an issue where the reported main light direction was incorrectly reporting the Z component of the vector.

## [3.1.0-preview.4] - 2020-01-10

### Added

- Update ARCore to version 1.14.

## [3.1.0-preview.2] - 2019-12-16

### Added

- Added support for HDR Light Estimation Mode
- Enforced platform-specific configurations for Light Estimation.  HDR Light Estimation mode is only supported when using the back-facing camera and can't be used while face tracking.  Ambient Light Estimation mode is only supported when not using EnvironmentProbes and will be taken over by the EnvironmentProbeSubsystem.
- Exposed native camera configuration object by surfacing the index into the list of configurations on the native side.

### Fixed

- Fixed black camera background texture when environment probes where enabled while tracking faces.
- Update documentation links to point to the 3.1 versions.
- Updating dependent version of com.unity.xr.management package to eliminate build warning message.

## [3.1.0-preview.1] - 2019-11-21

### Added

- No longer checking for presence of player setting for ARCore enabled in Unity 2020.1 or newer. This setting has been removed from Unity and was deprecated in 2019.3.

### Added

- Updated the background shader in unison with the changes for the new `AROcclusionManager` component and in preparation for the future when ARCore supports occlusion.

## [3.0.4] - 2020-04-08

### Changed

- Changed hard dependency on XR Management to 3.0.6.

## [3.0.3] - 2020-04-06

### Changed

- Default ARCore loader for XR Management will now only start and stop the implementations of XRInputSubsystem, XRCameraSubsystem, and XRSessionSubsystem when the _Initialize on Startup_ option in XR Management is enabled.

### Fixed

- Adding a minimum version restriction to the com.unity.inputsystem package for the conditional code that depends on that package.
- Fixed list of supported subsystems in the documentation.

## [3.0.1] - 2019-11-27

### Fixed

- Correcting script compilation error when the com.unity.inputsystem package is also included in the project.

## [3.0.0] - 2019-11-05

### Changed

- Renamed the concept of `Reference Points` to `Anchors`. Public API changes are in `AR Foundation` and `AR Subsystems` packages.

### Fixed

- Fixed issue for Mali GPU's where environment probes would be not have the texture bound properly.

## [3.0.0-preview.4] - 2019-10-22

### Added

- Add getter for the camera focus mode.
- Update to ARCore 1.12
- Add support for 60 fps devices.

### Fixed

- Fixed an issue where generating a reference image library could fail with "Access denied" on macOS and Linux. This was because spaces in project paths were not handled when setting execution privileges for `arcoreimg` on macOS and Linux.
- The GPU camera texture dimensions were incorrectly reported as the current screen dimensions. This has been fixed.
- The build could fail if reference images used for image tracking had a non-lowercase extension. This has been fixed.
- Improved error reporting when Google's ARCore SDK for Unity is detected in the project. The build will continue to fail even after removing Google's plug-in until the Editor is restarted. A note explaining this has been added to the error message.
- Fixed a potential crash when changing image libraries.

## [3.0.0-preview.3] - 2019-09-26

### Added

- Google's AR Presto library is now distributed as a separate AAR so that it can be overriden.
- Added support for both linear and gamma color spaces.
- Added support for environment probes.
- Added tracking input support for the [Input System](https://github.com/Unity-Technologies/InputSystem)

### Fixed

- Fixed image tracking when building with IL2CPP.

## [3.0.0-preview.2] - 2019-09-05

### Added

- Update [ARSubsystems](https://docs.unity3d.com/Packages/com.unity.xr.arsubsystems@3.1) dependency to 3.0.0-preview.2

## [3.0.0-preview.1] - 2019-08-21

### Added

- Add support for [XR Management](https://docs.unity3d.com/Packages/com.unity.xr.management@3.1/manual/index.html).
- Add support for building image libraries on linux.
- Add support for runtime modifiable reference image libraries. This allows you to add new reference images for image tracking at runtime.

## [2.2.0-preview.2] - 2019-07-19

### Fixed

- Previously, we reported the tracking state of augmented images using an [ARCore API](https://developers.google.com/ar/reference/c/group/trackable#group__trackable_1ga5759851a9b20b4df46d74d4ce4a1376c) that always returned `TrackingState.Tracking`, even when the image had been removed from view. We now use the method [ArAugmentedImage_getTrackingMethod](https://developers.google.com/ar/reference/c/group/augmented-image#group__augmented__image_1ga82c184a0984a95254476696cf57c12a3) instead, which will change the tracking state to `TrackingState.Limited` if the image cannot be seen.

## [2.2.0-preview.2] - 2019-08-09

### Added

- Add support for Lightweight Render Pipeline and Universal Render Pipeline.

## [2.2.0-preview.1] - 2019-07-16

### Added

- Update ARCore to version 1.10.
- Add support for getting the ambient light intensity in lumens.

## [2.1.0] - 2019-06-25

### Changed

- 2019.3 verified release

## [2.1.0-preview.7] - 2019-06-18

### Added

- Add support for `NotTrackingReason`.
- Update to ARCore 1.9.
- Add support for matching the ARCore framerate with the Unity one. See `ARSession.matchFrameRate`.

### Fixed

- Conditionally compile subsystem registrations. This means the subsystems wont't register themselves in the Editor (and won't generate warnings if there are other subsystems for other platforms).

## [2.1.0-preview.5] - 2019-05-27

### Fixed

- Enabling managed code stripping or IL2CPP would cause apps to fail to initialize ARCore on the first launch, just after the prompt for camera permissions. This has been fixed.
- Remove debug log from the image tracking subsystem.
- Fix documnentation links
- Reference image library generation would fail if the user's culture settings were not US-standard (e.g., using "," instead of "." for decimal values). This has been fixed.
- Fix `XRSessionSubsystem.Reset`. Previously, this did not properly resume a running session.
- The session subsystem reported that it did not support ARCore APK installation, which meant that consumers of this API (e.g., ARFoundation) would never prompt for installation and report the device as unsupported. This has been fixed.

## [2.1.0-preview.4] - 2019-05-08

### Added

- Add support for [image tracking](https://developers.google.com/ar/discover/concepts#augmented_images).
- Add support for [face tracking](https://developers.google.com/ar/develop/unity/augmented-faces/).
- Add support for [multithreaded rendering](https://unity3d.com/learn/tutorials/topics/best-practices/multithreaded-rendering-graphics-jobs#Multithreaded%20Rendering).

## [1.0.0-preview.24] - 2018-12-13

### Added

- Support x86, ARMv7, and ARM64 Android architetures (previously was limited to ARMv7).
- Plane detection modes: Add ability to selectively enable detection for horizontal, vertical, or both types of planes.
- Add a build check for the "Graphcis Jobs (Experimental)" player setting, which forces multithreaded rendering and causes ARCore to fail.
- Add a build check for the presence of Google's ARCore SDK for Unity, which cannot be used with this package.
- Add support for setting the camera focus mode.
- Add C header file necessary to interpret native pointers. See `Includes~/UnityXRNativePtrs.h`
- Implement `CameraConfiguration` support, allowing you to enumerate and set the resolution used by the hardware camera.
- Update to ARCore v1.6.0
- Add linker validation when building with the IL2CPP scripting backend to avoid stripping the Unity.XR.ARCore assembly.
- Add native pointer support for native AR objects
- Added support for `XRCameraExtensions` API to get the raw camera image data on the CPU. See the [ARFoundation manual documentation](https://docs.unity3d.com/Packages/com.unity.xr.arfoundation@1.0/manual/cpu-camera-image.html) for more information.
- Added a pre build step to ensure the Gradle build system is used.
- The build will fail if anything other than the OpenGLES3 graphics API is selected as the primary graphics API.
- Implemented TryGetColorCorrection, which provides light estimation information for RGB color correction.
- Add ArAnchor [attachment](https://developers.google.com/ar/develop/developer-guides/anchors) support.
- Add ARCoreSettings to Player Settings menu. Allows you to select whether ARCore is 'optional' or 'required'.
- Add Editor as an include platform to ensure ARCore extensions work. This was preventing the availability check from running.
- Add support for availability check and apk install
- Created a Legacy XRInput interface to automate the switch between 2018.1 and 2018.2 XRInput versions.
This is the first preview release of the ARCore package for multi-platform AR.

In this release we are shipping a working iteration of the ARCore package for
Unity's native multi-platform AR support.
Included in the package are dynamic libraries, configuration files, binaries
and project files needed to adapt ARCore to the Unity multi-platform AR API.

### Changed

- Remove 2018.1 and 2018.2 compatibility.
- Re-add `using` directive needed for 2018.1.
- Fail the build if x86 or ARM 64 are selected as Target Architectures in the Android Player Settings.
- Throw a build error instead of a warning if using Vulkan (ARCore requires an OpenGL context)
- Improved usability of ARCoreSettings
  - Remove CreateAssetMenu item -- provide one path to create the asset.
  - xmldoc referred to ARKit instead of ARCore
  - Make currentSettings public so users can override this easily.
- Improved ARCore build error message
    'Error building Player: BuildFailedException: "ARCore Supported" (Player Settings > XR Settings) refers to the built-in ARCore support in Unity and conflicts with the ARCore package.') that doesn't explain that that the "ARCore package" is in fact the "Google ARCore XR Plug-in" package. The package name should match from the package manager window.
- Change dependency to `ARExtensions` preview.2
- Only report display and projection matrices if we actually get them from ARCore.

### Fixed

- Updated background shader to workaround a bug which can cause green and blue color values to appear swapped on some devices when HDR is enabled.
- Fixed a bug which prevented the CameraImage API from working in 2018.3+
- Correctly set camera texture dimensions.
- The background texture was not rendered correctly if a renderable `GameObject` in the scene had negative scale. This has been fixed.
- Fixed issue [AR Camera does not work with video player on ARCore](https://issuetracker.unity3d.com/issues/ar-camera-does-not-work-with-video-player-on-arcore). The pass through video would conflict with the Video Player, producing a flickering effect. This has been fixed.
- Correctly detect whether the "ARCore Supported" checkbox is checked during player build in 2018.2+
- Updated for compatibility with Unity 2018.3 and later.
- Slinece unused variable warning.
- Plane tracking state would return a cached value if the session was not active. Now, `ARPlane.trackingState` will return `TrackingState.Unavailable` for every plane if the session is not active.
- Fixed lack of reporting timestamp to the `ARCameraFrameEventArgs`.
- Do not include Android build pipeline when not on Android
- Fixed a crash on startup on some devices.
- Camera texture appears as soon as ARCore provides it, rather than waiting for tracking to be established.
- Fix typo in ARCoreSettings (`requirment` => `requirement`)
- Fixed a crash following ARCore apk install. There is a (rare) race condition when installing the ARCore apk, where ARCore will try to initialize before the apk is completely ready. This can still happen, but the app no longer crashes. When it does happen, the SDK will report that AR is supported and ready, but AR will not function properly until the app is restarted.
- Fix a bug which prevented the ARSession from restarting once destroyed.
- Fixed crash when ARCore is not present or otherwise unable to initialize.

