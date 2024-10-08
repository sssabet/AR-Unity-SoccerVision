using System;
using System.Runtime.InteropServices;
using System.Text;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace UnityEngine.XR.ARCore
{
    /// <summary>
    /// A recording configuration struct that contains the configuration for session recording.
    /// See <see cref="ArSession.StartRecording(ArRecordingConfig)"/>.
    /// </summary>
    /// <remarks>
    /// A <see cref="ArRecordingConfig"/> represents a native object that must be disposed (by calling
    /// <see cref="ArRecordingConfig.Dispose"/>) to prevent memory leaks. Consider using a `using` statement for
    /// convenience:
    /// <example>
    /// <code source="../Tests/Runtime/CodeExamples.cs" language="csharp" region="ArRecordingConfig_example"/>
    /// </example>
    ///
    /// This is a C# wrapper for ARCore's
    /// [native ArRecordingConfig](https://developers.google.com/ar/reference/c/group/ar-recording-config)
    /// </remarks>
    public struct ArRecordingConfig : IEquatable<ArRecordingConfig>, IDisposable
    {
        IntPtr m_Self;

        ArRecordingConfig(IntPtr value) => m_Self = value;

        /// <summary>
        /// Create a <see cref="ArRecordingConfig"/> from an existing native pointer. The native pointer must point
        /// to an existing <see cref="ArRecordingConfig"/>.
        /// </summary>
        /// <param name="value">A pointer to an existing native <see cref="ArRecordingConfig"/>.</param>
        /// <returns>Returns an <see cref="ArRecordingConfig"/> whose underlying native pointer is
        ///     <paramref name="value"/>.</returns>
        public static ArRecordingConfig FromIntPtr(IntPtr value) => new ArRecordingConfig(value);

        /// <summary>
        /// Gets the underlying native pointer for this <see cref="ArRecordingConfig"/>.
        /// </summary>
        /// <returns>Returns the underlying native pointer for this <see cref="ArRecordingConfig"/>.</returns>
        public IntPtr AsIntPtr() => m_Self;

        /// <summary>
        /// Casts an <see cref="ArRecordingConfig"/> to its underlying native pointer.
        /// </summary>
        /// <param name="config">The <see cref="ArRecordingConfig"/> to cast.</param>
        /// <returns>Returns the underlying native pointer for <paramref name="config"/></returns>
        public static explicit operator IntPtr(ArRecordingConfig config) => config.AsIntPtr();

        /// <summary>
        /// Tests for equality.
        /// </summary>
        /// <remarks>
        /// Two <see cref="ArRecordingConfig"/>s are considered equal if their underlying pointers are equal.
        /// </remarks>
        /// <param name="other">The <see cref="ArRecordingConfig"/> to compare against.</param>
        /// <returns>Returns `true` if the underlying native pointers are the same. Returns `false` otherwise.</returns>
        public bool Equals(ArRecordingConfig other) => m_Self == other.m_Self;

        /// <summary>
        /// Tests for equality.
        /// </summary>
        /// <param name="obj">An <see cref="object"/> to compare against.</param>
        /// <returns>Returns `true` if <paramref name="obj"/> is an <see cref="ArRecordingConfig"/> and it compares
        ///     equal to this one using <see cref="Equals(UnityEngine.XR.ARCore.ArRecordingConfig)"/>.</returns>
        public override bool Equals(object obj) => obj is ArRecordingConfig other && Equals(other);

        /// <summary>
        /// Generates a hash code suitable for use with a `HashSet` or `Dictionary`
        /// </summary>
        /// <returns>Returns a hash code for this <see cref="ArRecordingConfig"/>.</returns>
        public override int GetHashCode() => m_Self.GetHashCode();

        /// <summary>
        /// Tests for equality. Same as <see cref="Equals(UnityEngine.XR.ARCore.ArRecordingConfig)"/>.
        /// </summary>
        /// <param name="lhs">The <see cref="ArRecordingConfig"/> to compare with <paramref name="rhs"/>.</param>
        /// <param name="rhs">The <see cref="ArRecordingConfig"/> to compare with <paramref name="lhs"/>.</param>
        /// <returns>Returns `true` if <paramref name="lhs"/> is equal to <paramref name="rhs"/> using
        ///     <see cref="Equals(UnityEngine.XR.ARCore.ArRecordingConfig)"/>. Returns `false` otherwise.</returns>
        public static bool operator ==(ArRecordingConfig lhs, ArRecordingConfig rhs) => lhs.Equals(rhs);

        /// <summary>
        /// Tests for inequality. Same as the negation of <see cref="Equals(UnityEngine.XR.ARCore.ArRecordingConfig)"/>.
        /// </summary>
        /// <param name="lhs">The <see cref="ArRecordingConfig"/> to compare with <paramref name="rhs"/>.</param>
        /// <param name="rhs">The <see cref="ArRecordingConfig"/> to compare with <paramref name="lhs"/>.</param>
        /// <returns>Returns `false` if <paramref name="lhs"/> is equal to <paramref name="rhs"/> using
        ///     <see cref="Equals(UnityEngine.XR.ARCore.ArRecordingConfig)"/>. Returns `true` otherwise.</returns>
        public static bool operator !=(ArRecordingConfig lhs, ArRecordingConfig rhs) => !lhs.Equals(rhs);

        /// <summary>
        /// Tests for equality.
        /// </summary>
        /// <remarks>
        /// This equality operator lets you to compare an <see cref="ArRecordingConfig"/> with `null` to determine whether its
        /// underlying pointer is null. This allows for a more natural comparison with the native ARCore object:
        /// <example>
        /// <code>
        /// bool TestForNull(ArRecordingConfig obj)
        /// {
        ///     if (obj == null)
        ///     {
        ///         // obj.AsIntPtr() is IntPtr.Zero
        ///     }
        /// }
        /// </code>
        /// </example>
        /// </remarks>
        /// <param name="lhs">The nullable <see cref="ArRecordingConfig"/> to compare with <paramref name="rhs"/>.</param>
        /// <param name="rhs">The nullable <see cref="ArRecordingConfig"/> to compare with <paramref name="lhs"/>.</param>
        /// <returns>Returns true if any of these conditions are met:
        /// - <paramref name="lhs"/> and <paramref name="rhs"/> are both not null and their underlying pointers are equal.
        /// - <paramref name="lhs"/> is null and <paramref name="rhs"/>'s underlying pointer is null.
        /// - <paramref name="rhs"/> is null and <paramref name="lhs"/>'s underlying pointer is null.
        /// - Both <paramref name="lhs"/> and <paramref name="rhs"/> are null.
        ///
        /// Returns false otherwise.
        /// </returns>
        public static bool operator ==(ArRecordingConfig? lhs, ArRecordingConfig? rhs) => NativeObject.ArePointersEqual(lhs?.m_Self, rhs?.m_Self);

        /// <summary>
        /// Tests for inequality.
        /// </summary>
        /// <remarks>
        /// This inequality operator lets you to compare an <see cref="ArRecordingConfig"/> with `null` to determine whether its
        /// underlying pointer is null. This allows for a more natural comparison with the native ARCore object:
        /// <example>
        /// <code>
        /// bool TestForNull(ArRecordingConfig obj)
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
        public static bool operator !=(ArRecordingConfig? lhs, ArRecordingConfig? rhs) => !(lhs == rhs);

        /// <summary>
        /// Creates a dataset recording config object. This object must be disposed with <see cref="Dispose"/>.
        /// </summary>
        /// <param name="session">The ARCore session.</param>
        public ArRecordingConfig(ArSession session) => Create(session, out this);

        [DllImport("arcore_sdk_c", EntryPoint = "ArRecordingConfig_create")]
        static extern void Create(ArSession session, out ArRecordingConfig outConfig);

        /// <summary>
        /// Releases memory used by this recording config object.
        /// </summary>
        public void Dispose()
        {
            if (this != null)
            {
                Destroy(this);
            }

            m_Self = IntPtr.Zero;
        }

        [DllImport("arcore_sdk_c", EntryPoint = "ArRecordingConfig_destroy")]
        static extern void Destroy(ArRecordingConfig config);

        /// <summary>
        /// Gets the file path to save an MP4 dataset file for this recording.
        /// </summary>
        /// <param name="session">The ARCore session.</param>
        /// <returns>Returns the path to the MP4 dataset file to which the recording should be saved.</returns>
        public string GetMp4DatasetFilePath(ArSession session)
        {
            GetMp4DatasetFilePath(session, this, out var value);
            using (value)
            {
                return value.ToString(Encoding.UTF8);
            }
        }

        [DllImport("arcore_sdk_c", EntryPoint = "ArRecordingConfig_getMp4DatasetFilePath")]
        static extern void GetMp4DatasetFilePath(ArSession session, ArRecordingConfig config, out ArString outMp4DatasetFilePath);

        /// <summary>
        /// Sets the file path to save an MP4 dataset file for the recording.
        /// </summary>
        /// <param name="session">The ARCore session.</param>
        /// <param name="path">The file path to which an MP4 dataset should be written.</param>
        /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="path"/> is `null`.</exception>
        public void SetMp4DatasetFilePath(ArSession session, string path)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));

            unsafe
            {
                using (var bytes = path.ToBytes(Encoding.UTF8, Allocator.Temp))
                {
                    SetMp4DatasetFilePath(session, this, (byte*)bytes.GetUnsafePtr());
                }
            }
        }

        [DllImport("arcore_sdk_c", EntryPoint = "ArRecordingConfig_setMp4DatasetFilePath")]
        static extern unsafe void SetMp4DatasetFilePath(ArSession session, ArRecordingConfig config, byte* mp4DatasetFilePath);

        /// <summary>
        /// Gets the setting that indicates whether this recording should stop automatically when the ARCore session is paused.
        /// </summary>
        /// <param name="session">The ARCore session.</param>
        /// <returns>Returns `true` if this recording should stop when the ARCore session is paused. Returns `false` otherwise.</returns>
        public bool GetAutoStopOnPause(ArSession session)
        {
            GetAutoStopOnPause(session, this, out var value);
            return value != 0;
        }

        [DllImport("arcore_sdk_c", EntryPoint = "ArRecordingConfig_getAutoStopOnPause")]
        static extern void GetAutoStopOnPause(ArSession session, ArRecordingConfig config, out int outConfigEnabled);

        /// <summary>
        /// Sets whether this recording should stop automatically when the ARCore session is paused.
        /// </summary>
        /// <param name="session">The ARCore session.</param>
        /// <param name="value">If `true`, this recording will stop automatically when the ARCore session is paused. If `false`, the recording will continue.</param>
        public void SetAutoStopOnPause(ArSession session, bool value) => SetAutoStopOnPause(session, this, value ? 1 : 0);

        [DllImport("arcore_sdk_c", EntryPoint = "ArRecordingConfig_setAutoStopOnPause")]
        static extern void SetAutoStopOnPause(ArSession session, ArRecordingConfig config, int configEnabled);

        /// <summary>
        /// Gets the clockwise rotation in degrees that should be applied to the recorded image.
        /// </summary>
        /// <param name="session">The ARCore session.</param>
        /// <returns>Returns the rotation in degrees that will be applied to the recorded image. Possible values are 0, 90, 180, 270, or -1 if unspecified.</returns>
        public int GetRecordingRotation(ArSession session)
        {
            GetRecordingRotation(session, this, out var value);
            return value;
        }

        [DllImport("arcore_sdk_c", EntryPoint = "ArRecordingConfig_getRecordingRotation")]
        static extern void GetRecordingRotation(ArSession session, ArRecordingConfig config, out int outRecordingRotation);

        /// <summary>
        /// Specifies the clockwise rotation in degrees that should be applied to the recorded image.
        /// </summary>
        /// <param name="session">The ARCore session.</param>
        /// <param name="value">The clockwise rotation in degrees (0, 90, 180, or 270).</param>
        public void SetRecordingRotation(ArSession session, int value) => SetRecordingRotation(session, this, value);

        [DllImport("arcore_sdk_c", EntryPoint = "ArRecordingConfig_setRecordingRotation")]
        static extern void SetRecordingRotation(ArSession session, ArRecordingConfig config, int recordingRotation);
    }
}
