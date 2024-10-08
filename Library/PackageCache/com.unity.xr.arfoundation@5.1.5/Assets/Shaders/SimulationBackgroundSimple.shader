Shader "Unlit/Simulation Background Simple"
{
    Properties
    {
        _textureSingle ("TextureSingle", 2D) = "white" {}
    }
    SubShader
    {
        Tags
        {
            "Queue" = "Background"
            "RenderType" = "Background"
            "ForceNoShadowCasting" = "True"
        }

        Pass
        {
            Cull Off
            ZTest LEqual
            ZWrite On
            Lighting Off
            LOD 100
            Tags
            {
                "LightMode" = "Always"
            }

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            struct fragment_output
            {
                float4 color : SV_Target;
                float depth : SV_Depth;
            };

            sampler2D _textureSingle;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fragment_output frag (v2f i)
            {
                // sample the texture
                fixed4 col = tex2D(_textureSingle, i.uv);
                fragment_output o;
                o.color = col;
#if defined(UNITY_REVERSED_Z)
                o.depth = 0.0f;
#else
                o.depth = 1.0f;
#endif
                return o;
            }
            ENDCG
        }
    }
}
