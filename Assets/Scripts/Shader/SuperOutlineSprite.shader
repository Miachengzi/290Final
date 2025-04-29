Shader "Custom/SuperOutlineSprite"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        _OutlineColor ("Outline Color", Color) = (1,0.627,0.627,1) // FFA0A0
        _OutlineThickness ("Outline Thickness", Range(0, 0.2)) = 0.05
    }

    SubShader
    {
        Tags
        { 
            "Queue"="Transparent"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                float2 texcoord : TEXCOORD0;
                float2 texcoord2[8] : TEXCOORD1;
            };

            sampler2D _MainTex;
            float4 _MainTex_TexelSize;
            fixed4 _Color;
            fixed4 _OutlineColor;
            float _OutlineThickness;

            v2f vert(appdata_t IN)
            {
                v2f OUT;
                OUT.vertex = UnityObjectToClipPos(IN.vertex);
                OUT.texcoord = IN.texcoord;
                
                // 预计算8个方向的采样坐标
                float2 pixelSize = _MainTex_TexelSize.xy * _OutlineThickness * 100;
                OUT.texcoord2[0] = IN.texcoord + float2(0, 1) * pixelSize;
                OUT.texcoord2[1] = IN.texcoord + float2(0, -1) * pixelSize;
                OUT.texcoord2[2] = IN.texcoord + float2(1, 0) * pixelSize;
                OUT.texcoord2[3] = IN.texcoord + float2(-1, 0) * pixelSize;
                OUT.texcoord2[4] = IN.texcoord + float2(0.7, 0.7) * pixelSize;
                OUT.texcoord2[5] = IN.texcoord + float2(-0.7, 0.7) * pixelSize;
                OUT.texcoord2[6] = IN.texcoord + float2(0.7, -0.7) * pixelSize;
                OUT.texcoord2[7] = IN.texcoord + float2(-0.7, -0.7) * pixelSize;
                
                return OUT;
            }

            fixed4 frag(v2f IN) : SV_Target
            {
                fixed4 mainColor = tex2D(_MainTex, IN.texcoord) * _Color;
                
                // 如果主纹理有颜色就直接返回
                if(mainColor.a > 0.1)
                    return mainColor;
                
                // 检查8个方向的像素
                for(int i = 0; i < 8; i++)
                {
                    if(tex2D(_MainTex, IN.texcoord2[i]).a > 0.1)
                    {
                        return _OutlineColor;
                    }
                }
                
                return fixed4(0,0,0,0);
            }
            ENDCG
        }
    }
}