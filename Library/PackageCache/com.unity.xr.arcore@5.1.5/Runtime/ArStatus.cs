namespace UnityEngine.XR.ARCore
{
    /// <summary>
    /// Return code indicating success or failure of a method in the ARCore SDK.
    /// </summary>
    public enum ArStatus
    {
        /// <summary>
        /// The operation was successful.
        /// </summary>
        Success = 0,

        /// <summary>
        /// One of the arguments was invalid; either `null` or not appropriate for
        /// the operation requested.
        /// </summary>
        ErrorInvalidArgument = -1,

        /// <summary>
        /// An internal error occurred that the application should not attempt to
        /// recover from.
        /// </summary>
        ErrorFatal = -2,

        /// <summary>
        /// An operation was attempted that requires the session be running, but the
        /// session was paused.
        /// </summary>
        ErrorSessionPaused = -3,

        /// <summary>
        /// An operation was attempted that requires the session be paused, but the
        /// session was running.
        /// </summary>
        ErrorSessionNotPaused = -4,

        /// <summary>
        /// An operation was attempted that requires the session be in the
        /// [TrackingState.Tracking](xref:UnityEngine.XR.ARSubsystems.TrackingState.Tracking) state,
        /// but the session was not.
        /// </summary>
        ErrorNotTracking = -5,

        /// <summary>
        /// No textures are available to the ARCore session.
        /// </summary>
        ErrorTextureNotSet = -6,

        /// <summary>
        /// An operation requires a GL context but one was not available.
        /// </summary>
        ErrorMissingGLContext = -7,

        /// <summary>
        /// The ARCore session configuration is unsupported.
        /// </summary>
        ErrorUnsupportedConfiguration = -8,

        /// <summary>
        /// The application does not have Android camera permission.
        /// </summary>
        ErrorCameraPermissionNotGranted = -9,

        /// <summary>
        /// Acquire failed because the object being acquired was already released.
        /// </summary>
        ErrorDeadlineExceeded = -10,

        /// <summary>
        /// There are no available resources to complete the operation. In cases of
        /// @c acquire functions returning this error, this can be avoided by
        /// releasing previously acquired objects before acquiring new ones.
        /// </summary>
        ErrorResourceExhausted = -11,

        /// <summary>
        /// Acquire failed because the data isn't available yet for the current
        /// frame. For example, acquiring image metadata may fail with this error
        /// because the camera hasn't fully started.
        /// </summary>
        ErrorNotYetAvailable = -12,

        /// <summary>
        /// The Android camera has been reallocated to a higher priority application
        /// or is otherwise unavailable.
        /// </summary>
        ErrorCameraNotAvailable = -13,

        /// <summary>
        /// The host/resolve function call failed because the Session is not
        /// configured for Cloud Anchors.
        /// </summary>
        ErrorCloudAnchorsNotConfigured = -14,

        /// <summary>
        /// Failed to configure the ARCore session because the specified configuration
        /// required the Android INTERNET permission, which the application did not
        /// have.
        /// </summary>
        ErrorInternetPermissionNotGranted = -15,

        /// <summary>
        /// Could not create a new cloud anchor because the anchor is
        /// not a type of anchor that is currently supported for hosting.
        /// </summary>
        ErrorAnchorNotSupportedForHosting = -16,

        /// <summary>
        /// Attempted to add an image with insufficient quality (e.g., too few
        /// features) to the image database.
        /// </summary>
        ErrorImageInsufficientQuality = -17,

        /// <summary>
        /// The data passed in for this operation was not in a valid format.
        /// </summary>
        ErrorDataInvalidFormat = -18,

        /// <summary>
        /// The data passed in for this operation is not supported by this version
        /// of the SDK.
        /// </summary>
        ErrorDataUnsupportedVersion = -19,

        /// <summary>
        /// A function has been invoked at an illegal or inappropriate time. A
        /// message will be printed to logcat with additional details for the
        /// developer.
        /// </summary>
        ErrorIllegalState = -20,

        /// <summary>
        /// Recording failed.
        /// </summary>
        ErrorRecordingFailed = -23,

        /// <summary>
        /// Playback failed.
        /// </summary>
        ErrorPlaybackFailed = -24,

        /// <summary>
        /// Operation is unsupported with the current session.
        /// </summary>
        ErrorSessionUnsupported = -25,

        /// <summary>
        /// The requested metadata tag cannot be found in input metadata.
        /// </summary>
        ErrorMetadataNotFound = -26,

        /// <summary>
        /// The ARCore APK is not installed on this device.
        /// </summary>
        UnavailableARCoreNotInstalled = -100,

        /// <summary>
        /// The device is not currently compatible with ARCore.
        /// </summary>
        UnavailableDeviceNotCompatible = -101,

        /// <summary>
        /// The ARCore APK currently installed on device is too old and needs to be
        /// updated.
        /// </summary>
        UnavailableApkTooOld = -103,

        /// <summary>
        /// The ARCore APK currently installed no longer supports the ARCore SDK
        /// that the application was built with.
        /// </summary>
        UnavailableSdkTooOld = -104,

        /// <summary>
        /// The user declined installation of the ARCore APK during this run of the
        /// application and the current request was not marked as user-initiated.
        /// </summary>
        UnavailableUserDeclinedInstallation = -105
    }
}
