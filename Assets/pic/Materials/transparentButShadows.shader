Shader "Custom/TransparentShadowReceiver"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1, 1, 1, 1)
        _ShadowIntensity ("Shadow Intensity", Range(0, 1)) = 0.6
        _ShadowColor ("Shadow Color", Color) = (0, 0, 0, 0.7)
    }
    
    SubShader
    {
        Tags { 
            "Queue" = "Transparent-10"
            "RenderType" = "Transparent"
        }
        
        // Проход для получения теней
        Pass
        {
            Tags { "LightMode" = "ShadowCaster" }
            
            ZWrite On
            ZTest LEqual
            ColorMask 0
            Cull Off
            
            CGPROGRAM
            #pragma vertex vertShadow
            #pragma fragment fragShadow
            #pragma multi_compile_shadowcaster
            #include "UnityCG.cginc"
            
            sampler2D _MainTex;
            float4 _MainTex_ST;
            
            struct v2f_shadow
            {
                V2F_SHADOW_CASTER;
                float2 uv : TEXCOORD1;
            };
            
            v2f_shadow vertShadow(appdata_base v)
            {
                v2f_shadow o;
                TRANSFER_SHADOW_CASTER_NORMALOFFSET(o);
                o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                return o;
            }
            
            float4 fragShadow(v2f_shadow i) : SV_Target
            {
                float alpha = tex2D(_MainTex, i.uv).a;
                clip(alpha - 0.01);
                SHADOW_CASTER_FRAGMENT(i);
            }
            ENDCG
        }
        
        // Основной проход (прозрачный приемщик теней с текстурой)
        Pass
        {
            Tags { "LightMode" = "ForwardBase" }
            
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fwdbase
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            
            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                SHADOW_COORDS(1)
            };
            
            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Color;
            float _ShadowIntensity;
            float4 _ShadowColor;
            
            v2f vert(appdata_base v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                TRANSFER_SHADOW(o);
                return o;
            }
            
            fixed4 frag(v2f i) : SV_Target
            {
                // Получаем цвет текстуры
                fixed4 texColor = tex2D(_MainTex, i.uv) * _Color;
                
                // Получаем тень
                float shadow = SHADOW_ATTENUATION(i);
                
                // Если объект в тени, смешиваем цвет текстуры с цветом тени
                if (shadow < 0.5)
                {
                    float shadowAlpha = (1 - shadow) * _ShadowIntensity;
                    texColor.rgb = lerp(texColor.rgb, _ShadowColor.rgb, shadowAlpha);
                }
                
                return texColor;
            }
            ENDCG
        }
    }
    
    Fallback "Transparent/VertexLit"
}