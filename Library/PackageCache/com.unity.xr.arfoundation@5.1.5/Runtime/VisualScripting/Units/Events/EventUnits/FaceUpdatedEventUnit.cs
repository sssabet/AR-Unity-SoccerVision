#if VISUALSCRIPTING_1_8_OR_NEWER

using System;
using Unity.VisualScripting;

namespace UnityEngine.XR.ARFoundation.VisualScripting
{
    /// <summary>
    /// Uses an <see cref="ARFaceListener"/> to listen for an AR Face updated event on the visual scripting event bus and
    /// assign the <see cref="ARFaceUpdatedEventArgs"/> to a newly triggered Flow.
    /// </summary>
    [UnitTitle("On Face Updated")]
    [UnitCategory(Constants.k_Event_Unit_Category)]
    [UnitOrder((int)Constants.ARFoundationFeatureOrder.FaceTracking)]
    [TypeIcon(typeof(ARFace))]
    public sealed class FaceUpdatedEventUnit : GameObjectEventUnit<ARFaceUpdatedEventArgs>
    {
        /// <summary>
        /// <see cref="ARFaceUpdatedEventArgs"/> output.
        /// </summary>
        [DoNotSerialize]
        [PortLabel("Updated ARFace")]
        public ValueOutput faceOut { get; private set; }

        /// <summary>
        /// The hook name.
        /// </summary>
        protected override string hookName => Constants.EventHookNames.faceUpdated;

        /// <summary>
        /// The <see cref="MessageListener"/> type.
        /// </summary>
        public override Type MessageListenerType => typeof(ARFaceListener);

        /// <summary>
        /// Unit definition.
        /// </summary>
        protected override void Definition()
        {
            base.Definition();
            faceOut = ValueOutput<ARFace>(nameof(faceOut));
        }

        /// <summary>
        /// Assigns <paramref name="args"/> to <paramref name="flow"/>.
        /// </summary>
        /// <param name="flow">The flow.</param>
        /// <param name="args">The arguments.</param>
        protected override void AssignArguments(Flow flow, ARFaceUpdatedEventArgs args)
        {
            flow.SetValue(faceOut, args.face);
        }
    }
}

#endif // VISUALSCRIPTING_1_8_OR_NEWER
