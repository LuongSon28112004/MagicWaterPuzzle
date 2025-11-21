Shader "Custom/Glass_Builtin_Grab"
{
    Properties
    {
        _MainTex ("Albedo", 2D) = "white" {}
        _NormalMap ("Normal Map", 2D) = "bump" {}
        _Tint ("Tint", Color) = (0.8,0.7,0.6,0.25)
        _RimColor ("Rim Color", Color) = (1,1,1,0.15)
        _RimPower ("Rim Power", Range(1,8)) = 5
        _Gloss ("Gloss", Range(0,1)) = 0.45
        _RefractionStrength ("Refraction Strength", Range(0,0.08)) = 0.025
        _BumpScale ("Bump Scale", Range(0,2)) = 1
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 300
        
        GrabPass { "_GrabTex" }

        // Main Surface Shader --------------------------------------
        CGPROGRAM
        #pragma surface surf Standard alpha:fade
        #pragma target 3.0

        sampler2D _MainTex;
        sampler2D _NormalMap;
        fixed4 _Tint;
        fixed4 _RimColor;
        half _Gloss;
        float _RimPower;
        float _BumpScale;

        struct Input
        {
            float2 uv_MainTex;
            float2 uv_NormalMap;
            float3 viewDir;
        };

        void surf(Input IN, inout SurfaceOutputStandard o)
        {
            fixed4 albedo = tex2D(_MainTex, IN.uv_MainTex);
            o.Albedo = albedo.rgb * _Tint.rgb;

            fixed3 n = UnpackNormal(tex2D(_NormalMap, IN.uv_NormalMap));
            n.xy *= _BumpScale;
            o.Normal = n;

            o.Smoothness = _Gloss;
            o.Metallic = 0;

            float rim = pow(1 - saturate(dot(normalize(IN.viewDir), o.Normal)), _RimPower);
            o.Emission = _RimColor.rgb * rim;

            o.Alpha = _Tint.a * (0.8 + rim * 0.2);
        }
        ENDCG

        // Refraction Pass -----------------------------------------
        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _GrabTex;
            sampler2D _NormalMap;
            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _NormalMap_ST;
            float _RefractionStrength;
            float _BumpScale;
            float4 _Tint;

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float2 uvN : TEXCOORD1;
                float4 screenPos : TEXCOORD2;
            };

            v2f vert(appdata_full v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                o.uvN = TRANSFORM_TEX(v.texcoord, _NormalMap);
                o.screenPos = ComputeScreenPos(o.pos);
                return o;
            }

            fixed4 frag(v2f IN) : SV_Target
            {
                float2 uvScreen = IN.screenPos.xy / IN.screenPos.w;

                float3 n = tex2D(_NormalMap, IN.uvN).xyz * 2 - 1;
                n.xy *= _BumpScale;

                float2 offset = n.xy * _RefractionStrength;
                float2 uvRefract = saturate(uvScreen + offset);

                fixed4 refractCol = tex2D(_GrabTex, uvRefract);
                fixed4 baseCol   = tex2D(_MainTex, IN.uv) * _Tint;

                return lerp(refractCol, baseCol, 0.35);
            }
            ENDCG
        }
    }

    FallBack "Transparent/Diffuse"
}
