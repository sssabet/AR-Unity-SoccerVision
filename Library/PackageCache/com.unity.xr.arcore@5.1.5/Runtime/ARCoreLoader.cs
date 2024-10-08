using System;
using System.Collections.Generic;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.Management;

namespace UnityEngine.XR.ARCore
{
    /// <summary>
    /// Manages the lifecycle of ARCore subsystems.
    /// </summary>
    public class ARCoreLoader : XRLoaderHelper
    {
        static List<XRSessionSubsystemDescriptor> s_SessionSubsystemDescriptors = new List<XRSessionSubsystemDescriptor>();
        static List<XRCameraSubsystemDescriptor> s_CameraSubsystemDescriptors = new List<XRCameraSubsystemDescriptor>();
        static List<XRPointCloudSubsystemDescriptor> s_PointCloudSubsystemDescriptors = new List<XRPointCloudSubsystemDescriptor>();
        static List<XRPlaneSubsystemDescriptor> s_PlaneSubsystemDescriptors = new List<XRPlaneSubsystemDescriptor>();
        static List<XRAnchorSubsystemDescriptor> s_AnchorSubsystemDescriptors = new List<XRAnchorSubsystemDescriptor>();
        static List<XRRaycastSubsystemDescriptor> s_RaycastSubsystemDescriptors = new List<XRRaycastSubsystemDescriptor>();
        static List<XRImageTrackingSubsystemDescriptor> s_ImageTrackingSubsystemDescriptors = new List<XRImageTrackingSubsystemDescriptor>();
        static List<XRInputSubsystemDescriptor> s_InputSubsystemDescriptors = new List<XRInputSubsystemDescriptor>();
        static List<XRFaceSubsystemDescriptor> s_FaceSubsystemDescriptors = new List<XRFaceSubsystemDescriptor>();
        static List<XREnvironmentProbeSubsystemDescriptor> s_EnvironmentProbeSubsystemDescriptors = new List<XREnvironmentProbeSubsystemDescriptor>();
        static List<XROcclusionSubsystemDescriptor> s_OcclusionSubsystemDescriptors = new List<XROcclusionSubsystemDescriptor>();

        /// <summary>
        /// The `XRSessionSubsystem` whose lifecycle is managed by this loader.
        /// </summary>
        /// <value>The session subsystem instance.</value>
        public XRSessionSubsystem sessionSubsystem => GetLoadedSubsystem<XRSessionSubsystem>();

        /// <summary>
        /// The `XRCameraSubsystem` whose lifecycle is managed by this loader.
        /// </summary>
        /// <value>The camera subsystem instance.</value>
        public XRCameraSubsystem cameraSubsystem => GetLoadedSubsystem<XRCameraSubsystem>();

        /// <summary>
        /// The [XRDepthSubsystem](xref:UnityEngine.XR.ARSubsystems.XRDepthSubsystem) whose lifecycle is managed by this loader.
        /// This field is obsolete. Use <see cref="pointCloudSubsystem"/> instead.
        /// </summary>
        /// <value>The XR point cloud subsystem instance.</value>
        [Obsolete("Use pointCloudSubsystem instead. (2022-02-03)")]
        public XRDepthSubsystem depthSubsystem => GetLoadedSubsystem<XRDepthSubsystem>();
        
        /// <summary>
        /// The `XRPointCloudSubsystem` whose lifecycle is managed by this loader.
        /// </summary>
        /// <value>The XR point cloud subsystem instance.</value>
        public XRPointCloudSubsystem pointCloudSubsystem => GetLoadedSubsystem<XRPointCloudSubsystem>();

        /// <summary>
        /// The `XRPlaneSubsystem` whose lifecycle is managed by this loader.
        /// </summary>
        /// <value>The XR plane subsystem instance.</value>
        public XRPlaneSubsystem planeSubsystem => GetLoadedSubsystem<XRPlaneSubsystem>();

        /// <summary>
        /// The `XRAnchorSubsystem` whose lifecycle is managed by this loader.
        /// </summary>
        /// <value>The XR anchor subsystem instance.</value>
        public XRAnchorSubsystem anchorSubsystem => GetLoadedSubsystem<XRAnchorSubsystem>();

        /// <summary>
        /// The `XRRaycastSubsystem` whose lifecycle is managed by this loader.
        /// </summary>
        /// <value>The XR raycast subsystem instance.</value>
        public XRRaycastSubsystem raycastSubsystem => GetLoadedSubsystem<XRRaycastSubsystem>();

        /// <summary>
        /// The `XRImageTrackingSubsystem` whose lifecycle is managed by this loader.
        /// </summary>
        /// <value>The XR image tracking subsystem instance.</value>
        public XRImageTrackingSubsystem imageTrackingSubsystem => GetLoadedSubsystem<XRImageTrackingSubsystem>();

        /// <summary>
        /// The `XRInputSubsystem` whose lifecycle is managed by this loader.
        /// </summary>
        /// <value>The XR input subsystem instance.</value>
        public XRInputSubsystem inputSubsystem => GetLoadedSubsystem<XRInputSubsystem>();

        /// <summary>
        /// The `XRFaceSubsystem` whose lifecycle is managed by this loader.
        /// </summary>
        /// <value>The XR face subsystem instance.</value>
        public XRFaceSubsystem faceSubsystem => GetLoadedSubsystem<XRFaceSubsystem>();

        /// <summary>
        /// The `XREnvironmentProbeSubsystem` whose lifecycle is managed by this loader.
        /// </summary>
        /// <value>The XR environment probe subsystem instance.</value>
        public XREnvironmentProbeSubsystem environmentProbeSubsystem => GetLoadedSubsystem<XREnvironmentProbeSubsystem>();

        /// <summary>
        /// The `XROcclusionSubsystem` whose lifecycle is managed by this loader.
        /// </summary>
        /// <value>The XR occlusion subsystem instance.</value>
        public XROcclusionSubsystem occlusionSubsystem => GetLoadedSubsystem<XROcclusionSubsystem>();

        /// <summary>
        /// Initializes the loader.
        /// </summary>
        /// <returns>`True` if the session subsystem was successfully created, otherwise `false`.</returns>
        public override bool Initialize()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            CreateSubsystem<XRSessionSubsystemDescriptor, XRSessionSubsystem>(s_SessionSubsystemDescriptors, "ARCore-Session");
            CreateSubsystem<XRCameraSubsystemDescriptor, XRCameraSubsystem>(s_CameraSubsystemDescriptors, "ARCore-Camera");
            CreateSubsystem<XRPointCloudSubsystemDescriptor, XRPointCloudSubsystem>(s_PointCloudSubsystemDescriptors, "ARCore-PointCloud");
            CreateSubsystem<XRPlaneSubsystemDescriptor, XRPlaneSubsystem>(s_PlaneSubsystemDescriptors, "ARCore-Plane");
            CreateSubsystem<XRAnchorSubsystemDescriptor, XRAnchorSubsystem>(s_AnchorSubsystemDescriptors, "ARCore-Anchor");
            CreateSubsystem<XRRaycastSubsystemDescriptor, XRRaycastSubsystem>(s_RaycastSubsystemDescriptors, "ARCore-Raycast");
            CreateSubsystem<XRImageTrackingSubsystemDescriptor, XRImageTrackingSubsystem>(s_ImageTrackingSubsystemDescriptors, "ARCore-ImageTracking");
            CreateSubsystem<XRInputSubsystemDescriptor, XRInputSubsystem>(s_InputSubsystemDescriptors, "ARCore-Input");
            CreateSubsystem<XRFaceSubsystemDescriptor, XRFaceSubsystem>(s_FaceSubsystemDescriptors, "ARCore-Face");
            CreateSubsystem<XREnvironmentProbeSubsystemDescriptor, XREnvironmentProbeSubsystem>(s_EnvironmentProbeSubsystemDescriptors, "ARCore-EnvironmentProbe");
            CreateSubsystem<XROcclusionSubsystemDescriptor, XROcclusionSubsystem>(s_OcclusionSubsystemDescriptors, "ARCore-Occlusion");

            if (sessionSubsystem == null)
            {
                Debug.LogError("Failed to load session subsystem.");
            }

            return sessionSubsystem != null;
#else
            return false;
#endif
        }

        /// <summary>
        /// Starts all subsystems.
        /// </summary>
        /// <returns>`True` if the subsystems were started, otherwise `false`.</returns>
        public override bool Start()
        {
            return true;
        }

        /// <summary>
        /// Stops all subsystems.
        /// </summary>
        /// <returns>`True` if the subsystems were stopped, otherwise `false`.</returns>
        public override bool Stop()
        {
            return true;
        }

        /// <summary>
        /// Destroys each subsystem.
        /// </summary>
        /// <returns>Always returns `true`.</returns>
        public override bool Deinitialize()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            DestroySubsystem<XRCameraSubsystem>();
            DestroySubsystem<XRPointCloudSubsystem>();
            DestroySubsystem<XRPlaneSubsystem>();
            DestroySubsystem<XRAnchorSubsystem>();
            DestroySubsystem<XRRaycastSubsystem>();
            DestroySubsystem<XRImageTrackingSubsystem>();
            DestroySubsystem<XRInputSubsystem>();
            DestroySubsystem<XRFaceSubsystem>();
            DestroySubsystem<XREnvironmentProbeSubsystem>();
            DestroySubsystem<XROcclusionSubsystem>();
            DestroySubsystem<XRSessionSubsystem>();
#endif
            return true;
        }
    }
}
