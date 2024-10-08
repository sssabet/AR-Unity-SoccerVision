namespace UnityEngine.XR.ARCore
{
    /// <summary>
    /// Describe the possible recording statuses.
    /// </summary>
    public enum ArRecordingStatus
    {
        /// <summary>
        /// The dataset recorder is not recording.
        /// </summary>
        None = 0,

        /// <summary>
        /// The dataset recorder is recording normally.
        /// </summary>
        Ok = 1,

        /// <summary>
        /// The dataset recorder encountered an error while recording.
        /// </summary>
        IOError = 2,
    };

    /// <summary>
    /// Extension methods for the <see cref="ArRecordingStatus"/> enum.
    /// </summary>
    public static class ArRecordingStatusExtensions
    {
        /// <summary>
        /// Determines whether this status indicates that a session is currently being recorded.
        /// </summary>
        /// <param name="status">The <see cref="ArRecordingStatus"/> value to check.</param>
        /// <returns>Returns `true` if <paramref name="status"/> is <see cref="ArRecordingStatus.Ok"/>. Returns `false`
        ///     otherwise.</returns>
        public static bool Recording(this ArRecordingStatus status) => status == ArRecordingStatus.Ok;
    }
}
