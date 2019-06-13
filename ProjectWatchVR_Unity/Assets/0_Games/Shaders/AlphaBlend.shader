Shader "Custom/AlphaBlend"
{
    Properties
        {
            _Color ("Color", Color) = (1,1,1,1)
            _MainTex ("Albedo (RGB)", 2D) = "white" {}
            _Alpha ("Alpha", Range(0,1)) = 0.5
        }
        SubShader
        {
             Tags {"RenderType"="Transparent" "Queue"="Transparent"}
            LOD 200
             Pass {
                 ColorMask 0
             }
             // Render normally
     
                 ZWrite Off
                 Blend SrcAlpha OneMinusSrcAlpha
                 ColorMask RGB
 
            CGPROGRAM
 
            #pragma surface surf CustomUnlit fullforwardshadows alpha:fade
            #pragma target 3.0
 
            sampler2D _MainTex;
            half4 LightingCustomUnlit(SurfaceOutput s, half3 lightDir, half atten)
            {
                half4 c;
                c.rgb = s.Albedo ;
                c.a = s.Alpha;
                return c;
            }

            struct Input {
                float2 uv_MainTex;
            };
 
            half _Alpha;
            fixed4 _Color;
 
            void surf (Input IN, inout SurfaceOutput o)
            {
                fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
                o.Albedo = c.rgb;
                o.Alpha = _Alpha * c.a;
            }
            ENDCG
        }
        FallBack "Diffuse"
}
