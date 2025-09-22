Shader "Unlit/BoosterArrows"
{
    Properties
    {
        _BaseColor ("Base Color", Color) = (0,0,0,1)
        _ArrowColor ("Arrow Color", Color) = (1,0,0,1)
        _OutlineColor ("Outline Color", Color) = (1,1,0,1)
        _ArrowCount ("Arrow Count", Float) = 3
        _ArrowWidth ("Arrow Width", Range(0.05,0.5)) = 0.2
        _ArrowHeight ("Arrow Height", Range(0.1,1)) = 0.5
        _OutlineThickness ("Outline Thickness", Range(0.005,0.1)) = 0.03
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
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

            float4 _BaseColor;
            float4 _ArrowColor;
            float4 _OutlineColor;
            float _ArrowCount;
            float _ArrowWidth;
            float _ArrowHeight;
            float _OutlineThickness;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            // Returns 1 if inside arrow, 0 otherwise
            float arrowShape(float2 uv, float width, float height)
            {
                uv.x = uv.x - 0.5;
                uv.y = uv.y / height;
                float body = step(-width*0.5, uv.x) * step(uv.x, width*0.5) * step(0, uv.y) * step(uv.y, 0.7);
                float head = step(0.7, uv.y) * step(abs(uv.x), width * (1.0 - (uv.y-0.7)/0.3));
                return max(body, head);
            }

            // Returns 1 if inside arrow with expanded width/thickness, 0 otherwise
            float arrowOutline(float2 uv, float width, float height, float thickness)
            {
                float inner = arrowShape(uv, width, height);
                float outer = arrowShape(uv, width + thickness, height + thickness);
                return outer - inner;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float4 col = _BaseColor;
                float arrowCount = max(1, _ArrowCount);
                float2 uv = i.uv;
                uv.y = frac(uv.y * arrowCount);

                float a = arrowShape(uv, _ArrowWidth, _ArrowHeight);
                float outline = arrowOutline(uv, _ArrowWidth, _ArrowHeight, _OutlineThickness);

                if (outline > 0)
                    col.rgb = _OutlineColor.rgb;
                if (a > 0)
                    col.rgb = _ArrowColor.rgb;

                col.a = 1;
                return col;
            }
            ENDCG
        }
    }
    FallBack "Unlit/Color"
}
