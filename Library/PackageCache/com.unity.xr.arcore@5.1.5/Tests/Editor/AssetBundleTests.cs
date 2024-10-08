using System.IO;
using System.Linq;
using NUnit.Framework;
using Unity.Collections.LowLevel.Unsafe;
using UnityEditor.XR.ARSubsystems;
using UnityEngine;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.ARCore;

namespace UnityEditor.XR.ARCore.Tests
{
    [TestFixture]
    class AssetBundleTests
    {
        const string k_BasePath = "Packages/com.unity.xr.arcore/Tests/Editor";

        const string k_AssetsPath = k_BasePath + "/Assets";

        const string k_OutputPath = k_BasePath + "/GeneratedAssetBundles";

        const string k_ReferenceImageLibraryName = "TestReferenceImageLibrary";

        const string k_ReferenceImageLibraryPath = k_AssetsPath + "/" + k_ReferenceImageLibraryName + ".asset";

        const string k_BundleName = "arcore_test_bundle";

        AssetBundle m_AssetBundle;

        [OneTimeSetUp]
        public void Setup()
        {
            Directory.CreateDirectory(k_OutputPath);
            AssetImporter.GetAtPath(k_ReferenceImageLibraryPath).assetBundleName = k_BundleName;

            // Run the preprocessor for Android
            ARBuildProcessor.PreprocessBuild(BuildTarget.Android);

            if (BuildPipeline.IsBuildTargetSupported(BuildTargetGroup.Android, BuildTarget.Android))
            {
                // Build asset bundles
                var assetBundleManifest = BuildPipeline.BuildAssetBundles(k_OutputPath, BuildAssetBundleOptions.ForceRebuildAssetBundle, BuildTarget.Android);

                // Our asset bundle should be among the generated asset bundles
                Assert.IsTrue(assetBundleManifest.GetAllAssetBundles().Contains(k_BundleName));

                // Load the asset bundle we just generated
                m_AssetBundle = AssetBundle.LoadFromFile($"{k_OutputPath}/{k_BundleName}");
            }
        }

        static void DeleteDirectory(string path)
        {
            if (Directory.Exists(path))
            {
                Directory.Delete(path, true);
            }

            var metaFile = $"{path}.meta";
            if (File.Exists(metaFile))
            {
                File.Delete(metaFile);
            }
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            DeleteDirectory(k_OutputPath);
            AssetImporter.GetAtPath(k_ReferenceImageLibraryPath).assetBundleName = string.Empty;

            // Clear internal data stores
            foreach (var library in AssetDatabase
                .FindAssets($"t:{nameof(XRReferenceImageLibrary)}")
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(AssetDatabase.LoadAssetAtPath<XRReferenceImageLibrary>))
            {
                library.ClearDataStore();
            }
        }

        [Test]
        public void ReferenceImagesAreStoredInAssetBundle()
        {
            // Requires Android player support
            if (m_AssetBundle == null)
                return;

            // The data store should now contain the ARCore reference image data
            var processedLibrary = AssetDatabase.LoadAssetAtPath<XRReferenceImageLibrary>(k_ReferenceImageLibraryPath);
            Assert.IsTrue(processedLibrary.dataStore.ContainsKey(ARCoreImageDatabase.dataStoreKey));
            var expectedBytes = processedLibrary.dataStore[ARCoreImageDatabase.dataStoreKey];
            Assert.Greater(expectedBytes?.Length, 0);

            // Load the asset bundle and extract the reference image library
            var referenceImageLibrary = m_AssetBundle.LoadAsset<XRReferenceImageLibrary>(k_ReferenceImageLibraryName);

            // Get the ARCore reference image library bytes
            Assert.IsTrue(referenceImageLibrary.dataStore.ContainsKey(ARCoreImageDatabase.dataStoreKey));
            var actualBytes = referenceImageLibrary.dataStore[ARCoreImageDatabase.dataStoreKey];

            AssertBytesAreEqual(expectedBytes, actualBytes);
        }

        static unsafe void AssertBytesAreEqual(byte[] expectedBytes, byte[] actualBytes)
        {
            // Neither may be null
            Assert.NotNull(expectedBytes);
            Assert.NotNull(actualBytes);

            // Compare the two sets of bytes
            Assert.AreEqual(expectedBytes.Length, actualBytes.Length);
            fixed (byte* expected = expectedBytes)
            fixed (byte* actual = actualBytes)
            {
                Assert.AreEqual(0, UnsafeUtility.MemCmp(expected, actual, expectedBytes.Length));
            }
        }
    }
}
