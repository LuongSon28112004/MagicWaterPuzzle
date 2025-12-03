Shader "Custom/ProceduralWaterfall"
{
    Properties
    {
        _Tint ("Water Tint", Color) = (0.4, 0.6, 1, 1)
        _FlowSpeed ("Flow Speed", Float) = 1.5
        _NoiseScale ("Noise Scale", Float) = 5
        _Distortion ("Distortion", Range(0,1)) = 0.3

        _FresnelPower ("Fresnel Power", Range(1,8)) = 3
        _FresnelStrength ("Fresnel Strength", Range(0,1)) = 0.4
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite On


        CGPROGRAM
        #pragma surface surf Standard alpha

        fixed4 _Tint;
        float _FlowSpeed;
        float _NoiseScale;
        float _Distortion;

        float _FresnelPower;
        float _FresnelStrength;

        // ====== Noise Function (Không cần texture) ======
        float hash(float2 p)
        {
            return frac(sin(dot(p, float2(127.1, 311.7))) * 43758.5453);
        }

        float noise(float2 p)
        {
            float2 i = floor(p);
            float2 f = frac(p);

            float a = hash(i);
            float b = hash(i + float2(1,0));
            float c = hash(i + float2(0,1));
            float d = hash(i + float2(1,1));

            float2 u = f * f * (3 - 2*f);

            return lerp(lerp(a, b, u.x), lerp(c, d, u.x), u.y);
        }

        // =================================================

        struct Input 
        {
            float2 uv_MainTex;
            float3 viewDir;
        };

        void surf(Input IN, inout SurfaceOutputStandard o)
        {
            float2 uv = IN.uv_MainTex;
            
            // Tạo hiệu ứng nước chảy xuống
            float2 uvFlow = float2(
                uv.x + noise(uv * _NoiseScale + _Time.y * 0.3) * _Distortion,
                uv.y + _Time.y * _FlowSpeed
            );

            float n = noise(uvFlow * _NoiseScale);

            // Tạo "sọc" nước mạnh giống thác đổ
            float stripes = smoothstep(0.4, 1.0, frac(uvFlow.y * 4 + n * 0.5));

            fixed3 color = _Tint.rgb;
            color *= lerp(0.6, 1.5, stripes);

            // Fresnel
            float fres = pow(1 - saturate(dot(normalize(IN.viewDir), float3(0,0,-1))), _FresnelPower);
            color += fres * _FresnelStrength;

            // Đẩy sáng nhẹ để giống nước
            color = pow(color, 0.9);

            o.Albedo = color;
            o.Alpha = 1;
        }
        ENDCG
    }
}
