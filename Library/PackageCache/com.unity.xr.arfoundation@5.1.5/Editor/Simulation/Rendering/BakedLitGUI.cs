#if !INCLUDE_RENDER_PIPELINES_UNIVERSAL
using System;

namespace UnityEditor.XR.Simulation.Rendering
{
    /// <summary>
    /// Direct copy of UnityEditor.Rendering.Universal.ShaderGUI.BakedLitGUI for use in custom material inspector when
    /// using built-in renderer.
    /// </summary>
    static class BakedLitGUI
    {
        public struct BakedLitProperties
        {
            // Surface Input Props
            public MaterialProperty bumpMapProp;

            public BakedLitProperties(MaterialProperty[] properties)
            {
                // Surface Input Props
                bumpMapProp = BaseShaderGUI.FindProperty("_BumpMap", properties, false);
            }
        }

        public static void Inputs(BakedLitProperties properties, MaterialEditor materialEditor)
        {
            BaseShaderGUI.DrawNormalArea(materialEditor, properties.bumpMapProp);
        }
    }
}
#endif
