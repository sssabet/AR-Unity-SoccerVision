#if VISUALSCRIPTING_1_8_OR_NEWER

using System;
using Unity.VisualScripting;
using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.ARFoundation.VisualScripting
{
    /// <summary>
    /// Uses a <see cref="ARTrackedObjectManagerListener"/> to listen for a tracked-objects-changed event on the visual
    /// scripting event bus and assign the <see cref="ARTrackedObjectsChangedEventArgs"/> to a newly triggered Flow.
    /// </summary>
    [UnitTitle("On Tracked Objects Changed")]
    [UnitCategory(Constants.k_Event_Unit_Category)]
    [UnitOrder((int)Constants.ARFoundationFeatureOrder.ObjectTracking)]
    [TypeIcon(typeof(ARTrackedObjectManager))]
    public sealed class TrackedObjectsChangedEventUnit : TrackablesChangedEventUnit<
        ARTrackedObjectManager,
        XRObjectTrackingSubsystem,
        XRObjectTrackingSubsystemDescriptor,
        XRObjectTrackingSubsystem.Provider,
        XRTrackedObject,
        ARTrackedObject,
        ARTrackedObjectsChangedEventArgs,
        ARTrackedObjectManagerListener>
    {
        /// <summary>
        /// The hook name.
        /// </summary>
        protected override string hookName => Constants.EventHookNames.trackedObjectsChanged;

        /// <summary>
        /// Assigns <paramref name="args"/> to <paramref name="flow"/>.
        /// </summary>
        /// <param name="flow">The flow.</param>
        /// <param name="args">The arguments.</param>
        protected override void AssignArguments(Flow flow, ARTrackedObjectsChangedEventArgs args)
        {
            flow.SetValue(added, args.added);
            flow.SetValue(updated, args.updated);
            flow.SetValue(removed, args.removed);
        }
    }
}

#endif // VISUALSCRIPTING_1_8_OR_NEWER
