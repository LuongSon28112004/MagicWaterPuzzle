Shader "Custom/BetterFlowWater_Upgraded"
{
    Properties
    {
        _MainTex ("Water Texture", 2D) = "white" {}
        _NormalMap ("Normal Map", 2D) = "bump" {}
        
        _FlowSpeed1 ("Flow Speed Layer 1", Float) = 1.2
        _FlowSpeed2 ("Flow Speed Layer 2", Float) = -0.6

        _WarpStrength ("UV Warp Strength", Range(0,0.1)) = 0.03

        _Tint ("Color Tint", Color) = (0.7,0.9,1,1)
        _NormalStrength ("Normal Strength", Range(0,2)) = 1

        _MinBrightness ("Minimum Brightness", Range(0, 1)) = 0.15
        
        _FresnelPower ("Fresnel Power", Range(0.5, 6)) = 3
        _FresnelStrength ("Fresnel Strength", Range(0,1)) = 0.4

        _FoamStrength ("Foam Strength", Range(0,1)) = 0.4
    }

    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha

        CGPROGRAM
        #pragma surface surf Standard alpha

        sampler2D _MainTex;
        sampler2D _NormalMap;

        float _FlowSpeed1;
        float _FlowSpeed2;
        float _WarpStrength;

        float _NormalStrength;
        float _MinBrightness;

        fixed4 _Tint;

        float _FresnelPower;
        float _FresnelStrength;

        float _FoamStrength;

        struct Input 
        {
            float2 uv_MainTex;
            float2 uv_NormalMap;
            float3 viewDir;
        };

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            float2 uv = IN.uv_MainTex;

            // ---- UV warp (distortion theo normal map) ----
            float2 warp = (tex2D(_NormalMap, uv * 2).rg - 0.5) * _WarpStrength;
            uv += warp;

            // Hai lớp texture chảy khác hướng nhau
            float2 uv1 = uv;
            float2 uv2 = uv;

            uv1.y += _Time.y * _FlowSpeed1;
            uv2.y += _Time.y * _FlowSpeed2;

            // Trộn 2 layer texture tạo cảm giác nước chuyển động dày
            fixed4 c1 = tex2D(_MainTex, uv1);
            fixed4 c2 = tex2D(_MainTex, uv2);
            fixed4 color = lerp(c1, c2, 0.5) * _Tint;

            // Giữ độ sáng tối thiểu
            color.rgb = max(color.rgb, _MinBrightness);

            // Làm sáng nhẹ (giữ nước trong)
            color.rgb = pow(color.rgb, 0.85);

            // Normal map
            fixed3 normalTex = UnpackNormal(tex2D(_NormalMap, uv1)) * _NormalStrength;
            o.Normal = normalTex;

            // Fresnel
            float fresnel = pow(1 - saturate(dot(normalize(IN.viewDir), o.Normal)), _FresnelPower);
            fresnel *= _FresnelStrength;

            // ---- Foam (bọt nước) khi tốc độ cao ----
            float foam = saturate((c1.r + c2.r) * 0.5 * 2);
            foam = pow(foam, 3) * _FoamStrength;

            // Hòa màu tổng hợp
            o.Albedo = color.rgb + fresnel * 0.5 + foam;
            o.Alpha = color.a;
        }
        ENDCG
    }
}
