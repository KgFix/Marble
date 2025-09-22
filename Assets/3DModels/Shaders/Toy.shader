Shader "Custom/ToyToon"
{
    Properties
    {
        _Color ("Main Color", Color) = (1,1,1,1)
        _RampThreshold ("Ramp Threshold", Range(0,1)) = 0.5
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 normalDir : TEXCOORD0;
            };

            float4 _Color;
            float _RampThreshold;

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float3 N = normalize(i.normalDir);
                float3 L = normalize(_WorldSpaceLightPos0.xyz);
                float NdotL = dot(N, L);
                float shade = NdotL > _RampThreshold ? 1.0 : 0.5;
                return fixed4(_Color.rgb * shade, 1.0);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
