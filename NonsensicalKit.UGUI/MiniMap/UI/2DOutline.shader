Shader "2DOutline"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}

        _BaseOutlineWidth("OutlineBaseWidth", Range(0,5)) = 1
        _ExpandRangeOut("ExpandRangeOut", Range(0,100)) = 4
        _ExpandRangeIn("ExpandRangeIn", Range(0,100)) = 4
        _PulseSpeed("PulseSpeed", Range(0,10)) = 2

        _LineColor("OutlineColor", Color) = (1,1,1,1)

        _BaseColor("BaseColorTint", Color) = (1,1,1,1)
        _BaseColorStrength("Base Color Strength", Range(0,1)) = 1

        // Unity UI/Mask 标准属性
        [HideInInspector] _StencilComp ("Stencil Comparison", Float) = 8
        [HideInInspector] _Stencil ("Stencil ID", Float) = 0
        [HideInInspector] _StencilOp ("Stencil Operation", Float) = 0
        [HideInInspector] _StencilWriteMask ("Stencil Write Mask", Float) = 255
        [HideInInspector] _StencilReadMask ("Stencil Read Mask", Float) = 255
        [HideInInspector] _ColorMask ("Color Mask", Float) = 15
    }

    SubShader
    {
        Tags {
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }

        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            Stencil
            {
                Ref [_Stencil]
                Comp [_StencilComp]
                Pass [_StencilOp]
                ReadMask [_StencilReadMask]
                WriteMask [_StencilWriteMask]
            }
            
            ColorMask [_ColorMask]

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0
            #pragma only_renderers gles gles3 webgl d3d11 metal vulkan
            #include "UnityCG.cginc"

            // WebGL 强制高精度
            #ifdef GL_ES
            precision highp float;
            precision highp int;
            #endif

            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            // ⭐ 运行时可修改的参数 - 使用 uniform 关键字确保可以动态更新
            uniform float _BaseOutlineWidth;
            uniform float _ExpandRangeOut;
            uniform float _ExpandRangeIn;
            uniform float _PulseSpeed;

            uniform float4 _LineColor;
            uniform float4 _BaseColor;
            uniform float _BaseColorStrength;

            // ⭐ 使用16个方向优化性能，视觉效果依然很好
            static const float2 DIR16[16] = {
                float2(1.000000, 0.000000),
                float2(0.923880, 0.382683),
                float2(0.707107, 0.707107),
                float2(0.382683, 0.923880),
                float2(0.000000, 1.000000),
                float2(-0.382683, 0.923880),
                float2(-0.707107, 0.707107),
                float2(-0.923880, 0.382683),
                float2(-1.000000, 0.000000),
                float2(-0.923880, -0.382683),
                float2(-0.707107, -0.707107),
                float2(-0.382683, -0.923880),
                float2(0.000000, -1.000000),
                float2(0.382683, -0.923880),
                float2(0.707107, -0.707107),
                float2(0.923880, -0.382683)
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float4 col = tex2D(_MainTex, i.uv);

                // BaseColor 混合
                col.rgb = lerp(col.rgb, _BaseColor.rgb, _BaseColorStrength);

                // 脉冲动画
                float anim = sin(_Time.y * _PulseSpeed) * 0.5 + 0.5;

                // 动态扩散半径
                float expandOut = _BaseOutlineWidth + anim * _ExpandRangeOut;
                float expandIn  = _BaseOutlineWidth + anim * _ExpandRangeIn;

                float scale = 0.005;

                float alphaSumOut = 0;
                float alphaSumIn  = 0;

                // ⭐ 完全手动展开循环，确保 WebGL 兼容
                #define SAMPLE_DIR(idx) \
                    alphaSumOut += tex2D(_MainTex, i.uv + DIR16[idx] * expandOut * scale).a; \
                    alphaSumIn  += 1.0 - tex2D(_MainTex, i.uv + DIR16[idx] * expandIn * scale).a;

                SAMPLE_DIR(0)
                SAMPLE_DIR(1)
                SAMPLE_DIR(2)
                SAMPLE_DIR(3)
                SAMPLE_DIR(4)
                SAMPLE_DIR(5)
                SAMPLE_DIR(6)
                SAMPLE_DIR(7)
                SAMPLE_DIR(8)
                SAMPLE_DIR(9)
                SAMPLE_DIR(10)
                SAMPLE_DIR(11)
                SAMPLE_DIR(12)
                SAMPLE_DIR(13)
                SAMPLE_DIR(14)
                SAMPLE_DIR(15)

                #undef SAMPLE_DIR

                // 描边检测逻辑
                bool isOutlineOut = (col.a < 0.1 && alphaSumOut > 0);
                bool isOutlineIn  = (col.a > 0.1 && alphaSumIn  > 0);

                float strength = saturate(anim);

                if (isOutlineOut || isOutlineIn)
                    return float4(_LineColor.rgb, strength);

                return col;
            }

            ENDCG
        }
    }
    
    // 添加回退 Shader
    FallBack "UI/Default"
}