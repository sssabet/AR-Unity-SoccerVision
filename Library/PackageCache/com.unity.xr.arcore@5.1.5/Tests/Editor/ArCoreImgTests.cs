using System;
using System.IO;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.XR.ARSubsystems;
using UnityEditor.XR.ARSubsystems;

namespace UnityEditor.XR.ARCore.Tests
{
    [TestFixture]
    class ArCoreImgTests
    {
        const string k_TestTextureAssetName = "Unity.XR.ARCore.Editor.Tests-UnityLogo White On Black";

        static string GetTestTexturePath()
        {
            // Spaces in asset names are treated as separate searches
            var assetNameToFirstSpace = k_TestTextureAssetName.Substring(0, k_TestTextureAssetName.IndexOf(' '));

            var textureGuid = AssetDatabase.FindAssets($"{assetNameToFirstSpace} t:{nameof(Texture2D)}").FirstOrDefault();
            Assert.NotNull(textureGuid, $"Texture '{k_TestTextureAssetName}' not found.");

            var path = AssetDatabase.GUIDToAssetPath(textureGuid);
            Assert.NotNull(path, $"Could not determine path to '{k_TestTextureAssetName}'");

            return path;
        }

        static Texture2D GetTestTexture()
        {
            var texture = AssetDatabase.LoadAssetAtPath<Texture2D>(GetTestTexturePath());
            Assert.NotNull(texture);

            return texture;
        }

        [Test]
        public void ExecuteArCoreImg()
        {
            var library = ScriptableObject.CreateInstance<XRReferenceImageLibrary>();
            library.name = "Test Library";
            library.Add();
            Assert.AreEqual(1, library.count);

            var texture = GetTestTexture();

            library.SetTexture(0, texture, false);
            library.SetName(0, "Unity Logo");
            library.SetSize(0, new Vector2(0.1f, 0.1f));
            library.SetSpecifySize(0, true);

            var data = ArCoreImg.BuildDb(library);
            Assert.NotNull(data, "Image library is empty.");
            Assert.AreEqual(2174, data.Length, $"Image library contains {data.Length} bytes (expected 2174)");
        }

        [Test]
        public void ArCoreImgHandlesSpacesInPathNames()
        {
            var tempDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"), "path with spaces");
            try
            {
                Directory.CreateDirectory(tempDirectory);

                var inputListPath = Path.Combine(tempDirectory, "input list.txt");
                var imagePath = Path.GetFullPath(GetTestTexturePath());
                File.WriteAllText(inputListPath, $"Unity Logo|{imagePath}");

                var outputPath = Path.Combine(tempDirectory, "output database.imgdb");
                var (stdout, stderr, exitCode) = ArCoreImg.BuildDb(inputListPath, outputPath);

                Assert.AreEqual(0, exitCode, $"arcoreimg exited with code ({exitCode}). stderr:\n{stderr}");
                Assert.IsTrue(File.Exists(outputPath), $"Expected output file {outputPath} was not created. stdout:\n{stdout}\nstderr:\n{stderr}");
            }
            finally
            {
                if (Directory.Exists(tempDirectory))
                {
                    Directory.Delete(tempDirectory, true);
                }
            }
        }
    }
}
