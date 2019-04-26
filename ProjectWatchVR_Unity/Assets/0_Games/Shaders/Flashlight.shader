Shader "Custom/Flashlight" {
    Properties {
      _MainTex ("Texture", 2D) = "white" {}
      _Mask ("Texture", 2D) = "white" {}
    }
    SubShader {
      Tags { "RenderType" = "Transparent" "Queue"="Transparent" }
					Blend SrcAlpha OneMinusSrcAlpha

      CGPROGRAM
      #pragma surface surf UnlitShading alpha
			        #pragma target 3.0

      struct Input {
				float2 uv_MainTex;
				float2 uv_Mask;
		   	float4 screenPos;
      };

	half4 LightingUnlitShading(SurfaceOutput s, half3 lightDir, half atten)
	{
			half4 c;
			c.rgb = s.Albedo;
			c.a = s.Alpha;
			return c;
	}

	sampler2D _MainTex;
	sampler2D _Mask;
	void surf (Input IN, inout SurfaceOutput o) {
			float2 coords = IN.screenPos.xy / IN.screenPos.w;
			o.Albedo = tex2D (_MainTex, coords).rgb;
			float3 mask = tex2D (_Mask, IN.uv_Mask).rgb;
			o.Alpha = mask;

	}
	ENDCG
    } 
    Fallback "Diffuse"
  }