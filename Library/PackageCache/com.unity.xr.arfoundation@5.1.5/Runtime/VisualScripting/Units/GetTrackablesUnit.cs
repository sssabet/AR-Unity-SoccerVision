#if VISUALSCRIPTING_1_8_OR_NEWER

using System;
using System.Collections.Generic;
using System.Text;
using Unity.VisualScripting;
using UnityEngine.SubsystemsImplementation;
using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.ARFoundation.VisualScripting
{
    /// <summary>
    /// Outputs all trackables managed by the input <c>ARTrackableManager</c> using its
    /// <see cref="ARTrackableManager{TSubsystem, TSubsystemDescriptor, TProvider, TSessionRelativeData, TTrackable}.trackables"/> property.
    /// </summary>
    /// <typeparam name="TManager">The AR Trackable Manager type.</typeparam>
    /// <typeparam name="TSubsystem">The tracking subsystem type.</typeparam>
    /// <typeparam name="TSubsystemDescriptor">The subsystem descriptor type.</typeparam>
    /// <typeparam name="TProvider">The subsystem provider type.</typeparam>
    /// <typeparam name="TSessionRelativeData">The session relative data type.</typeparam>
    /// <typeparam name="TTrackable">The AR Trackable type.</typeparam>
    /// <remarks>
    /// <see cref="TrackableCollection{TTrackable}"/> is not a serializable type, therefore we need a custom unit to
    /// copy the contents into a serializable List to use this data in a visual scripting context.
    /// </remarks>
    public abstract class GetTrackablesUnit<TManager, TSubsystem, TSubsystemDescriptor, TProvider, TSessionRelativeData, TTrackable>
        : Unit
        where TManager : ARTrackableManager<TSubsystem, TSubsystemDescriptor, TProvider, TSessionRelativeData, TTrackable>
        where TSubsystem : TrackingSubsystem<TSessionRelativeData, TSubsystem, TSubsystemDescriptor, TProvider>, new()
        where TSubsystemDescriptor : SubsystemDescriptorWithProvider<TSubsystem, TProvider>
        where TProvider : SubsystemProvider<TSubsystem>
        where TSessionRelativeData : struct, ITrackable
        where TTrackable : ARTrackable<TSessionRelativeData, TTrackable>
    {
        const string k_ManagerInPortLabel = "Manager";

        // Used by GetTrackablesUnitDescriptor in the Editor assembly
        internal const string k_TrackablesInPortLabel = "List";

        static bool s_IsFirstFrame = true;

        StringBuilder m_LogBuilder = new();

        /// <summary>
        /// Flow input.
        /// </summary>
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlInput flowIn { get; private set; }

        /// <summary>
        /// Flow output.
        /// </summary>
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlOutput flowOut { get; private set; }

        /// <summary>
        /// AR Trackable Manager input.
        /// </summary>
        [DoNotSerialize]
        [PortLabel(k_ManagerInPortLabel)]
        public ValueInput managerIn { get; private set; }

        [DoNotSerialize]
        [PortLabel(k_TrackablesInPortLabel)]
        public ValueInput trackablesIn { get; private set; }

        /// <summary>
        /// Outputs a <see cref="List{ARTrackable}"/>.
        /// </summary>
        [DoNotSerialize]
        [PortLabel(k_TrackablesInPortLabel)]
        public ValueOutput trackablesOut { get; private set; }

        protected override void Definition()
        {
            flowIn = ControlInput(nameof(flowIn), ProcessFlow);
            flowOut = ControlOutput(nameof(flowOut));
            managerIn = ValueInput<TManager>(nameof(managerIn));
            trackablesIn = ValueInput<List<TTrackable>>(nameof(trackablesIn));
            trackablesOut = ValueOutput<List<TTrackable>>(nameof(trackablesOut));

            Requirement(managerIn, flowIn);
            Requirement(trackablesIn, flowIn);
            Succession(flowIn, flowOut);
            Assignment(flowIn, trackablesOut);
        }

        ControlOutput ProcessFlow(Flow flow)
        {
            var manager = flow.GetValue(managerIn) as ARTrackableManager<TSubsystem, TSubsystemDescriptor, TProvider, TSessionRelativeData, TTrackable>;
            if (manager == null)
            {
#if UNITY_2023_1_OR_NEWER
                manager = Object.FindAnyObjectByType<ARTrackableManager<TSubsystem, TSubsystemDescriptor, TProvider, TSessionRelativeData, TTrackable>>();
#else
                manager = Object.FindObjectOfType<ARTrackableManager<TSubsystem, TSubsystemDescriptor, TProvider, TSessionRelativeData, TTrackable>>();
#endif
            }
            if (manager == null)
                throw new InvalidOperationException(
                    $"Attempted to execute the {graph.title} Visual Scripting graph, which requires an active and enabled {typeof(TManager)} in your scene, but none was found.");

            if (!managerIn.hasValidConnection && s_IsFirstFrame)
            {
                if (managerIn.hasAnyConnection)
                    m_LogBuilder.Append($"{k_ManagerInPortLabel} Input Port on {GetType().Name} in {graph.title} graph has an invalid connection.");
                else
                    m_LogBuilder.Append($"{k_ManagerInPortLabel} Input Port on {GetType().Name} in {graph.title} graph is disconnected.");

                m_LogBuilder.Append(" AR Foundation searched and found a valid manager in your scene to fill this dependency," +
                    " but for improved performance you should ensure that this port connection is valid.");

                Debug.LogWarning(m_LogBuilder.ToString());
                m_LogBuilder.Clear();
            }

            if (flow.GetValue(trackablesIn) is not List<TTrackable> trackables)
            {
                trackables = new List<TTrackable>();

                if (s_IsFirstFrame)
                {
                    if (trackablesIn.hasAnyConnection)
                        m_LogBuilder.Append($"{k_TrackablesInPortLabel} Input Port on {GetType().Name} in {graph.title} graph has an invalid connection.");
                    else
                        m_LogBuilder.Append($"{k_TrackablesInPortLabel} Input Port on {GetType().Name} in {graph.title} graph is disconnected.");

                    m_LogBuilder.Append(" AR Foundation allocated a new list to use as output, but this memory will require garbage collection." +
                        " For improved performance you should ensure that this port connection is valid.");
                }
            }
            s_IsFirstFrame = false;

            trackables.Clear();
            foreach (var t in manager.trackables)
            {
                trackables.Add(t);
            }

            flow.SetValue(trackablesOut, trackables);
            return flowOut;
        }
    }
}

#endif // VISUALSCRIPTING_1_8_OR_NEWER
