Shader "Custom/ChromeMarbleGlow"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _GlowColor ("Glow Color", Color) = (0.8,0.9,1,1)
        _GlowStrength ("Glow Strength", Range(0,2)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 1
        _Smoothness ("Smoothness", Range(0,1)) = 0.9
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
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float3 worldNormal : TEXCOORD1;
                float3 viewDir : TEXCOORD2;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _GlowColor;
            float _GlowStrength;
            float _Metallic;
            float _Smoothness;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.viewDir = normalize(_WorldSpaceCameraPos - worldPos);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Base color
                fixed4 col = tex2D(_MainTex, i.uv);

                // Rim (glow) effect
                float rim = 1.0 - saturate(dot(i.viewDir, i.worldNormal));
                float glow = pow(rim, 2.5) * _GlowStrength;

                // Fresnel for chrome/metallic look
                float fresnel = pow(1.0 - saturate(dot(i.viewDir, i.worldNormal)), 4.0) * _Metallic;

                // Combine base, fresnel, and glow
                col.rgb = lerp(col.rgb, _GlowColor.rgb, glow);
                col.rgb += fresnel * _GlowColor.rgb * _Smoothness;

                col.a = 1;
                return col;
            }
            ENDCG
        }
    }
}
