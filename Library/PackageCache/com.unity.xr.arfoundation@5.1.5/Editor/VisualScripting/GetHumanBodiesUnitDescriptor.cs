#if VISUALSCRIPTING_1_8_OR_NEWER

using Unity.VisualScripting;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARFoundation.VisualScripting;
using UnityEngine.XR.ARSubsystems;

namespace UnityEditor.XR.ARFoundation.VisualScripting
{
    /// <summary>
    /// Unit descriptor for the <see cref="GetHumanBodiesUnit"/>.
    /// </summary>
    /// <seealso href="https://docs.unity3d.com/Packages/com.unity.visualscripting@1.8/manual/vs-create-custom-node-add-docs.html"/>
    [Descriptor(typeof(GetHumanBodiesUnit))]
    public sealed class GetHumanBodiesUnitDescriptor : GetTrackablesUnitDescriptor<
        ARHumanBodyManager,
        XRHumanBodySubsystem,
        XRHumanBodySubsystemDescriptor,
        XRHumanBodySubsystem.Provider,
        XRHumanBody,
        ARHumanBody>
    {
        /// <inheritdoc/>
        protected override string unitDescription => $"Save all Human Bodies to the input {k_ListPortLabel}.";

        /// <inheritdoc/>
        protected override string trackablesInPortDescrption =>
            $"Where to save the Human Bodies.\n" +
            "This node clears the list, then adds the AR Human Bodies.\n" +
            "If you do not connect this port, this node allocates a new list instead.";

        /// <inheritdoc/>
        protected override string trackablesOutPortDescription =>
            $"The same {k_ListPortLabel} you connected to the Input port, now containing all Human Bodies.";

        /// <inheritdoc/>
        public GetHumanBodiesUnitDescriptor(GetHumanBodiesUnit target) : base(target)
        {
        }
    }
}

#endif // VISUALSCRIPTING_1_8_OR_NEWER
