using System;
using System.IO;
using UnityEditor;
using UnityEditor.XR.Management;
using UnityEngine.XR.Management;

namespace UnityEngine.XR.TestTooling
{
    abstract class ManagementTestSetup
    {
        protected static readonly string[] s_TestGeneralSettings = { "Temp", "Test" };
        protected static readonly string[] s_TempSettingsPath = {"Temp", "Test", "Settings" };

        protected string m_TestPathToGeneralSettings;
        protected string m_TestPathToSettings;

        protected XRManagerSettings m_TestManager = null;
        protected XRGeneralSettings m_XrGeneralSettings = null;
        protected XRGeneralSettingsPerBuildTarget m_BuildTargetSettings = null;

        Object m_CurrentSettings = null;

        /// <summary>
        /// When true, AssetDatabase.AddObjectToAsset will not be called to add XRManagerSettings to XRGeneralSettings.
        /// </summary>
        protected virtual bool testManagerUpgradePath => false;

        protected virtual void SetupTest()
        {
            m_TestManager = ScriptableObject.CreateInstance<XRManagerSettings>();

            m_XrGeneralSettings = ScriptableObject.CreateInstance<XRGeneralSettings>() as XRGeneralSettings;
            m_XrGeneralSettings.Manager = m_TestManager;

            m_TestPathToSettings = GetAssetPathForComponents(s_TempSettingsPath);
            m_TestPathToSettings = Path.Combine(m_TestPathToSettings, "Test_XRGeneralSettings.asset");
            if (!string.IsNullOrEmpty(m_TestPathToSettings))
            {
                AssetDatabase.CreateAsset(m_XrGeneralSettings, m_TestPathToSettings);

                if (!testManagerUpgradePath)
                    AssetDatabase.AddObjectToAsset(m_TestManager, m_XrGeneralSettings);

                AssetDatabase.SaveAssets();
            }

            m_TestPathToGeneralSettings = GetAssetPathForComponents(s_TestGeneralSettings);
            m_TestPathToGeneralSettings = Path.Combine(m_TestPathToGeneralSettings, "Test_XRGeneralSettingsPerBuildTarget.asset");

            m_BuildTargetSettings = ScriptableObject.CreateInstance<XRGeneralSettingsPerBuildTarget>();
            m_BuildTargetSettings.SetSettingsForBuildTarget(BuildTargetGroup.Standalone, m_XrGeneralSettings);
            if (!string.IsNullOrEmpty(m_TestPathToSettings))
            {
                AssetDatabase.CreateAsset(m_BuildTargetSettings, m_TestPathToGeneralSettings);
                AssetDatabase.SaveAssets();

                EditorBuildSettings.TryGetConfigObject(XRGeneralSettings.k_SettingsKey, out m_CurrentSettings);
                EditorBuildSettings.AddConfigObject(XRGeneralSettings.k_SettingsKey, m_BuildTargetSettings, true);
            }
        }

        protected virtual void TearDownTest()
        {
            EditorBuildSettings.RemoveConfigObject(XRGeneralSettings.k_SettingsKey);

            if (!string.IsNullOrEmpty(m_TestPathToGeneralSettings))
                AssetDatabase.DeleteAsset(m_TestPathToGeneralSettings);

            if (!string.IsNullOrEmpty(m_TestPathToSettings))
                AssetDatabase.DeleteAsset(m_TestPathToSettings);

            m_XrGeneralSettings.Manager = null;
            Object.DestroyImmediate(m_XrGeneralSettings);
            m_XrGeneralSettings = null;

            Object.DestroyImmediate(m_TestManager);
            m_TestManager = null;

            Object.DestroyImmediate(m_BuildTargetSettings);
            m_BuildTargetSettings = null;

            if (m_CurrentSettings != null)
                EditorBuildSettings.AddConfigObject(XRGeneralSettings.k_SettingsKey, m_CurrentSettings, true);
            else
                EditorBuildSettings.RemoveConfigObject(XRGeneralSettings.k_SettingsKey);

            AssetDatabase.DeleteAsset(Path.Combine("Assets","Temp"));
        }

        protected static string GetAssetPathForComponents(string[] pathComponents, string root = "Assets")
        {
            if (pathComponents.Length <= 0)
                return null;

            var path = root;
            foreach (var pc in pathComponents)
            {
                var subFolder = Path.Combine(path, pc);
                var shouldCreate = true;
                foreach (var f in AssetDatabase.GetSubFolders(path))
                {
                    if (String.Compare(Path.GetFullPath(f), Path.GetFullPath(subFolder), true) == 0)
                    {
                        shouldCreate = false;
                        break;
                    }
                }

                if (shouldCreate)
                    AssetDatabase.CreateFolder(path, pc);

                path = subFolder;
            }

            return path;
        }
    }
}
