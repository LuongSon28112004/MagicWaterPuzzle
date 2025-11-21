Shader "Custom/BlueGelTransparent"
{
    Properties
    {
        _Color("Main Color", Color) = (0.2, 0.4, 1, 0.3)   // alpha quan trọng
        _EdgeColor("Edge Highlight", Color) = (0.4, 0.6, 1, 1)
        _ShineIntensity("Shine Intensity", Range(0, 1)) = 0.45
        _Smooth("Smoothness", Range(0,1)) = 0.5
        _Transparency("Transparency", Range(0,1)) = 0.3
    }

    SubShader
    {
        Tags { 
            "RenderType"="Transparent"
            "Queue"="Transparent"
        }

        LOD 200
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off  

        CGPROGRAM
        #pragma surface surf Standard alpha:fade fullforwardshadows

        struct Input
        {
            float3 viewDir;
        };

        fixed4 _Color;
        fixed4 _EdgeColor;
        float _ShineIntensity;
        float _Smooth;
        float _Transparency;

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            o.Albedo = _Color.rgb;

            float edge = saturate(dot(normalize(IN.viewDir), float3(0,0,1)));
            edge = pow(edge, 6);

            o.Albedo = lerp(_EdgeColor.rgb, o.Albedo, edge);

            o.Smoothness = _Smooth;
            o.Metallic = _ShineIntensity;

            o.Alpha = _Color.a * (1.0 - _Transparency);
        }
        ENDCG
    }

    FallBack "Transparent/Diffuse"
}
