#if VISUALSCRIPTING_1_8_OR_NEWER

using Unity.VisualScripting;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARFoundation.VisualScripting;
using UnityEngine.XR.ARSubsystems;

namespace UnityEditor.XR.ARFoundation.VisualScripting
{
    /// <summary>
    /// Unit descriptor for the <see cref="GetParticipantsUnit"/>.
    /// </summary>
    /// <seealso href="https://docs.unity3d.com/Packages/com.unity.visualscripting@1.8/manual/vs-create-custom-node-add-docs.html"/>
    [Descriptor(typeof(GetParticipantsUnit))]
    public sealed class GetParticipantsUnitDescriptor : GetTrackablesUnitDescriptor<
        ARParticipantManager,
        XRParticipantSubsystem,
        XRParticipantSubsystemDescriptor,
        XRParticipantSubsystem.Provider,
        XRParticipant,
        ARParticipant>
    {
        /// <inheritdoc/>
        public GetParticipantsUnitDescriptor(GetParticipantsUnit target) : base(target)
        {
        }
    }
}

#endif // VISUALSCRIPTING_1_8_OR_NEWER
