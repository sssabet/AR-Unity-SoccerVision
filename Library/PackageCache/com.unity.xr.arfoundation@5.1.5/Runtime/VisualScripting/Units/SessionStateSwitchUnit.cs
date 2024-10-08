#if VISUALSCRIPTING_1_8_OR_NEWER

using System;
using Unity.VisualScripting;

namespace UnityEngine.XR.ARFoundation.VisualScripting
{
    /// <summary>
    /// A switch unit that breaks down an <see cref="ARSessionState"/> and triggers one of multiple
    /// control outputs based on the state value.
    /// </summary>
    [UnitTitle("Session State Switch")]
    [UnitCategory(Constants.k_ARFoundation_Unit_Category)]
    [UnitOrder((int)Constants.ARFoundationFeatureOrder.Session)]
    [TypeIcon(typeof(ARSessionState))]
    public sealed class SessionStateSwitchUnit : Unit
    {
        /// <summary>
        /// Flow input.
        /// </summary>
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlInput flowIn { get; private set; }

        /// <summary>
        /// <see cref="ARSessionState"/> input.
        /// </summary>
        [DoNotSerialize]
        [PortLabel("AR Session State")]
        public ValueInput sessionStateIn { get; private set; }

        /// <summary>
        /// Flow output if AR Session state is <see cref="ARSessionState.None"/>.
        /// </summary>
        [DoNotSerialize]
        public ControlOutput none { get; private set; }

        /// <summary>
        /// Flow output if AR Session state is <see cref="ARSessionState.Unsupported"/>.
        /// </summary>
        [DoNotSerialize]
        public ControlOutput unsupported { get; private set; }

        /// <summary>
        /// Flow output if AR Session state is <see cref="ARSessionState.CheckingAvailability"/>.
        /// </summary>
        [DoNotSerialize]
        public ControlOutput checkingAvailability { get; private set; }

        /// <summary>
        /// Flow output if AR Session state is <see cref="ARSessionState.NeedsInstall"/>.
        /// </summary>
        [DoNotSerialize]
        public ControlOutput needsInstall { get; private set; }

        /// <summary>
        /// Flow output if AR Session state is <see cref="ARSessionState.Installing"/>.
        /// </summary>
        [DoNotSerialize]
        public ControlOutput installing { get; private set; }

        /// <summary>
        /// Flow output if AR Session state is <see cref="ARSessionState.Ready"/>.
        /// </summary>
        [DoNotSerialize]
        public ControlOutput ready { get; private set; }

        /// <summary>
        /// Flow output if AR Session state is <see cref="ARSessionState.SessionInitializing"/>.
        /// </summary>
        [DoNotSerialize]
        public ControlOutput sessionInitializing { get; private set; }

        /// <summary>
        /// Flow output if AR Session state is <see cref="ARSessionState.SessionTracking"/>.
        /// </summary>
        [DoNotSerialize]
        public ControlOutput sessionTracking { get; private set; }

        /// <summary>
        /// Unit definition.
        /// </summary>
        protected override void Definition()
        {
            flowIn = ControlInput(nameof(flowIn), ProcessFlow);
            sessionStateIn = ValueInput<ARSessionState>(nameof(sessionStateIn));
            none = ControlOutput(nameof(none));
            unsupported = ControlOutput(nameof(unsupported));
            checkingAvailability = ControlOutput(nameof(checkingAvailability));
            needsInstall = ControlOutput(nameof(needsInstall));
            installing = ControlOutput(nameof(installing));
            ready = ControlOutput(nameof(ready));
            sessionInitializing = ControlOutput(nameof(sessionInitializing));
            sessionTracking = ControlOutput(nameof(sessionTracking));

            Requirement(sessionStateIn, flowIn);
            Succession(flowIn, none);
            Succession(flowIn, unsupported);
            Succession(flowIn, checkingAvailability);
            Succession(flowIn, needsInstall);
            Succession(flowIn, installing);
            Succession(flowIn, ready);
            Succession(flowIn, sessionInitializing);
            Succession(flowIn, sessionTracking);
        }

        ControlOutput ProcessFlow(Flow flow)
        {
            if (!(flow.GetValue(sessionStateIn) is ARSessionState sessionState))
                throw new ArgumentException($"{nameof(sessionStateIn)} must be of type {nameof(ARSessionState)}.");

            return sessionState switch
            {
                ARSessionState.None => none,
                ARSessionState.Unsupported => unsupported,
                ARSessionState.CheckingAvailability => checkingAvailability,
                ARSessionState.NeedsInstall => needsInstall,
                ARSessionState.Installing => installing,
                ARSessionState.Ready => ready,
                ARSessionState.SessionInitializing => sessionInitializing,
                ARSessionState.SessionTracking => sessionTracking,
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}

#endif // VISUALSCRIPTING_1_8_OR_NEWER
