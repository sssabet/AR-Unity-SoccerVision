#if VISUALSCRIPTING_1_8_OR_NEWER

using Unity.VisualScripting;
using UnityEngine.XR.ARFoundation.VisualScripting;

namespace UnityEditor.XR.ARFoundation.VisualScripting
{
    /// <summary>
    /// Unit descriptor for the <see cref="SessionStateSwitchUnit"/>.
    /// </summary>
    /// <seealso href="https://docs.unity3d.com/Packages/com.unity.visualscripting@1.8/manual/vs-create-custom-node-add-docs.html"/>
    [Descriptor(typeof(SessionStateSwitchUnit))]
    public sealed class SessionStateSwitchUnitDescriptor : UnitDescriptor<SessionStateSwitchUnit>
    {
        const string k_UnitDescription = "Trigger one of the output flows based on the input ARSessionState.";

        /// <inheritdoc/>
        public SessionStateSwitchUnitDescriptor(SessionStateSwitchUnit target) : base(target)
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

            switch (port.key)
            {
                case nameof(target.sessionStateIn):
                    portDescription.summary =
                        "An ARSessionState object. You can get this via the On AR Session State Changed node.";
                    break;
                case nameof(target.none):
                    portDescription.summary =
                        "Triggers if the session state is None.\nAR has not been initialized and availability is unknown.";
                    break;
                case nameof(target.unsupported):
                    portDescription.summary =
                        "Triggers if the session state is Unsupported.\nThe device does not support AR.";
                    break;
                case nameof(target.checkingAvailability):
                    portDescription.summary =
                        "Triggers if the session state is CheckingAvailability.\nThe session subsystem is currently checking availability of AR on the device.";
                    break;
                case nameof(target.needsInstall):
                    portDescription.summary =
                        "Triggers if the session state is NeedsInstall.\nThe device supports AR, but requires additional software to be installed.";
                    break;
                case nameof(target.installing):
                    portDescription.summary =
                        "Triggers if the session state is Installing.\nAR software is currently installing.";
                    break;
                case nameof(target.ready):
                    portDescription.summary =
                        "Triggers if the session state is Ready.\nThe device supports AR, and any necessary software is installed.";
                    break;
                case nameof(target.sessionInitializing):
                    portDescription.summary =
                        "Triggers if the session state is SessionInitializing.\nThis usually means AR is running, but not yet tracking successfully.";
                    break;
                case nameof(target.sessionTracking):
                    portDescription.summary =
                        "Triggers if the session state is SessionTracking.\n" +
                        "The AR session is running and tracking successfully.\n" +
                        "The device is able to determine its position and orientation in the world.";
                    break;
            }
        }
    }
}

#endif // VISUALSCRIPTING_1_8_OR_NEWER
