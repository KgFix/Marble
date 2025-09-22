Shader "Custom/SunsetSkybox"
{
    Properties
    {
        _TopColor ("Top Color", Color) = (0.15,0.09,0.25,1)      // Deep purple/blue
        _MiddleColor ("Middle Color", Color) = (1,0.4,0.1,1)     // Orange
        _BottomColor ("Bottom Color", Color) = (1,0.7,0.3,1)     // Light orange/yellow
        _MiddleHeight ("Middle Height", Range(0,1)) = 0.4
    }
    SubShader
    {
        Tags { "Queue"="Background" }
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 dir : TEXCOORD0;
            };

            float4 _TopColor;
            float4 _MiddleColor;
            float4 _BottomColor;
            float _MiddleHeight;

            v2f vert (appdata_base v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.dir = v.vertex.xyz;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float t = saturate(i.dir.y * 0.5 + 0.5);
                float m = smoothstep(_MiddleHeight - 0.1, _MiddleHeight + 0.1, t);
                float b = smoothstep(0.0, _MiddleHeight, t);
                float4 color = lerp(_BottomColor, _MiddleColor, b);
                color = lerp(color, _TopColor, m);
                return color;
            }
            ENDCG
        }
    }
    FallBack Off
}
