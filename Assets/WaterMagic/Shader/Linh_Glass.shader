Shader "Linh/FrostedGlassStrongSnow"
{
    Properties
    {
        [Header(Frost Settings)]
        _FrostTex("Frost Noise Texture", 2D) = "white" {}
        _FrostIntensity("Frost Intensity", Range(0, 1)) = 0.8
        _FrostBlur("Frost Blur Strength", Range(0, 1)) = 0.45
        _FrostColor("Frost Color", Color) = (0.92, 0.96, 1, 1)

        [Header(Snow Emission)]
        _SnowGlowIntensity("Snow Glow Intensity", Range(0, 3)) = 1.4

        [Header(Matcap Settings)] 
        _MatcapTex("Matcap Texture", 2D) = "white" {}

        _Texture("Base Texture", 2D) = "white" {}
        _Brightness("Brightness", Float) = 1
    }

    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 300
        Blend SrcAlpha OneMinusSrcAlpha

        CGPROGRAM
        #pragma surface surf Standard alpha:fade emission
        #pragma target 3.0

        sampler2D _MatcapTex;
        sampler2D _Texture;
        sampler2D _FrostTex;

        float _FrostIntensity;
        float _FrostBlur;
        float4 _FrostColor;
        float _Brightness;
        float _SnowGlowIntensity;

        struct Input
        {
            float2 uv_Texture;
            float2 uv_FrostTex;
            float3 viewDir;
            float3 worldNormal;
        };

        half4 Matcap(float3 normal, float3 viewDir)
        {
            float3 r = reflect(viewDir, normal);
            float2 uv = r.xy * 0.5 + 0.5;
            return tex2D(_MatcapTex, uv);
        }

        void surf(Input IN, inout SurfaceOutputStandard o)
        {
            float4 baseCol = tex2D(_Texture, IN.uv_Texture) * _Brightness;

            float frost = tex2D(_FrostTex, IN.uv_FrostTex * (1 + _FrostBlur * 5)).r;
            float frostAmount = frost * _FrostIntensity;

            float edge = 1 - saturate(dot(normalize(IN.viewDir), normalize(IN.worldNormal)));
            edge = pow(edge, 4);
            frostAmount += edge * 0.7;

            float4 snowHighlight = float4(0.7, 0.82, 1, 1);
            float4 frozenColor = lerp(baseCol, _FrostColor, frostAmount);
            frozenColor = lerp(frozenColor, snowHighlight, frostAmount * 0.3);

            float4 mc = Matcap(normalize(IN.worldNormal), normalize(IN.viewDir));
            frozenColor = lerp(frozenColor, mc, frostAmount * 0.2);

            frozenColor.rgb += frostAmount * 0.15;

            o.Albedo = frozenColor.rgb;
            o.Smoothness = 0.02;
            o.Metallic = 0;
            o.Alpha = 1 - frostAmount * 0.5;

            // EMISSION – tuyết phát sáng
            float3 emissionColor = (_FrostColor.rgb + float3(0.15, 0.25, 0.5)) * frostAmount * _SnowGlowIntensity;
            o.Emission = emissionColor;
        }
        ENDCG
    }
}
