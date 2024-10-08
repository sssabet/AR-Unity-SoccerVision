using System;
using Unity.Collections;
using UnityEngine.SubsystemsImplementation;

namespace UnityEngine.XR.ARSubsystems
{
    /// <summary>
    /// This subsystem provides information regarding anchors. An anchor is a pose (position and rotation) in the physical
    /// environment that is tracked by an XR device. Anchors are updated as the device refines its understanding of the
    /// environment, allowing you to reliably place virtual content at a real-world pose.
    /// </summary>
    /// <remarks>
    /// This is a base class with an abstract provider type to be implemented by provider plug-in packages.
    /// This class itself does not implement anchor tracking.
    /// </remarks>
    public class XRAnchorSubsystem
        : TrackingSubsystem<XRAnchor, XRAnchorSubsystem, XRAnchorSubsystemDescriptor, XRAnchorSubsystem.Provider>
    {
        /// <summary>
        /// Do not invoke this constructor directly.
        /// </summary>
        /// <remarks>
        /// If you are implementing your own custom subsystem [Lifecycle management](xref:xr-plug-in-management-provider#lifecycle-management),
        /// use the [SubsystemManager](xref:UnityEngine.SubsystemManager)
        /// to enumerate the available <see cref="XRAnchorSubsystemDescriptor"/>s, then call
        /// <see cref="ISubsystemDescriptor.Create">XRAnchorSubsystemDescriptor.Create()</see> on the desired descriptor.
        /// </remarks>
        public XRAnchorSubsystem() { }

        /// <summary>
        /// Get the changes to anchors (added, updated, and removed) since the last call to this method.
        /// </summary>
        /// <param name="allocator">An allocator to use for the <c>NativeArray</c>s in <see cref="TrackableChanges{T}"/>.</param>
        /// <returns>Changes since the last call to this method.</returns>
        public override TrackableChanges<XRAnchor> GetChanges(Allocator allocator)
        {
            if (!running)
                throw new InvalidOperationException($"Can't call {nameof(GetChanges)} without \"Start\"ing the anchor subsystem!");

            var changes = provider.GetChanges(XRAnchor.defaultValue, allocator);
#if DEVELOPMENT_BUILD || UNITY_EDITOR
            m_ValidationUtility.ValidateAndDisposeIfThrown(changes);
#endif
            return changes;
        }

        /// <summary>
        /// Attempts to create a new anchor at the provided <paramref name="pose"/>.
        /// </summary>
        /// <param name="pose">The pose, in session space, of the new anchor.</param>
        /// <param name="anchor">The new anchor. Only valid if this method returns <see langword="true"/>.</param>
        /// <returns><see langword="true"/> if the output anchor was added. Otherwise, <see langword="false"/>.</returns>
        public bool TryAddAnchor(Pose pose, out XRAnchor anchor)
        {
            return provider.TryAddAnchor(pose, out anchor);
        }

        /// <summary>
        /// Attempts to create a new anchor "attached" to the trackable with id <paramref name="trackableToAffix"/>.
        /// The behavior of the anchor depends on the type of trackable to which this anchor is attached.
        /// </summary>
        /// <param name="trackableToAffix">The id of the trackable to which to attach.</param>
        /// <param name="pose">The pose, in session space, of the anchor to create.</param>
        /// <param name="anchor">The new anchor. Only valid if this method returns <c>true</c>.</param>
        /// <returns><c>true</c> if the new anchor was added, otherwise <c>false</c>.</returns>
        public bool TryAttachAnchor(TrackableId trackableToAffix, Pose pose, out XRAnchor anchor)
        {
            return provider.TryAttachAnchor(trackableToAffix, pose, out anchor);
        }

        /// <summary>
        /// Attempts to remove an existing anchor with <see cref="TrackableId"/> <paramref name="anchorId"/>.
        /// </summary>
        /// <param name="anchorId">The id of an existing anchor to remove.</param>
        /// <returns><c>true</c> if the anchor was removed, otherwise <c>false</c>.</returns>
        public bool TryRemoveAnchor(TrackableId anchorId)
        {
            return provider.TryRemoveAnchor(anchorId);
        }

        /// <summary>
        /// An abstract class to be implemented by providers of this subsystem.
        /// </summary>
        public abstract class Provider : SubsystemProvider<XRAnchorSubsystem>
        {
            /// <summary>
            /// Gets a <see cref="TrackableChanges{T}"/> struct containing any changes to anchors since the last
            /// time you called this method. You are responsible to <see cref="TrackableChanges{T}.Dispose"/> the returned
            /// <c>TrackableChanges</c> instance.
            /// </summary>
            /// <param name="defaultAnchor">The default anchor. You should use this to initialize the returned
            ///   <see cref="TrackableChanges{T}"/> instance by passing it to the constructor
            ///   <see cref="TrackableChanges{T}(int,int,int,Allocator,T)"/>.
            /// </param>
            /// <param name="allocator">An <c>Allocator</c> to use when allocating the returned <c>NativeArray</c>s.</param>
            /// <returns>The changes to anchors since the last call to this method.</returns>
            public abstract TrackableChanges<XRAnchor> GetChanges(XRAnchor defaultAnchor, Allocator allocator);

            /// <summary>
            /// Should create a new anchor with the provided <paramref name="pose"/>.
            /// </summary>
            /// <param name="pose">The pose, in session space, of the new anchor.</param>
            /// <param name="anchor">The new anchor. Must be valid only if this method returns <c>true</c>.</param>
            /// <returns>Should return <c>true</c> if the new anchor was added, otherwise <c>false</c>.</returns>
            public virtual bool TryAddAnchor(Pose pose, out XRAnchor anchor)
            {
                anchor = default;
                return false;
            }

            /// <summary>
            /// Should create a new anchor "attached" to the trackable with id <paramref name="attachedToId"/>.
            /// The behavior of the anchor depends on the type of trackable to which this anchor is attached and
            /// might be implementation-defined.
            /// </summary>
            /// <param name="attachedToId">The id of the trackable to which to attach.</param>
            /// <param name="pose">The pose, in session space, of the anchor to create.</param>
            /// <param name="anchor">The new anchor. Must be valid only if this method returns <c>true</c>.</param>
            /// <returns><c>true</c> if the new anchor was added, otherwise <c>false</c>.</returns>
            public virtual bool TryAttachAnchor(
                TrackableId attachedToId,
                Pose pose,
                out XRAnchor anchor)
            {
                anchor = default;
                return false;
            }

            /// <summary>
            /// Should remove an existing anchor with <see cref="TrackableId"/> <paramref name="anchorId"/>.
            /// </summary>
            /// <param name="anchorId">The id of an existing anchor to remove.</param>
            /// <returns>Should return <c>true</c> if the anchor was removed, otherwise <c>false</c>. If the anchor
            /// does not exist, return <c>false</c>.</returns>
            public virtual bool TryRemoveAnchor(TrackableId anchorId) => false;
        }

#if DEVELOPMENT_BUILD || UNITY_EDITOR
        ValidationUtility<XRAnchor> m_ValidationUtility = new();
#endif
    }
}
