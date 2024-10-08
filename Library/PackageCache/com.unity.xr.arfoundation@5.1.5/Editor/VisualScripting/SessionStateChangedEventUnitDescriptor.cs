#if VISUALSCRIPTING_1_8_OR_NEWER

using Unity.VisualScripting;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARFoundation.VisualScripting;

namespace UnityEditor.XR.ARFoundation.VisualScripting
{
    /// <summary>
    /// Unit descriptor for the <see cref="SessionStateChangedEventUnit"/>.
    /// </summary>
    /// <seealso href="https://docs.unity3d.com/Packages/com.unity.visualscripting@1.8/manual/vs-create-custom-node-add-docs.html"/>
    [Descriptor(typeof(SessionStateChangedEventUnit))]
    public sealed class SessionStateChangedEventUnitDescriptor : UnitDescriptor<SessionStateChangedEventUnit>
    {
        const string k_UnitDescription = "Triggers when the AR Session state changes.";

        static readonly string k_SessionStateOutPortDescription =
            $"The new {nameof(ARSessionState)}. You can connect this to a Session State Switch node to take different" +
            " actions based on the state.";

        /// <inheritdoc/>
        public SessionStateChangedEventUnitDescriptor(SessionStateChangedEventUnit target) : base(target)
        {
        }

        /// <summary>
        /// Render a summary under the unit title in the Graph Inspector.
        /// </summary>
        /// <returns>The summary.</returns>
        protected override string DefinedSummary() => k_UnitDescription;

        /// <summary>
        /// Render summaries of each port in the Graph Inspector.
        /// </summary>
        /// <param name="port">The port.</param>
        /// <param name="portDescription">The port description to write to.</param>
        protected override void DefinedPort(IUnitPort port, UnitPortDescription portDescription)
        {
            base.DefinedPort(port, portDescription);

            if (port.key.Equals(nameof(target.sessionStateOut)))
                portDescription.summary = k_SessionStateOutPortDescription;
        }
    }
}

#endif // VISUALSCRIPTING_1_8_OR_NEWER
