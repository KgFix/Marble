// Full Shader File: URP Toon Shading with a Texture, Geometry Outline, and MULTI-LIGHT support.
Shader "Custom/URPToonWithTextureAndMultiLightOutline"
{
    Properties
    {
        // --- NEW: Added a texture property ---
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Base Color Tint", Color) = (1, 1, 1, 1)
        _OutlineColor ("Outline Color", Color) = (0, 0, 0, 1)
        _OutlineThickness ("Outline Thickness (Pixels)", Range(1, 10)) = 2
        _ToonLevels ("Toon Shading Steps", Range(1, 8)) = 4
    }
    SubShader
    {
        Tags { "RenderPipeline"="UniversalPipeline" "RenderType"="Opaque" }

        // PASS 1: GEOMETRY SHADER OUTLINE (This pass is unchanged)
        Pass
        {
            Name "GeometryOutline"
            Cull Off
            ZWrite Off

            HLSLPROGRAM
            #pragma vertex vert
            #pragma geometry geom
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            CBUFFER_START(UnityPerMaterial)
                float4 _OutlineColor;
                float _OutlineThickness;
            CBUFFER_END

            struct Attributes
            {
                float4 positionOS : POSITION;
            };

            struct v2g
            {
                float4 positionCS : SV_POSITION;
            };
            
            struct g2f
            {
                float4 positionCS : SV_POSITION;
            };

            v2g vert(Attributes IN)
            {
                v2g OUT;
                OUT.positionCS = TransformObjectToHClip(IN.positionOS.xyz);
                return OUT;
            }

            [maxvertexcount(18)]
            void geom(triangle v2g IN[3], inout TriangleStream<g2f> stream)
            {
                float4 clipPos[3] = { IN[0].positionCS, IN[1].positionCS, IN[2].positionCS };
                float2 screenPos0 = clipPos[0].xy / clipPos[0].w;
                float2 screenPos1 = clipPos[1].xy / clipPos[1].w;
                float2 screenPos2 = clipPos[2].xy / clipPos[2].w;
                float winding = (screenPos1.x - screenPos0.x) * (screenPos2.y - screenPos0.y) - (screenPos2.x - screenPos0.x) * (screenPos1.y - screenPos0.y);

                if (winding > 0)
                {
                    g2f OUT;
                    float2 thickness = _OutlineThickness / _ScreenParams.xy;
                    for (int i = 0; i < 3; i++)
                    {
                        int currentIndex = i;
                        int nextIndex = (i + 1) % 3;
                        float4 p1 = clipPos[currentIndex];
                        float4 p2 = clipPos[nextIndex];
                        float2 screenEdge = normalize((p2.xy/p2.w) - (p1.xy/p1.w));
                        float2 perpendicular = float2(-screenEdge.y, screenEdge.x);
                        float4 offset1 = float4(perpendicular * thickness * p1.w, 0, 0);
                        float4 offset2 = float4(perpendicular * thickness * p2.w, 0, 0);

                        OUT.positionCS = p1;        stream.Append(OUT);
                        OUT.positionCS = p2;        stream.Append(OUT);
                        OUT.positionCS = p1 + offset1; stream.Append(OUT);
                        stream.RestartStrip();
                        OUT.positionCS = p2;        stream.Append(OUT);
                        OUT.positionCS = p2 + offset2; stream.Append(OUT);
                        OUT.positionCS = p1 + offset1; stream.Append(OUT);
                        stream.RestartStrip();
                    }
                }
            }

            half4 frag(g2f IN) : SV_TARGET
            {
                return _OutlineColor;
            }
            ENDHLSL
        }

        // PASS 2: UPGRADED TOON SHADING (Now supports a texture and multiple lights)
        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode" = "UniversalForward" }
            Cull Back

            HLSLPROGRAM
            #pragma vertex vert_toon
            #pragma fragment frag_toon

            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile _ _ADDITIONAL_LIGHTS_V2
            #pragma multi_compile _ _ADDITIONAL_LIGHT_SHADOWS
            #pragma multi_compile _ _SHADOWS_SOFT

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            struct Attributes_Toon
            {
                float4 positionOS : POSITION;
                float3 normalOS   : NORMAL;
                // --- NEW: Added UV coordinates to the vertex input ---
                float2 uv         : TEXCOORD0;
            };

            struct Varyings_Toon
            {
                float4 positionCS   : SV_POSITION;
                float3 worldNormal  : TEXCOORD0;
                float3 worldPos     : TEXCOORD1;
                float4 shadowCoord  : TEXCOORD2;
                // --- NEW: Added UV coordinates to be passed to the fragment shader ---
                float2 uv           : TEXCOORD3;
            };

            CBUFFER_START(UnityPerMaterial)
                float4 _Color;
                float _ToonLevels;
                // --- NEW: This float4 is automatically populated by Unity for tiling and offset ---
                float4 _MainTex_ST;
            CBUFFER_END

            // --- NEW: Declare the texture and its sampler ---
            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            Varyings_Toon vert_toon(Attributes_Toon IN)
            {
                Varyings_Toon OUT;
                OUT.worldNormal = TransformObjectToWorldNormal(IN.normalOS);
                OUT.worldPos = TransformObjectToWorld(IN.positionOS.xyz);
                OUT.positionCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.shadowCoord = TransformWorldToShadowCoord(OUT.worldPos);

                // --- NEW: Transform and pass the UVs to the fragment shader ---
                OUT.uv = TRANSFORM_TEX(IN.uv, _MainTex);

                return OUT;
            }

            half4 frag_toon(Varyings_Toon IN) : SV_Target
            {
                // --- NEW: Sample the texture using the UV coordinates ---
                half4 texColor = _MainTex.Sample(sampler_MainTex, IN.uv);

                float3 normal = normalize(IN.worldNormal);
                half3 totalLightColor = 0;

                // 1. Calculate Main Light Contribution
                #if defined(_MAIN_LIGHT_SHADOWS) || defined(_MAIN_LIGHT_SHADOWS_CASCADE)
                    Light mainLight = GetMainLight(IN.shadowCoord);
                    float mainLightIntensity = saturate(dot(normal, mainLight.direction)) * mainLight.shadowAttenuation;
                    float mainToon = floor(mainLightIntensity * _ToonLevels) / _ToonLevels;
                    totalLightColor += mainToon * mainLight.color;
                #endif

                // 2. Loop Through All Additional Lights
                int additionalLightsCount = GetAdditionalLightsCount();
                for (int i = 0; i < additionalLightsCount; ++i)
                {
                    Light additionalLight = GetAdditionalLight(i, IN.worldPos);
                    half attenuation = additionalLight.distanceAttenuation * additionalLight.shadowAttenuation;
                    float lightIntensity = saturate(dot(normal, additionalLight.direction)) * attenuation;
                    float toon = floor(lightIntensity * _ToonLevels) / _ToonLevels;
                    totalLightColor += toon * additionalLight.color;
                }

                // 3. Add Ambient Light
                half3 ambient = SampleSH(IN.worldNormal);

                // 4. Combine Everything
                // --- MODIFIED: Multiply the texture color with the tint and the calculated light ---
                half3 finalColor = texColor.rgb * _Color.rgb * (totalLightColor + ambient);

                return half4(finalColor, texColor.a);
            }
            ENDHLSL
        }
    }
    FallBack "Universal Render Pipeline/Lit"
}