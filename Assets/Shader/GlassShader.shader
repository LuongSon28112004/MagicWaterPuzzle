Shader "Custom/GlassWaterPipe_BuiltIn"
{
    Properties
    {
        _GlassColor ("Glass Color", Color) = (1,1,1,0.25)
        _SpecStrength ("Specular Strength", Range(0,0.3)) = 0.1
        _Gloss ("Gloss", Range(0,1)) = 0.85

        _FresnelPower ("Fresnel Power", Range(1,6)) = 4
        _FresnelIntensity ("Fresnel Intensity", Range(0,1)) = 0.5
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
        Cull Back

        CGPROGRAM
        #pragma surface surf BlinnPhong alpha:fade

        struct Input
        {
            float3 viewDir;
        };

        fixed4 _GlassColor;
        float _SpecStrength;
        float _Gloss;
        float _FresnelPower;
        float _FresnelIntensity;

        void surf (Input IN, inout SurfaceOutput o)
        {
            // ===== Màu kính =====
            o.Albedo = _GlassColor.rgb;

            // ===== Specular rất nhẹ (KHÔNG phản chiếu ảnh) =====
            o.Specular = _SpecStrength;
            o.Gloss = _Gloss;

            // ===== Fresnel viền kính =====
            float fresnel = pow(
                1 - saturate(dot(normalize(IN.viewDir), o.Normal)),
                _FresnelPower
            );

            // ===== Độ trong suốt =====
            o.Alpha = saturate(
                _GlassColor.a + fresnel * _FresnelIntensity
            );
        }
        ENDCG
    }

    FallBack "Transparent/VertexLit"
}
