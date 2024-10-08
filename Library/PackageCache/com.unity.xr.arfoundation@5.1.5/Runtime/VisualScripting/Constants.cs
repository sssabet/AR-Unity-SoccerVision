namespace UnityEngine.XR.ARFoundation.VisualScripting
{
    static class Constants
    {
        public const string k_ARFoundation_Unit_Category = "AR Foundation";
        public const string k_Event_Unit_Category = "Events/AR Foundation";

        public enum ARFoundationFeatureOrder
        {
            Session,
            DeviceTracking,
            Camera,
            PlaneDetection,
            ImageTracking,
            ObjectTracking,
            FaceTracking,
            BodyTracking,
            PointClouds,
            Raycasts,
            Anchors,
            Meshing,
            EnvironmentProbes,
            Occlusion,
            Participants,
        }

        public static class EventHookNames
        {
            public const string planesChanged = nameof(planesChanged);
            public const string trackedImagesChanged = nameof(trackedImagesChanged);
            public const string facesChanged = nameof(facesChanged);
            public const string humanBodiesChanged = nameof(humanBodiesChanged);
            public const string trackedObjectsChanged = nameof(trackedObjectsChanged);
            public const string anchorsChanged = nameof(anchorsChanged);
            public const string pointCloudsChanged = nameof(pointCloudsChanged);
            public const string environmentProbesChanged = nameof(environmentProbesChanged);
            public const string participantsChanged = nameof(participantsChanged);
            public const string sessionStateChanged = nameof(sessionStateChanged);
            public const string faceUpdated = nameof(faceUpdated);
            public const string cameraFrameReceived = nameof(cameraFrameReceived);
        }
    }
}
