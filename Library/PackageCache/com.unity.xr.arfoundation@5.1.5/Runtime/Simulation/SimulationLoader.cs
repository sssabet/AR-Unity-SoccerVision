using System.Collections.Generic;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.Management;

namespace UnityEngine.XR.Simulation
{
    /// <summary>
    /// Manages the lifecycle of Simulation subsystems.
    /// </summary>
    public class SimulationLoader : XRLoaderHelper
    {
        static List<XRSessionSubsystemDescriptor> s_SessionSubsystemDescriptors = new List<XRSessionSubsystemDescriptor>();
        static List<XRCameraSubsystemDescriptor> s_CameraSubsystemDescriptors = new List<XRCameraSubsystemDescriptor>();
        static List<XRInputSubsystemDescriptor> s_InputSubsystemDescriptors = new List<XRInputSubsystemDescriptor>();
        static List<XRPlaneSubsystemDescriptor> s_PlaneSubsystemDescriptors = new List<XRPlaneSubsystemDescriptor>();
        static List<XRPointCloudSubsystemDescriptor> s_PointCloudSubsystemDescriptors = new List<XRPointCloudSubsystemDescriptor>();
        static List<XRImageTrackingSubsystemDescriptor> s_ImageTrackingSubsystemDescriptors = new List<XRImageTrackingSubsystemDescriptor>();
        static List<XRRaycastSubsystemDescriptor> s_RaycastSubsystemDescriptors = new List<XRRaycastSubsystemDescriptor>();
        static List<XRMeshSubsystemDescriptor> s_MeshSubsystemDescriptors  = new List<XRMeshSubsystemDescriptor>();
        static List<XREnvironmentProbeSubsystemDescriptor> s_ProbeSubsystemDescriptors = new();
        static List<XRAnchorSubsystemDescriptor> s_AnchorSubsystemDescriptors = new();

        /// <summary>
        /// Initializes the loader.
        /// </summary>
        /// <returns>`True` if the session subsystem was successfully created, otherwise `false`.</returns>
        public override bool Initialize()
        {
            CreateSubsystem<XRSessionSubsystemDescriptor, XRSessionSubsystem>(s_SessionSubsystemDescriptors, SimulationSessionSubsystem.k_SubsystemId);
            CreateSubsystem<XRCameraSubsystemDescriptor, XRCameraSubsystem>(s_CameraSubsystemDescriptors, SimulationCameraSubsystem.k_SubsystemId);
            CreateSubsystem<XRInputSubsystemDescriptor, XRInputSubsystem>(s_InputSubsystemDescriptors, "XRSimulation-Input");
            CreateSubsystem<XRPlaneSubsystemDescriptor, XRPlaneSubsystem>(s_PlaneSubsystemDescriptors, SimulationPlaneSubsystem.k_SubsystemId);
            CreateSubsystem<XRPointCloudSubsystemDescriptor, XRPointCloudSubsystem>(s_PointCloudSubsystemDescriptors, SimulationPointCloudSubsystem.k_SubsystemId);
            CreateSubsystem<XRImageTrackingSubsystemDescriptor, XRImageTrackingSubsystem>(s_ImageTrackingSubsystemDescriptors, SimulationImageTrackingSubsystem.k_SubsystemId);
            CreateSubsystem<XRRaycastSubsystemDescriptor, XRRaycastSubsystem>(s_RaycastSubsystemDescriptors, SimulationRaycastSubsystem.k_SubsystemId);
            CreateSubsystem<XRMeshSubsystemDescriptor, XRMeshSubsystem>(s_MeshSubsystemDescriptors, SimulationMeshSubsystem.k_SubsystemId);
            CreateSubsystem<XREnvironmentProbeSubsystemDescriptor, XREnvironmentProbeSubsystem>(s_ProbeSubsystemDescriptors, SimulationEnvironmentProbeSubsystem.k_SubsystemId);
            CreateSubsystem<XRAnchorSubsystemDescriptor, XRAnchorSubsystem>(s_AnchorSubsystemDescriptors, SimulationAnchorSubsystem.k_SubsystemId);

            var sessionSubsystem = GetLoadedSubsystem<XRSessionSubsystem>();
            if (sessionSubsystem == null)
                Debug.LogError("Failed to load session subsystem.");

            return sessionSubsystem != null;
        }

        /// <summary>
        /// Destroys each subsystem.
        /// </summary>
        /// <returns>Always returns `true`.</returns>
        public override bool Deinitialize()
        {
            DestroySubsystem<XRAnchorSubsystem>();
            DestroySubsystem<XREnvironmentProbeSubsystem>();
            DestroySubsystem<XRMeshSubsystem>();
            DestroySubsystem<XRRaycastSubsystem>();
            DestroySubsystem<XRImageTrackingSubsystem>();
            DestroySubsystem<XRPointCloudSubsystem>();
            DestroySubsystem<XRPlaneSubsystem>();
            DestroySubsystem<XRInputSubsystem>();
            DestroySubsystem<XRCameraSubsystem>();
            DestroySubsystem<XRSessionSubsystem>();

            return true;
        }
    }
}
