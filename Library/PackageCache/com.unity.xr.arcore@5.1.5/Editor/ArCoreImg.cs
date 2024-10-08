using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.XR.ARSubsystems;

namespace UnityEditor.XR.ARCore
{
    static class ArCoreImg
    {
        public abstract class ReferenceImageException : Exception
        {
            public XRReferenceImage referenceImage { get; }

            public ReferenceImageException(XRReferenceImage referenceImage) => this.referenceImage = referenceImage;
        }

        public class MissingTextureException : ReferenceImageException
        {
            public MissingTextureException(XRReferenceImage referenceImage) : base(referenceImage) { }
        }

        public class EncodeToPNGFailedException : ReferenceImageException
        {
            public EncodeToPNGFailedException(XRReferenceImage referenceImage) : base(referenceImage) { }
        }

        public class BuildDatabaseFailedException : Exception
        {
            public int exitCode { get; }

            public string stdErr { get; }

            public BuildDatabaseFailedException(int exitCode, string stdErr)
                : base($"arcoreimg failed with exit code {exitCode} and stderr:\n{stdErr}")
                => (this.exitCode, this.stdErr) = (exitCode, stdErr);
        }

        public class EmptyDatabaseException : Exception
        {
            public string stdOut { get; }
            public string stdErr { get; }

            public EmptyDatabaseException(string stdOut, string stdErr) => (this.stdOut, this.stdErr) = (stdOut, stdErr);
        }

        static string packagePath => Path.GetFullPath("Packages/com.unity.xr.arcore");

#if UNITY_EDITOR_WIN
        static string extension => ".exe";
#else
        static string extension => "";
#endif

#if UNITY_EDITOR_WIN
        static string platformName => "Windows";
#elif UNITY_EDITOR_OSX
        static string platformName => "MacOS";
#elif UNITY_EDITOR_LINUX
        static string platformName => "Linux";
#else
        static string platformName => throw new NotSupportedException();
#endif

        public static (string stdOut, string stdErr, int exitCode) BuildDb(string inputImageListPath, string outputDbPath)
        {
            var cliPath = Path.Combine(packagePath, "Tools~", platformName, $"arcoreimg{extension}");

#if UNITY_EDITOR_OSX || UNITY_EDITOR_LINUX
            Cli.Execute("/bin/chmod", $"+x \"{cliPath}\"");
#endif

            if (File.Exists(outputDbPath))
            {
                File.Delete(outputDbPath);
            }

            return Cli.Execute(cliPath, new[]
            {
                "build-db",
                $"--input_image_list_path=\"{inputImageListPath}\"",
                $"--output_db_path=\"{outputDbPath}\"",
            });
        }

        static string GetTempFileNameWithoutExtension() => Guid.NewGuid().ToString("N");

        public static byte[] BuildDb(XRReferenceImageLibrary library)
        {
            var tempDirectory = Path.Combine(Path.GetTempPath(), GetTempFileNameWithoutExtension());

            try
            {
                Directory.CreateDirectory(tempDirectory);
                var imageListPath = library.ToInputImageListPath(tempDirectory);
                var dbPath = Path.Combine(tempDirectory, $"{GetTempFileNameWithoutExtension()}.imgdb");
                var (stdOut, stdErr, exitCode) = BuildDb(imageListPath, dbPath);

                if (exitCode != 0)
                    throw new BuildDatabaseFailedException(exitCode, stdErr);

                if (!File.Exists(dbPath))
                    throw new EmptyDatabaseException(stdOut, stdErr);

                return File.ReadAllBytes(dbPath);
            }
            finally
            {
                if (Directory.Exists(tempDirectory))
                {
                    Directory.Delete(tempDirectory, true);
                }
            }
        }

        static readonly string[] k_SupportedExtensions =
        {
            ".jpg",
            ".jpeg",
            ".png"
        };

        static string CopyTo(this XRReferenceImage referenceImage, string destinationDirectory)
        {
            var assetPath = AssetDatabase.GUIDToAssetPath(referenceImage.textureGuid.ToString("N"));
            var texture = AssetDatabase.LoadAssetAtPath<Texture2D>(assetPath);
            if (texture == null)
                throw new MissingTextureException(referenceImage);

            var fileExtension = Path.GetExtension(assetPath);
            if (k_SupportedExtensions.Any(supportedExtension => fileExtension.Equals(supportedExtension, StringComparison.OrdinalIgnoreCase)))
            {
                var pathWithLowercaseExtension = Path.Combine(destinationDirectory, $"{GetTempFileNameWithoutExtension()}{fileExtension.ToLower()}");
                File.Copy(assetPath, pathWithLowercaseExtension);
                return pathWithLowercaseExtension;
            }
            else
            {
                var path = Path.Combine(destinationDirectory, $"{GetTempFileNameWithoutExtension()}.png");
                var bytes = texture.EncodeToPNG();
                if (bytes == null)
                    throw new EncodeToPNGFailedException(referenceImage);

                File.WriteAllBytes(path, bytes);
                return path;
            }
        }

        static string ToImgDBEntry(this XRReferenceImage referenceImage, string destinationDirectory)
        {
            IEnumerable<string> EntryParts(XRReferenceImage referenceImage, string destinationDirectory)
            {
                yield return referenceImage.guid.ToString("N");
                yield return referenceImage.CopyTo(destinationDirectory);
                if (referenceImage.specifySize)
                {
                    yield return referenceImage.width.ToString("G", CultureInfo.InvariantCulture);
                }
            }

            return string.Join("|", EntryParts(referenceImage, destinationDirectory));
        }

        static string ToInputImageListPath(this XRReferenceImageLibrary library, string destinationDirectory)
        {
            var entries = new List<string>();
            foreach (var referenceImage in library)
            {
                entries.Add(referenceImage.ToImgDBEntry(destinationDirectory));
            }

            var path = Path.Combine(destinationDirectory, $"{GetTempFileNameWithoutExtension()}.txt");
            File.WriteAllText(path, string.Join("\n", entries));

            return path;
        }
    }
}
