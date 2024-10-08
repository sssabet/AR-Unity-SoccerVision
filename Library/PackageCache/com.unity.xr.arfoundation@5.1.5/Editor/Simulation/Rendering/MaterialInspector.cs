using System;
using UnityEngine;
#if INCLUDE_RENDER_PIPELINES_UNIVERSAL
using UnityEditor.Rendering.Universal.ShaderGUI;
#endif

namespace UnityEditor.XR.Simulation.Rendering
{
    /// <remarks>
    /// Adapted from `UnityEditor.Rendering.Universal.ShaderGUI.LitShader`
    /// </remarks>
    class MaterialInspector : BaseShaderGUI
    {
        /// <summary>
        /// Legacy Standard Blend Modes used in the "_Mode" Property
        /// </summary>
        public enum LegacyBlendMode
        {
            Opaque,
            Cutout,
            Fade,   // Old school alpha-blending mode, fresnel does not affect amount of transparency
            Transparent // Physically plausible transparency mode, implemented as alpha pre-multiply
        }

        const string k_XRayName = "X-Ray";

        /// <remarks>
        /// Keep inconsistent naming for easier comparing with `UnityEditor.Rendering.Universal.ShaderGUI.LitShader`.
        /// </remarks>
        LitGUI.LitProperties litProperties;

        /// <inheritdoc/>
        public override void FindProperties(MaterialProperty[] properties)
        {
            base.FindProperties(properties);
            litProperties = new LitGUI.LitProperties(properties);
        }

#if !INCLUDE_RENDER_PIPELINES_UNIVERSAL
        /// <summary>
        /// When URP is present, <see cref="BaseShaderGUI.MaterialChanged"/> is deprecated and replaced by
        /// <see cref="ShaderGUI.ValidateMaterial"/>.
        /// </summary>
        public override void MaterialChanged(Material material) => ValidateMaterial(material);
#endif

        /// <inheritdoc/>
        public override void ValidateMaterial(Material material)
        {
            if (material == null)
                throw new ArgumentNullException("material");

            if (m_FirstTimeApply)
                SyncWithLegacyValues(material);

            SetMaterialKeywords(material, LitGUI.SetMaterialKeywords);
            SetLegacyAndDefaultShaderKeywords(material);
        }

        /// <inheritdoc/>
        public override void DrawSurfaceOptions(Material material)
        {
            using (new EditorGUI.DisabledScope(material.shader.name.Contains(k_XRayName)))
            {
                base.DrawSurfaceOptions(material);
            }
        }

        /// <inheritdoc/>
        public override void DrawSurfaceInputs(Material material)
        {
            base.DrawSurfaceInputs(material);

            if (!material.shader.name.Contains(k_XRayName))
            {
                LitGUI.Inputs(litProperties, materialEditor, material);
                DrawEmissionProperties(material, true);
            }
            else
            {
                DrawNormalArea(materialEditor, litProperties.bumpMapProp, litProperties.bumpScaleProp);
                litProperties.metallicGlossMap.textureValue = null;

                EditorGUI.BeginChangeCheck();
                EditorGUI.showMixedValue = litProperties.smoothness.hasMixedValue;
                var metallic = EditorGUILayout.Slider(LitGUI.Styles.metallicMapText, litProperties.metallic.floatValue, 0f, 1f);
                if (EditorGUI.EndChangeCheck())
                    litProperties.metallic.floatValue = metallic;
                EditorGUI.showMixedValue = false;

                EditorGUI.BeginChangeCheck();
                EditorGUI.showMixedValue = litProperties.smoothness.hasMixedValue;
                var smoothness = EditorGUILayout.Slider(LitGUI.Styles.smoothnessText, litProperties.smoothness.floatValue, 0f, 1f);
                if (EditorGUI.EndChangeCheck())
                    litProperties.smoothness.floatValue = smoothness;
                EditorGUI.showMixedValue = false;
                litProperties.smoothnessMapChannel.floatValue = 0f;
            }

            SetLegacyAndDefaultShaderValues(material);
            DrawTileOffset(materialEditor, baseMapProp);
        }

        /// <inheritdoc/>
        public override void DrawAdvancedOptions(Material material)
        {
            using (new EditorGUI.DisabledScope(material.shader.name.Contains(k_XRayName)))
            {
                if (litProperties.reflections != null && litProperties.highlights != null)
                {
                    EditorGUI.BeginChangeCheck();
                    materialEditor.ShaderProperty(litProperties.highlights, LitGUI.Styles.highlightsText);
                    materialEditor.ShaderProperty(litProperties.reflections, LitGUI.Styles.reflectionsText);
                    if(EditorGUI.EndChangeCheck())
                    {
                        ValidateMaterial(material);
                    }
                }

                base.DrawAdvancedOptions(material);
            }
        }

        /// <inheritdoc/>
        public override void AssignNewShaderToMaterial(Material material, Shader oldShader, Shader newShader)
        {
            if (material == null)
                throw new ArgumentNullException("material");

            // _Emission property is lost after assigning Standard shader to the material
            // thus transfer it before assigning the new shader
            if (material.HasProperty("_Emission"))
            {
                material.SetColor("_EmissionColor", material.GetColor("_Emission"));
            }

            base.AssignNewShaderToMaterial(material, oldShader, newShader);

            SyncWithLegacyValues(material);

            if (oldShader == null || !oldShader.name.Contains("Legacy Shaders/"))
            {
                SetupMaterialBlendMode(material);
                return;
            }

            SurfaceType surfaceType = SurfaceType.Opaque;
            BlendMode blendMode = BlendMode.Alpha;
            if (oldShader.name.Contains("/Transparent/Cutout/"))
            {
                surfaceType = SurfaceType.Opaque;
                material.SetFloat("_AlphaClip", 1);
            }
            else if (oldShader.name.Contains("/Transparent/"))
            {
                // NOTE: legacy shaders did not provide physically based transparency
                // therefore Fade mode
                surfaceType = SurfaceType.Transparent;
                blendMode = BlendMode.Alpha;
            }

            material.SetFloat("_Surface", (float)surfaceType);
            material.SetFloat("_Blend", (float)blendMode);

            if (oldShader.name.Equals("Standard (Specular setup)"))
            {
                material.SetFloat("_WorkflowMode", (float)LitGUI.WorkflowMode.Specular);
                Texture texture = material.GetTexture("_SpecGlossMap");
                if (texture != null)
                    material.SetTexture("_MetallicSpecGlossMap", texture);
            }
            else
            {
                material.SetFloat("_WorkflowMode", (float)LitGUI.WorkflowMode.Metallic);
                Texture texture = material.GetTexture("_MetallicGlossMap");
                if (texture != null)
                    material.SetTexture("_MetallicSpecGlossMap", texture);
            }

            ValidateMaterial(material);
        }

        void SyncWithLegacyValues(Material material)
        {
            material.SetColor("_BaseColor", material.GetColor("_Color"));
            material.SetTexture("_BaseMap", material.GetTexture("_MainTex"));
            material.SetFloat("_Smoothness", material.GetFloat("_Glossiness"));

            if (!material.shader.name.Contains(k_XRayName))
            {
                var modeValue = material.GetFloat("_Mode");
                if (modeValue >= 0)
                {
                    var legacyMode = (LegacyBlendMode)material.GetFloat("_Mode");
                    switch (legacyMode)
                    {
                        case LegacyBlendMode.Opaque:
                            material.SetFloat("_Surface", (float)SurfaceType.Opaque);
                            material.SetFloat("_AlphaClip", 0);
                            break;
                        case LegacyBlendMode.Cutout:
                            material.SetFloat("_Surface", (float)SurfaceType.Opaque);
                            material.SetFloat("_AlphaClip", 1);
                            break;
                        case LegacyBlendMode.Fade:  // Old school alpha-blending mode, fresnel does not affect amount of transparency
                            material.SetFloat("_Surface", (float)SurfaceType.Transparent);
                            material.SetFloat("_Blend", (float)BlendMode.Alpha);
                            material.SetFloat("_AlphaClip", 0);
                            break;
                        case LegacyBlendMode.Transparent: // Physically plausible transparency mode, implemented as alpha pre-multiply
                            material.SetFloat("_Surface", (float)SurfaceType.Transparent);
                            material.SetFloat("_Blend", (float)BlendMode.Premultiply);
                            material.SetFloat("_AlphaClip", 0);
                            break;
                    }
                }

                // This is distructive but there is not one to one conversions for all modes
                material.SetFloat("_Mode", -1);
            }
            else
            {
                SetXRayBlendMode(material);
            }
        }

        void SetLegacyAndDefaultShaderValues(Material material)
        {
            if (material.shader.name.Contains(k_XRayName))
                SetXRayBlendMode(material);

            material.SetColor("_Color", material.GetColor("_BaseColor"));
            material.SetTexture("_MainTex", material.GetTexture("_BaseMap"));
            material.SetFloat("_Glossiness", material.GetFloat("_Smoothness"));

            material.SetFloat("_WorkflowMode", (float)LitGUI.WorkflowMode.Metallic);
        }

        static void SetLegacyAndDefaultShaderKeywords(Material material)
        {
            SurfaceType surfaceType = (SurfaceType)material.GetFloat("_Surface");
            BlendMode blendMode = (BlendMode)material.GetFloat("_Blend");
            if (surfaceType != SurfaceType.Opaque && (blendMode == BlendMode.Alpha || blendMode == BlendMode.Additive))
                material.EnableKeyword("_ALPHABLEND_ON");
            else
                material.DisableKeyword("_ALPHABLEND_ON");
        }

        void SetXRayBlendMode(Material material)
        {
            material.SetFloat("_Surface", (float)SurfaceType.Opaque);
            material.SetFloat("_Cull", (float)RenderFace.Front); // backface culling
            material.doubleSidedGI = false;
            material.SetFloat("_AlphaClip", 0); // No alpha clipping

            if (receiveShadowsProp != null)
                material.SetFloat("_ReceiveShadows", 1.0f); // true
        }
    }
}
