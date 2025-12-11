Shader "KIM/IceRockBreak"
{
    Properties
    {
        _MainTex ("Base Texture", 2D) = "white" {}
        _MatCap ("MatCap", 2D) = "white" {}
        _CrackTex ("Crack Mask", 2D) = "black" {}
        _BreakAmount ("Break Amount", Range(0,1)) = 0
        _BreakStrength ("Break Strength", Range(0,1)) = 0.5
        _NoiseScale ("Noise Scale", Float) = 5
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 300

        CGPROGRAM
        #pragma surface surf Standard vertex:vert
        #pragma target 3.0

        sampler2D _MainTex;
        sampler2D _MatCap;
        sampler2D _CrackTex;

        float _BreakAmount;
        float _BreakStrength;
        float _NoiseScale;

        struct Input
        {
            float2 uv_MainTex;
            float3 viewDir;
            float3 worldNormal;
        };

        // Simple noise
        float hash(float3 p)
        {
            return frac(sin(dot(p, float3(12.9898,78.233,37.719))) * 43758.5453);
        }

        float noise(float3 p)
        {
            float3 i = floor(p);
            float3 f = frac(p);

            f = f * f * (3 - 2 * f);

            float n =
                lerp(
                    lerp(hash(i + float3(0,0,0)), hash(i + float3(1,0,0)), f.x),
                    lerp(hash(i + float3(0,1,0)), hash(i + float3(1,1,0)), f.x),
                f.y);

            return n;
        }

        // VERTEX – đẩy model vỡ ra
        void vert(inout appdata_full v)
        {
            float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;

            float n = noise(worldPos * _NoiseScale);

            float breakMask = saturate((n - (1 - _BreakAmount)));

            float3 dir = normalize(v.normal);
            v.vertex.xyz += dir * breakMask * _BreakStrength;
        }

        // SURFACE
        void surf(Input IN, inout SurfaceOutputStandard o)
        {
            float4 baseCol = tex2D(_MainTex, IN.uv_MainTex);
            float crack = tex2D(_CrackTex, IN.uv_MainTex).r;

            // Áp matcap
            float2 matcapUV = IN.worldNormal.xy * 0.5 + 0.5;
            float4 mc = tex2D(_MatCap, matcapUV);

            // Trộn matcap + texture
            o.Albedo = baseCol.rgb * mc.rgb;

            // Crack làm sáng vết nứt băng
            o.Emission = crack * _BreakAmount * 3;

            // Từ từ trong suốt ở đường nứt
            o.Alpha = saturate(baseCol.a - crack * _BreakAmount * 0.6);
        }

        ENDCG
    }
}
