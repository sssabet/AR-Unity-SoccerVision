#if VISUALSCRIPTING_1_8_OR_NEWER

using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine.SubsystemsImplementation;
using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.ARFoundation.VisualScripting
{
    /// <summary>
    /// Uses a <typeparamref name="TListener"/> to listen for a trackables-changed event on the visual scripting event bus
    /// and assign the <typeparamref name="TEventArgs"/> to a newly triggered Flow.
    /// </summary>
    /// <typeparam name="TManager">The manager type.</typeparam>
    /// <typeparam name="TSubsystem">The subsystem type.</typeparam>
    /// <typeparam name="TSubsystemDescriptor">The subsystem descriptor type.</typeparam>
    /// <typeparam name="TProvider">The subsystem provider type.</typeparam>
    /// <typeparam name="TSessionRelativeData">The session-relative data type.</typeparam>
    /// <typeparam name="TTrackable">The type of trackable for which we are listening.</typeparam>
    /// <typeparam name="TEventArgs">The trackables-changed event arguments type.</typeparam>
    /// <typeparam name="TListener">The <see cref="MessageListener"/> type.</typeparam>
    public abstract class TrackablesChangedEventUnit<TManager, TSubsystem, TSubsystemDescriptor, TProvider, TSessionRelativeData, TTrackable, TEventArgs, TListener>
        : GameObjectEventUnit<TEventArgs>
        where TManager : ARTrackableManager<TSubsystem, TSubsystemDescriptor, TProvider, TSessionRelativeData, TTrackable>
        where TSubsystem : TrackingSubsystem<TSessionRelativeData, TSubsystem, TSubsystemDescriptor, TProvider>, new()
        where TSubsystemDescriptor : SubsystemDescriptorWithProvider<TSubsystem, TProvider>
        where TProvider : SubsystemProvider<TSubsystem>
        where TSessionRelativeData : struct, ITrackable
        where TTrackable : ARTrackable<TSessionRelativeData, TTrackable>
        where TListener : MessageListener
    {
        /// <summary>
        /// Trackables added since the last frame.
        /// </summary>
        [DoNotSerialize]
        public ValueOutput added { get; private set; }

        /// <summary>
        /// Trackables updated since the last frame.
        /// </summary>
        [DoNotSerialize]
        public ValueOutput updated { get; private set; }

        /// <summary>
        /// Trackables removed since the last frame.
        /// </summary>
        [DoNotSerialize]
        public ValueOutput removed { get; private set; }

        /// <summary>
        /// The <see cref="MessageListener"/> type.
        /// </summary>
        public override Type MessageListenerType => typeof(TListener);

        /// <summary>
        /// Unit definition.
        /// </summary>
        protected override void Definition()
        {
            base.Definition();

            added = ValueOutput<List<TTrackable>>(nameof(added));
            updated = ValueOutput<List<TTrackable>>(nameof(updated));
            removed = ValueOutput<List<TTrackable>>(nameof(removed));
        }
    }
}

#endif // VISUALSCRIPTING_1_8_OR_NEWER
