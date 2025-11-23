Shader "Custom/GlassColumn_Improved"
{
    Properties
    {
        _Color ("Tint Color", Color) = (1,1,1,0.2)
        _Gloss ("Glossiness", Range(0, 1)) = 0.95
        _FresnelPower ("Fresnel Power", Range(0.5, 8)) = 3
        _HighlightWidth ("Highlight Width", Range(0.01, 1)) = 0.1
        _HighlightIntensity ("Highlight Intensity", Range(0, 3)) = 1.2
        _RefractStrength ("Refraction Strength", Range(0, 1)) = 0.05
    }

    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off

        GrabPass { "_GrabTexture" }        // Lấy hình nền đang render vào

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows alpha

        sampler2D _GrabTexture;

        struct Input
        {
            float3 viewDir;
            float3 worldPos;
            float4 screenPos;
        };

        float4 _Color;
        float _Gloss;
        float _FresnelPower;
        float _HighlightWidth;
        float _HighlightIntensity;
        float _RefractStrength;

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Base
            o.Albedo = _Color.rgb;
            o.Smoothness = _Gloss;
            o.Metallic = 0;

            //-----------------------------
            // Fresnel mạnh rõ hơn
            //-----------------------------
            float fresnel = pow(
                1 - saturate(dot(normalize(IN.viewDir), o.Normal)),
                _FresnelPower
            );

            //-----------------------------
            // Highlight dọc thân cột
            //-----------------------------
            float x = frac(IN.worldPos.x * 0.5);
            float highlight = exp(-pow((x - 0.5) / _HighlightWidth, 2)) 
                                * _HighlightIntensity;

            //--------------------------------
            // Refraction - khúc xạ giả Glass
            //--------------------------------
            float2 grabUV = IN.screenPos.xy / IN.screenPos.w;
            grabUV += (o.Normal.xy * _RefractStrength);

            float3 refractedCol = tex2D(_GrabTexture, grabUV).rgb;

            //--------------------------------
            // Mix màu kính và refraction
            //--------------------------------
            o.Albedo = lerp(refractedCol, o.Albedo, 0.25 + fresnel * 0.5);

            //--------------------------------
            // Alpha trong suốt đẹp hơn
            //--------------------------------
            float glassAlpha =
                _Color.a * 0.7 +
                fresnel * 0.3 +
                highlight * 0.2;

            o.Alpha = saturate(glassAlpha);
        }
        ENDCG
    }
}
