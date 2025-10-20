Shader "Custom/ToonShader"
{
    // These are the properties that will show up in the Inspector in Unity.
    Properties
    {
        _Color ("Main Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        [Header(Toon Shading)]
        _ToneMap ("Tone Ramp", 2D) = "gray" {}
        _ShadowSharpness ("Shadow Sharpness", Range(1, 10)) = 2
        [Header(Rim Lighting)]
        _RimColor ("Rim Color", Color) = (1,1,1,1)
        _RimAmount ("Rim Amount", Range(0, 1)) = 0.716
        _RimThreshold ("Rim Threshold", Range(0, 1)) = 0.1
    }
    SubShader
    {
        // This Pass is for rendering with the main directional light in the scene.
        Tags { "RenderType"="Opaque" "LightMode"="ForwardBase" }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // This line is essential for getting light data from Unity.
            #pragma multi_compile_fwdbase

            #include "UnityCG.cginc"
            #include "AutoLight.cginc"

            // Link the Properties from above to variables we can use in the code.
            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _Color;
            sampler2D _ToneMap;
            float _ShadowSharpness;
            fixed4 _RimColor;
            float _RimAmount;
            float _RimThreshold;

            // This struct defines the data passed from the vertex shader to the fragment shader.
            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 worldNormal : TEXCOORD1;
                float3 viewDir : TEXCOORD2;
                LIGHTING_COORDS(3,4) // Macro for shadows
            };

            // THE VERTEX SHADER
            // This runs for every vertex of your 3D model. Its job is to calculate screen position.
            v2f vert (appdata_base v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                o.viewDir = normalize(UnityWorldSpaceViewDir(mul(unity_ObjectToWorld, v.vertex).xyz));
                TRANSFER_VERTEX_TO_FRAGMENT(o); // Macro for shadows
                return o;
            }

            // THE FRAGMENT (PIXEL) SHADER
            // This runs for every pixel on the model's surface. Its job is to return a final color.
            fixed4 frag (v2f i) : SV_Target
            {
                // Get the base color from the texture and tint it with the main color.
                fixed4 albedo = tex2D(_MainTex, i.uv) * _Color;

                // --- Lighting Calculation ---
                float3 lightDir = _WorldSpaceLightPos0.xyz; // Direction of the main light
                float3 normal = normalize(i.worldNormal);

                // This is the core of all lighting: the dot product.
                // It gives a value from -1 to 1 based on the angle between the surface and the light.
                // We use saturate() to clamp it to a 0-1 range.
                float NdotL = saturate(dot(normal, lightDir));

                // Shadow calculation
                float shadow = SHADOW_ATTENUATION(i);
                
                // Combine light and shadow
                float lightIntensity = NdotL * shadow;

                // --- Toon Shading (Cel Shading) ---
                // We sample our "Tone Ramp" texture to get the hard-edged shadow look.
                // Instead of a smooth 0-1 gradient, this remaps it to the colors in our ramp.
                fixed3 ramp = tex2D(_ToneMap, float2(lightIntensity, 0.5)).rgb;

                // --- Rim Lighting ---
                // This creates a highlight on the edges of the object.
                float NdotV = saturate(dot(normal, i.viewDir));
                float rim_raw = 1.0 - NdotV;
                float rim = smoothstep(_RimThreshold, _RimThreshold + 0.1, rim_raw) * pow(NdotL, _RimAmount);
                fixed3 rimLight = rim * _RimColor.rgb;

                // --- Final Color ---
                // Combine all the parts: albedo, toon shading, and rim lighting.
                fixed3 finalColor = albedo.rgb * ramp + rimLight;

                return fixed4(finalColor, 1.0);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
