Shader "NAMTRANGLINH/9C/Modules_Main"
{
    Properties
    {
        _Color("Main Color", Color) = (1, 0.29, 0.70, 1)
        _Smoothness("Smoothness", Range(0,1)) = 0.6
        _Metallic("Metallic", Range(0,1)) = 0.0
    }

    SubShader
    {
        Tags 
        { 
            "RenderType"="Opaque"
            "Queue"="Geometry"
        }

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows
        #pragma target 3.0

        fixed4 _Color;
        float _Smoothness;
        float _Metallic;

        struct Input
        {
            float dummy;
        };

        void surf(Input IN, inout SurfaceOutputStandard o)
        {
            o.Albedo = _Color.rgb;
            o.Smoothness = _Smoothness;
            o.Metallic = _Metallic;
            o.Alpha = _Color.a;
        }
        ENDCG
    }

    FallBack "Diffuse"
}
