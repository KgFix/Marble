Shader "Unlit/LaserWaveGlowGradient"
{
    Properties
    {
        _MainTex ("Laser Wave Texture", 2D) = "white" {}
        _MaskTex ("Fade Mask (1D)", 2D) = "white" {}
        _ColorA ("Start Color", Color) = (1,0.5,0.7,1)
        _ColorB ("End Color", Color) = (1,1,0.3,1)
        _GlowColor ("Glow Color", Color) = (1,0.5,0,1)
        _GlowIntensity ("Glow Intensity", Range(0,5)) = 2
        _GlowFalloff ("Glow Falloff", Range(0.1,2)) = 1
        _XSpeed ("X Speed", Float) = 0.8
        _YSpeed ("Y Speed", Float) = 0.0
        _Alpha ("Alpha", Range(0,1)) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            sampler2D _MaskTex;
            float4 _MainTex_ST;
            fixed4 _ColorA;
            fixed4 _ColorB;
            fixed4 _GlowColor;
            float _GlowIntensity;
            float _GlowFalloff;
            float _XSpeed;
            float _YSpeed;
            float _Alpha;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
{
    float2 speed = float2(_XSpeed, _YSpeed);
    float2 movingUV = i.uv + speed * _Time.y;
    movingUV = frac(movingUV);

    // Mirror every second repetition along X
    float xTile = floor(i.uv.x + speed.x * _Time.y);
    float mirroredX = (fmod(xTile, 2) == 0) ? movingUV.x : 1.0 - movingUV.x;
    float2 mirroredUV = float2(mirroredX, movingUV.y);

    fixed4 texCol = tex2D(_MainTex, mirroredUV);

    // Gradient color along X axis
    fixed4 gradCol = lerp(_ColorA, _ColorB, i.uv.x);

    // Apply gradient to texture
    fixed3 laserColor = texCol.rgb * gradCol.rgb;

    // Glow calculation (stronger, colored)
    float glow = exp(-pow(abs(i.uv.y - 0.5) / _GlowFalloff, 2)) * _GlowIntensity;
    fixed3 glowColor = _GlowColor.rgb * glow;

    // Combine: laser color + glow
    fixed4 col;
    col.rgb = laserColor + glowColor;
    col.a = texCol.a * _Alpha * gradCol.a;
    col.a += _GlowColor.a * glow * 0.5 * _Alpha;

    UNITY_APPLY_FOG(i.fogCoord, col);
    return col;
}

            ENDCG
        }
    }
}
