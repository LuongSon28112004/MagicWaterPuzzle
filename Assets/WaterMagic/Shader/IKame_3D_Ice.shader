Shader "IKame/3D/Ice_Improved"
{
    Properties
    {
        _Color ("Tint Color", Color) = (0.7, 0.9, 1, 1)
        _MainTex ("Base Texture", 2D) = "white" {}
        [NoScaleOffset] _MatCap ("MatCap (RGB)", 2D) = "white" {}

        _FresnelColor ("Fresnel Color", Color) = (0.5, 0.8, 1, 1)
        _FresnelPower ("Fresnel Power", Range(0.1, 10)) = 3

        _Smoothness ("Smoothness", Range(0,1)) = 0.85
        _Metallic ("Metallic", Range(0,1)) = 0.1
        _Transparency ("Transparency", Range(0,1)) = 0.3
    }

    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 300
        Blend SrcAlpha OneMinusSrcAlpha

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows alpha:fade
        #pragma target 3.0

        sampler2D _MainTex;
        sampler2D _MatCap;

        fixed4 _Color;

        fixed4 _FresnelColor;
        float _FresnelPower;

        float _Smoothness;
        float _Metallic;
        float _Transparency;

        struct Input
        {
            float2 uv_MainTex;
            float3 viewDir;
            float3 worldNormal;
        };

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            fixed4 tex = tex2D(_MainTex, IN.uv_MainTex) * _Color;

            // Fresnel
            float fresnel = pow(1 - saturate(dot(IN.viewDir, IN.worldNormal)), _FresnelPower);
            float3 fresnelCol = fresnel * _FresnelColor.rgb;

            // Matcap
            float2 matCapUV = IN.worldNormal.xy * 0.5 + 0.5;
            float3 matcap = tex2D(_MatCap, matCapUV).rgb;

            // Combine
            o.Albedo = tex.rgb + matcap * 0.5 + fresnelCol;

            o.Smoothness = _Smoothness;
            o.Metallic = _Metallic;

            // Ice transparency
            o.Alpha = tex.a * (1 - _Transparency) + fresnel * _Transparency;
        }
        ENDCG
    }
}
