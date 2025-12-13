Shader "NAMTRANGLINH/3D/Modules_RGB"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}

        _R ("Red",   Range(0,1)) = 1
        _G ("Green", Range(0,1)) = 1
        _B ("Blue",  Range(0,1)) = 1
        _A ("Alpha", Range(0,1)) = 1
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Standard
        #pragma target 3.0

        sampler2D _MainTex;
        float _R, _G, _B, _A;

        struct Input
        {
            float2 uv_MainTex;
        };

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            fixed4 tex = tex2D(_MainTex, IN.uv_MainTex);

            fixed4 color = fixed4(_R, _G, _B, _A);

            fixed4 finalColor = tex * color;

            o.Albedo = finalColor.rgb;
            o.Alpha  = finalColor.a;
        }
        ENDCG
    }
}
