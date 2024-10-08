#if VISUALSCRIPTING_1_8_OR_NEWER

using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SubsystemsImplementation;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARFoundation.VisualScripting;
using UnityEngine.XR.ARSubsystems;

namespace UnityEditor.XR.ARFoundation.VisualScripting
{
    /// <summary>
    /// Base unit descriptor type for units derived from
    /// <see cref="GetTrackablesUnit{TManager,TSubsystem,TSubsystemDescriptor,TProvider,TSessionRelativeData,TTrackable}"/>.
    /// </summary>
    /// <typeparam name="TManager">The manager type.</typeparam>
    /// <typeparam name="TSubsystem">The subsystem type.</typeparam>
    /// <typeparam name="TSubsystemDescriptor">The subsystem descriptor type.</typeparam>
    /// <typeparam name="TProvider">The subsystem provider type.</typeparam>
    /// <typeparam name="TSessionRelativeData">The session-relative data type.</typeparam>
    /// <typeparam name="TTrackable">The trackable type.</typeparam>
    /// <seealso href="https://docs.unity3d.com/Packages/com.unity.visualscripting@1.8/manual/vs-create-custom-node-add-docs.html"/>
    public abstract class GetTrackablesUnitDescriptor<TManager, TSubsystem, TSubsystemDescriptor, TProvider, TSessionRelativeData, TTrackable>
        : UnitDescriptor<GetTrackablesUnit<TManager, TSubsystem, TSubsystemDescriptor, TProvider, TSessionRelativeData, TTrackable>>
        where TManager : ARTrackableManager<TSubsystem, TSubsystemDescriptor, TProvider, TSessionRelativeData, TTrackable>
        where TSubsystem : TrackingSubsystem<TSessionRelativeData, TSubsystem, TSubsystemDescriptor, TProvider>, new()
        where TSubsystemDescriptor : SubsystemDescriptorWithProvider<TSubsystem, TProvider>
        where TProvider : SubsystemProvider<TSubsystem>
        where TSessionRelativeData : struct, ITrackable
        where TTrackable : ARTrackable<TSessionRelativeData, TTrackable>
    {
        /// <summary>
        /// The label of the input List port.
        /// </summary>
        protected const string k_ListPortLabel =
            GetTrackablesUnit<TManager, TSubsystem, TSubsystemDescriptor, TProvider, TSessionRelativeData, TTrackable>.k_TrackablesInPortLabel;

        /// <summary>
        /// The unit description.
        /// </summary>
        protected virtual string unitDescription =>
            $"Save all {typeof(TTrackable).DisplayName()}s to the input {k_ListPortLabel}.";

        /// <summary>
        /// The manager input port description.
        /// </summary>
        protected virtual string managerInPortDescription =>
            $"An active an enabled {typeof(TManager).DisplayName()}.\n" +
            $"If you do not connect this port, this node searches for an enabled {typeof(TManager).DisplayName()} component" +
            " in the scene instead, and throws an exception if none is found.";

        /// <summary>
        /// The trackables list input port description.
        /// </summary>
        protected virtual string trackablesInPortDescrption =>
            $"Where to save the {typeof(TTrackable).DisplayName()}s.\n" +
            $"This node clears the list, then adds the {typeof(TTrackable).DisplayName()}s.\n" +
            "If you do not connect this port, this node allocates a new list instead";

        /// <summary>
        /// The trackables list output port description.
        /// </summary>
        protected virtual string trackablesOutPortDescription =>
            $"The same {k_ListPortLabel} you connected to the Input port, now containing all {typeof(TTrackable).DisplayName()}s.";

        /// <inheritdoc/>
        protected GetTrackablesUnitDescriptor(
            GetTrackablesUnit<TManager, TSubsystem, TSubsystemDescriptor, TProvider, TSessionRelativeData, TTrackable> target)
            : base(target)
        {
        }

        /// <summary>
        /// Render a summary under the unit title in the Graph Inspector.
        /// </summary>
        /// <returns>The summary.</returns>
        protected override string DefinedSummary() => unitDescription;

        /// <summary>
        /// Render summaries of each port in the Graph Inspector.
        /// </summary>
        /// <param name="port">The port.</param>
        /// <param name="portDescription">The port description to write to.</param>
        protected override void DefinedPort(IUnitPort port, UnitPortDescription portDescription)
        {
            base.DefinedPort(port, portDescription);

            switch (port.key)
            {
                case nameof(target.managerIn):
                    portDescription.summary = managerInPortDescription;
                    break;
                case nameof(target.trackablesIn):
                    portDescription.summary = trackablesInPortDescrption;
                    break;
                case nameof(target.trackablesOut):
                    portDescription.summary = trackablesOutPortDescription;
                    break;
            }
        }
    }
}

#endif // VISUALSCRIPTING_1_8_OR_NEWER
