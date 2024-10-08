using System;
using System.Linq;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine.XR.ARCore;
using UnityEditor.XR.ARSubsystems;
using UnityEngine.XR.ARSubsystems;

namespace UnityEditor.XR.ARCore
{
    class ARCoreImageLibraryBuildProcessor : IPreprocessBuildWithReport, ARBuildProcessor.IPreprocessBuild
    {
        public int callbackOrder => 2;

        static void Rethrow(XRReferenceImageLibrary library, string message, Exception innerException) =>
            throw new Exception($"\n-\n-\n-\nError building {nameof(XRReferenceImageLibrary)} {AssetDatabase.GetAssetPath(library)}: {message}\n-\n-\n-", innerException);

        static void BuildAssets()
        {
            var assets = AssetDatabase.FindAssets($"t:{nameof(XRReferenceImageLibrary)}");
            var libraries = assets
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(AssetDatabase.LoadAssetAtPath<XRReferenceImageLibrary>);

            var index = 0;

            foreach (var library in libraries)
            {
                index++;
                EditorUtility.DisplayProgressBar(
                    $"Compiling {nameof(XRReferenceImageLibrary)} ({index} of {assets.Length})",
                    $"{AssetDatabase.GetAssetPath(library)} ({library.count} image{(library.count == 1 ? "" : "s")})",
                    (float)index / assets.Length);

                try
                {
                    library.SetDataForKey(ARCoreImageDatabase.dataStoreKey, ArCoreImg.BuildDb(library));
                }
                catch (ArCoreImg.MissingTextureException e)
                {
                    Rethrow(library, $"{nameof(XRReferenceImage)} named '{e.referenceImage.name}' is missing a texture.", e);
                }
                catch (ArCoreImg.EncodeToPNGFailedException e)
                {
                    Rethrow(library, $"{nameof(XRReferenceImage)} named '{e.referenceImage.name}' could not be encoded to a PNG. Please check other errors in the console window.", e);
                }
                catch (ArCoreImg.BuildDatabaseFailedException e)
                {
                    Rethrow(library, $"The arcoreimg command line tool exited with code ({e.exitCode}) and stderr:\n{e.stdErr}", e);
                }
                catch (ArCoreImg.EmptyDatabaseException e)
                {
                    Rethrow(library, $"The arcoreimg command line tool ran successfully but did not produce an image database. This is likely a bug. Please provide these details to Unity:\n=== begin arcoreimg output ===\nstdout:\n{e.stdOut}\nstderr:\n{e.stdErr}\n=== end arcoreimg output===\n", e);
                }
            }
            EditorUtility.ClearProgressBar();
        }

        void ARBuildProcessor.IPreprocessBuild.OnPreprocessBuild(PreprocessBuildEventArgs buildEventArgs)
        {
            if (buildEventArgs.activeLoadersForBuildTarget?.OfType<ARCoreLoader>().Any() == false)
                return;

            BuildAssets();
        }

        void IPreprocessBuildWithReport.OnPreprocessBuild(BuildReport report)
        {
            if (report.summary.platform != BuildTarget.Android)
                return;

            if (!ARCorePreprocessBuild.isARCoreLoaderEnabled)
                return;

            BuildAssets();
        }
    }
}
