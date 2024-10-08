#if VISUALSCRIPTING_1_8_OR_NEWER

using System;
using Unity.VisualScripting;
using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.ARFoundation.VisualScripting
{
    /// <summary>
    /// Uses an <see cref="ARTrackedImageManagerListener"/> to listen for a tracked-images-changed event on the visual scripting
    /// event bus and assign the <see cref="ARTrackedImagesChangedEventArgs"/> to a newly triggered Flow.
    /// </summary>
    [UnitTitle("On Tracked Images Changed")]
    [UnitCategory(Constants.k_Event_Unit_Category)]
    [UnitOrder((int)Constants.ARFoundationFeatureOrder.ImageTracking)]
    [TypeIcon(typeof(ARTrackedImageManager))]
    public sealed class TrackedImagesChangedEventUnit : TrackablesChangedEventUnit<
        ARTrackedImageManager,
        XRImageTrackingSubsystem,
        XRImageTrackingSubsystemDescriptor,
        XRImageTrackingSubsystem.Provider,
        XRTrackedImage,
        ARTrackedImage,
        ARTrackedImagesChangedEventArgs,
        ARTrackedImageManagerListener>
    {
        /// <summary>
        /// The hook name.
        /// </summary>
        protected override string hookName => Constants.EventHookNames.trackedImagesChanged;

        /// <summary>
        /// Assigns <paramref name="args"/> to <paramref name="flow"/>.
        /// </summary>
        /// <param name="flow">The flow.</param>
        /// <param name="args">The arguments.</param>
        protected override void AssignArguments(Flow flow, ARTrackedImagesChangedEventArgs args)
        {
            flow.SetValue(added, args.added);
            flow.SetValue(updated, args.updated);
            flow.SetValue(removed, args.removed);
        }
    }
}

#endif // VISUALSCRIPTING_1_8_OR_NEWER
