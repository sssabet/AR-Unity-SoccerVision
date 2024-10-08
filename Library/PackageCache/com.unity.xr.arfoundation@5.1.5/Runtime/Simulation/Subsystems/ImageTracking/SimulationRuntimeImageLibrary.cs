using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine.XR.ARFoundation.InternalUtils;
using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.Simulation
{
    /// <summary>
    /// Minimal implementation of <see cref="RuntimeReferenceImageLibrary"/> for simulation use.
    /// </summary>
    class SimulationRuntimeImageLibrary : MutableRuntimeReferenceImageLibrary, IDisposable
    {
        static readonly TextureFormat[] k_TextureFormats = {
            TextureFormat.Alpha8,
            TextureFormat.R8,
            TextureFormat.R16,
            TextureFormat.RFloat,
            TextureFormat.RGB24,
            TextureFormat.RGBA32,
            TextureFormat.ARGB32,
            TextureFormat.BGRA32,
        };

        readonly List<XRReferenceImage> m_Images = new();
        readonly List<(IntPtr texPtr, Texture2D texture)> m_ValidatedTextures = new();
        readonly int m_MutableStartIndex;

        /// <inheritdoc/>
        public override int count => m_Images.Count;

        /// <inheritdoc/>
        public override bool supportsValidation => true;

        /// <inheritdoc/>
        public override int supportedTextureFormatCount => k_TextureFormats.Length;

        /// <summary>Constructs a <see cref="SimulationRuntimeImageLibrary"/> from a given <see cref="XRReferenceImageLibrary"/></summary>
        /// <param name="library">The <see cref="XRReferenceImageLibrary"/> to collect images from.</param>
        public SimulationRuntimeImageLibrary(XRReferenceImageLibrary library)
        {
            if (library != null)
            {
                foreach (var image in library)
                    m_Images.Add(image);

                m_MutableStartIndex = m_Images.Count;
            }
            else
            {
                m_MutableStartIndex = 0;
            }
        }

        /// <inheritdoc/>
        protected override XRReferenceImage GetReferenceImageAt(int index) => m_Images[index];

        /// <summary>
        /// Given a texture, returns an <see cref="XRReferenceImage"/> from the library with a matching texture,
        /// or <c>null</c> if no match was found.
        /// </summary>
        /// <param name="texture">The texture whose <see cref="XRReferenceImage"/> we are seeking.</param>
        /// <returns>An <see cref="XRReferenceImage"/> with a matching texture, or <c>null</c> if not found.</returns>
        public XRReferenceImage? GetReferenceImageWithTexture(Texture2D texture)
        {
            foreach (var referenceImage in m_Images)
            {
                if (referenceImage.texture == texture)
                    return referenceImage;
            }

            return null;
        }

        /// <inheritdoc/>
        protected override AddReferenceImageJobState ScheduleAddImageWithValidationJobImpl(
            NativeSlice<byte> imageBytes, Vector2Int sizeInPixels,
            TextureFormat format, XRReferenceImage referenceImage, JobHandle inputDependencies)
        {
            var tex = referenceImage.texture;
            try
            {
                m_Images.Add(new XRReferenceImage(
                    SerializableGuidUtility.AsSerializedGuid(referenceImage.guid),
                    SerializableGuidUtility.AsSerializedGuid(referenceImage.textureGuid),
                    referenceImage.size,
                    referenceImage.name, tex));
            }
            catch (Exception)
            {
                if (tex != null)
                {
                    Object.Destroy(tex);
                }
                throw;
            }

            var texPtr = tex.GetNativeTexturePtr();
            m_ValidatedTextures.Add((texPtr, tex));

            return CreateAddJobState(texPtr, inputDependencies);
        }

        /// <inheritdoc/>
        protected override JobHandle ScheduleAddImageJobImpl(
            NativeSlice<byte> imageBytes, Vector2Int sizeInPixels, TextureFormat format, 
            XRReferenceImage referenceImage, JobHandle inputDependencies) =>
            ScheduleAddImageWithValidationJobImpl(imageBytes, sizeInPixels, format, referenceImage, inputDependencies).jobHandle;

        /// <inheritdoc/>
        protected override AddReferenceImageJobStatus GetAddReferenceImageJobStatus(AddReferenceImageJobState state)
        {
            for (var i = 0; i < m_ValidatedTextures.Count; i++)
            {
                var validated = m_ValidatedTextures[i];
                if (validated.texPtr != state.AsIntPtr())
                    continue;

                return AddReferenceImageJobStatus.Success;
            }

            return AddReferenceImageJobStatus.ErrorUnknown;
        }

        /// <inheritdoc/>
        protected override TextureFormat GetSupportedTextureFormatAtImpl(int index) => k_TextureFormats[index];

        /// <inheritdoc/>
        public void Dispose()
        {
            if (m_Images.Count == 0)
                return;

            for (var i = m_MutableStartIndex; i < m_Images.Count; i++)
            {
                Object.Destroy(m_Images[i].texture);
            }

            m_Images.Clear();
            m_ValidatedTextures.Clear();
        }
    }
}
