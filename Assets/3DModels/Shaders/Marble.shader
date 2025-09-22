Shader "Custom/PlayerMarble"
{
    Properties
    {
        _MarbleColor1 ("Marble Color 1", Color) = (0.8,0.8,0.85,1)
        _MarbleColor2 ("Marble Color 2", Color) = (0.2,0.2,0.25,1)
        _Metallic ("Metallic", Range(0,1)) = 1
        _Smoothness ("Smoothness", Range(0,1)) = 0.9
        _MarbleScale ("Marble Scale", Float) = 8.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 300

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
                float3 worldPos : TEXCOORD0;
                float3 worldNormal : TEXCOORD1;
            };

            fixed4 _MarbleColor1;
            fixed4 _MarbleColor2;
            float _Metallic;
            float _Smoothness;
            float _MarbleScale;

            float noise(float3 p)
            {
                return frac(sin(dot(p, float3(12.9898,78.233,37.719))) * 43758.5453);
            }

            float marblePattern(float3 p)
            {
                float n = 0;
                float scale = 1.0;
                float amplitude = 1.0;
                for (int i = 0; i < 5; i++)
                {
                    n += noise(p * scale) * amplitude;
                    scale *= 2.0;
                    amplitude *= 0.5;
                }
                return n;
            }

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float3 p = i.worldPos * _MarbleScale;
                float veins = sin(p.x + marblePattern(p));
                float t = smoothstep(-0.2, 0.2, veins);
                fixed3 marbleColor = lerp(_MarbleColor1.rgb, _MarbleColor2.rgb, t);

                float3 N = normalize(i.worldNormal);
                float3 V = normalize(_WorldSpaceCameraPos - i.worldPos);
                float3 L = normalize(_WorldSpaceLightPos0.xyz);
                float3 H = normalize(L + V);

                float fresnel = pow(1.0 - saturate(dot(N, V)), 5.0);

                float NdotL = saturate(dot(N, L));
                float NdotV = saturate(dot(N, V));
                float NdotH = saturate(dot(N, H));
                float VdotH = saturate(dot(V, H));

                float roughness = 1.0 - _Smoothness;
                float alpha = roughness * roughness;
                float alpha2 = alpha * alpha;
                float denom = (NdotH * NdotH) * (alpha2 - 1.0) + 1.0;
                float D = alpha2 / (UNITY_PI * denom * denom);

                float3 F0 = lerp(float3(0.04,0.04,0.04), marbleColor, _Metallic);
                float3 F = F0 + (1.0 - F0) * pow(1.0 - VdotH, 5.0);

                float k = (alpha + 1.0) * (alpha + 1.0) / 8.0;
                float G_V = NdotV / (NdotV * (1.0 - k) + k);
                float G_L = NdotL / (NdotL * (1.0 - k) + k);
                float G = G_V * G_L;

                float3 specular = (D * F * G) / (4.0 * NdotL * NdotV + 0.001);
                float3 color = marbleColor * (1.0 - _Metallic) * NdotL + specular;
                color = lerp(color, F, fresnel * _Metallic);

                return float4(color, 1.0);
            }
            ENDCG
        }
    }
    FallBack "Standard"
}
