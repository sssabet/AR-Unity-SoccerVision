#if VISUALSCRIPTING_1_8_OR_NEWER

using System;
using Unity.VisualScripting;
using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.ARFoundation.VisualScripting
{
    /// <summary>
    /// Uses an <see cref="ARFaceManagerListener"/> to listen for a faces-changed event on the visual scripting event bus and
    /// assign the <see cref="ARFacesChangedEventArgs"/> to a newly triggered Flow.
    /// </summary>
    /// <seealso cref="ARFaceManagerListener"/>
    [UnitTitle("On Faces Changed")]
    [UnitCategory(Constants.k_Event_Unit_Category)]
    [UnitOrder((int)Constants.ARFoundationFeatureOrder.FaceTracking)]
    [TypeIcon(typeof(ARFaceManager))]
    public sealed class FacesChangedEventUnit : TrackablesChangedEventUnit<
        ARFaceManager,
        XRFaceSubsystem,
        XRFaceSubsystemDescriptor,
        XRFaceSubsystem.Provider,
        XRFace,
        ARFace,
        ARFacesChangedEventArgs,
        ARFaceManagerListener>
    {
        /// <summary>
        /// The hook name.
        /// </summary>
        protected override string hookName => Constants.EventHookNames.facesChanged;

        /// <summary>
        /// Assigns <paramref name="args"/> to <paramref name="flow"/>.
        /// </summary>
        /// <param name="flow">The flow.</param>
        /// <param name="args">The arguments.</param>
        protected override void AssignArguments(Flow flow, ARFacesChangedEventArgs args)
        {
            flow.SetValue(added, args.added);
            flow.SetValue(updated, args.updated);
            flow.SetValue(removed, args.removed);
        }
    }
}

#endif // VISUALSCRIPTING_1_8_OR_NEWER
