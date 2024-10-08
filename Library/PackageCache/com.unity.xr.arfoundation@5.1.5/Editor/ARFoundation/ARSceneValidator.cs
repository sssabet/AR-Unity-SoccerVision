using System;
using System.Collections.Generic;
using System.Text;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEditor.Callbacks;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARFoundation.InternalUtils;
using UnityEditor.XR.Management;

namespace UnityEditor.XR.ARFoundation
{
    class ARSceneValidator : IProcessSceneWithReport
    {
        static List<string> s_ScenesWithARTypes = new();

        static int s_SessionCount;

        static readonly Type[] k_ARTypes =
        {
            typeof(ARCameraBackground),
            typeof(ARPlaneManager),
            typeof(ARPointCloudManager),
            typeof(ARAnchorManager)
        };

        [PostProcessBuild]
        static void OnPostProcessBuild(BuildTarget target, string pathToBuiltProject)
        {
            // Note: if user's project uses a DontDestroyOnLoad technique to preserve ARSession across scenes, they
            // likely intentionally do not have ARSession components in every AR scene. For this reason the validator checks
            // s_SessionCount == 0 and not s_SessionCount == s_ScenesWithARTypes.Count.
            if (s_ScenesWithARTypes.Count > 0 && s_SessionCount == 0)
            {
                var sb = new StringBuilder();
                sb.AppendLine(
                    "The following scenes contain AR components but no ARSession. " +
                    "The ARSession component controls the AR lifecycle, so these components will not do anything at runtime. " +
                    "Was this intended?");

                foreach (var sceneName in s_ScenesWithARTypes)
                {
                    sb.AppendLine($"\t{sceneName}");
                }

                Debug.LogWarningFormat(sb.ToString());
            }

            var generalSettings = XRGeneralSettingsPerBuildTarget.XRGeneralSettingsForBuildTarget(BuildPipeline.GetBuildTargetGroup(target));
            if(generalSettings != null && generalSettings.Manager != null && generalSettings.Manager.activeLoaders != null)
            {
                 int loaderCount = generalSettings.Manager.activeLoaders.Count;
                 if(loaderCount <= 0 && s_SessionCount > 0)
                 {
                     Debug.LogWarning(
                     "There are scenes that contain an ARSession, but no XR plug-in providers have been selected for the current platform. " +
                     "To make a plug-in provider available at runtime go to Project Settings > XR Plug-in Management and enable at least one for the target platform.");
                 }
            }

            s_ScenesWithARTypes.Clear();
            s_SessionCount = 0;
        }

        int IOrderedCallback.callbackOrder => 0;

        void IProcessSceneWithReport.OnProcessScene(Scene scene, BuildReport report)
        {
            if (sceneContainsARTypes)
            {
                s_ScenesWithARTypes.Add(SceneManager.GetActiveScene().name);
            }

            s_SessionCount += FindObjectsUtility.FindObjectsByType<ARSession>().Length;
        }

        static bool sceneContainsARTypes
        {
            get
            {
                foreach (var type in k_ARTypes)
                {
                    foreach (var component in FindObjectsUtility.FindObjectsByType(type))
                    {
                        var monobehaviour = component as MonoBehaviour;
                        if (monobehaviour != null && monobehaviour.enabled)
                            return true;
                    }
                }

                return false;
            }
        }
    }
}
