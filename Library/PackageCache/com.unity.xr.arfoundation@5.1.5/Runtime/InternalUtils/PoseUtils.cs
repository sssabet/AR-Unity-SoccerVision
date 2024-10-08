namespace UnityEngine.XR.ARFoundation.InternalUtils
{
    static class PoseUtils
    {
        /// <summary>
        /// Returns the offset between <paramref name="a"/> and <paramref name="b"/>, calculated as
        /// <c>(b.position - a.position, b.rotation * Quaternion.Inverse(a.rotation))</c>.
        /// </summary>
        /// <param name="a">Pose a.</param>
        /// <param name="b">Pose b.</param>
        /// <returns>The offset between <paramref name="a"/> and <paramref name="b"/>.</returns>
        public static Pose CalculateOffset(Pose a, Pose b)
        {
            return new Pose(b.position - a.position, b.rotation * Quaternion.Inverse(a.rotation));
        }

        /// <summary>
        /// Adds an offset to <paramref name="pose"/>, calculated as
        /// <c>(a.position + offset.position, offset.rotation * pose.rotation).</c>
        /// </summary>
        /// <param name="pose">The pose.</param>
        /// <param name="offset">The offset.</param>
        /// <returns><c>pose</c> + <c>offset</c></returns>
        public static Pose WithOffset(this Pose pose, Pose offset)
        {
            return new Pose(pose.position + offset.position, offset.rotation * pose.rotation);
        }
    }
}
