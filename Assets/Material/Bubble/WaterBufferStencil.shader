Shader "Custom/WaterBubble_Stencil"
{
    Properties
    {
        _Color("Color", Color) = (1,1,1,0.6)
        _MainTex("Texture", 2D) = "white" {}

        _Brightness("Brightness", Float) = 1.6
        _CoreStrength("Core Strength", Float) = 1.2
        _EdgeContrast("Edge Contrast", Float) = 1.8
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent+1"
            "RenderType"="Transparent"
            "IgnoreProjector"="True"
        }

        // STENCIL GIỮ NGUYÊN
        Stencil
        {
            Ref 1
            Comp Equal
            Pass Keep
        }

        // Additive nhẹ + alpha
        Blend SrcAlpha One
        ZWrite Off
        Cull Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Color;

            float _Brightness;
            float _CoreStrength;
            float _EdgeContrast;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 uv = i.uv;
                fixed4 tex = tex2D(_MainTex, uv);

                // --- Alpha contrast (biên rõ hơn)
                float alpha = pow(tex.a, _EdgeContrast);

                // --- Core sáng ở giữa bubble
                float2 centerUV = uv - 0.5;
                float dist = length(centerUV);
                float core = saturate(1.0 - dist * 2.0);
                core = pow(core, 2.0) * _CoreStrength;

                // --- Color
                float3 col = tex.rgb * _Color.rgb;
                col *= _Brightness;
                col += core;

                return float4(col, alpha * _Color.a);
            }
            ENDCG
        }
    }
}
