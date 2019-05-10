Shader "Custom/Flashlight" {
    Properties {
      _MainTex ("Texture", 2D) = "white" {}
      _Mask ("Texture", 2D) = "white" {}
    }
    SubShader {
      Tags { "RenderType" = "Opaque" "Queue"="Overlay" }
					Blend SrcAlpha OneMinusSrcAlpha
			zWrite off
			 ZTest Always
      CGPROGRAM
      #pragma surface surf UnlitShading alpha
			        #pragma target 3.0

      struct Input {
				float2 uv_MainTex: TEXCOORD0;
				float2 uv2_Mask: TEXCOORD1;
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
			#if UNITY_SINGLE_PASS_STEREO
					// If Single-Pass Stereo mode is active, transform the
					// coordinates to get the correct output UV for the current eye.
					float4 scaleOffset = unity_StereoScaleOffset[unity_StereoEyeIndex];
					coords = (coords - scaleOffset.zw) / scaleOffset.xy;
			#endif
			o.Albedo = tex2D (_MainTex, IN.uv_MainTex).rgb;
			float3 mask = tex2D (_Mask, IN.uv2_Mask).rgb;
			o.Alpha = mask;

	}
	ENDCG
    } 
    Fallback "Diffuse"
  }