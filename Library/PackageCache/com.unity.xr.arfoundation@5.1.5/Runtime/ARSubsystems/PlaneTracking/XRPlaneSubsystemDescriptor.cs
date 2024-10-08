using System;
using UnityEngine.SubsystemsImplementation;

namespace UnityEngine.XR.ARSubsystems
{
    /// <summary>
    /// Indicates the capabilities supported by a provider of the <see cref="XRPlaneSubsystem"/>. Provider
    /// implementations must derive from <see cref="XRPlaneSubsystem.Provider"/> and may override virtual class members.
    /// </summary>
    public class XRPlaneSubsystemDescriptor : SubsystemDescriptorWithProvider<XRPlaneSubsystem, XRPlaneSubsystem.Provider>
    {
        /// <summary>
        /// Indicates whether the provider implementation supports the detection of horizontal planes,
        /// such as the floor.
        /// If <see langword="false"/>, <see cref="BoundedPlane"/> trackables returned by
        /// <see cref="XRPlaneSubsystem.GetChanges">XRPlaneSubsystem.GetChanges</see> must not have their
        /// <see cref="BoundedPlane.alignment"/> values set to either <see cref="PlaneAlignment.HorizontalDown"/> or
        /// <see cref="PlaneAlignment.HorizontalUp"/>.
        /// </summary>
        /// <value><see langword="true"/> if the implementation supports horizontal plane detection.
        /// Otherwise, <see langword="false"/>.</value>
        public bool supportsHorizontalPlaneDetection { get; }

        /// <summary>
        /// Indicates whether the provider implementation supports the detection of vertical planes, such as walls.
        /// If <see langword="false"/>, <see cref="BoundedPlane"/> trackables returned by
        /// <see cref="XRPlaneSubsystem.GetChanges">XRPlaneSubsystem.GetChanges</see> must not have their
        /// <see cref="BoundedPlane.alignment"/> value set to <see cref="PlaneAlignment.Vertical"/>.
        /// </summary>
        /// <value><see langword="true"/> if the implementation supports vertical plane detection.
        /// Otherwise, <see langword="false"/>.</value>
        public bool supportsVerticalPlaneDetection { get; }

        /// <summary>
        /// Indicates whether the provider implementation supports the detection of planes that are aligned with
        /// neither the horizontal nor vertical axes.
        /// If <see langword="false"/>, <see cref="BoundedPlane"/> trackables returned by
        /// <see cref="XRPlaneSubsystem.GetChanges">XRPlaneSubsystem.GetChanges</see> must not have their
        /// <see cref="BoundedPlane.alignment"/> value set to <see cref="PlaneAlignment.NotAxisAligned"/>.
        /// </summary>
        /// <value><see langword="true"/> if the implementation supports the detection of planes oriented at arbitrary angles.
        /// Otherwise, <see langword="false"/>.</value>
        public bool supportsArbitraryPlaneDetection { get; }

        /// <summary>
        /// Indicates whether the provider implementation supports boundary vertices for its planes.
        /// If <see langword="false"/>, <see cref="XRPlaneSubsystem.GetBoundary">XRPlaneSubsystem.GetBoundary</see>
        /// must throw a <see cref="NotSupportedException"/>.
        /// </summary>
        /// <value><see langword="true"/> if the implementation supports boundary vertices for its planes.
        /// Otherwise, <see langword="false"/>.</value>
        public bool supportsBoundaryVertices { get; }

        /// <summary>
        /// Indicates whether the provider implementation can provide a value for
        /// <see cref="BoundedPlane.classification">BoundedPlane.classification</see>. If <see langword="false"/>, all
        /// planes returned by <see cref="XRPlaneSubsystem.GetChanges">XRPlaneSubsystem.GetChanges</see> will have a
        /// <c>classification</c> value of <see cref="PlaneClassification.None"/>.
        /// </summary>
        /// <value><see langword="true"/> if the implementation supports plane classification.
        /// Otherwise, <see langword="false"/>.</value>
        public bool supportsClassification { get; }

        /// <summary>
        /// Contains the parameters necessary to construct a new <see cref="XRPlaneSubsystemDescriptor"/> instance.
        /// </summary>
        public struct Cinfo : IEquatable<Cinfo>
        {
            /// <summary>
            /// The unique identifier of the provider implementation. No specific format is required.
            /// </summary>
            public string id { get; set; }

            /// <summary>
            /// The provider implementation type to use for instantiation.
            /// </summary>
            /// <value>The provider implementation type.</value>
            public Type providerType { get; set; }

            /// <summary>
            /// The <see cref="XRPlaneSubsystem"/>-derived type to use for instantiation. The instantiated instance of
            /// this type will forward casted calls to its provider.
            /// </summary>
            /// <value>The subsystem implementation type.
            /// If <see langword="null"/>, <see cref="XRPlaneSubsystem"/> will be instantiated.</value>
            public Type subsystemTypeOverride { get; set; }

            /// <summary>
            /// The concrete <c>Type</c> which will be instantiated if <c>Create</c> is called on this subsystem descriptor.
            /// </summary>
            [Obsolete("XRPlaneSubsystem no longer supports the deprecated set of base classes for subsystems as of Unity 2020.2. Use providerType and, optionally, subsystemTypeOverride instead.", true)]
            public Type subsystemImplementationType { get; set; }

            /// <summary>
            /// Indicates whether the provider implementation supports the detection of horizontal planes,
            /// such as the floor.
            /// If <see langword="false"/>, <see cref="BoundedPlane"/> trackables returned by
            /// <see cref="XRPlaneSubsystem.GetChanges">XRPlaneSubsystem.GetChanges</see> must not have their
            /// <see cref="BoundedPlane.alignment"/> value set to either <see cref="PlaneAlignment.HorizontalDown"/> or
            /// <see cref="PlaneAlignment.HorizontalUp"/>.
            /// </summary>
            /// <value><see langword="true"/> if the implementation supports horizontal plane detection.
            /// Otherwise, <see langword="false"/>.</value>
            public bool supportsHorizontalPlaneDetection { get; set; }

            /// <summary>
            /// Indicates whether the provider implementation supports the detection of vertical planes, such as walls.
            /// If <see langword="false"/>, <see cref="BoundedPlane"/> trackables returned by
            /// <see cref="XRPlaneSubsystem.GetChanges">XRPlaneSubsystem.GetChanges</see> must not have their
            /// <see cref="BoundedPlane.alignment"/> value set to <see cref="PlaneAlignment.Vertical"/>.
            /// </summary>
            /// <value><see langword="true"/> if the implementation supports vertical plane detection.
            /// Otherwise, <see langword="false"/>.</value>
            public bool supportsVerticalPlaneDetection { get; set; }

            /// <summary>
            /// Indicates whether the provider implementation supports the detection of planes that are aligned with
            /// neither the horizontal nor vertical axes.
            /// If <see langword="false"/>, <see cref="BoundedPlane"/> trackables returned by
            /// <see cref="XRPlaneSubsystem.GetChanges">XRPlaneSubsystem.GetChanges</see> must not have their
            /// <see cref="BoundedPlane.alignment"/> value set to <see cref="PlaneAlignment.NotAxisAligned"/>.
            /// </summary>
            /// <value><see langword="true"/> if the implementation supports the detection of planes oriented at arbitrary angles.
            /// Otherwise, <see langword="false"/>.</value>
            public bool supportsArbitraryPlaneDetection { get; set; }

            /// <summary>
            /// Indicates whether the provider implementation supports boundary vertices for its planes.
            /// If <see langword="false"/>, <see cref="XRPlaneSubsystem.GetBoundary">XRPlaneSubsystem.GetBoundary</see>
            /// must throw a <see cref="NotSupportedException"/>.
            /// </summary>
            /// <value><see langword="true"/> if the implementation supports boundary vertices for its planes.
            /// Otherwise, <see langword="false"/>.</value>
            public bool supportsBoundaryVertices { get; set; }

            /// <summary>
            /// Indicates whether the provider implementation can provide a value for
            /// <see cref="BoundedPlane.classification">BoundedPlane.classification</see>. If <see langword="false"/>, all
            /// planes returned by <see cref="XRPlaneSubsystem.GetChanges">XRPlaneSubsystem.GetChanges</see> will have a
            /// <c>classification</c> value of <see cref="PlaneClassification.None"/>.
            /// </summary>
            /// <value><see langword="true"/> if the implementation supports plane classification.
            /// Otherwise, <see langword="false"/>.</value>
            public bool supportsClassification { get; set; }

            /// <summary>
            /// Tests for equality.
            /// </summary>
            /// <param name="other">The other <see cref="Cinfo"/> to compare against.</param>
            /// <returns><see langword="true"/> if every field in <paramref name="other"/> is equal to this instance.
            /// Otherwise, <see langword="false"/>.</returns>
            public bool Equals(Cinfo other)
            {
                return
                    ReferenceEquals(id, other.id) &&
                    ReferenceEquals(providerType, other.providerType) &&
                    ReferenceEquals(subsystemTypeOverride, other.subsystemTypeOverride) &&
                    supportsHorizontalPlaneDetection == other.supportsHorizontalPlaneDetection &&
                    supportsVerticalPlaneDetection == other.supportsVerticalPlaneDetection &&
                    supportsArbitraryPlaneDetection == other.supportsArbitraryPlaneDetection &&
                    supportsClassification == other.supportsClassification &&
                    supportsBoundaryVertices == other.supportsBoundaryVertices;
            }

            /// <summary>
            /// Tests for equality.
            /// </summary>
            /// <param name="obj">The <c>object</c> to compare against.</param>
            /// <returns><see langword="true"/> if <paramref name="obj"/> is of type <see cref="Cinfo"/> and
            /// <see cref="Equals(Cinfo)"/> also returns <see langword="true"/>.
            /// Otherwise, <see langword="false"/>.</returns>
            public override bool Equals(object obj)
            {
                if (!(obj is Cinfo))
                    return false;

                return Equals((Cinfo)obj);
            }

            /// <summary>
            /// Generates a hash suitable for use with containers like `HashSet` and `Dictionary`.
            /// </summary>
            /// <returns>A hash code generated from this object's fields.</returns>
            public override int GetHashCode()
            {
                unchecked
                {
                    int hashCode = HashCodeUtil.ReferenceHash(id);
                    hashCode = (hashCode * 486187739) + HashCodeUtil.ReferenceHash(providerType);
                    hashCode = (hashCode * 486187739) + HashCodeUtil.ReferenceHash(subsystemTypeOverride);
                    hashCode = (hashCode * 486187739) + supportsHorizontalPlaneDetection.GetHashCode();
                    hashCode = (hashCode * 486187739) + supportsVerticalPlaneDetection.GetHashCode();
                    hashCode = (hashCode * 486187739) + supportsArbitraryPlaneDetection.GetHashCode();
                    hashCode = (hashCode * 486187739) + supportsBoundaryVertices.GetHashCode();
                    hashCode = (hashCode * 486187739) + supportsClassification.GetHashCode();
                    return hashCode;
                }
            }

            /// <summary>
            /// Tests for equality. Equivalent to <see cref="Equals(Cinfo)"/>.
            /// </summary>
            /// <param name="lhs">The left-hand side of the comparison.</param>
            /// <param name="rhs">The right-hand side of the comparison.</param>
            /// <returns><see langword="true"/> if <paramref name="lhs"/> is equal to <paramref name="rhs"/>.
            /// Otherwise, <see langword="false"/>.</returns>
            public static bool operator ==(Cinfo lhs, Cinfo rhs) => lhs.Equals(rhs);

            /// <summary>
            /// Tests for inequality. Equivalent to `!`<see cref="Equals(Cinfo)"/>.
            /// </summary>
            /// <param name="lhs">The left-hand side of the comparison.</param>
            /// <param name="rhs">The right-hand side of the comparison.</param>
            /// <returns><see langword="true"/> if <paramref name="lhs"/> is not equal to <paramref name="rhs"/>.
            /// Otherwise, <see langword="false"/>.</returns>
            public static bool operator !=(Cinfo lhs, Cinfo rhs) => !lhs.Equals(rhs);
        }

        /// <summary>
        /// Creates a new subsystem descriptor instance and registers it with the [SubsystemManager](xref:UnityEngine.SubsystemManager).
        /// </summary>
        /// <param name="cinfo">Construction info for the descriptor.</param>
        public static void Create(Cinfo cinfo)
        {
            var descriptor = new XRPlaneSubsystemDescriptor(cinfo);
            SubsystemDescriptorStore.RegisterDescriptor(descriptor);
        }

        XRPlaneSubsystemDescriptor(Cinfo cinfo)
        {
            id = cinfo.id;
            providerType = cinfo.providerType;
            subsystemTypeOverride = cinfo.subsystemTypeOverride;
            supportsHorizontalPlaneDetection = cinfo.supportsHorizontalPlaneDetection;
            supportsVerticalPlaneDetection = cinfo.supportsVerticalPlaneDetection;
            supportsArbitraryPlaneDetection = cinfo.supportsArbitraryPlaneDetection;
            supportsBoundaryVertices = cinfo.supportsBoundaryVertices;
            supportsClassification = cinfo.supportsClassification;
        }
    }
}
