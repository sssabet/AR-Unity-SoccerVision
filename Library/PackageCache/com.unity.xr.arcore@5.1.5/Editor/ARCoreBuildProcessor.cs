using System;
using System.IO;
using System.Linq;
using System.Xml;
using UnityEditor.Android;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.XR.ARSubsystems;
using UnityEditor.XR.Management;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.XR.ARCore;
using Diag = System.Diagnostics;

namespace UnityEditor.XR.ARCore
{
    class ARCorePreprocessBuild : IPreprocessBuildWithReport, IPostprocessBuildWithReport
    {
        // Needs to be > 0 to make sure we remove the shader since the Input System overwrites the preloaded assets array
        public int callbackOrder => 1;

        static readonly Version k_MinimumGradleVersion = new(5, 6, 4);

        internal const string gradleLauncherPrefix = "gradle-launcher-";

        void IPreprocessBuildWithReport.OnPreprocessBuild(BuildReport report)
        {
            if (report.summary.platform != BuildTarget.Android)
            {
                // Sometimes (e.g., build failure), the shader can get "stuck" in the Preloaded Assets array.
                // Make sure that if we are not building for Android, we remove that shader.
                foreach (var backgroundShaderName in ARCoreCameraSubsystem.backgroundShaderNames)
                    BuildHelper.RemoveShaderFromProject(backgroundShaderName);

                return;
            }

            SetRuntimePluginCopyDelegate();

            var androidXrSettings = XRGeneralSettingsPerBuildTarget.XRGeneralSettingsForBuildTarget(BuildPipeline.GetBuildTargetGroup(BuildTarget.Android));
            if (androidXrSettings == null)
                return;

            foreach (var loader in androidXrSettings.Manager.activeLoaders)
            {
                if (loader is ARCoreLoader)
                {
                    EnsureGoogleARCoreIsNotPresent();
                    EnsureMinSdkVersion();
                    EnsureOnlyOpenGLES3IsUsed();
                    EnsureGradleIsUsed();
                    EnsureGradleVersionIsSupported();
                    Check64BitArch();

                    foreach (var backgroundShaderName in ARCoreCameraSubsystem.backgroundShaderNames)
                        BuildHelper.AddBackgroundShaderToProject(backgroundShaderName);

                    break;
                }
            }
        }

        void IPostprocessBuildWithReport.OnPostprocessBuild(BuildReport report)
        {
            if (report.summary.platform != BuildTarget.Android)
                return;

            foreach (var backgroundShaderName in ARCoreCameraSubsystem.backgroundShaderNames)
                BuildHelper.RemoveShaderFromProject(backgroundShaderName);

            RemoveGeneratedStreamingAssets();
        }

        static void EnsureGradleIsUsed()
        {
            if (EditorUserBuildSettings.androidBuildSystem != AndroidBuildSystem.Gradle)
                throw new BuildFailedException("Google ARCore XR Plug-in requires the Gradle build system. See File > Build Settings... > Android");
        }

        static void EnsureGradleVersionIsSupported()
        {
            var settings = ARCoreSettings.GetOrCreateSettings();
            if (settings.ignoreGradleVersion)
                return;

            if (Gradle.TryGetVersion(out var gradleVersion, out var diagnosticMessage))
            {
                if (gradleVersion < k_MinimumGradleVersion)
                {
                    var errorMessage = $"ARCore requires at least Gradle version {k_MinimumGradleVersion} ({gradleVersion} detected). Visit https://developers.google.com/ar/develop/unity/android-11-build for further details.";
                    var selection = EditorUtility.DisplayDialogComplex(
                        "Gradle update required",
                        errorMessage,
                        "Cancel build", "Continue anyway", "Continue and don't warn me again");

                    switch (selection)
                    {
                        // Cancel the build
                        case 0: throw new BuildFailedException(errorMessage);

                        // Continue as normal
                        case 1: break;

                        // Continue, and never ask again
                        case 2:
                            settings.ignoreGradleVersion = true;
                            break;
                    }
                }
            }
            else
            {
                Debug.LogWarning($"ARCore requires Gradle {k_MinimumGradleVersion} or later. The Gradle version could not be determined because \"{diagnosticMessage}\"");
            }
        }

        static void EnsureMinSdkVersion()
        {
#if UNITY_2023_2_OR_NEWER
            const int minSdkVersionInEditor = 23;
#else
            const int minSdkVersionInEditor = 22;
#endif

            var arcoreSettings = ARCoreSettings.GetOrCreateSettings();
            var minSdkVersion = arcoreSettings.requirement == ARCoreSettings.Requirement.Optional ? minSdkVersionInEditor : 24;

            if ((int)PlayerSettings.Android.minSdkVersion < minSdkVersion)
                throw new BuildFailedException($"ARCore {arcoreSettings.requirement} apps require a minimum SDK version of {minSdkVersion}. Currently set to {PlayerSettings.Android.minSdkVersion}");
        }

        static void EnsureGoogleARCoreIsNotPresent()
        {
            var googleARAssetPath = AssetDatabase.GUIDToAssetPath("afb3e05691ff94d2cbad20643e5c5879");
            if (!string.IsNullOrEmpty(googleARAssetPath))
            {
                throw new BuildFailedException("GoogleARCore detected. Google's \"ARCore SDK for Unity\" and Unity's \"Google ARCore XR Plug-in\" package cannot be used together. If you have already removed GoogleARCore, you may need to restart the Editor.");
            }
        }

        static void EnsureOnlyOpenGLES3IsUsed()
        {
            var graphicsApis = PlayerSettings.GetGraphicsAPIs(BuildTarget.Android);
            if (graphicsApis.Length > 0)
            {
                var graphicsApi = graphicsApis[0];
                if (graphicsApi != GraphicsDeviceType.OpenGLES3)
                    throw new BuildFailedException(
                        string.Format("You have enabled the {0} graphics API, which is not supported by ARCore.", graphicsApi));
            }
        }

        static void RemoveDirectoryWithMetafile(string directory)
        {
            if (Directory.Exists(directory))
            {
                Directory.Delete(directory, true);
            }

            var meta = $"{directory}.meta";
            if (File.Exists(meta))
            {
                File.Delete(meta);
            }
        }

        static void RemoveGeneratedStreamingAssets()
        {
            RemoveDirectoryWithMetafile(ARCoreImageTrackingSubsystem.k_StreamingAssetsPath);
            if (s_ShouldDeleteStreamingAssetsFolder)
            {
                RemoveDirectoryWithMetafile(Application.streamingAssetsPath);
            }
        }

        static bool s_ShouldDeleteStreamingAssetsFolder;

        static readonly string[] k_RuntimePluginNames =
        {
            "UnityARCore.aar",
            "ARPresto.aar",
            "arcore_client.aar"
        };

        internal static bool isARCoreLoaderEnabled
        {
            get
            {
                var generalSettings = XRGeneralSettingsPerBuildTarget.XRGeneralSettingsForBuildTarget(BuildPipeline.GetBuildTargetGroup(BuildTarget.Android));
                return generalSettings != null && generalSettings.Manager.activeLoaders.OfType<ARCoreLoader>().Any();
            }
        }

        static void SetRuntimePluginCopyDelegate()
        {
            foreach (var plugin in PluginImporter.GetAllImporters())
            {
                if (plugin.isNativePlugin &&
                    k_RuntimePluginNames.Any(pluginName => plugin.assetPath.Contains(pluginName)))
                {
                    plugin.SetIncludeInBuildDelegate(path => isARCoreLoaderEnabled);
                }
            }
        }

        static void Check64BitArch()
        {
            // In editor versions 2021.1 and above, a warning is already shown for IL2CPP with ARMv7 only build config.
            // So, we only need to check for Mono scripting backend.
            if (PlayerSettings.GetScriptingBackend(NamedBuildTarget.Android) == ScriptingImplementation.Mono2x)
            {
                Debug.LogWarning("Missing ARM64 architecture which is required for Android 64-bit devices. See https://developers.google.com/ar/64bit.\nSelect IL2CPP  in 'Project Settings > Player > Other Settings > Scripting Backend' and select ARM64 in 'Project Settings > Player > Other Settings > Target Architectures'.");
            }
        }
    }

    class ARCoreManifest : IPostGenerateGradleAndroidProject
    {
        const string k_AndroidUri = "http://schemas.android.com/apk/res/android";
        const string k_AndroidNameValue = "com.google.ar.core";
        const string k_AndroidManifestPath = "/src/main/AndroidManifest.xml";
        const string k_AndroidHardwareCameraAr = "android.hardware.camera.ar";
        const string k_AndroidPermissionCamera = "android.permission.CAMERA";
        const string k_AndroidDepth = "com.google.ar.core.depth";

        public int callbackOrder => 2;

        static XmlNode FindFirstChild(XmlNode node, string tag)
        {
            if (node.HasChildNodes)
            {
                for (int i = 0; i < node.ChildNodes.Count; ++i)
                {
                    var child = node.ChildNodes[i];
                    if (child.Name == tag)
                        return child;
                }
            }

            return null;
        }

        static void AppendNewAttribute(XmlDocument doc, XmlElement element, string attributeName, string attributeValue)
        {
            var attribute = doc.CreateAttribute(attributeName, k_AndroidUri);
            attribute.Value = attributeValue;
            element.Attributes.Append(attribute);
        }

        static void FindOrCreateTagWithAttribute(XmlDocument doc, XmlNode containingNode, string tagName,
            string attributeName, string attributeValue)
        {
            if (containingNode.HasChildNodes)
            {
                for (int i = 0; i < containingNode.ChildNodes.Count; ++i)
                {
                    var child = containingNode.ChildNodes[i];
                    if (child.Name == tagName)
                    {
                        var childElement = child as XmlElement;
                        if (childElement != null && childElement.HasAttributes)
                        {
                            var attribute = childElement.GetAttributeNode(attributeName, k_AndroidUri);
                            if (attribute != null && attribute.Value == attributeValue)
                                return;
                        }
                    }
                }
            }

            // Didn't find it, so create it
            var element = doc.CreateElement(tagName);
            AppendNewAttribute(doc, element, attributeName, attributeValue);
            containingNode.AppendChild(element);
        }

        static void FindOrCreateTagWithAttributes(XmlDocument doc, XmlNode containingNode, string tagName,
            string firstAttributeName, string firstAttributeValue, string secondAttributeName, string secondAttributeValue)
        {
            if (containingNode.HasChildNodes)
            {
                for (int i = 0; i < containingNode.ChildNodes.Count; ++i)
                {
                    var childNode = containingNode.ChildNodes[i];
                    if (childNode.Name == tagName)
                    {
                        var childElement = childNode as XmlElement;
                        if (childElement != null && childElement.HasAttributes)
                        {
                            var firstAttribute = childElement.GetAttributeNode(firstAttributeName, k_AndroidUri);
                            if (firstAttribute == null || firstAttribute.Value != firstAttributeValue)
                                continue;

                            var secondAttribute = childElement.GetAttributeNode(secondAttributeName, k_AndroidUri);
                            if (secondAttribute != null)
                            {
                                secondAttribute.Value = secondAttributeValue;
                                return;
                            }

                            // Create it
                            AppendNewAttribute(doc, childElement, secondAttributeName, secondAttributeValue);
                            return;
                        }
                    }
                }
            }

            // Didn't find it, so create it
            var element = doc.CreateElement(tagName);
            AppendNewAttribute(doc, element, firstAttributeName, firstAttributeValue);
            AppendNewAttribute(doc, element, secondAttributeName, secondAttributeValue);
            containingNode.AppendChild(element);
        }

        // This ensures the Android Manifest corresponds to
        // https://developers.google.com/ar/develop/java/enable-arcore
        public void OnPostGenerateGradleAndroidProject(string path)
        {
            if (!ARCorePreprocessBuild.isARCoreLoaderEnabled)
                return;

            string manifestPath = path + k_AndroidManifestPath;
            var manifestDoc = new XmlDocument();
            manifestDoc.Load(manifestPath);

            var manifestNode = FindFirstChild(manifestDoc, "manifest");
            if (manifestNode == null)
                return;

            var applicationNode = FindFirstChild(manifestNode, "application");
            if (applicationNode == null)
                return;

            FindOrCreateTagWithAttribute(manifestDoc, manifestNode, "uses-permission", "name", k_AndroidPermissionCamera);
            FindOrCreateTagWithAttributes(manifestDoc, applicationNode, "meta-data", "name", "unityplayer.SkipPermissionsDialog", "value", "true");

            var settings = ARCoreSettings.GetOrCreateSettings();
            if (settings.requirement == ARCoreSettings.Requirement.Optional)
            {
                FindOrCreateTagWithAttributes(manifestDoc, applicationNode, "meta-data", "name", k_AndroidNameValue, "value", "optional");
            }
            else if (settings.requirement == ARCoreSettings.Requirement.Required)
            {
                FindOrCreateTagWithAttributes(manifestDoc, manifestNode, "uses-feature", "name", k_AndroidHardwareCameraAr, "required", "true");
                FindOrCreateTagWithAttributes(manifestDoc, applicationNode, "meta-data", "name", k_AndroidNameValue, "value", "required");
            }

            if(settings.depth == ARCoreSettings.Requirement.Required)
            {
                FindOrCreateTagWithAttributes(manifestDoc, manifestNode, "uses-feature", "name", k_AndroidDepth, "required", "true");
            }
            manifestDoc.Save(manifestPath);
        }
    }
}
