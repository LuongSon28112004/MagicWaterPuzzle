Shader "Custom/GlassImproved"
{
    Properties
    {
        _Color("Glass Tint Color", Color) = (0.5, 0.6, 1, 0.2)
        _Smoothness("Smoothness", Range(0,1)) = 0.95
        _Metallic("Metallic", Range(0,1)) = 0.05
        _Transparency("Transparency", Range(0,1)) = 0.15

        // Giúp tạo hiệu ứng viền sáng – giống Fresnel
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
            // Fresnel cơ bản – tạo viền sáng kiểu thủy tinh
            float fresnel = 1.0 - saturate(dot(normalize(IN.viewDir), o.Normal));
            fresnel = pow(fresnel, 3) * _EdgeBrightness;

            // Base color + Fresnel tăng sáng viền
            float3 finalColor = _Color.rgb;
            finalColor += fresnel;

            o.Albedo = finalColor;
            o.Metallic = _Metallic;
            o.Smoothness = _Smoothness;

            // Đảm bảo độ trong suốt
            o.Alpha = (1.0 - _Transparency) * _Color.a;
        }
        ENDCG
    }

    FallBack "Transparent/Diffuse"
}
