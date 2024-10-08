#if VISUALSCRIPTING_1_8_OR_NEWER

using System;
using Unity.VisualScripting;
using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.ARFoundation.VisualScripting
{
    /// <summary>
    /// Uses an <see cref="ARPlaneManagerListener"/> to listen for a planes-changed event on the visual scripting event bus
    /// and assign the <see cref="ARPlanesChangedEventArgs"/> to a newly triggered Flow.
    /// </summary>
    [UnitTitle("On Planes Changed")]
    [UnitCategory(Constants.k_Event_Unit_Category)]
    [UnitOrder((int)Constants.ARFoundationFeatureOrder.PlaneDetection)]
    [TypeIcon(typeof(ARPlaneManager))]
    public sealed class PlanesChangedEventUnit : TrackablesChangedEventUnit<
        ARPlaneManager,
        XRPlaneSubsystem,
        XRPlaneSubsystemDescriptor,
        XRPlaneSubsystem.Provider,
        BoundedPlane,
        ARPlane,
        ARPlanesChangedEventArgs,
        ARPlaneManagerListener>
    {
        /// <summary>
        /// The hook name.
        /// </summary>
        protected override string hookName => Constants.EventHookNames.planesChanged;

        /// <summary>
        /// Assigns <paramref name="args"/> to <paramref name="flow"/>.
        /// </summary>
        /// <param name="flow">The flow.</param>
        /// <param name="args">The arguments.</param>
        protected override void AssignArguments(Flow flow, ARPlanesChangedEventArgs args)
        {
            flow.SetValue(added, args.added);
            flow.SetValue(updated, args.updated);
            flow.SetValue(removed, args.removed);
        }
    }
}

#endif // VISUALSCRIPTING_1_8_OR_NEWER
