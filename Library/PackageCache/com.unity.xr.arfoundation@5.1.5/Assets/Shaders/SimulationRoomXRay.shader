Shader "Simulation/Room X-Ray"
{
    // A standard shader variant that takes the global room properties and applies them to cut out a view into the geometry based on camera location
    Properties
    {
        // Simulation/Legacy/Room X-Ray properties
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _BumpMap ("Normal Map", 2D) = "bump" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
        [HideInInspector] _Mode ("__mode", Float) = 0.0

        // Specular vs Metallic workflow
        [HideInInspector] _WorkflowMode("WorkflowMode", Float) = 1.0

        [HideInInspector] [MainColor] _BaseColor("Color", Color) = (1,1,1,1)
        [HideInInspector] [MainTexture] _BaseMap("Albedo", 2D) = "white" {}

        [HideInInspector] _Cutoff("Alpha Cutoff", Range(0.0, 1.0)) = 0.5

        [HideInInspector] _Smoothness("Smoothness", Range(0.0, 1.0)) = 0.5
        [HideInInspector] _SmoothnessTextureChannel("Smoothness texture channel", Float) = 0

        [HideInInspector] _MetallicGlossMap("Metallic", 2D) = "white" {}

        [HideInInspector] _SpecColor("Specular", Color) = (0.2, 0.2, 0.2)
        [HideInInspector] _SpecGlossMap("Specular", 2D) = "white" {}

        [HideInInspector] [ToggleOff] _SpecularHighlights("Specular Highlights", Float) = 1.0
        [HideInInspector] [ToggleOff] _EnvironmentReflections("Environment Reflections", Float) = 1.0

        [HideInInspector] _BumpScale("Scale", Float) = 1.0

        [HideInInspector] _OcclusionStrength("Strength", Range(0.0, 1.0)) = 1.0
        [HideInInspector] _OcclusionMap("Occlusion", 2D) = "white" {}

        [HideInInspector] _EmissionColor("Color", Color) = (0,0,0)
        [HideInInspector] _EmissionMap("Emission", 2D) = "white" {}

        // Blending state
        [HideInInspector] _Surface("__surface", Float) = 0.0
        [HideInInspector] _Blend("__blend", Float) = 0.0
        [HideInInspector] _AlphaClip("__clip", Float) = 0.0
        [HideInInspector] _SrcBlend("__src", Float) = 1.0
        [HideInInspector] _DstBlend("__dst", Float) = 0.0
        [HideInInspector] _ZWrite("__zw", Float) = 1.0
        [HideInInspector] _Cull("__cull", Float) = 2.0

        [HideInInspector] _ReceiveShadows("Receive Shadows", Float) = 1.0
        // Editmode props
        [HideInInspector] _QueueOffset("Queue offset", Float) = 0.0

        // ObsoleteProperties
        [HideInInspector] _GlossMapScale("Smoothness", Float) = 0.0
        [HideInInspector] _GlossyReflections("EnvironmentReflections", Float) = 0.0
    }

    SubShader
    {
        // Universal Pipeline tag is required. If Universal render pipeline is not set in the graphics settings
        // this Subshader will fail. One can add a subshader below or fallback to Standard built-in to make this
        // material work with both Universal Render Pipeline and Builtin Unity Pipeline
        Tags {"RenderType" = "Opaque" "Queue"="Geometry-10" "RenderPipeline" = "UniversalPipeline" "IgnoreProjector" = "True"}
        LOD 300

        // ------------------------------------------------------------------
        //  Forward pass. Shades all light in a single pass. GI + emission + Fog
        UsePass "Simulation/URP/Room X-Ray/ForwardLit"

        // ------------------------------------------------------------------
        //  Forward pass. No Lights X-Ray faded edge
        UsePass "Simulation/URP/Room X-Ray/ForwardUnlit"

        UsePass "Universal Render Pipeline/Lit/ShadowCaster"
        UsePass "Universal Render Pipeline/Lit/DepthOnly"
        // This pass it not used during regular rendering, only for lightmap baking.
        UsePass "Universal Render Pipeline/Lit/Meta"
        UsePass "Universal Render Pipeline/Lit/Universal2D"
    }

    Fallback "Simulation/Legacy/Room X-Ray"
    CustomEditor "UnityEditor.XR.Simulation.Rendering.MaterialInspector"
}
