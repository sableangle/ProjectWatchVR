Shader "Custom/SelfLightWithShadow" {
	Properties
	{
		_Color("Color", Color) = (1, 1, 1, 1)
		_MainTex("Albedo (RGB)", 2D) = "white" {}
		_SecTex("UV2 Map (RGB)", 2D) = "white" {}
	}

	SubShader
	{
		Tags
        {
			"Queue"="Geometry"
        	"RenderType" = "Opaque"
			"Glowable"="True"
        }
        LOD 200
		pass{
			ZWrite On
			ColorMask 0
		}
		//Blend SrcAlpha OneMinusSrcAlpha
        CGPROGRAM
        #pragma surface surf CelShadingForward addshadow
        #pragma target 3.0
		

        half4 LightingCelShadingForward(SurfaceOutput s, half3 lightDir, half atten)
        {
			half NdotL = dot(s.Normal, lightDir);

			if (NdotL <= 0.0)
			NdotL = 0;
			else if(NdotL >= 0.6 && NdotL < 1)
			NdotL = 0.3;
			else if(NdotL >= 0.3 && NdotL < 0.6)
			NdotL = 0.2;
			else if(NdotL >= 0 && NdotL < 0.3)
			NdotL = 0.1;
			else
			NdotL = 1;

			half4 c;
			c.rgb = s.Albedo * (NdotL * atten * 2);
			c.a = s.Alpha;

			return c;
        }


        sampler2D _MainTex;
		sampler2D _SecTex;
        fixed4 _Color;


        struct Input
        {
        	float2 uv_MainTex;
			float2 uv2_SecTex : TEXCOORD1;
        };


        void surf(Input IN, inout SurfaceOutput o)
        {
			
			
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
			fixed4 c2 = tex2D(_SecTex, IN.uv2_SecTex);
			
			fixed4 finalColor = c * c2 * _Color;
			o.Albedo = finalColor.rgb;
			o.Alpha = finalColor.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
