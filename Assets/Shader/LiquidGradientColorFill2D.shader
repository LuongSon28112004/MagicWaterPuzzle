Shader "Custom/LiquidPour2D_Full_Fixed"
{
    Properties
    {
        _TopColor       ("Top Color", Color) = (0.2, 0.6, 1, 1)
        _BottomColor    ("Bottom Color", Color) = (0, 0.25, 0.6, 1)

        _FillAmount     ("Fill Amount (0-1)", Range(0,1)) = 0.5

        _WaveSpeed      ("Wave Speed", Float) = 1
        _WaveStrength   ("Wave Strength", Float) = 0.01

        _RippleStrength ("Ripple Strength", Float) = 0.02
        _PourForce      ("Pour Force (0-1)", Range(0,1)) = 0
        _PourXPos       ("Pour Position X (UV 0-1)", Range(0,1)) = 0.5

        _FresnelPower   ("Fresnel Power", Float) = 5
        _ShineStrength  ("Shine Strength", Float) = 0.4
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex   vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv     : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos     : SV_POSITION;
                float2 uv      : TEXCOORD0;
                float3 viewDir : TEXCOORD1;
            };

            float4 _TopColor;
            float4 _BottomColor;

            float _FillAmount;

            float _WaveSpeed;
            float _WaveStrength;

            float _RippleStrength;
            float _PourForce;
            float _PourXPos;

            float _FresnelPower;
            float _ShineStrength;


            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv  = v.uv;

                float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.viewDir = normalize(_WorldSpaceCameraPos - worldPos);

                return o;
            }


            float4 frag(v2f i) : SV_Target
            {
                float2 uv = i.uv;
                float t   = _Time.y;

                // 1. Base water line
                float surface = _FillAmount;

                // 2. Standing waves
                float wave = sin(uv.x * 25.0 + t * _WaveSpeed)
                           * _WaveStrength;

                // 3. Ripple from pouring
                float dist   = abs(uv.x - _PourXPos);
                float spread = exp(-dist * 12.0);

                float ripple = sin(uv.x * 45.0 - t * 15.0)
                             * spread
                             * _PourForce
                             * _RippleStrength;

                // Final water surface
                surface += wave + ripple;

                // If pixel is above surface → transparent
                if (uv.y > surface)
                    return float4(0,0,0,0);

                //------------------------------------------------
                // 4. REPAIRED:
                // Height inside the water (0 bottom → 1 surface)
                //------------------------------------------------
                float inside = saturate( uv.y / surface );

                //------------------------------------------------
                // 5. Gradient based on actual liquid height
                //------------------------------------------------
                float4 waterColor = lerp(_BottomColor, _TopColor, inside);

                //------------------------------------------------
                // Fresnel highlight
                //------------------------------------------------
                float fres = pow(1 - abs(i.viewDir.z), _FresnelPower);

                float edge = smoothstep(surface - 0.03, surface + 0.03, uv.y);

                float4 shine = fres * edge * _ShineStrength;

                return waterColor + shine;
            }

            ENDCG
        }
    }
}
