using System;
using System.Runtime.InteropServices;
using System.Text;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace UnityEngine.XR.ARCore
{
    /// <summary>
    /// Represents the context for an ARCore session.
    /// </summary>
    /// <remarks>
    /// This is an opaque object that represents a native
    /// [ArSession](https://developers.google.com/ar/reference/c/group/ar-session).
    /// </remarks>
    /// <seealso cref="ARCoreSessionSubsystem.beforeSetConfiguration"/>
    /// <seealso cref="ARCoreBeforeSetConfigurationEventArgs"/>
    /// <seealso cref="ARCoreCameraSubsystem.beforeGetCameraConfiguration"/>
    /// <seealso cref="ARCoreBeforeGetCameraConfigurationEventArgs"/>
    public struct ArSession : IEquatable<ArSession>
    {
        IntPtr m_Self;

        ArSession(IntPtr value) => m_Self = value;

        /// <summary>
        /// Creates an <see cref="ArSession"/> from an existing native pointer. The native pointer must point
        /// to an existing <see cref="ArSession"/>.
        /// </summary>
        /// <param name="value">A pointer to an existing native <see cref="ArSession"/>.</param>
        /// <returns>Returns an <see cref="ArSession"/> whose underlying native pointer is
        ///     <paramref name="value"/>.</returns>
        public static ArSession FromIntPtr(IntPtr value) => new ArSession(value);

        /// <summary>
        /// Represents a null <see cref="ArSession"/>, i.e., one whose underlying native pointer is `null`.
        /// This property is obsolete. Use <see langword="default"/> instead.
        /// </summary>
        [Obsolete("Use default instead.")]
        public static ArSession Null => default;

        /// <summary>
        /// (Read Only) Indicates whether this <see cref="ArSession"/> is `null`.
        /// This property is deprecated. Use the equality operator (`==`) to compare with `null` instead.
        /// </summary>
        [Obsolete("Compare to null instead.")]
        public bool IsNull => m_Self == IntPtr.Zero;

        /// <summary>
        /// Gets the underlying native pointer for this <see cref="ArSession"/>.
        /// </summary>
        /// <returns>Returns the underlying native pointer for this <see cref="ArSession"/>.</returns>
        public IntPtr AsIntPtr() => m_Self;

        /// <summary>
        /// Casts an <see cref="ArSession"/> to its underlying native pointer.
        /// </summary>
        /// <param name="session">The <see cref="ArSession"/> to cast.</param>
        /// <returns>Returns the underlying native pointer for <paramref name="session"/></returns>
        public static explicit operator IntPtr(ArSession session) => session.AsIntPtr();

        /// <summary>
        /// Tests for equality.
        /// </summary>
        /// <remarks>
        /// Two <see cref="ArSession"/>s are considered equal if their underlying pointers are equal.
        /// </remarks>
        /// <param name="other">The <see cref="ArSession"/> to compare against.</param>
        /// <returns>Returns `true` if the underlying native pointers are the same. Returns `false` otherwise.</returns>
        public bool Equals(ArSession other) => m_Self == other.m_Self;

        /// <summary>
        /// Tests for equality.
        /// </summary>
        /// <param name="obj">An <see cref="object"/> to compare against.</param>
        /// <returns>Returns `true` if <paramref name="obj"/> is an <see cref="ArSession"/> and it compares
        ///     equal to this one using <see cref="Equals(UnityEngine.XR.ARCore.ArSession)"/>.</returns>
        public override bool Equals(object obj) => obj is ArSession other && Equals(other);

        /// <summary>
        /// Generates a hash code suitable for use with a `HashSet` or `Dictionary`
        /// </summary>
        /// <returns>Returns a hash code for this <see cref="ArSession"/>.</returns>
        public override int GetHashCode() => m_Self.GetHashCode();

        /// <summary>
        /// Tests for equality. Same as <see cref="Equals(UnityEngine.XR.ARCore.ArSession)"/>.
        /// </summary>
        /// <param name="lhs">The <see cref="ArSession"/> to compare with <paramref name="rhs"/>.</param>
        /// <param name="rhs">The <see cref="ArSession"/> to compare with <paramref name="lhs"/>.</param>
        /// <returns>Returns `true` if <paramref name="lhs"/> is equal to <paramref name="rhs"/> using
        ///     <see cref="Equals(UnityEngine.XR.ARCore.ArSession)"/>. Returns `false` otherwise.</returns>
        public static bool operator ==(ArSession lhs, ArSession rhs) => lhs.Equals(rhs);

        /// <summary>
        /// Tests for inequality. Same as the negation of <see cref="Equals(UnityEngine.XR.ARCore.ArSession)"/>.
        /// </summary>
        /// <param name="lhs">The <see cref="ArSession"/> to compare with <paramref name="rhs"/>.</param>
        /// <param name="rhs">The <see cref="ArSession"/> to compare with <paramref name="lhs"/>.</param>
        /// <returns>Returns `false` if <paramref name="lhs"/> is equal to <paramref name="rhs"/> using
        ///     <see cref="Equals(UnityEngine.XR.ARCore.ArSession)"/>. Returns `true` otherwise.</returns>
        public static bool operator !=(ArSession lhs, ArSession rhs) => !lhs.Equals(rhs);

        /// <summary>
        /// Tests for equality.
        /// </summary>
        /// <remarks>
        /// This equality operator lets you to compare an <see cref="ArSession"/> with `null` to determine whether its
        /// underlying pointer is null. This allows for a more natural comparison with the native ARCore object:
        /// <example>
        /// <code>
        /// bool TestForNull(ArSession obj)
        /// {
        ///     if (obj == null)
        ///     {
        ///         // obj.AsIntPtr() is IntPtr.Zero
        ///     }
        /// }
        /// </code>
        /// </example>
        /// </remarks>
        /// <param name="lhs">The nullable <see cref="ArSession"/> to compare with <paramref name="rhs"/>.</param>
        /// <param name="rhs">The nullable <see cref="ArSession"/> to compare with <paramref name="lhs"/>.</param>
        /// <returns>Returns true if any of these conditions are met:
        /// - <paramref name="lhs"/> and <paramref name="rhs"/> are both not null and their underlying pointers are equal.
        /// - <paramref name="lhs"/> is null and <paramref name="rhs"/>'s underlying pointer is null.
        /// - <paramref name="rhs"/> is null and <paramref name="lhs"/>'s underlying pointer is null.
        /// - Both <paramref name="lhs"/> and <paramref name="rhs"/> are null.
        ///
        /// Returns false otherwise.
        /// </returns>
        public static bool operator ==(ArSession? lhs, ArSession? rhs) => NativeObject.ArePointersEqual(lhs?.m_Self, rhs?.m_Self);

        /// <summary>
        /// Tests for inequality.
        /// </summary>
        /// <remarks>
        /// This inequality operator lets you to compare an <see cref="ArSession"/> with `null` to determine whether its
        /// underlying pointer is null. This allows for a more natural comparison with the native ARCore object:
        /// <example>
        /// <code>
        /// bool TestForNull(ArSession obj)
        /// {
        ///     if (obj != null)
        ///     {
        ///         // obj.AsIntPtr() is not IntPtr.Zero
        ///     }
        /// }
        /// </code>
        /// </example>
        /// </remarks>
        /// <param name="lhs">The native object to compare with <paramref name="rhs"/>.</param>
        /// <param name="rhs">The native object to compare with <paramref name="lhs"/>.</param>
        /// <returns>Returns false if any of these conditions are met:
        /// - <paramref name="lhs"/> and <paramref name="rhs"/> are both not null and their underlying pointers are equal.
        /// - <paramref name="lhs"/> is null and <paramref name="rhs"/>'s underlying pointer is null.
        /// - <paramref name="rhs"/> is null and <paramref name="lhs"/>'s underlying pointer is null.
        /// - Both <paramref name="lhs"/> and <paramref name="rhs"/> are null.
        ///
        /// Returns true otherwise.
        /// </returns>
        public static bool operator !=(ArSession? lhs, ArSession? rhs) => !(lhs == rhs);

        /// <summary>
        /// Sets an MP4 dataset file to playback instead of live camera feed.
        /// </summary>
        /// <remarks>
        /// Restrictions:
        /// - Can only be called while the session is paused. Playback of the MP4
        /// dataset file starts once the session is resumed.
        /// - The MP4 dataset file must use the same camera facing direction as is
        /// configured in the session.
        ///
        /// When an MP4 dataset file is set:
        /// - All existing trackables (i.e., anchors and trackables) immediately enter tracking state [TrackingState.None](xref:UnityEngine.XR.ARSubsystems.TrackingState.None).
        /// - The desired focus mode is ignored, and does not affect the previously recorded camera images.
        /// - The current camera configuration is immediately set to the default for the device the MP4 dataset file was recorded on.
        /// - Calls to retrieve the supported camera configurations return camera configs supported by the device the MP4 dataset file was recorded on.
        /// - Setting a previously obtained camera config has no effect.
        /// </remarks>
        /// <param name="path">A file path to a MP4 dataset file or `null` to use the live camera feed.</param>
        /// <returns>
        /// - Returns <see cref="ArStatus.Success"/> if successful.
        /// - Returns <see cref="ArStatus.ErrorSessionNotPaused"/> if called when session is not paused.
        /// - Returns <see cref="ArStatus.ErrorSessionUnsupported"/> if playback is incompatible with selected features.
        /// - Returns <see cref="ArStatus.ErrorPlaybackFailed"/> if an error occurred with the MP4 dataset file such as not being able to open the file or the file is unable to be decoded.
        /// </returns>
        public ArStatus SetPlaybackDataset(string path)
        {
            unsafe
            {
                if (string.IsNullOrEmpty(path))
                {
                    return SetPlaybackDataset(this, null);
                }

                using (var bytes = path.ToBytes(Encoding.UTF8, Allocator.Temp))
                {
                    return SetPlaybackDataset(this, (byte*)bytes.GetUnsafePtr());
                }
            }
        }

        [DllImport("arcore_sdk_c", EntryPoint = "ArSession_setPlaybackDataset")]
        static extern unsafe ArStatus SetPlaybackDataset(ArSession session, byte* mp4DatasetFilePath);

        /// <summary>
        /// (Read Only) The playback status.
        /// </summary>
        /// <value>Whether or not the session is playing back a recording (or has stopped because of an error).</value>
        public ArPlaybackStatus playbackStatus
        {
            get
            {
                GetPlaybackStatus(this, out var value);
                return value;
            }
        }

        [DllImport("arcore_sdk_c", EntryPoint = "ArSession_getPlaybackStatus")]
        static extern void GetPlaybackStatus(ArSession session, out ArPlaybackStatus outPlaybackStatus);

        /// <summary>
        /// Starts a new MP4 dataset file recording that is written to the specific filesystem path.
        /// </summary>
        /// <remarks>
        /// Existing files are overwritten.
        ///
        /// The MP4 video stream (VGA) bitrate is 5Mbps (40Mb per minute).
        ///
        /// Recording introduces additional overhead and may affect app performance.
        /// </remarks>
        /// <param name="recordingConfig">The configuration defined for recording.</param>
        /// <returns>Returns <see cref="ArStatus.Success"/> if successful. Returns one of the following values otherwise:
        /// - <see cref="ArStatus.ErrorIllegalState"/>
        /// - <see cref="ArStatus.ErrorInvalidArgument"/>
        /// - <see cref="ArStatus.ErrorRecordingFailed"/>
        /// </returns>
        public ArStatus StartRecording(ArRecordingConfig recordingConfig) => StartRecording(this, recordingConfig);

        [DllImport("arcore_sdk_c", EntryPoint = "ArSession_startRecording")]
        static extern ArStatus StartRecording(ArSession session, ArRecordingConfig recordingConfig);

        /// <summary>
        /// Stops recording and flushes unwritten data to disk. The MP4 dataset file is ready to read after this
        /// call.
        /// </summary>
        /// <remarks>
        /// Recording can be stopped automatically when the session is paused,
        /// if auto stop is enabled via <see cref="ArRecordingConfig.SetAutoStopOnPause"/>.
        /// Recording errors that would be thrown in <see cref="StopRecording"/> are silently
        /// ignored on session pause.
        /// </remarks>
        /// <returns>Returns <see cref="ArStatus.Success"/> if successful. Returns
        ///     <see cref="ArStatus.ErrorRecordingFailed"/> otherwise.</returns>
        public ArStatus StopRecording() => StopRecording(this);

        [DllImport("arcore_sdk_c", EntryPoint = "ArSession_stopRecording")]
        static extern ArStatus StopRecording(ArSession session);

        /// <summary>
        /// (Read Only) The current recording status.
        /// </summary>
        /// <value>Whether or not the session is recording (or has stopped because of an error).</value>
        public ArRecordingStatus recordingStatus
        {
            get
            {
                GetRecordingStatus(this, out var value);
                return value;
            }
        }

        [DllImport("arcore_sdk_c", EntryPoint = "ArSession_getRecordingStatus")]
        static extern void GetRecordingStatus(ArSession session, out ArRecordingStatus outRecordingStatus);
    }
}
