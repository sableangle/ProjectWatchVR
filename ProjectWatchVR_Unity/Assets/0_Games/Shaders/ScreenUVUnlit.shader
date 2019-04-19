Shader "Custom/SreeenUV" {
    Properties {
      _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader {
      Tags { "RenderType" = "Opaque" }
      CGPROGRAM
      #pragma surface surf UnlitShading
      struct Input {
          float2 uv_MainTex;
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
	void surf (Input IN, inout SurfaceOutput o) {
		 float2 coords = IN.screenPos.xy / IN.screenPos.w;
		o.Albedo = tex2D (_MainTex, coords).rgb;
	}
	ENDCG
    } 
    Fallback "Diffuse"
  }