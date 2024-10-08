#if VISUALSCRIPTING_1_8_OR_NEWER

using Unity.VisualScripting;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARFoundation.VisualScripting;
using UnityEngine.XR.ARSubsystems;

namespace UnityEditor.XR.ARFoundation.VisualScripting
{
    /// <summary>
    /// Unit descriptor for the <see cref="TrackedObjectsChangedEventUnit"/>.
    /// </summary>
    /// <seealso href="https://docs.unity3d.com/Packages/com.unity.visualscripting@1.8/manual/vs-create-custom-node-add-docs.html"/>
    [Descriptor(typeof(TrackedObjectsChangedEventUnit))]
    public sealed class TrackedObjectsChangedEventUnitDescriptor : TrackablesChangedEventUnitDescriptor<
        ARTrackedObjectManager,
        XRObjectTrackingSubsystem,
        XRObjectTrackingSubsystemDescriptor,
        XRObjectTrackingSubsystem.Provider,
        XRTrackedObject,
        ARTrackedObject,
        ARTrackedObjectsChangedEventArgs,
        ARTrackedObjectManagerListener>
    {
        /// <inheritdoc/>
        public TrackedObjectsChangedEventUnitDescriptor(TrackedObjectsChangedEventUnit target) : base(target)
        {
        }
    }
}

#endif // VISUALSCRIPTING_1_8_OR_NEWER
