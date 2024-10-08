#if UNITY_INPUT_SYSTEM
using UnityEngine.Scripting;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Layouts;

namespace UnityEngine.XR.ARSubsystems
{
    /// <summary>
    /// A Handheld AR device layout that represents a mobile AR device, for use with the
    /// [Input System package](xref:https://docs.unity3d.com/Packages/com.unity.inputsystem@1.0/api/index.html).
    /// </summary>
    [Preserve]
    [InputControlLayout(displayName = "AR Handheld Device", isGenericTypeOfDevice = true)]
    public class HandheldARInputDevice : UnityEngine.InputSystem.InputDevice
    {
        /// <summary>
        /// The position in 3D space of the device.
        /// </summary>
        [Preserve]
        [InputControl]
        public Vector3Control devicePosition { get; private set; }
        /// <summary>
        /// The rotation in 3D space of the device.
        /// </summary>
        [Preserve]
        [InputControl]
        public QuaternionControl deviceRotation { get; private set; }

        /// <summary>
        /// Finishes setting up the device by storing the child controls.
        /// </summary>
        protected override void FinishSetup()
        {
            base.FinishSetup();

            devicePosition = GetChildControl<Vector3Control>("devicePosition");
            deviceRotation = GetChildControl<QuaternionControl>("deviceRotation");
        }
    }
}
#endif
