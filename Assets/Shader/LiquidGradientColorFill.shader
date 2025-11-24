Shader "Custom/MultiLayerLiquid_Stacked_8"
{
Properties
{
_LayerCount ("Number of Layers", Range(1,8)) = 3

    _Color1 ("Layer 1 Color (Top)", Color) = (1,0,0,1)
    _Fill1  ("Layer 1 Fill", Range(0,1)) = 0.2

    _Color2 ("Layer 2 Color", Color) = (0,1,0,1)
    _Fill2  ("Layer 2 Fill", Range(0,1)) = 0.2

    _Color3 ("Layer 3 Color", Color) = (0,0,1,1)
    _Fill3  ("Layer 3 Fill", Range(0,1)) = 0.2

    _Color4 ("Layer 4 Color", Color) = (1,1,0,1)
    _Fill4  ("Layer 4 Fill", Range(0,1)) = 0.2

    _Color5 ("Layer 5 Color", Color) = (1,0,1,1)
    _Fill5  ("Layer 5 Fill", Range(0,1)) = 0.2

    _Color6 ("Layer 6 Color", Color) = (0,1,1,1)
    _Fill6  ("Layer 6 Fill", Range(0,1)) = 0.2

    _Color7 ("Layer 7 Color", Color) = (1,0.5,0,1)
    _Fill7  ("Layer 7 Fill", Range(0,1)) = 0.2

    _Color8 ("Layer 8 Color", Color) = (0.3,0.3,0.3,1)
    _Fill8  ("Layer 8 Fill", Range(0,1)) = 0.2

    _WaveSpeed ("Wave Speed", Float) = 1
    _WaveStrength ("Wave Strength", Float) = 0.01
    _RippleStrength ("Ripple Strength", Float) = 0.02
    _PourForce ("Pour Force", Range(0,1)) = 0
    _PourXPos ("Pour X Position", Range(0,1)) = 0.5

    _FresnelPower ("Fresnel Power", Float) = 5
    _ShineStrength ("Shine Strength", Float) = 0.4
}

SubShader
{
    Tags { "Queue"="Transparent" "RenderType"="Transparent" }
    Blend SrcAlpha OneMinusSrcAlpha

    Pass
    {
        CGPROGRAM
        #pragma vertex vert
        #pragma fragment frag

        #include "UnityCG.cginc"

        struct appdata {
            float4 vertex : POSITION;
            float2 uv     : TEXCOORD0;
        };

        struct v2f {
            float4 pos     : SV_POSITION;
            float2 uv      : TEXCOORD0;
            float3 viewDir : TEXCOORD1;
        };

        float _LayerCount;

        float4 _Color1, _Color2, _Color3, _Color4;
        float4 _Color5, _Color6, _Color7, _Color8;

        float _Fill1, _Fill2, _Fill3, _Fill4;
        float _Fill5, _Fill6, _Fill7, _Fill8;

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
            float t = _Time.y;

            float wave = sin(uv.x * 20 + t * _WaveSpeed) 
                         * _WaveStrength;

            float dist = abs(uv.x - _PourXPos);

            float ripple = sin(uv.x * 40 - t * 16)
                           * exp(-dist * 10)
                           * _PourForce
                           * _RippleStrength;

            float offset = wave + ripple;

            float y = uv.y - offset;

            float fills[8] = {
                _Fill1, _Fill2, _Fill3, _Fill4,
                _Fill5, _Fill6, _Fill7, _Fill8
            };

            float4 colors[8] = {
                _Color1, _Color2, _Color3, _Color4,
                _Color5, _Color6, _Color7, _Color8
            };

            float top = 1;

            for (int L = 0; L < _LayerCount; L++)
            {
                float nextTop = top - fills[L];

                if (y <= top && y > nextTop)
                {
                    float3 V = normalize(i.viewDir);
                    float fres = pow(1 - abs(V.z), _FresnelPower);
                    float4 shine = fres * _ShineStrength;
                    return colors[L] + shine;
                }

                top = nextTop;
            }

            return float4(0,0,0,0);
        }
        ENDCG
    }
}

}


// cách chỉnh sửa.
// [Range(0,8)]
// public int layerCount = 3;

// public Color[] colors = new Color[8];
// [Range(0,1)]
// public float[] fills = new float[8];

// void ApplyLiquidSettings()
// {
//     liquidMat.SetFloat("_LayerCount", layerCount);

//     for (int i = 0; i < layerCount; i++)
//     {
//         liquidMat.SetColor("_Color" + (i+1), colors[i]);
//         liquidMat.SetFloat("_Fill" + (i+1), fills[i]);
//     }
// }

// Các tham số khác
// liquidMat.SetFloat("_WaveSpeed", 1.5f);
// liquidMat.SetFloat("_WaveStrength", 0.02f);
// liquidMat.SetFloat("_RippleStrength", 0.04f);
// liquidMat.SetFloat("_PourForce", 0.5f);
// liquidMat.SetFloat("_PourXPos", 0.6f);

// liquidMat.SetFloat("_FresnelPower", 5);
// liquidMat.SetFloat("_ShineStrength", 0.4f);


// for (int i = 0; i < meshRenderers.Length; i++)
// {
//     var r = meshRenderers[i];
//     if (r == null) continue;

//     r.sharedMaterial = mat;
//     r.GetPropertyBlock(materialPropertyBlock);
//     if (pixelSO != null)
//     {
//         materialPropertyBlock.SetColor("_OutlineColor", pixelSO.outlineColor);
//         materialPropertyBlock.SetFloat("_Outline", 3);
//         materialPropertyBlock.SetFloat("_OutlineWidth", 0f);
//         r.SetPropertyBlock(materialPropertyBlock);
//     }
//     else
//     {
//         materialPropertyBlock.SetFloat("_OutlineWidth", 0f);
//         r.SetPropertyBlock(materialPropertyBlock);
//     }

// } 

