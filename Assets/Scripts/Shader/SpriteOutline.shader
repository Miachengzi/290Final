Shader "Custom/SpriteOutline"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        _OutlineColor ("Outline Color", Color) = (1,0.627,0.627,1)
        //_OutlineSize ("Outline Size", Range(0, 100)) = 3
        _OutlineSize ("Outline Size", Range(0, 0.2)) = 0.05
        
        //_OutlineThickness ("Outline Thickness", Range(0, 0.2)) = 0.05
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        Blend One OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                fixed4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            fixed4 _Color;
            fixed4 _OutlineColor;
            float _OutlineSize;
            sampler2D _MainTex;

            v2f vert(appdata_t IN)
            {
                v2f OUT;
                OUT.vertex = UnityObjectToClipPos(IN.vertex);
                OUT.texcoord = IN.texcoord;
                OUT.color = IN.color * _Color;
                return OUT;
            }

            // 在原有Shader基础上添加多级检测
            fixed4 frag(v2f IN) : SV_Target
            {
                fixed4 c = tex2D(_MainTex, IN.texcoord) * IN.color;
                float pixelSize = _OutlineSize / 300;
                
                // 第一级检测（近距离）
                float outline = 0;
                for(int i = -1; i <= 1; i++)
                {
                    for(int j = -1; j <= 1; j++)
                    {
                        if(i == 0 && j == 0) continue;
                        float alpha = tex2D(_MainTex, IN.texcoord + float2(i, j) * pixelSize).a;
                        outline = outline > alpha ? outline : alpha;
                    }
                }
                
                // 第二级检测（远距离）
                if(outline < 0.5)
                {
                    for(int i = -2; i <= 2; i++)
                    {
                        for(int j = -2; j <= 2; j++)
                        {
                            if(abs(i) < 2 && abs(j) < 2) continue;
                            float alpha = tex2D(_MainTex, IN.texcoord + float2(i, j) * pixelSize * 1.5).a;
                            outline = outline > alpha ? outline : alpha;
                        }
                    }
                }
                
                if (c.a < 0.5 && outline > 0.5)
                {
                    return _OutlineColor;
                }
                
                c.rgb *= c.a;
                return c;
            }
            ENDCG
        }
    }
}