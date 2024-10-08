---
uid: arcore-session
---
# Session

This page is a supplement to the AR Foundation [Session](xref:arfoundation-session) manual. The following sections only contain information about APIs where ARCore exhibits unique platform-specific behavior.

[!include[](../snippets/arf-docs-tip.md)]

## Check if AR is supported

ARCore implements [XRSessionSubsystem.GetAvailabilityAsync](xref:UnityEngine.XR.ARSubsystems.XRSessionSubsystem.GetAvailabilityAsync). Use this function to determine if the device ARCore is currently running on is supported. ARCore's list of supported devices is frequently updated to include additional devices. For a full list of devices that ARCore supports, refer to [ARCore supported devices](https://developers.google.com/ar/discover/supported-devices).

If ARCore isn't already installed on a device, your app needs to check with the Google Play store to see if there's a version of ARCore that supports that device. To do this, use `GetAvailabilityAsync` to return a `Promise` that you can use in a coroutine. For ARCore, this check can take some time.

If the device is supported, but ARCore is not installed or requires an update, call [XRSessionSubsystem.InstallAsync](UnityEngine.XR.ARSubsystems.XRSessionSubsystem.InstallAsync), which also returns a `Promise`.
