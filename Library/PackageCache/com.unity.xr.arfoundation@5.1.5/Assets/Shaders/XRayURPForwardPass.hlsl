#ifndef XRAY_URP_FORWARD_LIT_PASS_INCLUDED
#define XRAY_URP_FORWARD_LIT_PASS_INCLUDED

 // Used in URPv7.2.x+ for 'positionWS' in `Varyings`
#if !defined(REQUIRES_WORLD_SPACE_POS_INTERPOLATOR)
#define REQUIRES_WORLD_SPACE_POS_INTERPOLATOR
#endif

 // Used in URPv7.1.x for 'positionWS' in `Varyings`
#if !defined(_ADDITIONAL_LIGHTS)
#define _ADDITIONAL_LIGHTS
#endif

#include "XRayCommon.cginc"
#include "Packages/com.unity.render-pipelines.universal/Shaders/LitForwardPass.hlsl"

// Used in Standard (Physically Based) shader
half4 XRayLitPassFragment(Varyings input) : SV_Target
{
    UNITY_SETUP_INSTANCE_ID(input);
    UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

    SurfaceData surfaceData;
    InitializeStandardLitSurfaceData(input.uv, surfaceData);

    InputData inputData;
    InitializeInputData(input, surfaceData.normalTS, inputData);

    half4 color = UniversalFragmentPBR(inputData, surfaceData.albedo, surfaceData.metallic, surfaceData.specular, surfaceData.smoothness, surfaceData.occlusion, surfaceData.emission, surfaceData.alpha);

#if SIMULATION_XRAY_ENABLED
    half lightValue = getXRayFade(input.positionWS);
#else
    half lightValue = 1.0h;
#endif

    color.rgb = MixFog(color.rgb, inputData.fogCoord) * lightValue;
    color.a = color.a * lightValue;
    return color;
}

half4 XRayUnLitPassFragment(Varyings input) : SV_Target
{
    UNITY_SETUP_INSTANCE_ID(input);
    UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

    SurfaceData surfaceData;
    InitializeStandardLitSurfaceData(input.uv, surfaceData);

    InputData inputData;
    InitializeInputData(input, surfaceData.normalTS, inputData);

    half4 color = half4(0.0h, 0.0h, 0.0h, 1.0h);

#if SIMULATION_XRAY_ENABLED
    half lightValue = getXRayEdgeFade(input.positionWS);
#else
    half lightValue = 0.0h;
#endif

    color.rgb = MixFog(color.rgb, inputData.fogCoord);
    color.a = color.a * lightValue;
    return color;
}

#endif
