Shader "Custom/WoodPlank"
{
    Properties
    {
        _WoodColor1 ("Wood Color 1", Color) = (0.4,0.2,0.1,1)
        _WoodColor2 ("Wood Color 2", Color) = (0.7,0.5,0.3,1)
        _WoodScale ("Wood Scale", Float) = 10.0
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
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            float4 _WoodColor1;
            float4 _WoodColor2;
            float _WoodScale;

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv * _WoodScale;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float grain = sin(i.uv.x + sin(i.uv.y * 2.0)) * 0.5 + 0.5;
                return lerp(_WoodColor1, _WoodColor2, grain);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
