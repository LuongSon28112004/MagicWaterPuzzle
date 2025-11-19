Shader "Custom/GlassColumn"
{
    Properties
    {
        _Color ("Tint Color", Color) = (1,1,1,0.5)
        _Gloss ("Glossiness", Range(0, 1)) = 0.8
        _FresnelPower ("Fresnel Power", Range(0.5, 8)) = 3
        _HighlightWidth ("Highlight Width", Range(0.01, 1)) = 0.1
        _HighlightIntensity ("Highlight Intensity", Range(0, 3)) = 1.2
    }

    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows alpha

        struct Input
        {
            float3 viewDir;      // hướng camera
            float3 worldPos;     // vị trí để tạo hiệu ứng cột dọc
        };

        float4 _Color;
        float _Gloss;
        float _FresnelPower;
        float _HighlightWidth;
        float _HighlightIntensity;

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Tint color
            o.Albedo = _Color.rgb;
            o.Smoothness = _Gloss;
            o.Metallic = 0.0;

            // Fresnel
            float fresnel = pow(1 - saturate(dot(normalize(IN.viewDir), o.Normal)), _FresnelPower);

            // Highlight dọc theo trục X (giống hình mẫu)
            float x = frac(IN.worldPos.x * 0.5);    // lặp lại nhẹ
            float highlight = exp(-pow((x - 0.5) / _HighlightWidth, 2)) * _HighlightIntensity;

            // Alpha kính trong suốt
            o.Alpha = _Color.a + fresnel * 0.2 + highlight * 0.15;
        }
        ENDCG
    }
}
