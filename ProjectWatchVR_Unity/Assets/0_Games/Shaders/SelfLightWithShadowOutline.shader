// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/SelfLightWithShadowOutline" {
	Properties
	{
		_Color("Color", Color) = (1, 1, 1, 1)
		_MainTex("Albedo (RGB)", 2D) = "white" {}
		 _OutlineColor ("Outline Color", Color) = (0, 0, 0, 1)
		 _OutlineZ ("_OutlineZ", Range (0.0, 1.0)) = .00
        _OutlineWidth ("Outline width", Range (0.0, 1.0)) = .000

        
	}

	SubShader
	{
		Pass
        {
            Cull front
             
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
             
            #include "UnityCG.cginc"
             
            struct appdata 
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };
             
            struct v2f
            {
                float4 pos : POSITION;
				float3 viewDir : TEXCOORD1;
        		float3 normalDir : TEXCOORD2;
            };
             
            uniform float _OutlineWidth;
            uniform float4 _OutlineColor;
            uniform float _OutlineZ;
            half _Amount;

            v2f vert(appdata v)
            {
                v2f o;
                 
                float3 norm = normalize(v.normal);
                v.vertex.xyz += v.normal * _OutlineWidth;
                o.pos = UnityObjectToClipPos(v.vertex);
  				o.normalDir = normalize(mul(float4(v.normal, 0), unity_WorldToObject).xyz); // normal direction
        		o.viewDir = normalize(WorldSpaceViewDir(v.vertex)); // view direction
                return o;
            }
             
            half4 frag(v2f i) : COLOR
            {
                return _OutlineColor;
            }
            ENDCG
        }
         
		Tags
        {
			"Queue"="Geometry"
        	"RenderType" = "Opaque"
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
			// half NdotL = dot(s.Normal, lightDir);

			// if (NdotL <= 0.0)
			// NdotL = 0;
			// else if(NdotL >= 0.6 && NdotL < 1)
			// NdotL = 0.3;
			// else if(NdotL >= 0.3 && NdotL < 0.6)
			// NdotL = 0.2;
			// else if(NdotL >= 0 && NdotL < 0.3)
			// NdotL = 0.1;
			// else
			// NdotL = 1;

			half4 c;
			
			float3 lambert = float(max(0.0, dot(s.Normal,lightDir))) * atten;
			c.rgb = s.Albedo * lambert;
			c.a = s.Alpha;
			return c;
        }


        sampler2D _MainTex;
        fixed4 _Color;
      

        struct Input
        {
        	float2 uv_MainTex;
        };


        void surf(Input IN, inout SurfaceOutput o)
        {
			
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
			
			fixed4 finalColor = c * _Color;
			o.Albedo = finalColor.rgb;
			o.Alpha = finalColor.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
