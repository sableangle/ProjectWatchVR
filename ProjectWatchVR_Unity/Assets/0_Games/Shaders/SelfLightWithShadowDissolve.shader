// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/SelfLightWithShadowDissolve" {
	Properties
	{
		_Color("Color", Color) = (1, 1, 1, 1)
		_MainTex("Albedo (RGB)", 2D) = "white" {}
		 _OutlineColor ("Outline Color", Color) = (0, 0, 0, 1)
		 _OutlineZ ("_OutlineZ", Range (0.0, 1.0)) = .00
        _OutlineWidth ("Outline width", Range (0.0, 1.0)) = .000

        //Dissolve properties
        _DissolveTexture("Dissolve Texutre", 2D) = "white" {} 
        _DissolveAmount("DissolveAmount", Range(0,1)) = 0
	}

	SubShader
	{
		Tags
        {
			"Queue"="Transparent"
        	"RenderType" = "Transparent"
        }
        LOD 200
		pass{
			ZWrite On
			ColorMask 0
		}
		//Blend SrcAlpha OneMinusSrcAlpha
        CGPROGRAM
        #pragma surface surf CelShadingForward addshadow alpha
        #pragma target 3.0
		

        half4 LightingCelShadingForward(SurfaceOutput s, half3 lightDir, half atten)
        {
			half4 c;
			
			float3 lambert = float(max(0.0, dot(s.Normal,lightDir))) * atten;
			c.rgb = s.Albedo * lambert;
			c.a = s.Alpha;
			return c;
        }


        sampler2D _MainTex;
        fixed4 _Color;
        //Dissolve properties
        sampler2D _DissolveTexture;
        half _DissolveAmount;

        struct Input
        {
        	float2 uv_MainTex;
        };


        void surf(Input IN, inout SurfaceOutput o)
        {
			 //Dissolve function
            half dissolve_value = tex2D(_DissolveTexture, IN.uv_MainTex).r;
            clip(dissolve_value - _DissolveAmount);
			o.Emission = fixed3(1, 1, 1) * step( dissolve_value - _DissolveAmount, 0.05f); //emits white color with 0.05 border size

			fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
			
			fixed4 finalColor = c * _Color;
			o.Albedo = finalColor.rgb;
			o.Alpha = finalColor.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
