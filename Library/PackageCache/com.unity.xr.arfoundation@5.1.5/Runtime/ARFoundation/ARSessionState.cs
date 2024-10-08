namespace UnityEngine.XR.ARFoundation
{
    /// <summary>
    /// Represents the current state of the AR system.
    /// </summary>
    public enum ARSessionState
    {
        /// <summary>
        /// AR has not been initialized and availability is unknown.
        /// You can call <see cref="ARSession.CheckAvailability"/> to check availability of AR on the device.
        /// </summary>
        None,

        /// <summary>
        /// AR is not supported on the device.
        /// </summary>
        Unsupported,

        /// <summary>
        /// The session subsystem is currently checking availability of AR on the device.
        /// The <see cref="ARSession.CheckAvailability"/> coroutine has not yet completed.
        /// </summary>
        CheckingAvailability,

        /// <summary>
        /// The device supports AR, but requires additional software to be installed.
        /// If the provider [supports runtime installation](xref:UnityEngine.XR.ARSubsystems.XRSessionSubsystemDescriptor.supportsInstall),
        /// you can call <see cref="ARSession.Install"/> to attempt installation of AR software on the device.
        /// </summary>
        NeedsInstall,

        /// <summary>
        /// AR software is currently installing.
        /// The <see cref="ARSession.Install"/> coroutine has not yet completed.
        /// </summary>
        Installing,

        /// <summary>
        /// The device supports AR, and any necessary software is installed.
        /// This state will automatically change to either <c>SessionInitializing</c> or <c>SessionTracking</c>.
        /// </summary>
        Ready,

        /// <summary>
        /// The AR session is currently initializing. This usually means AR is running, but not yet tracking successfully.
        /// </summary>
        SessionInitializing,

        /// <summary>
        /// The AR session is running and tracking successfully. The device is able to determine its position and orientation in the world.
        /// If tracking is lost during a session, this state may change to <c>SessionInitializing</c>.
        /// </summary>
        SessionTracking
    }
}
