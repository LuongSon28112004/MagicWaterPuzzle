Shader "KIM/3D/Block_Stone"
{
    Properties
    {
        _Color ("Stone Color", Color) = (0.75, 0.75, 0.75, 1)
        _MainTex ("Stone Texture", 2D) = "white" {}
        _NormalMap ("Normal Map", 2D) = "bump" {}

        _Roughness ("Roughness", Range(0,1)) = 0.8
        _Metallic ("Metallic", Range(0,1)) = 0.0

        _TextureStrength ("Texture Strength", Range(0,1)) = 0.5
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 300

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows
        #pragma target 3.0

        sampler2D _MainTex;
        sampler2D _NormalMap;

        fixed4 _Color;
        float _Roughness;
        float _Metallic;
        float _TextureStrength;

        struct Input
        {
            float2 uv_MainTex;
        };

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            fixed4 tex = tex2D(_MainTex, IN.uv_MainTex);

            // Trộn màu đá + texture
            fixed3 albedo = lerp(_Color.rgb, _Color.rgb * tex.rgb, _TextureStrength);

            o.Albedo = albedo;
            o.Normal = UnpackNormal(tex2D(_NormalMap, IN.uv_MainTex));

            o.Metallic = _Metallic;
            o.Smoothness = 1.0 - _Roughness;
            o.Alpha = 1;
        }
        ENDCG
    }

    FallBack "Standard"
}
