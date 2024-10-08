using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
using UnityEngine.XR.Simulation;
using Object = UnityEngine.Object;

namespace UnityEditor.XR.Simulation
{
    class SimulationSettingsBuildProcessor: IPreprocessBuildWithReport, IPostprocessBuildWithReport
    {
        // The assets will be moved to this location. Unless the assets are linked in a
        // scene, they will be stripped away by the build pipeline since they are not in
        // Resources folder
        static readonly string k_TempAssetRoot = Path.Combine("Assets", "XR");
        static readonly string k_TempAssetFolder = "Temp";
        static readonly string k_TempAssetPath = Path.Combine(k_TempAssetRoot, k_TempAssetFolder);

        Dictionary<string, string> m_ExcludedAssetPaths = new();
        bool m_ShouldCleanDirectory;

        public int callbackOrder => 1;

        public void OnPreprocessBuild(BuildReport report)
        {
            // Only keep the simulation settings in build if Simulation loader is active
            // and on supported stand alone platform
            var buildPlatform = report.summary.platform;
            if (SimulationEditorUtilities.CheckIsSimulationSubsystemEnabled() && buildPlatform is
                    BuildTarget.StandaloneWindows or
                    BuildTarget.StandaloneWindows64 or
                    BuildTarget.StandaloneLinux64 or
                    BuildTarget.StandaloneOSX)
                return;

            if (!AssetDatabase.IsValidFolder(k_TempAssetPath))
            {
                AssetDatabase.CreateFolder(k_TempAssetRoot, k_TempAssetFolder);
                m_ShouldCleanDirectory = true;
            }

            ExcludeAssetFromBuild(XRSimulationPreferences.Instance);
            ExcludeAssetFromBuild(XRSimulationRuntimeSettings.Instance);

            // Move assets to a temporary location
            foreach (var assetPath in m_ExcludedAssetPaths)
                TryMoveAsset(assetPath.Key, assetPath.Value);

            AssetDatabase.Refresh();

            WaitForBuildCompletion(report);
        }

        public void OnPostprocessBuild(BuildReport report)
        {
            if (m_ExcludedAssetPaths.Count < 1)
                return;

            // Move the assets back to the original location
            foreach (var assetPath in m_ExcludedAssetPaths)
                TryMoveAsset(assetPath.Value, assetPath.Key);

            m_ExcludedAssetPaths.Clear();

            if (m_ShouldCleanDirectory)
                AssetDatabase.DeleteAsset(k_TempAssetPath);

            AssetDatabase.Refresh();
        }

        async void WaitForBuildCompletion(BuildReport report)
        {
            while (report.summary.result == BuildResult.Unknown)
                await Task.Delay(1000);

            if (report.summary.result == BuildResult.Succeeded)
                return;

            OnPostprocessBuild(report);
        }

        /// <summary>
        /// Add <paramref name="asset"/> to the list of assets which will be removed
        /// from the <c>Resources</c> folder to prevent them from bundled in the player build.
        /// </summary>
        /// <param name="asset">The asset to exclude from the player build.</param>
        void ExcludeAssetFromBuild(Object asset)
        {
            var assetPath = AssetDatabase.GetAssetPath(asset);
            if (string.IsNullOrEmpty(assetPath))
            {
                Debug.LogWarning($"Invalid asset: {assetPath}");
                return;
            }

            var newPath = Path.Combine(k_TempAssetPath, Path.GetFileName(assetPath));
            if (newPath != assetPath)
                m_ExcludedAssetPaths[assetPath] = newPath;
            else if (m_ExcludedAssetPaths.ContainsKey(assetPath))
                m_ExcludedAssetPaths.Remove(assetPath);
        }

        /// <summary>
        /// Try to move an asset from <paramref name="currentPath"/> location
        /// to <paramref name="newPath"/> location.
        /// </summary>
        /// <param name="currentPath">The path where the asset is saved currently.</param>
        /// <param name="newPath">The path where the asset should move.</param>
        /// <returns>Returns `true` if the asset is moved successfully. Returns `false` otherwise.</returns>
        static bool TryMoveAsset(string currentPath, string newPath)
        {
            if (string.IsNullOrEmpty(currentPath) || string.IsNullOrEmpty(newPath))
                return false;

            var moveResult = AssetDatabase.ValidateMoveAsset(currentPath, newPath);
            if (moveResult != "")
            {
                Debug.LogWarning($"Failed to move the asset ({currentPath}) to a temporary location: {moveResult}");
                return false;
            }

            AssetDatabase.MoveAsset(currentPath, newPath);
            return true;
        }
    }
}
