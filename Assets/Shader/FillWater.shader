Shader "Custom/WaterFill_Ultra"
{
    Properties
    {
        _WaterColor("Water Color", Color) = (0,0.55,1,0.9)
        _FillAmount("Fill Amount", Range(0,1)) = 0.5

        _FillDir("Fill Direction", Vector) = (0,1,0,0)
        _MinValue("Min", Float) = -1
        _MaxValue("Max", Float) = 1

        _HighlightColor("Highlight Color", Color) = (1,1,1,1)
        _HighlightIntensity("Highlight Intensity", Float) = 1.2
        _HighlightWidth("Highlight Width", Float) = 0.06

        _FresnelStrength("Fresnel Strength", Float) = 0.4
        _FresnelPower("Fresnel Power", Float) = 3.5

        _DepthStrength("Depth Strength", Float) = 0.45

        _WaveScale("Wave Scale", Float) = 0.01
        _WaveSpeed("Wave Speed", Float) = 0.8

        _SpecularIntensity("Specular Intensity", Float) = 0.35
        _SpecularSize("Specular Size", Float) = 0.2
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            float4 _WaterColor;
            float _FillAmount;

            float3 _FillDir;
            float _MinValue;
            float _MaxValue;

            float4 _HighlightColor;
            float _HighlightIntensity;
            float _HighlightWidth;

            float _FresnelStrength;
            float _FresnelPower;

            float _DepthStrength;

            float _WaveScale;
            float _WaveSpeed;

            float _SpecularIntensity;
            float _SpecularSize;

            struct appdata {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f {
                float4 pos : SV_POSITION;
                float3 posOS : TEXCOORD0;
                float3 normalWS : TEXCOORD1;
                float3 viewDirWS : TEXCOORD2;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);

                o.posOS = v.vertex.xyz;
                o.normalWS = UnityObjectToWorldNormal(v.normal);

                float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.viewDirWS = normalize(_WorldSpaceCameraPos - worldPos);

                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float3 dir = normalize(_FillDir);
                float proj = dot(i.posOS, dir);

                float fillLine = lerp(_MinValue, _MaxValue, _FillAmount);

                // ----- 1. WAVE (very small – just to avoid perfect flat) -----
                float wave =
                    sin(i.posOS.x * 6 + _Time.y * _WaveSpeed) * _WaveScale * 0.6 +
                    cos(i.posOS.z * 7 + _Time.y * _WaveSpeed * 0.8) * _WaveScale * 0.4;

                float diff = fillLine - proj + wave;
                float alpha = smoothstep(0.0, 0.09, diff);

                if (alpha <= 0) discard;

                float surface = fillLine - proj;

                // ----- 2. HIGHLIGHT (Gaussian) -----
                float highlight = exp(-pow(surface / _HighlightWidth, 2.0));
                highlight *= _HighlightIntensity;

                // ----- 3. FRESNEL -----
                float fresnel = pow(1.0 - saturate(dot(i.normalWS, i.viewDirWS)), _FresnelPower);
                fresnel *= _FresnelStrength;

                // ----- 4. DEPTH SHADING -----
                float depth = saturate(surface * 1.6);
                depth = lerp(1.0, 1.0 - _DepthStrength, depth);

                // ----- 5. SPECULAR -----
                float spec = exp(-pow(length(i.posOS.xz) / _SpecularSize, 2.0));
                spec *= _SpecularIntensity;

                float3 col = _WaterColor.rgb;

                // blend specular
                col += spec;

                // blend highlight
                col = lerp(col, _HighlightColor.rgb, highlight);

                // blend fresnel border
                col = lerp(col, _HighlightColor.rgb, fresnel);

                // depth
                col *= depth;

                return float4(col, _WaterColor.a * alpha);
            }
            ENDCG
        }
    }
}
