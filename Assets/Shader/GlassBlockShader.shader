Shader "Custom/GlassImproved"
{
    Properties
    {
        _Color         ("Glass Tint Color", Color) = (0.5, 0.6, 1, 0.2)
        _Smoothness    ("Smoothness", Range(0,1))  = 0.95
        _Metallic      ("Metallic", Range(0,1))    = 0.05
        _Transparency  ("Transparency", Range(0,1)) = 0.15

        // Hiệu ứng viền sáng (Fresnel)
        _EdgeBrightness("Edge Brightness", Range(0,3)) = 1.6
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }

        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off

        CGPROGRAM
        #pragma surface surf Standard alpha:fade fullforwardshadows

        struct Input
        {
            float3 viewDir;
        };

        fixed4 _Color;
        float _Smoothness;
        float _Metallic;
        float _Transparency;
        float _EdgeBrightness;

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Fresnel: làm sáng viền thủy tinh
            float fresnel = 1.0 - saturate(dot(normalize(IN.viewDir), o.Normal));
            fresnel = pow(fresnel, 3) * _EdgeBrightness;

            // Màu thủy tinh + Fresnel
            float3 finalColor = _Color.rgb + fresnel;

            o.Albedo = float3(0,0,0);     // không fill màu
            o.Emission = finalColor;      // dùng emission để giữ viền sáng và tint
            o.Metallic   = _Metallic;
            o.Smoothness = _Smoothness;

            // Độ trong suốt
            o.Alpha = (1.0 - _Transparency) * _Color.a;
        }
        ENDCG
    }

    FallBack "Transparent/Diffuse"
}
