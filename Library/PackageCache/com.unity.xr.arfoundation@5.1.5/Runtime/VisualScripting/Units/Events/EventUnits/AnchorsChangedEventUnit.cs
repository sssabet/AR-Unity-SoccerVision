#if VISUALSCRIPTING_1_8_OR_NEWER

using System;
using Unity.VisualScripting;
using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.ARFoundation.VisualScripting
{
    /// <summary>
    /// Uses an <see cref="ARAnchorManagerListener"/> to listen for an anchors-changed event on the visual scripting event bus and
    /// assign the <see cref="ARAnchorsChangedEventArgs"/> to a newly triggered Flow.
    /// </summary>
    [UnitTitle("On Anchors Changed")]
    [UnitCategory(Constants.k_Event_Unit_Category)]
    [UnitOrder((int)Constants.ARFoundationFeatureOrder.Anchors)]
    [TypeIcon(typeof(ARAnchorManager))]
    public sealed class AnchorsChangedEventUnit : TrackablesChangedEventUnit<
        ARAnchorManager,
        XRAnchorSubsystem,
        XRAnchorSubsystemDescriptor,
        XRAnchorSubsystem.Provider,
        XRAnchor,
        ARAnchor,
        ARAnchorsChangedEventArgs,
        ARAnchorManagerListener>
    {
        /// <summary>
        /// The hook name.
        /// </summary>
        protected override string hookName => Constants.EventHookNames.anchorsChanged;

        /// <summary>
        /// Assigns <paramref name="args"/> to <paramref name="flow"/>.
        /// </summary>
        /// <param name="flow">The flow.</param>
        /// <param name="args">The arguments.</param>
        protected override void AssignArguments(Flow flow, ARAnchorsChangedEventArgs args)
        {
            flow.SetValue(added, args.added);
            flow.SetValue(updated, args.updated);
            flow.SetValue(removed, args.removed);
        }
    }
}

#endif // VISUALSCRIPTING_1_8_OR_NEWER
