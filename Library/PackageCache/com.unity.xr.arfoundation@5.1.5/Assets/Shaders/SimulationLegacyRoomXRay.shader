Shader "Simulation/Legacy/Room X-Ray"
{
    // A standard shader variant that takes the global room properties and applies them to cut out a view into the geometry based on camera location
    Properties
    {
        // Simulation/Legacy/Room X-Ray properties
        _Color ("Color", Color) = (1,1,1,1) // _BaseColor
        _MainTex ("Albedo (RGB)", 2D) = "white" {} // _BaseMap
        _BumpMap ("Normal Map", 2D) = "bump" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5 // _Smoothness
        _Metallic ("Metallic", Range(0,1)) = 0.0
        [HideInInspector] _Mode ("__mode", Float) = 0.0

        // Specular vs Metallic workflow
        [HideInInspector] _WorkflowMode("WorkflowMode", Float) = 1.0

        [MainColor] _BaseColor("Color", Color) = (1,1,1,1)
        [MainTexture] _BaseMap("Albedo", 2D) = "white" {}

        _Cutoff("Alpha Cutoff", Range(0.0, 1.0)) = 0.5

        _Smoothness("Smoothness", Range(0.0, 1.0)) = 0.5
        _SmoothnessTextureChannel("Smoothness texture channel", Float) = 0

        _MetallicGlossMap("Metallic", 2D) = "white" {}

        _SpecColor("Specular", Color) = (0.2, 0.2, 0.2)
        _SpecGlossMap("Specular", 2D) = "white" {}

        [ToggleOff] _SpecularHighlights("Specular Highlights", Float) = 1.0
        [ToggleOff] _EnvironmentReflections("Environment Reflections", Float) = 1.0

        _BumpScale("Scale", Float) = 1.0

        _OcclusionStrength("Strength", Range(0.0, 1.0)) = 1.0
        _OcclusionMap("Occlusion", 2D) = "white" {}

        _EmissionColor("Color", Color) = (0,0,0)
        _EmissionMap("Emission", 2D) = "white" {}

        // Blending state
        [HideInInspector] _Surface("__surface", Float) = 0.0
        [HideInInspector] _Blend("__blend", Float) = 0.0
        [HideInInspector] _AlphaClip("__clip", Float) = 0.0
        [HideInInspector] _SrcBlend("__src", Float) = 1.0
        [HideInInspector] _DstBlend("__dst", Float) = 0.0
        [HideInInspector] _ZWrite("__zw", Float) = 1.0
        [HideInInspector] _Cull("__cull", Float) = 2.0

        _ReceiveShadows("Receive Shadows", Float) = 1.0
        // Editmode props
        [HideInInspector] _QueueOffset("Queue offset", Float) = 0.0

        // ObsoleteProperties
        [HideInInspector] _GlossMapScale("Smoothness", Float) = 0.0
        [HideInInspector] _GlossyReflections("EnvironmentReflections", Float) = 0.0
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Geometry-10" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows addshadow nofog finalcolor:fadeEdge

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        #include "XRayCommon.cginc"
        #pragma multi_compile _ SIMULATION_XRAY_ENABLED
        #define LEGACY_IN_SRP

        sampler2D _MainTex;
        sampler2D _BumpMap;

        struct Input
        {
            float2 uv_MainTex;
            float2 uv_BumpMap;
            float3 worldPos;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
#if SIMULATION_XRAY_ENABLED
            half lightValue = getXRayFade(IN.worldPos);
#else
            half lightValue = 1;
#endif
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Normal = UnpackNormal (tex2D (_BumpMap, IN.uv_BumpMap));
            o.Alpha = lightValue;
        }

        void fadeEdge(Input IN, SurfaceOutputStandard o, inout fixed4 color)
        {
            color *= o.Alpha;
        }
        ENDCG

        Zwrite On
        blend srcalpha oneminussrcalpha

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf NoLight noshadow nofog keepalpha

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        #include "XRayCommon.cginc"
        #pragma multi_compile _ SIMULATION_XRAY_ENABLED

        struct Input
        {
            float3 worldPos;
        };

        void surf (Input IN, inout SurfaceOutput o)
        {
#if SIMULATION_XRAY_ENABLED
            half lightValue = getXRayEdgeFade(IN.worldPos);
#else
            half lightValue = 0;
#endif
            o.Albedo = 0;
            o.Alpha = lightValue;
        }

        half4 LightingNoLight (SurfaceOutput s, half3 lightDir, half atten)
        {
            half4 c;
            c.rgb = s.Albedo;
            c.a = s.Alpha;
            return c;
        }

        ENDCG
    }

    Fallback "Unlit/Transparent"
    CustomEditor "UnityEditor.XR.Simulation.Rendering.MaterialInspector"
}
