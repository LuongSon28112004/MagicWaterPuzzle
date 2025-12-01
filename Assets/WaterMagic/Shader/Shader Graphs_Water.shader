Shader "Custom/BetterFlowWater"
{
    Properties
    {
        _MainTex ("Water Texture", 2D) = "white" {}
        _NormalMap ("Normal Map", 2D) = "bump" {}
        
        _FlowSpeed1 ("Flow Speed Layer 1", Float) = 0.5
        _FlowSpeed2 ("Flow Speed Layer 2", Float) = -0.3
        
        _Tint ("Color Tint", Color) = (1,1,1,1)
        _NormalStrength ("Normal Strength", Range(0,2)) = 1

        _MinBrightness ("Minimum Brightness", Range(0, 1)) = 0.15
        
        _FresnelPower ("Fresnel Power", Range(0.5, 6)) = 3
        _FresnelStrength ("Fresnel Strength", Range(0,1)) = 0.4
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
        float _NormalStrength;
        float _MinBrightness;

        fixed4 _Tint;

        float _FresnelPower;
        float _FresnelStrength;

        struct Input 
        {
            float2 uv_MainTex;
            float2 uv_NormalMap;
            float3 viewDir;
        };

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            float2 uv1 = IN.uv_MainTex;
            float2 uv2 = IN.uv_MainTex;

            // 2 lớp UV chạy khác hướng
            uv1.y += _Time.y * _FlowSpeed1;
            uv2.y += _Time.y * _FlowSpeed2;

            // Trộn texture
            fixed4 c1 = tex2D(_MainTex, uv1);
            fixed4 c2 = tex2D(_MainTex, uv2);
            fixed4 c = lerp(c1, c2, 0.5) * _Tint;

            // Làm bớt tối
            c.rgb = max(c.rgb, _MinBrightness);

            // Tăng sáng nhẹ
            c.rgb = pow(c.rgb, 0.9);

            // Normal map
            fixed3 normalTex = UnpackNormal(tex2D(_NormalMap, uv1)) * _NormalStrength;
            o.Normal = normalTex;

            // Fresnel
            float fresnel = pow(1 - saturate(dot(normalize(IN.viewDir), o.Normal)), _FresnelPower);
            fresnel *= _FresnelStrength;

            // Tổng hợp màu cuối
            o.Albedo = c.rgb + fresnel * 0.5;
            o.Alpha = c.a;
        }
        ENDCG
    }
}
