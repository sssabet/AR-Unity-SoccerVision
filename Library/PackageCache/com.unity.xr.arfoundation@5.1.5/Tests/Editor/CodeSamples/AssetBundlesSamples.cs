using NUnit.Framework;

namespace UnityEditor.XR.ARSubsystems
{
    [TestFixture]
    class AssetBundlesSamples
    {
        #region export_assetbundles_function
        static void ExportAssetBundles(string directoryPath, BuildTarget buildTarget)
        {
            UnityEditor.XR.ARSubsystems.ARBuildProcessor.PreprocessBuild(buildTarget);
            BuildPipeline.BuildAssetBundles(
                directoryPath,
                BuildAssetBundleOptions.ForceRebuildAssetBundle,
                buildTarget);
        }
        #endregion
    }
}
