using System;
using System.Collections.Generic;
using System.IO;
using Unity.XR.CoreUtils;
using Unity.XR.CoreUtils.Editor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.XR.Simulation;

namespace UnityEditor.XR.Simulation
{
    /// <summary>
    /// Manager that handles collection of simulation environment prefabs in the Editor.
    /// </summary>
    [ScriptableSettingsPath(SimulationConstants.userSettingsPath)]
    class SimulationEnvironmentAssetsManager : EditorScriptableSettings<SimulationEnvironmentAssetsManager>
    {
        static readonly Comparer<string> k_PrefabPathsComparer = Comparer<string>.Default;

        const string k_PrefabFilter = "t:prefab";
        const string k_RefreshMenuItemCategory = "Assets";
        const string k_RefreshMenuItemName = "Refresh XR Environment List";
        const string k_RefreshMenuItemPath = k_RefreshMenuItemCategory + "/" + k_RefreshMenuItemName;

        /// <summary>
        /// Default file name for a newly created environment asset.
        /// </summary>
        public const string newEnvironmentFileName = "Simulation Environment.prefab";

        [SerializeField]
        [HideInInspector]
        List<string> m_EnvironmentPrefabPaths = new();

        [SerializeField]
        [HideInInspector]
        bool m_FallbackAtEndOfList;

        /// <summary>
        /// Number of environments available.
        /// </summary>
        public int environmentsCount => m_EnvironmentPrefabPaths.Count;

        /// <summary>
        /// Whether there is currently an active environment.
        /// </summary>
        public bool activeEnvironmentExists => XRSimulationPreferences.Instance.activeEnvironmentPrefab != null;

        public event Action activeEnvironmentChanged;
        public event Action availableEnvironmentsChanged;

        [MenuItem(k_RefreshMenuItemPath)]
        static void RefreshEnvironmentListMenuItem()
        {
            Instance.CollectEnvironments();
        }

        /// <summary>
        /// Gathers all environment assets handled by this manager and saves them to a list of available environments.
        /// </summary>
        public void CollectEnvironments()
        {
            var simulationPreferences = XRSimulationPreferences.Instance;
            var fallbackEnvPrefab = simulationPreferences.fallbackEnvironmentPrefab;
            var fallbackEnvPath = "";
            m_EnvironmentPrefabPaths.Clear();
            var prefabGuids = AssetDatabase.FindAssets(k_PrefabFilter);
            foreach (var guid in prefabGuids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var simEnvironment = AssetDatabase.LoadAssetAtPath<SimulationEnvironment>(path);
                if (simEnvironment == null)
                    continue;

                if (simEnvironment.gameObject == fallbackEnvPrefab)
                    fallbackEnvPath = path;
                else if (!simEnvironment.excludeFromSelectionUI)
                    m_EnvironmentPrefabPaths.Add(path);
            }

            m_EnvironmentPrefabPaths.Sort(k_PrefabPathsComparer);

            // Show fallback environment at the bottom
            m_FallbackAtEndOfList = !string.IsNullOrEmpty(fallbackEnvPath);
            if (m_FallbackAtEndOfList)
                m_EnvironmentPrefabPaths.Add(fallbackEnvPath);

            EditorUtility.SetDirty(this);
            availableEnvironmentsChanged?.Invoke();

            // If no environment is available, even the fallback, we treat this as the active environment being changed so UI can update
            if (m_EnvironmentPrefabPaths.Count == 0)
            {
                activeEnvironmentChanged?.Invoke();
                return;
            }

            if (simulationPreferences.environmentPrefab == null)
            {
                // Ensure active environment is set if possible
                SelectEnvironmentAtIndex(0);
            }
        }

        /// <summary>
        /// Sets the active environment to the one at the given index in the list of available environments.
        /// </summary>
        public void SelectEnvironmentAtIndex(int index)
        {
            var envCount = environmentsCount;
            if (index < 0 || index >= envCount)
                throw new IndexOutOfRangeException($"Cannot select environment at index {index} outside the range of available environments.");

            var simulationPreferences = XRSimulationPreferences.Instance;
            if (index == envCount - 1 && m_FallbackAtEndOfList)
            {
                // Ensure fallback is used by clearing out the environment prefab field
                simulationPreferences.environmentPrefab = null;
            }
            else
            {
                var environmentPath = m_EnvironmentPrefabPaths[index];
                var selectedEnvironmentPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(environmentPath);
                if (selectedEnvironmentPrefab == null)
                {
                    Debug.LogError($"Failed to load environment prefab '{environmentPath}'. " + 
                                   $"Try refreshing environments by going to {k_RefreshMenuItemCategory} > {k_RefreshMenuItemName}.");
                    
                    return;
                }

                simulationPreferences.environmentPrefab = selectedEnvironmentPrefab;
            }

            EditorUtility.SetDirty(simulationPreferences);
            activeEnvironmentChanged?.Invoke();
        }

        /// <summary>
        /// Gets the file path of the active environment asset.
        /// </summary>
        public string GetActiveEnvironmentPath()
        {
            var activeEnvironment = XRSimulationPreferences.Instance.activeEnvironmentPrefab;
            return activeEnvironment != null ? AssetDatabase.GetAssetPath(activeEnvironment) : null;
        }

        /// <summary>
        /// Gets the name of the active environment for displaying in UI.
        /// </summary>
        public string GetActiveEnvironmentDisplayName()
        {
            var activeEnvironment = XRSimulationPreferences.Instance.activeEnvironmentPrefab;
            return activeEnvironment != null ? activeEnvironment.name : null;
        }

        /// <summary>
        /// Gets the index of the active environment in the list of available environments.
        /// </summary>
        public int GetActiveEnvironmentIndex()
        {
            var activeEnvironment = XRSimulationPreferences.Instance.activeEnvironmentPrefab;
            if (activeEnvironment == null)
                return -1;

            var environmentPath = AssetDatabase.GetAssetPath(activeEnvironment);
            return m_EnvironmentPrefabPaths.IndexOf(environmentPath);
        }

        /// <summary>
        /// Fills out the given list with names for dropdown menu items corresponding to each environment in the list of available environments.
        /// </summary>
        public void GetAllEnvironmentMenuItemNames(List<string> names)
        {
            var count = environmentsCount;
            if (count == 0)
                return;

            for (var i = 0; i < count - 1; i++)
            {
                names.Add(GetDirectoryAndFile(m_EnvironmentPrefabPaths[i]));
            }

            // Fallback environment is a special case - just show the file name
            var lastPath = m_EnvironmentPrefabPaths[count - 1];
            names.Add(m_FallbackAtEndOfList ? Path.GetFileNameWithoutExtension(lastPath) : GetDirectoryAndFile(lastPath));
        }

        static string GetDirectoryAndFile(string path)
        {
            if (string.IsNullOrEmpty(path))
                return "";

            var length = path.Length;
            var startIndex = length - 1;
            var lastIndex = length;
            var slashCount = 0;
            for (; startIndex >= 0 && slashCount <= 1; --startIndex)
            {
                var c = path[startIndex];

                if (c == '/')
                    slashCount++;
                else if (lastIndex == length && c == '.')
                    lastIndex = startIndex;
            }

            if (startIndex < 0)
            {
                startIndex = 0;
            }
            else
            {
                // +1 to ignore last for loop iteration and +1 to ignore last '/'
                startIndex += 2;
            }

            return path.Substring(startIndex, lastIndex - startIndex);
        }

        /// <summary>
        /// Tries to create a new environment asset at the given path.
        /// </summary>
        /// <param name="assetPath">File path where the new environment should be created.</param>
        /// <param name="newEnvironmentIndex">Resulting index of the new environment in the list of available environments.</param>
        /// <returns>True if creation was successful, false otherwise.</returns>
        public bool TryCreateEnvironment(string assetPath, out int newEnvironmentIndex)
        {
            var newEnvironmentPath = AssetDatabase.GenerateUniqueAssetPath(assetPath);
            GameObject newEnvironmentGameObject = null;
            var defaultEnvironment = XRSimulationPreferences.Instance.fallbackEnvironmentPrefab;
            if (defaultEnvironment != null && defaultEnvironment.GetComponent<SimulationEnvironment>() != null)
            {
                var defaultEnvironmentPath = AssetDatabase.GetAssetPath(defaultEnvironment);
                if (AssetDatabase.CopyAsset(defaultEnvironmentPath, newEnvironmentPath))
                {
                    var newEnvironment = AssetDatabase.LoadAssetAtPath<SimulationEnvironment>(newEnvironmentPath);
                    newEnvironment.excludeFromSelectionUI = false;
                    EditorUtility.SetDirty(newEnvironment);
                    newEnvironmentGameObject = newEnvironment.gameObject;
                }
                else
                    Debug.LogWarning($"Failed to copy {defaultEnvironmentPath}. Creating blank environment.");
            }

            if (newEnvironmentGameObject == null)
            {
                var newEnvironmentInstance = new GameObject();
                newEnvironmentInstance.AddComponent<SimulationEnvironment>();
                var newPrefab = PrefabUtility.SaveAsPrefabAsset(newEnvironmentInstance, newEnvironmentPath, out var creationSuccess);
                if (creationSuccess)
                    newEnvironmentGameObject = newPrefab;

                DestroyImmediate(newEnvironmentInstance);
            }

            if (newEnvironmentGameObject != null)
            {
                ProjectWindowUtil.ShowCreatedAsset(newEnvironmentGameObject);
                newEnvironmentIndex = AddEnvironment(newEnvironmentPath);
                return true;
            }

            Debug.LogError($"Failed to create simulation environment at path {newEnvironmentPath}.");
            newEnvironmentIndex = -1;
            return false;
        }

        /// <summary>
        /// Tries to create a duplicate of the active environment asset at the given path.
        /// </summary>
        /// <param name="assetPath">File path where the new environment should be created.</param>
        /// <param name="newEnvironmentIndex">Resulting index of the new environment in the list of available environments.</param>
        /// <returns>True if duplication was successful, false otherwise.</returns>
        public bool TryDuplicateActiveEnvironment(string assetPath, out int newEnvironmentIndex)
        {
            var activeEnvironment = XRSimulationPreferences.Instance.activeEnvironmentPrefab;
            if (activeEnvironment == null)
            {
                Debug.LogError("No active environment available to duplicate.");
                newEnvironmentIndex = -1;
                return false;
            }

            var activeEnvironmentPath = AssetDatabase.GetAssetPath(activeEnvironment);
            var newEnvironmentPath = AssetDatabase.GenerateUniqueAssetPath(assetPath);
            if (AssetDatabase.CopyAsset(activeEnvironmentPath, newEnvironmentPath))
            {
                var newEnvironmentGameObject = AssetDatabase.LoadAssetAtPath<GameObject>(newEnvironmentPath);
                ProjectWindowUtil.ShowCreatedAsset(newEnvironmentGameObject);
                newEnvironmentIndex = AddEnvironment(newEnvironmentPath);
                return true;
            }

            Debug.LogError($"Failed to duplicate simulation environment at path {activeEnvironmentPath}.");
            newEnvironmentIndex = -1;
            return false;
        }

        /// <summary>
        /// Adds an environment to the list of available environments.
        /// </summary>
        /// <param name="environmentAssetPath">File path of the environment asset to add.</param>
        /// <returns>The index of the new environment in the list of available environments.</returns>
        int AddEnvironment(string environmentAssetPath)
        {
            // The environment might already exist in the list, if it was caught by the asset postprocessor first
            var existingIndex = m_EnvironmentPrefabPaths.IndexOf(environmentAssetPath);
            if (existingIndex >= 0)
                return existingIndex;
            
            var envCount = environmentsCount;
            var countMinusFallback = m_FallbackAtEndOfList && envCount > 0 ? envCount - 1 : envCount;
            var environmentIndex = countMinusFallback;
            for (var i = 0; i < countMinusFallback; i++)
            {
                if (k_PrefabPathsComparer.Compare(environmentAssetPath, m_EnvironmentPrefabPaths[i]) > 0)
                    continue;

                environmentIndex = i;
                break;
            }

            m_EnvironmentPrefabPaths.Insert(environmentIndex, environmentAssetPath);

            EditorUtility.SetDirty(this);
            availableEnvironmentsChanged?.Invoke();
            return environmentIndex;
        }

        /// <summary>
        /// Is the active environment asset editable?
        /// </summary>
        public bool IsActiveEnvironmentEditable()
        {
            var activeEnvironment = XRSimulationPreferences.Instance.activeEnvironmentPrefab;
            return activeEnvironment != null && !PrefabUtility.IsPartOfImmutablePrefab(activeEnvironment);
        }

        /// <summary>
        /// Opens the active environment asset for editing.
        /// </summary>
        public void OpenActiveEnvironmentForEditing()
        {
            var activeEnvironment = XRSimulationPreferences.Instance.activeEnvironmentPrefab;
            if (activeEnvironment != null)
                PrefabStageUtility.OpenPrefab(AssetDatabase.GetAssetPath(activeEnvironment));
        }


        public static GUID GetActiveEnvironmentAssetGuid()
        {
            var activeEnvironment = XRSimulationPreferences.Instance.activeEnvironmentPrefab;

            if (activeEnvironment == null)
                return default;

            var environmentPath = AssetDatabase.GetAssetPath(activeEnvironment);

            return AssetDatabase.GUIDFromAssetPath(environmentPath);
        }

        class EnvironmentAssetPostprocessor : AssetPostprocessor
        {
            static bool s_NeedsRefresh;
            
            void OnPostprocessPrefab(GameObject g)
            {
                if (g.GetComponent<SimulationEnvironment>() != null)
                    s_NeedsRefresh = true;

                // Settings instance may not exist on first import, and we should not try to create one
                if (BaseInstance == null)
                    return;

                if (BaseInstance.m_EnvironmentPrefabPaths.Contains(AssetDatabase.GetAssetPath(g)))
                    s_NeedsRefresh = true;
            }

            static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets,
                string[] movedFromAssetPaths)
            {
                if (!s_NeedsRefresh)
                {
                    if (BaseInstance == null)
                        return;

                    var environmentPaths = BaseInstance.m_EnvironmentPrefabPaths;
                    foreach (var deletedAssetPath in deletedAssets)
                    {
                        if (environmentPaths.Contains(deletedAssetPath))
                        {
                            s_NeedsRefresh = true;
                            break;
                        }
                    }
                }

                if (s_NeedsRefresh && BaseInstance != null)
                    BaseInstance.CollectEnvironments();

                s_NeedsRefresh = false;
            }
        }
    }
}
