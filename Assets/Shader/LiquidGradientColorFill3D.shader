Shader "Custom/Liquid_NoUV_Smooth_PourRipple_Fixed"
{
Properties
{
_TopColor ("Top Color", Color) = (0.2, 0.6, 1, 1)
_BottomColor ("Bottom Color", Color) = (0, 0.25, 0.6, 1)

    _FillAmount ("Fill Amount (0-1)", Range(0,1)) = 0.5

    _WaveSpeed ("Wave Speed", Float) = 1
    _WaveStrength ("Wave Strength", Float) = 0.05

    _PourForce ("Pour Force", Range(0,1)) = 0
    _PourPosX ("Pour Position X", Float) = 0
    _PourPosZ ("Pour Position Z", Float) = 0
    _RippleStrength ("Ripple Strength", Float) = 0.05

    _MinHeight ("Mesh Min Y", Float) = -1
    _MaxHeight ("Mesh Max Y", Float) = 1
}

SubShader
{
    Tags { "Queue"="Transparent" "RenderType"="Transparent" }
    Blend SrcAlpha OneMinusSrcAlpha

    Pass
    {
        Cull Off
        ZWrite Off

        CGPROGRAM
        #pragma vertex vert
        #pragma fragment frag
        #include "UnityCG.cginc"

        struct appdata
        {
            float4 vertex : POSITION;
        };

        struct v2f
        {
            float4 pos : SV_POSITION;
            float3 worldPos : TEXCOORD0;
        };

        float _FillAmount;
        float _WaveSpeed;
        float _WaveStrength;

        float _PourForce;
        float _RippleStrength;
        float _PourPosX;
        float _PourPosZ;

        float _MinHeight;
        float _MaxHeight;

        fixed4 _TopColor;
        fixed4 _BottomColor;

        v2f vert(appdata v)
        {
            v2f o;
            o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
            o.pos = UnityObjectToClipPos(v.vertex);
            return o;
        }

        float4 frag(v2f i) : SV_Target
        {
            float t = _Time.y;
            float2 uv = i.worldPos.xz;

            // Base gentle water waves
            float baseWave =
                sin(uv.x * 3 + t * _WaveSpeed) * 0.5 +
                sin(uv.y * 4 + t * _WaveSpeed * 1.2) * 0.5;

            baseWave *= _WaveStrength;

            // Ripple from pouring point
            float2 pourPos = float2(_PourPosX, _PourPosZ);
            float dist = distance(uv, pourPos);

            float ripple =
                sin(dist * 25 - t * 18) *
                exp(-dist * 3) *
                _PourForce *
                _RippleStrength;

            // Final water height
            float surface = _FillAmount + baseWave + ripple;

            //-------------------------------------------------------
            // Normalize worldPos.y to 0..1 based on mesh bounds
            //-------------------------------------------------------
            float height01 = (i.worldPos.y - _MinHeight) / (_MaxHeight - _MinHeight);
            height01 = saturate(height01);

            //-------------------------------------------------------
            // Fill masking
            //-------------------------------------------------------
            float mask = 1 - smoothstep(surface - 0.02, surface + 0.02, height01);

            //-------------------------------------------------------
            // Vertical color gradient
            //-------------------------------------------------------
            float4 col = lerp(_BottomColor, _TopColor, height01);
            col.a *= mask;

            return col;
        }

        ENDCG
    }
}
FallBack Off

}




