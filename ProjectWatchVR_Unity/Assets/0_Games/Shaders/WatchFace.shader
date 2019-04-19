Shader "Custom/WatchFace" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_TouchDotTex("Touch Dot (RGB)", 2D) = "white" {}
		_X ("X", Range(0,1)) = 0.5
		_Y ("Y", Range(0,1)) = 0.0
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200

		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf SimpleShadingForward fullforwardshadows

        half4 LightingSimpleShadingForward(SurfaceOutput s, half3 lightDir, half atten)
        {
			///half NdotL = dot(s.Normal, lightDir);

			half4 c;
			c.rgb = s.Albedo ;
			c.a = s.Alpha;

			return c;
        }

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;
		sampler2D _TouchDotTex;

		struct Input {
			float2 uv_MainTex;
			float2 uv_TouchDotTex;
		};

		half _X;
		half _Y;
		fixed4 _Color;

		void surf (Input IN, inout SurfaceOutput o) {
			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex);
			fixed4 decal = tex2D(_TouchDotTex, IN.uv_TouchDotTex);
			c.rgb = lerp (c.rgb, decal.rgb, decal.a);
			c *= _Color;
			o.Albedo = c.rgb ;
			o.Alpha = c.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
