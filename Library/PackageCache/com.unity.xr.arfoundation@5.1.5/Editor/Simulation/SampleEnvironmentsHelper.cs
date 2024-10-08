using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEditor.PackageManager.UI;
using UnityEngine;
using UnityEngine.Networking;

namespace UnityEditor.XR.Simulation
{
    static class SampleEnvironmentsHelper
    {
        static readonly string k_ContentPackageName = "com.unity.xr-content.xr-sim-environments";
        static readonly string k_ContentPackageVersion = "1.0.0";
        static readonly string k_ContentPackageFileName = $"{k_ContentPackageName}-{k_ContentPackageVersion}{k_TgzExtension}";
        static readonly string k_ContentPackageUrl =
            $"https://github.com/Unity-Technologies/{k_ContentPackageName}/releases/download/{k_ContentPackageVersion}/{k_ContentPackageFileName}";

        const string k_ContentPackagesFolder = "ContentPackages";
        const string k_TgzExtension = ".tgz";

        static UnityWebRequest s_WebRequest;
        static AddRequest s_AddRequest;
        static ListRequest s_ListRequest;

        public static bool processingInstallRequest =>
            s_WebRequest is { isDone: false } ||
            processingPackageRequest;

        public static bool processingPackageRequest =>
            s_AddRequest is { IsCompleted: false } ||
            s_ListRequest is { IsCompleted: false };

        public static event Action packageRequestStarted;
        public static event Action packageRequestCompleted;

        /// <summary>
        /// Try to find samples for the environments package. If the returned collection is empty, then that indicates the package
        /// has not been added to the project. If the package has been installed then there will be one sample in the returned collection.
        /// </summary>
        public static IEnumerable<Sample> FindPackageSamples()
        {
            return Sample.FindByPackage(k_ContentPackageName, k_ContentPackageVersion);
        }

        public static void InstallSampleEnvironments()
        {
            if (processingInstallRequest)
                return;

            // If we already have the package downloaded and added to the project then we should be able to get its sample,
            // in which case we can just import the sample.
            if (TryImportContentPackageSample(false))
                return;

            DownloadContentPackageAndImportSample();
        }

        static void DownloadContentPackageAndImportSample()
        {
            if (!Directory.Exists(k_ContentPackagesFolder))
                Directory.CreateDirectory(k_ContentPackagesFolder);

            s_WebRequest = new UnityWebRequest(k_ContentPackageUrl);
            var fileName = Path.Combine(k_ContentPackagesFolder, k_ContentPackageFileName);
            if (File.Exists(fileName))
                File.Delete(fileName);

            s_WebRequest.downloadHandler = new DownloadHandlerFile(fileName);
            s_WebRequest.SendWebRequest().completed += _ =>
            {
                if (s_WebRequest.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError($"Failed to download content package from {k_ContentPackageUrl}: {s_WebRequest.error}");
                    return;
                }

                s_AddRequest = Client.Add($"file:../{fileName}");
                EditorApplication.update += ProcessAddRequest;
                packageRequestStarted?.Invoke();
            };
        }

        static void ProcessAddRequest()
        {
            if (!s_AddRequest.IsCompleted)
                return;

            EditorApplication.update -= ProcessAddRequest;

            if (s_AddRequest.Status != StatusCode.Success)
            {
                var errorMessage = s_AddRequest.Error?.message;
                Debug.LogError($"Adding package {k_ContentPackageName} failed: {errorMessage}");
                packageRequestCompleted?.Invoke();
                return;
            }

            // We can't import the package sample until the upm cache updates, which it does after a list request.
            // So we make our own delayed list request and import after that succeeds.
            EditorApplication.delayCall += () =>
            {
                s_ListRequest = Client.List();
                EditorApplication.update += ProcessListRequest;
            };
        }

        static void ProcessListRequest()
        {
            if (!s_ListRequest.IsCompleted)
                return;

            EditorApplication.update -= ProcessListRequest;

            if (s_ListRequest.Status != StatusCode.Success)
            {
                var errorMessage = s_ListRequest.Error?.message;
                Debug.LogError($"Listing packages failed: {errorMessage}");
                packageRequestCompleted?.Invoke();
                return;
            }

            packageRequestCompleted?.Invoke();
            TryImportContentPackageSample(true);
        }

        static bool TryImportContentPackageSample(bool required)
        {
            var samples = FindPackageSamples();
            if (!samples.Any())
            {
                if (required)
                {
                    Debug.LogWarning($"Could not find sample for package {k_ContentPackageName}. " +
                                     $"Try selecting '{EnvironmentDropdown.k_ImportEnvironmentsText}' from the environment " +
                                     $"dropdown in the {XREnvironmentToolbarOverlay.toolbarDisplayName} toolbar.");
                }

                return false;
            }

            return ImportContentPackageSample(samples.First(), required);
        }

        static bool ImportContentPackageSample(Sample contentSample, bool required)
        {
            if (!contentSample.Import())
            {
                if (required)
                    Debug.LogError($"Failed to import content sample from package {k_ContentPackageName}.");

                return false;
            }

            AssetDatabase.onImportPackageItemsCompleted += OnImportPackageItemsCompleted;
            return true;
        }

        static void OnImportPackageItemsCompleted(string[] items)
        {
            AssetDatabase.onImportPackageItemsCompleted -= OnImportPackageItemsCompleted;
            SimulationEnvironmentAssetsManager.Instance.CollectEnvironments();
        }
    }
}
