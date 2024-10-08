#if VISUALSCRIPTING_1_8_OR_NEWER

using System;
using Unity.VisualScripting;
using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.ARFoundation.VisualScripting
{
    /// <summary>
    /// Uses an <see cref="ARPointCloudManagerListener"/> to listen for a point-clouds-changed event on the visual scripting
    /// event bus and assign the <see cref="ARPointCloudChangedEventArgs"/> to a newly triggered Flow.
    /// </summary>
    [UnitTitle("On Point Clouds Changed")]
    [UnitCategory(Constants.k_Event_Unit_Category)]
    [UnitOrder((int)Constants.ARFoundationFeatureOrder.PointClouds)]
    [TypeIcon(typeof(ARPointCloudManager))]
    public sealed class PointCloudsChangedEventUnit : TrackablesChangedEventUnit<
        ARPointCloudManager,
        XRPointCloudSubsystem,
        XRPointCloudSubsystemDescriptor,
        XRPointCloudSubsystem.Provider,
        XRPointCloud,
        ARPointCloud,
        ARPointCloudChangedEventArgs,
        ARPointCloudManagerListener>
    {
        /// <summary>
        /// The hook name.
        /// </summary>
        protected override string hookName => Constants.EventHookNames.pointCloudsChanged;

        /// <summary>
        /// Assigns <paramref name="args"/> to <paramref name="flow"/>.
        /// </summary>
        /// <param name="flow">The flow.</param>
        /// <param name="args">The arguments.</param>
        protected override void AssignArguments(Flow flow, ARPointCloudChangedEventArgs args)
        {
            flow.SetValue(added, args.added);
            flow.SetValue(updated, args.updated);
            flow.SetValue(removed, args.removed);
        }
    }
}

#endif // VISUALSCRIPTING_1_8_OR_NEWER
