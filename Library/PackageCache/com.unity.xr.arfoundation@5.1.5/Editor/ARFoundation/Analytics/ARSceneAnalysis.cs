using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.ARFoundation;

namespace UnityEditor.XR.ARFoundation
{
    class ARSceneAnalysis
    {
        static readonly Type[] k_ArManagerTypes =
        {
            typeof(ARPlaneManager),
            typeof(ARCameraManager),
            typeof(ARAnchorManager),
            typeof(AREnvironmentProbeManager),
            typeof(ARFaceManager),
            typeof(ARHumanBodyManager),
            typeof(ARInputManager),
            typeof(ARMeshManager),
            typeof(AROcclusionManager),
            typeof(ARParticipantManager),
            typeof(ARPointCloudManager),
            typeof(ARRaycastManager),
            typeof(ARTrackedImageManager),
            typeof(ARTrackedObjectManager)
        };

        // Local method use only -- created here to reduce garbage collection. Collections must be cleared before use
        static readonly List<GameObject> k_RootObjects = new();
        static readonly List<MonoBehaviour> k_ArManagers = new();

        public static ARUsageAnalyticsArgs.ARManagerInfo[] GetARManagersInfo(Scene scene)
        {
            // Static collections used below are cleared by the methods that use them
            GetComponentsWithTypes(scene, k_ArManagerTypes, k_ArManagers);

            var arManagerCount = k_ArManagers.Count;
            if (arManagerCount == 0)
                return null;

            var arManagersInfo = new ARUsageAnalyticsArgs.ARManagerInfo[arManagerCount];
            for (var i = 0; i < arManagerCount; ++i)
            {
                var arManager = k_ArManagers[i];
                arManagersInfo[i] = new ARUsageAnalyticsArgs.ARManagerInfo
                {
                    name = arManager.GetType().Name,
                    active = arManager.isActiveAndEnabled
                };
            }

            return arManagersInfo;
        }

        static void GetComponentsWithTypes(Scene scene, Type[] types, ICollection<MonoBehaviour> managers)
        {
            if (!scene.IsValid())
                return;

            // Static collections used below are cleared by the methods that use them
            scene.GetRootGameObjects(k_RootObjects);

            managers.Clear();
            if (k_RootObjects == null)
                return;

            foreach (var type in types)
            {
                foreach (var go in k_RootObjects)
                {
                    foreach (var component in go.GetComponentsInChildren(type, true))
                    {
                        if (component is MonoBehaviour monoBehaviour)
                            managers.Add(monoBehaviour);
                    }
                }
            }
        }
    }
}
