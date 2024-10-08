#if VISUALSCRIPTING_1_8_OR_NEWER

using System;
using Unity.VisualScripting;

namespace UnityEngine.XR.ARFoundation.VisualScripting
{
    /// <summary>
    /// Uses an <see cref="ARCameraManagerListener"/> to listen for a camera-frame-received event on the
    /// visual scripting event bus and assign the <see cref="ARCameraFrameEventArgs"/> to a newly triggered Flow.
    /// </summary>
    [UnitTitle("On Camera Frame Received")]
    [UnitCategory(Constants.k_Event_Unit_Category)]
    [UnitOrder((int)Constants.ARFoundationFeatureOrder.Camera)]
    [TypeIcon(typeof(ARCameraManager))]
    public sealed class CameraFrameReceivedEventUnit : GameObjectEventUnit<ARCameraFrameEventArgs>
    {
        /// <summary>
        /// <see cref="ARCameraFrameEventArgs"/> output.
        /// </summary>
        [DoNotSerialize]
        public ValueOutput frameEventArgs { get; private set; }

        /// <summary>
        /// The hook name.
        /// </summary>
        protected override string hookName => Constants.EventHookNames.cameraFrameReceived;

        /// <summary>
        /// The <see cref="MessageListener"/> type.
        /// </summary>
        public override Type MessageListenerType => typeof(ARCameraManagerListener);

        /// <summary>
        /// Unit definition.
        /// </summary>
        protected override void Definition()
        {
            base.Definition();
            frameEventArgs = ValueOutput<ARCameraFrameEventArgs>(nameof(frameEventArgs));
        }

        /// <summary>
        /// Assigns <paramref name="args"/> to <paramref name="flow"/>.
        /// </summary>
        /// <param name="flow">The flow.</param>
        /// <param name="args">The arguments.</param>
        protected override void AssignArguments(Flow flow, ARCameraFrameEventArgs args)
        {
            flow.SetValue(frameEventArgs, args);
        }
    }
}

#endif // VISUALSCRIPTING_1_8_OR_NEWER
