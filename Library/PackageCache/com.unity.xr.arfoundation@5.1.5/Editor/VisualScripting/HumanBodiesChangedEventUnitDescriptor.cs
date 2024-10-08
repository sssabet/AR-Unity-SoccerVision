#if VISUALSCRIPTING_1_8_OR_NEWER

using Unity.VisualScripting;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARFoundation.VisualScripting;
using UnityEngine.XR.ARSubsystems;

namespace UnityEditor.XR.ARFoundation.VisualScripting
{
    /// <summary>
    /// Unit descriptor for the <see cref="HumanBodiesChangedEventUnit"/>.
    /// </summary>
    /// <seealso href="https://docs.unity3d.com/Packages/com.unity.visualscripting@1.8/manual/vs-create-custom-node-add-docs.html"/>
    [Descriptor(typeof(HumanBodiesChangedEventUnit))]
    public sealed class HumanBodiesChangedEventUnitDescriptor : TrackablesChangedEventUnitDescriptor<
        ARHumanBodyManager,
        XRHumanBodySubsystem,
        XRHumanBodySubsystemDescriptor,
        XRHumanBodySubsystem.Provider,
        XRHumanBody,
        ARHumanBody,
        ARHumanBodiesChangedEventArgs,
        ARHumanBodyManagerListener>
    {
        /// <inheritdoc/>
        protected override string unitDescription =>
            "Triggers when AR Human Bodies have changed.\n" +
            "AR Human Bodies can be added, updated, and/or removed every frame if there is" +
            $" an enabled {nameof(ARHumanBodyManager)} in the scene.";

        /// <inheritdoc/>
        protected override string addedPortDescription => "List of AR Human Bodies that have been added.";

        /// <inheritdoc/>
        protected override string updatedPortDescription => "List of AR Human Bodies that have been updated.";

        /// <inheritdoc/>
        protected override string removedPortDescription => "List of AR Human Bodies that have been removed.";

        /// <inheritdoc/>
        public HumanBodiesChangedEventUnitDescriptor(HumanBodiesChangedEventUnit target) : base(target)
        {
        }
    }
}

#endif // VISUALSCRIPTING_1_8_OR_NEWER
