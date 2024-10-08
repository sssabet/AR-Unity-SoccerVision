#if VISUALSCRIPTING_1_8_OR_NEWER

using Unity.VisualScripting;
using UnityEngine.SubsystemsImplementation;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARFoundation.VisualScripting;
using UnityEngine.XR.ARSubsystems;

namespace UnityEditor.XR.ARFoundation.VisualScripting
{
    /// <summary>
    /// Base unit descriptor for units derived from
    /// <see cref="TrackablesChangedEventUnit{TManager, TSubsystem, TSubsystemDescriptor, TProvider, TSessionRelativeData, TTrackable, TEventArgs, TListener}"/>.
    /// </summary>
    /// <typeparam name="TManager">The manager type.</typeparam>
    /// <typeparam name="TSubsystem">The subsystem type.</typeparam>
    /// <typeparam name="TSubsystemDescriptor">The subsystem descriptor type.</typeparam>
    /// <typeparam name="TProvider">The subsystem provider type.</typeparam>
    /// <typeparam name="TSessionRelativeData">The session-relative data type.</typeparam>
    /// <typeparam name="TTrackable">The type of trackable for which we are listening.</typeparam>
    /// <typeparam name="TEventArgs">The trackables changed event arguments type.</typeparam>
    /// <typeparam name="TListener">The <see cref="MessageListener"/> type.</typeparam>
    /// <seealso href="https://docs.unity3d.com/Packages/com.unity.visualscripting@1.8/manual/vs-create-custom-node-add-docs.html"/>
    public abstract class TrackablesChangedEventUnitDescriptor<TManager, TSubsystem, TSubsystemDescriptor, TProvider, TSessionRelativeData, TTrackable, TEventArgs, TListener>
        : UnitDescriptor<TrackablesChangedEventUnit<TManager, TSubsystem, TSubsystemDescriptor, TProvider, TSessionRelativeData, TTrackable, TEventArgs, TListener>>
        where TManager : ARTrackableManager<TSubsystem, TSubsystemDescriptor, TProvider, TSessionRelativeData, TTrackable>
        where TSubsystem : TrackingSubsystem<TSessionRelativeData, TSubsystem, TSubsystemDescriptor, TProvider>, new()
        where TSubsystemDescriptor : SubsystemDescriptorWithProvider<TSubsystem, TProvider>
        where TProvider : SubsystemProvider<TSubsystem>
        where TSessionRelativeData : struct, ITrackable
        where TTrackable : ARTrackable<TSessionRelativeData, TTrackable>
        where TListener : MessageListener
    {
        /// <summary>
        /// The unit description.
        /// </summary>
        protected virtual string unitDescription =>
            $"Triggers when {typeof(TTrackable).DisplayName()}s have changed.\n" +
            $"{typeof(TTrackable).DisplayName()}s can be added, updated, and/or removed every frame if there is" +
            $" an enabled {typeof(TManager).DisplayName()} in the scene.";

        /// <summary>
        /// The Target port description.
        /// </summary>
        protected virtual string targetPortDescription =>
            $"Target GameObject should have an enabled {typeof(TManager).DisplayName()} component.\n" +
            $" If you do not connect this port, this node searches for an enabled {typeof(TManager).DisplayName()} instead," +
            " and throws an exception if none is found.";

        /// <summary>
        /// The Added port description.
        /// </summary>
        protected virtual string addedPortDescription =>
            $"List of {typeof(TTrackable).DisplayName()}s that have been added.";

        /// <summary>
        /// The Updated port description.
        /// </summary>
        protected virtual string updatedPortDescription =>
            $"List of {typeof(TTrackable).DisplayName()}s that have been updated.";

        /// <summary>
        /// The Removed port description.
        /// </summary>
        protected virtual string removedPortDescription =>
            $"List of {typeof(TTrackable).DisplayName()}s that have been removed.";

        /// <inheritdoc/>
        protected TrackablesChangedEventUnitDescriptor(
            TrackablesChangedEventUnit<
                TManager,
                TSubsystem,
                TSubsystemDescriptor,
                TProvider,
                TSessionRelativeData,
                TTrackable,
                TEventArgs,
                TListener> target)
            : base(target) { }

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
                case nameof(target.target):
                    portDescription.summary = targetPortDescription;
                    break;
                case nameof(target.added):
                    portDescription.summary = addedPortDescription;
                    break;
                case nameof(target.updated):
                    portDescription.summary = updatedPortDescription;
                    break;
                case nameof(target.removed):
                    portDescription.summary = removedPortDescription;
                    break;
            }
        }
    }
}

#endif // VISUALSCRIPTING_1_8_OR_NEWER
