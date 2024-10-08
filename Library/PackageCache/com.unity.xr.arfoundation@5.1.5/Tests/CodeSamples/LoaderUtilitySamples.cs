using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.ARFoundation.Tests
{
    class LoaderUtilitySamples
    {
        class PlaneCheck
        {
            #region CheckIfPlanesLoaded
            void Start()
            {
                if (LoaderUtility
                        .GetActiveLoader()?
                        .GetLoadedSubsystem<XRPlaneSubsystem>() != null)
                {
                    // XRPlaneSubsystem was loaded. The platform supports plane detection.
                }
            }
            #endregion
        }

        class CameraCheck
        {
            #region CheckIfCameraLoaded
            void Start()
            {
                if (LoaderUtility
                        .GetActiveLoader()?
                        .GetLoadedSubsystem<XRCameraSubsystem>() != null)
                {
                    // XRCameraSubsystem was loaded. The platform supports the camera subsystem.
                }
            }
            #endregion
        }

        class OcclusionCheck
        {
            #region CheckIfOcclusionLoaded
            void Start()
            {
                if (LoaderUtility
                        .GetActiveLoader()?
                        .GetLoadedSubsystem<XROcclusionSubsystem>() != null)
                {
                    // XROcclusionSubsystem was loaded. The platform supports occlusion.
                }
            }
            #endregion
        }
        
        class FaceCheck
        {
            #region CheckIfFaceLoaded
            void Start()
            {
                if (LoaderUtility
                        .GetActiveLoader()?
                        .GetLoadedSubsystem<XRFaceSubsystem>() != null)
                {
                    // XRFaceSubsystem was loaded. The platform supports face detection.
                }
            }
            #endregion
        }
    }
}
