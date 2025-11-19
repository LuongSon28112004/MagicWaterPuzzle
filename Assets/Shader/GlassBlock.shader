Shader "Custom/AlphaBlendTint"
{
    // Các biến (Properties) có thể điều chỉnh trong Material Inspector
    Properties
    {
        _Color ("Tint Color", Color) = (1,1,1,1) // Màu sắc và Alpha (A)
        _MainTex ("Texture (RGB)", 2D) = "white" {} // Texture chính
    }

    SubShader
    {
        // ----------------------------------------------------
        // CÁC THIẾT LẬP RENDER QUAN TRỌNG CHO ĐỘ TRONG SUỐT
        // ----------------------------------------------------
        
        // 1. Tags: Đảm bảo đối tượng được vẽ sau các vật thể đục
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        
        // 2. Blending: Thiết lập công thức pha trộn màu (Alpha Blend)
        // SrcAlpha: Màu nguồn (shader) nhân với giá trị Alpha của chính nó.
        // OneMinusSrcAlpha: Màu đích (màn hình) nhân với (1 - Alpha của nguồn).
        // Công thức: Màu Mới = (Màu Nguồn * Alpha) + (Màu Đích * (1 - Alpha))
        Blend SrcAlpha OneMinusSrcAlpha
        
        // 3. ZWrite: Tắt ghi vào Depth Buffer để cho phép các vật thể phía sau hiện lên
        ZWrite Off
        
        // 4. Culling: Mặc định là Back (Backface Culling)
        Cull Back
        
        // ----------------------------------------------------
        
        Pass
        {
            CGPROGRAM
            
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            // Các struct
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            // Khai báo các biến từ Properties
            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _Color;

            // Hàm Vertex: Tính toán vị trí điểm
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            // Hàm Fragment: Tính toán màu sắc pixel
            fixed4 frag (v2f i) : SV_Target
            {
                // Lấy màu từ Texture
                fixed4 texColor = tex2D(_MainTex, i.uv);
                
                // Phối màu (Tint) bằng cách nhân màu Texture với màu _Color của Material
                fixed4 finalColor = texColor * _Color;
                
                // Giá trị Alpha (finalColor.a) sẽ được dùng trong công thức Blend ở trên
                // để xác định mức độ trong suốt.
                return finalColor;
            }
            
            ENDCG
        }
    }
    // Thiết lập Fallback nếu SubShader trên không hoạt động
    FallBack "Diffuse"
}