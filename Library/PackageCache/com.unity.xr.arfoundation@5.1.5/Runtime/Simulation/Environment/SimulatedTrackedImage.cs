using System;
using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.Simulation
{
    /// <summary>
    /// Marks an object in a simulation environment as a source from which to provide a tracked image.
    /// This component is required by the <see cref="SimulationImageTrackingSubsystem"/> on all GameObjects
    /// which represent tracked images in an environment.
    /// </summary>
    class SimulatedTrackedImage : MonoBehaviour
    {
        const float k_MinSideLengthMeters = .01f;

        [SerializeField, Tooltip("The image to track.")]
        Texture2D m_Image;

        [SerializeField, Tooltip("The world-space size of the image, in meters.")]
        Vector2 m_ImagePhysicalSizeMeters = new(k_MinSideLengthMeters, k_MinSideLengthMeters);

        /// <summary>
        /// The tracked image's texture. If the user does not provide a texture at edit time, a new texture will
        /// be generated at runtime.
        /// </summary>
        public Texture2D texture => m_Image ??= new Texture2D(0, 0);

        /// <summary>
        /// The world-space width and height of the tracked image.
        /// </summary>
        public Vector2 size => m_ImagePhysicalSizeMeters;

        /// <summary>
        /// The <see cref="TrackableId"/> for the tracked image.
        /// </summary>
        public TrackableId trackableId { get; private set; } = TrackableId.invalidId;

        /// <summary>
        /// A unique 128-bit ID associated with the content of the tracked image.
        /// </summary>
        /// <remarks>
        /// This method should only be used as a fallback strategy to generate a GUID,
        /// in the event that the <see cref="SimulationImageTrackingSubsystem"/>'s
        /// runtime reference image library does not contain a reference image matching our <c>image</c>.
        /// </remarks>
        public Guid fallbackSourceImageId { get; private set; } = Guid.Empty;

        TrackableId GenerateTrackableID()
        {
            var unsignedInstanceId = (ulong)Math.Abs(Convert.ToInt64(gameObject.GetInstanceID()));
            return new TrackableId(unsignedInstanceId, 0);
        }

        Guid GenerateSourceImageId()
        {
            var unsignedInstanceId = (ulong)Math.Abs(Convert.ToInt64(texture.GetInstanceID()));
            return GuidUtil.Compose(unsignedInstanceId, 0);
        }

        /// <summary>
        /// Prevent users from entering an invalid value for the image's physical size.
        /// </summary>
        void OnValidate()
        {
            if (m_ImagePhysicalSizeMeters.x >= k_MinSideLengthMeters && m_ImagePhysicalSizeMeters.y >= k_MinSideLengthMeters)
                return;

            m_ImagePhysicalSizeMeters = new Vector2(
                m_ImagePhysicalSizeMeters.x < k_MinSideLengthMeters ? k_MinSideLengthMeters : m_ImagePhysicalSizeMeters.x,
                m_ImagePhysicalSizeMeters.y < k_MinSideLengthMeters ? k_MinSideLengthMeters : m_ImagePhysicalSizeMeters.y);
        }

        void Awake()
        {
            fallbackSourceImageId = GenerateSourceImageId();
            trackableId = GenerateTrackableID();
        }
    }
}
