using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.Simulation
{
    /// <summary>
    /// Helper class to <see cref="SimulationTrackedImageDiscoverer"/>. Defines the logic by which a simulation
    /// <c>Camera</c> discovers and tracks <see cref="SimulatedTrackedImage"/>s in the environment.
    /// </summary>
    class TrackedImageDiscoveryStrategy
    {
        /// <summary>
        /// The tracking quality threshold above which a simulated tracked image is considered to be found and tracked
        /// by the camera. See <c>ComputeOverallTrackingQuality</c>.
        /// </summary>
        const float k_QualityThresholdForTracking = 0.3f;

        /// <summary>
        /// The frustum tolerance threshold beyond which a simulated tracked image is considered to be out of the
        /// camera's view. A value of 1 means that as soon as the center of the image is out of frustum, tracking is
        /// lost. Values greater than 1 allow tracking to persist with less than half of the image in frustum, while
        /// values less than 1 require more than half of the image to be in frustum for the image to be tracked.
        /// </summary>
        const float k_CameraFrustumTolerance = 1.1f;

        /// <summary>
        /// Camera belonging to the scene's <c>XROrigin</c>.
        /// </summary>
        Camera m_Camera;

        /// <summary>
        /// Physics scene used for simulation.
        /// </summary>
        PhysicsScene m_SimPhysicsScene;

        readonly RaycastHit[] m_RayHits = new RaycastHit[16];

        public TrackedImageDiscoveryStrategy(Camera camera, PhysicsScene simPhysicsScene)
        {
            m_Camera = camera;
            m_SimPhysicsScene = simPhysicsScene;
        }

        /// <summary>
        /// Given an image, a Camera, and a Physics Scene, returns <c>TrackingState.Tracking</c> if the Camera can
        /// track the image from its current location. Otherwise returns <c>TrackingState.Limited</c>.
        /// </summary>
        public TrackingState ComputeTrackingState(SimulatedTrackedImage image)
        {
            var trackingQuality = ComputeOverallTrackingQuality(image);
            return trackingQuality <= k_QualityThresholdForTracking ? TrackingState.Limited : TrackingState.Tracking;
        }

        /// <summary>
        /// Given an image, a Camera, and a Physics Scene, returns a float between 0 and 1 representing the Camera's
        /// ability to track the image, where 1 indicates perfect tracking, and 0 indicates that the Camera can't
        /// track the image at all.
        /// </summary>
        float ComputeOverallTrackingQuality(SimulatedTrackedImage image)
        {
            var imageTransform = image.transform;
            var imagePos = imageTransform.position;
            var imageSurfaceNormal = imageTransform.up;

            var camTransform = m_Camera.transform;
            var camPos = camTransform.position;
            var camForward = camTransform.forward;

            if (!IsPointInCameraFrustum(imagePos, k_CameraFrustumTolerance))
                return 0;

            var distanceQuality = ComputeDistanceQuality(image.size, Vector3.Distance(imagePos, camPos));
            if (distanceQuality <= 0f)
                return 0;

            if (IsImageOccluded(imagePos, camPos))
                return 0;

            var lookDirQuality1 = ComputeLookDirectionQualityBySurfaceNormal(imageSurfaceNormal, camForward);
            var lookDirQuality2 = ComputeLookDirectionQualityByPosition(imagePos, camPos, camForward);
            return distanceQuality * lookDirQuality1 * lookDirQuality2;
        }

        /// <summary>
        /// Given a point in world space and a Camera, returns true if the point is within the camera's frustum
        /// (field of view).
        /// </summary>
        /// <param name="worldPoint">Point in world space to test.</param>
        /// <param name="tolerance">
        /// Allows the failure point to be defined inside or outside of the true frustum.
        /// 0-1 is inside the frustum, > 1 is outside.
        /// </param>
        /// <returns><c>true</c> if <c>worldPoint</c> is inside the frustum tolerance, <c>false</c> otherwise</returns>
        bool IsPointInCameraFrustum(Vector3 worldPoint, float tolerance = 1f)
        {
            var viewPoint = m_Camera.WorldToViewportPoint(worldPoint);
            var negativeTolerance = 1f - tolerance;
            var xInBounds = viewPoint.x > negativeTolerance && viewPoint.x <= tolerance;
            var yInBounds = viewPoint.y > negativeTolerance && viewPoint.y <= tolerance;
            var zInBounds = viewPoint.z > 0;
            return xInBounds && yInBounds && zInBounds;
        }

        /// <summary>
        /// Given a Physics Scene and positions of an image and a Camera, casts a ray from the Camera to the image and
        /// returns true if the ray was occluded by some other object in the scene.
        /// </summary>
        /// <param name="imagePosition">The position of the image.</param>
        /// <param name="camPosition">The position of the Camera.</param>
        /// <param name="ignoreClose">
        /// Whether to ignore raycast hits very close to the image, which could be false positive results caused by
        /// colliders on the image surface or the image collider itself.
        /// </param>
        /// <returns><c>true</c> if the image is occluded, <c>false</c> otherwise.</returns>
        bool IsImageOccluded(Vector3 imagePosition, Vector3 camPosition, bool ignoreClose = true)
        {
            var direction = imagePosition - camPosition;
            var hitCount  = m_SimPhysicsScene.Raycast(camPosition, direction, m_RayHits, direction.magnitude);

            if (!ignoreClose)
                return hitCount > 0;

            var ignoredCount = 0;

            // we want to ignore collisions within 0.05m
            const float ignoreHitSqrMagnitude = 0.05f * 0.05f;

            for (var i = 0; i < hitCount; i++)
            {
                if (Vector3.SqrMagnitude(imagePosition - m_RayHits[i].point) < ignoreHitSqrMagnitude)
                    ignoredCount++;
            }

            return hitCount > ignoredCount;
        }

        /// <summary>
        /// Given the surface normal of an image and the look direction of the camera, returns a value between
        /// 0 and 1. A return value of 1 indicates that the camera is looking squarely at the surface of the image,
        /// while 0 indicates that the camera is pointed perpendicular or away from the surface of the image.
        /// </summary>
        /// <param name="imageSurfaceNormal">The normalized surface normal vector of the image..</param>
        /// <param name="camForward">The normalized forward vector of the camera.</param>
        static float ComputeLookDirectionQualityBySurfaceNormal(Vector3 imageSurfaceNormal, Vector3 camForward)
        {
            const float ideal = -1f;
            // 0 = 90 degrees, view angle slightly over 90 degrees can still track in some situations
            const float max = 0.1f;
            const float rangeSize = max - ideal;

            var dot = Vector3.Dot(camForward, imageSurfaceNormal);
            if (dot > max)
                return 0f;

            var diff = dot - ideal;
            var portion = diff / rangeSize;
            return Mathf.Lerp(1f, 0.2f, portion);
        }

        /// <summary>
        /// Given the positions of an image and a camera, and the look direction of the camera, returns a
        /// value between 0.6 and 1. A return value of 1 indicates that the camera is looking exactly in the direction
        /// of the image position, while 0.6 is a floor indicating the camera is looking away from the image position.
        /// </summary>
        /// <param name="imagePosition">The position of the image.</param>
        /// <param name="camPosition">The position of the image.</param>
        /// <param name="camForward">The normalized forward vector of the camera.</param>
        static float ComputeLookDirectionQualityByPosition(Vector3 imagePosition, Vector3 camPosition, Vector3 camForward)
        {
            var camToImageDirection = Vector3.Normalize(imagePosition - camPosition);
            var dot = Vector3.Dot(Vector3.Normalize(camForward), camToImageDirection);

            const float min = 0.6f;
            const float ideal = 1f;
            const float rangeSize = ideal - min;
            var diff = ideal - dot;
            var portion = diff / rangeSize;
            return Mathf.Lerp(ideal, min, portion);
        }

        /// <summary>
        /// Given an image's size and its distance from the camera, returns a value between 0 and 1. A return value of
        /// 1 indicates that the image is close enough to be perfectly visible, while 0 indicates that the image
        /// is too far away to track.
        /// </summary>
        static float ComputeDistanceQuality(Vector2 size, float distance)
        {
            // the reference size assumes that a marker that has an average side length of 10cm will begin tracking at 2.5m.
            const float referenceSize = 0.1f;
            const float referenceDistance = 2.5f;
            var sizeVariance = (size.x + size.y) * 0.5f / referenceSize;
            var maxRange = sizeVariance * referenceDistance;

            if (distance > maxRange)
                return 0f;

            const float perfectTrackingRangePortion = 0.4f;
            var perfectRange = maxRange * perfectTrackingRangePortion;
            if (distance < perfectRange)
                return 1f;

            var portion = 1f - (distance - perfectRange) / (maxRange - perfectRange);
            return Mathf.Lerp(0.5f, 1f, portion);
        }
    }
}
