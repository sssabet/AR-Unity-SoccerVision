#if VISUALSCRIPTING_1_8_OR_NEWER

using System;
using Unity.VisualScripting;
using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.ARFoundation.VisualScripting
{
    /// <summary>
    /// Uses an <see cref="AREnvironmentProbeManagerListener"/> to listen for an environment-probes-changed event
    /// on the visual scripting event bus and assign the <see cref="ARAnchorsChangedEventArgs"/> to a newly triggered Flow.
    /// </summary>
    /// <seealso cref="AREnvironmentProbeManagerListener"/>
    [UnitTitle("On Environment Probes Changed")]
    [UnitCategory(Constants.k_Event_Unit_Category)]
    [UnitOrder((int)Constants.ARFoundationFeatureOrder.EnvironmentProbes)]
    [TypeIcon(typeof(AREnvironmentProbeManager))]
    public sealed class EnvironmentProbesChangedEventUnit : TrackablesChangedEventUnit<
        AREnvironmentProbeManager,
        XREnvironmentProbeSubsystem,
        XREnvironmentProbeSubsystemDescriptor,
        XREnvironmentProbeSubsystem.Provider,
        XREnvironmentProbe,
        AREnvironmentProbe,
        AREnvironmentProbesChangedEvent,
        AREnvironmentProbeManagerListener>
    {
        /// <summary>
        /// The hook name.
        /// </summary>
        protected override string hookName => Constants.EventHookNames.environmentProbesChanged;

        /// <summary>
        /// Assigns <paramref name="args"/> to <paramref name="flow"/>.
        /// </summary>
        /// <param name="flow">The flow.</param>
        /// <param name="args">The arguments.</param>
        protected override void AssignArguments(Flow flow, AREnvironmentProbesChangedEvent args)
        {
            flow.SetValue(added, args.added);
            flow.SetValue(updated, args.updated);
            flow.SetValue(removed, args.removed);
        }
    }
}

#endif // VISUALSCRIPTING_1_8_0R_NEWER
