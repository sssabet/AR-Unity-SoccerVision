using System;
using Unity.Collections;
using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.Simulation
{
    struct DiscoveredPlane : IEquatable<DiscoveredPlane>, IDisposable
    {
        static readonly DiscoveredPlane s_Default = new(
                TrackableId.invalidId,
                TrackableId.invalidId,
                Pose.identity,
                Vector2.zero,
                Vector2.zero,
                PlaneAlignment.None,
                TrackingState.None,
                PlaneClassification.None,
                default);

        /// <summary>
        /// Gets a default-initialized <see cref="DiscoveredPlane"/>. This can be
        /// different from the zero-initialized version, e.g., the <see cref="pose"/>
        /// is <c>Pose.identity</c> instead of zero-initialized.
        /// </summary>
        public static DiscoveredPlane defaultValue => s_Default;

        TrackableId m_TrackableId;
        TrackableId m_SubsumedById;
        Vector2 m_Center;
        Pose m_Pose;
        Vector2 m_Size;
        PlaneAlignment m_Alignment;
        TrackingState m_TrackingState;
        PlaneClassification m_Classification;
        NativeArray<Vector2> m_Vertices;

        /// <summary>
        /// The <see cref="TrackableId"/> associated with this plane.
        /// </summary>
        public TrackableId trackableId => m_TrackableId;

        /// <summary>
        /// The <see cref="TrackableId"/> associated with the plane which subsumed this one. Will be <see cref="TrackableId.invalidId"/>
        /// if this plane has not been subsumed.
        /// </summary>
        public TrackableId subsumedById => m_SubsumedById;

        /// <summary>
        /// The <c>Pose</c>, in session space, of the plane.
        /// </summary>
        public Pose pose => m_Pose;

        /// <summary>
        /// The center of the plane in plane space (relative to its <see cref="pose"/>).
        /// </summary>
        public Vector2 center => m_Center;

        /// <summary>
        /// The extents of the plane (half dimensions) in meters.
        /// </summary>
        public Vector2 extents => m_Size * 0.5f;

        /// <summary>
        /// The size (dimensions) of the plane in meters.
        /// </summary>
        public Vector2 size => m_Size;

        /// <summary>
        /// The <see cref="PlaneAlignment"/> of the plane.
        /// </summary>
        public PlaneAlignment alignment => m_Alignment;

        /// <summary>
        /// The <see cref="TrackingState"/> of the plane.
        /// </summary>
        public TrackingState trackingState => m_TrackingState;

        /// <summary>
        /// The <see cref="PlaneClassification"/> of the plane.
        /// </summary>
        public PlaneClassification classification => m_Classification;

        /// <summary>
        /// Creates a read-only alias to the <c>NativeArray</c> of the boundary vertices.
        /// The caller cannot Dispose it.
        /// </summary>
        public NativeArray<Vector2>.ReadOnly vertices => !m_Vertices.IsCreated ? default : m_Vertices.AsReadOnly();

        public BoundedPlane boundedPlane => new(
            m_TrackableId,
            m_SubsumedById,
            m_Pose,
            m_Center,
            m_Size,
            m_Alignment,
            m_TrackingState,
            IntPtr.Zero,
            m_Classification);

        /// <summary>
        /// Constructs a new <see cref="DiscoveredPlane"/>. This is just a data container
        /// for a plane's simulation data. These are typically created by
        /// <see cref="XRPlaneSubsystem.GetChanges(Unity.Collections.Allocator)"/>.
        /// </summary>
        /// <param name="trackableId">The <see cref="TrackableId"/> associated with the plane.</param>
        /// <param name="subsumedById">The plane which subsumed this one. Use <see cref="TrackableId.invalidId"/> if it has not been subsumed.</param>
        /// <param name="pose">The <c>Pose</c> associated with the plane.</param>
        /// <param name="center">The center of the plane, in plane space (relative to <paramref name="pose"/>).</param>
        /// <param name="size">The dimensions associated with the plane.</param>
        /// <param name="alignment">The <see cref="PlaneAlignment"/> associated with the plane.</param>
        /// <param name="trackingState">The <see cref="TrackingState"/> associated with the plane.</param>
        /// <param name="classification">The <see cref="PlaneClassification"/> associated with the plane.</param>
        /// <param name="vertices">The <c>NativeArray</c> of the boundary associated with the plane.</param>
        public DiscoveredPlane(
            TrackableId trackableId,
            TrackableId subsumedById,
            Pose pose,
            Vector2 center,
            Vector2 size,
            PlaneAlignment alignment,
            TrackingState trackingState,
            PlaneClassification classification,
            NativeArray<Vector2> vertices)
        {
            m_TrackableId = trackableId;
            m_SubsumedById = subsumedById;
            m_Pose = pose;
            m_Center = center;
            m_Size = size;
            m_Alignment = alignment;
            m_TrackingState = trackingState;
            m_Classification = classification;

            m_Vertices = vertices.IsCreated ? new NativeArray<Vector2>(vertices, Allocator.Persistent) : default;
        }

        /// <summary>
        /// Generates a new string that describes the plane's properties, suitable for debugging purposes.
        /// </summary>
        /// <returns>A string that describes the plane's properties.</returns>
        public override string ToString()
        {
            return string.Format(
                "Plane:\n\ttrackableId: {0}\n\tsubsumedById: {1}\n\tpose: {2}\n\tcenter: {3}\n\tsize: {4}\n\talignment: {5}\n\tclassification: {6}\n\ttrackingState: {7}",
                trackableId,
                subsumedById,
                pose,
                center,
                size,
                alignment,
                classification,
                trackingState);
        }

        /// <summary>
        /// Disposes the <c>NativeArray</c> of vertices.
        /// </summary>
        public void Dispose()
        {
            if (m_Vertices.IsCreated)
                m_Vertices.Dispose();
        }

        /// <summary>
        /// Tests for equality.
        /// </summary>
        /// <param name="obj">The `object` to compare against.</param>
        /// <returns>`True` if <paramref name="obj"/> is of type <see cref="DiscoveredPlane"/> and
        /// <see cref="Equals(DiscoveredPlane)"/> also returns `true`; otherwise `false`.</returns>
        public override bool Equals(object obj) => (obj is DiscoveredPlane other) && Equals(other);

        /// <summary>
        /// Generates a hash suitable for use with containers like `HashSet` and `Dictionary`.
        /// </summary>
        /// <returns>A hash code generated from this object's fields.</returns>
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = m_TrackableId.GetHashCode();
                hashCode = (hashCode * 486187739) + m_SubsumedById.GetHashCode();
                hashCode = (hashCode * 486187739) + m_Pose.GetHashCode();
                hashCode = (hashCode * 486187739) + m_Center.GetHashCode();
                hashCode = (hashCode * 486187739) + m_Size.GetHashCode();
                hashCode = (hashCode * 486187739) + ((int)m_Alignment).GetHashCode();
                hashCode = (hashCode * 486187739) + ((int)m_Classification).GetHashCode();
                hashCode = (hashCode * 486187739) + ((int)m_TrackingState).GetHashCode();
                hashCode = (hashCode * 486187739) + m_Vertices.GetHashCode();
                return hashCode;
            }
        }

        /// <summary>
        /// Tests for equality. Same as <see cref="Equals(DiscoveredPlane)"/>.
        /// </summary>
        /// <param name="lhs">The left-hand side of the comparison.</param>
        /// <param name="rhs">The right-hand side of the comparison.</param>
        /// <returns>`True` if <paramref name="lhs"/> is equal to <paramref name="rhs"/>, otherwise `false`.</returns>
        public static bool operator ==(DiscoveredPlane lhs, DiscoveredPlane rhs) => lhs.Equals(rhs);

        /// <summary>
        /// Tests for inequality. Same as `!`<see cref="Equals(DiscoveredPlane)"/>.
        /// </summary>
        /// <param name="lhs">The left-hand side of the comparison.</param>
        /// <param name="rhs">The right-hand side of the comparison.</param>
        /// <returns>`True` if <paramref name="lhs"/> is not equal to <paramref name="rhs"/>, otherwise `false`.</returns>
        public static bool operator !=(DiscoveredPlane lhs, DiscoveredPlane rhs) => !lhs.Equals(rhs);

        /// <summary>
        /// Tests for equality.
        /// </summary>
        /// <summary>
        /// Tests for equality.
        /// </summary>
        /// <param name="other">The other <see cref="DiscoveredPlane"/> to compare against.</param>
        /// <returns>`True` if every field in <paramref name="other"/> is equal to this <see cref="DiscoveredPlane"/>, otherwise false.</returns>
        public bool Equals(DiscoveredPlane other)
        {
            return
                m_TrackableId.Equals(other.m_TrackableId) &&
                m_SubsumedById.Equals(other.m_SubsumedById) &&
                m_Pose.Equals(other.m_Pose) &&
                m_Center.Equals(other.m_Center) &&
                m_Size.Equals(other.m_Size) &&
                (m_Alignment == other.m_Alignment) &&
                (m_Classification == other.m_Classification) &&
                (m_TrackingState == other.m_TrackingState) &&
                (m_Vertices == other.m_Vertices);
        }
    }
}
