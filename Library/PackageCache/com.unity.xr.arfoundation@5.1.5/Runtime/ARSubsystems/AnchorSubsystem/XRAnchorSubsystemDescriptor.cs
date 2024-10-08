using System;
using UnityEngine.SubsystemsImplementation;

namespace UnityEngine.XR.ARSubsystems
{
    /// <summary>
    /// Indicates the capabilities supported by a provider of the <see cref="XRAnchorSubsystem"/>. Provider
    /// implementations must derive from <see cref="XRAnchorSubsystem.Provider"/> and may override virtual class members.
    /// </summary>
    public class XRAnchorSubsystemDescriptor :
        SubsystemDescriptorWithProvider<XRAnchorSubsystem, XRAnchorSubsystem.Provider>
    {
        /// <summary>
        /// Indicates whether the provider implementation supports attachments (that is, the ability to attach an anchor to a trackable).
        /// If <see langword="false"/>, <see cref="XRAnchorSubsystem.TryAttachAnchor">XRAnchorSubsystem.TryAttachAnchor</see>
        /// must always return <see langword="false"/>.
        /// </summary>
        public bool supportsTrackableAttachments { get; }

        /// <summary>
        /// Contains the parameters necessary to construct a new <see cref="XRAnchorSubsystemDescriptor"/> instance.
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
            /// The <see cref="XRAnchorSubsystem"/>-derived type to use for instantiation. The instantiated instance of
            /// this type will forward casted calls to its provider.
            /// </summary>
            /// <value>The subsystem implementation type.
            /// If <see langword="null"/>, <see cref="XRAnchorSubsystem"/> will be instantiated.</value>
            public Type subsystemTypeOverride { get; set; }

            /// <summary>
            /// The concrete <c>Type</c> of the subsystem which will be instantiated if a subsystem is created from this descriptor.
            /// </summary>
            [Obsolete("XRAnchorSubsystem no longer supports the deprecated set of base classes for subsystems as of Unity 2020.2. Use providerType and, optionally, subsystemTypeOverride instead.", true)]
            public Type subsystemImplementationType { get; set; }

            /// <summary>
            /// Indicates whether the provider implementation supports attachments (that is, the ability to attach an anchor to a trackable).
            /// If <see langword="false"/>, <see cref="XRAnchorSubsystem.TryAttachAnchor">XRAnchorSubsystem.TryAttachAnchor</see>
            /// must always return <see langword="false"/>.
            /// </summary>
            public bool supportsTrackableAttachments { get; set; }

            /// <summary>
            /// Generates a hash suitable for use with containers like `HashSet` and `Dictionary`.
            /// </summary>
            /// <returns>A hash code generated from this object's fields.</returns>
            public override int GetHashCode()
            {
                unchecked
                {
                    int hash = HashCodeUtil.ReferenceHash(id);
                    hash = 486187739 * hash + HashCodeUtil.ReferenceHash(providerType);
                    hash = 486187739 * hash + HashCodeUtil.ReferenceHash(subsystemTypeOverride);
                    hash = 486187738 * hash + supportsTrackableAttachments.GetHashCode();
                    return hash;
                }
            }

            /// <summary>
            /// Tests for equality.
            /// </summary>
            /// <param name="obj">The `object` to compare against.</param>
            /// <returns><see langword="true"/> if <paramref name="obj"/> is of type <see cref="Cinfo"/> and
            /// <see cref="Equals(Cinfo)"/> also returns <see langword="true"/>.
            /// Otherwise, <see langword="false"/>.</returns>
            public override bool Equals(object obj) => (obj is Cinfo other) && Equals(other);

            /// <summary>
            /// Tests for equality.
            /// </summary>
            /// <param name="other">The other <see cref="Cinfo"/> to compare against.</param>
            /// <returns><see langword="true"/> if every field in <paramref name="other"/> is equal to this <see cref="Cinfo"/>.
            /// Otherwise, <see langword="false"/>.</returns>
            public bool Equals(Cinfo other)
            {
                return
                    string.Equals(id, other.id) &&
                    ReferenceEquals(providerType, other.providerType) &&
                    ReferenceEquals(subsystemTypeOverride, other.subsystemTypeOverride) &&
                    supportsTrackableAttachments == other.supportsTrackableAttachments;
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
            SubsystemDescriptorStore.RegisterDescriptor(new XRAnchorSubsystemDescriptor(cinfo));
        }

        XRAnchorSubsystemDescriptor(Cinfo cinfo)
        {
            id = cinfo.id;
            providerType = cinfo.providerType;
            subsystemTypeOverride = cinfo.subsystemTypeOverride;
            supportsTrackableAttachments = cinfo.supportsTrackableAttachments;
        }
    }
}
