using System.IO;
using System.Linq;
using NUnit.Framework;
using UnityEditor;
using UnityEngine.TestTools;
using UnityEngine.XR.Management;

namespace UnityEngine.XR.TestTooling
{
    abstract class LoaderTestSetup<TXRLoader> : ManagementTestSetup, IPrebuildSetup, IPostBuildCleanup
        where TXRLoader : XRLoader
    {
        protected TXRLoader m_Loader = null;
        protected string m_Path;
        bool m_IsRunning = false;

        static bool initManagerOnStart
        {
            set
            {
                if (XRGeneralSettings.Instance != null)
                    XRGeneralSettings.Instance.InitManagerOnStart = value;
            }
        }

        protected override void SetupTest()
        {
            base.SetupTest();

            Assert.IsNotNull(m_XrGeneralSettings);

            // Assets created in the Temp folder wil be deleted by
            // ManagementSetup as it deletes the entire Temp folder

            // Setup Loader
            m_Loader = ScriptableObject.CreateInstance<TXRLoader>();
            m_Path ??= GetAssetPathForComponents(s_TempSettingsPath);
            AssetDatabase.CreateAsset(m_Loader, Path.Combine(m_Path, $"Test_{typeof(TXRLoader).Name}.asset"));

            // Add loader
            UpdateLoaders(true);

            AssetDatabase.SaveAssets();
        }

        protected override void TearDownTest()
        {
            if (m_IsRunning)
                StopAndShutdown();

            // Remove loader
            UpdateLoaders(false);
            m_Loader = null;

            base.TearDownTest();
        }

        protected void InitializeAndStart()
        {
            if (m_Loader != null && m_Loader.Initialize())
                    m_IsRunning = m_Loader.Start();
        }

        protected void StopAndShutdown()
        {
            if (m_Loader != null)
            {
                m_Loader.Stop();
                m_Loader.Deinitialize();
                m_IsRunning = false;
            }
        }

        protected void RestartProvider()
        {
            StopAndShutdown();
            InitializeAndStart();
        }

        // update the list of loaders in XR Management by
        // either adding or removing the current loader
        void UpdateLoaders(bool addLoader)
        {
            var loaders = m_XrGeneralSettings.Manager.activeLoaders.ToList();

            if (addLoader)
                loaders.Add(m_Loader);
            else
                loaders.Remove(m_Loader);

            m_XrGeneralSettings.Manager.TrySetLoaders(loaders);
        }

        // IPrebuildSetup - Build time setup
        void IPrebuildSetup.Setup()
        {
            initManagerOnStart = false;
        }

        // IPostBuildCleanup - Build time cleanup
        void IPostBuildCleanup.Cleanup()
        {
            initManagerOnStart = true;
        }
    }

    abstract class LoaderTestSetup<TXRLoader, TSettings> : LoaderTestSetup<TXRLoader>
        where TXRLoader : XRLoader
        where TSettings : ScriptableObject
    {
        protected TSettings m_Settings = null;

        protected abstract string settingsKey { get; }

        protected override void SetupTest()
        {
            base.SetupTest();

            m_Path ??= GetAssetPathForComponents(s_TempSettingsPath);

            // Setup Settings
            m_Settings = ScriptableObject.CreateInstance<TSettings>();
            AssetDatabase.CreateAsset(m_Settings, Path.Combine(m_Path, $"Test_{typeof(TSettings).Name}.asset"));
            EditorBuildSettings.AddConfigObject(settingsKey, m_Settings, true);

            AssetDatabase.SaveAssets();
        }
    }
}
