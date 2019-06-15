// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/StincilMask"
{
     Properties {
		_StencilMask("Stencil Mask", Int) = 0
	}
	SubShader {
		Tags { "RenderType"="Opaque" "Queue"="Geometry-50" } //注意渲染顺序是先渲染此模板，再渲染要遮罩的物体
		ColorMask 0           //不会写入颜色值，此shader专门作为Stencil测试
		ZWrite Off		
		Stencil{
			Ref [_StencilMask]  //开启Stencil测试
			Comp  Always        //和Stencil缓冲区的值做比较，这里表示不管_StencelMask的大小，始终通过
			Pass Replace	    //将_StencilMask值写入Stencil缓冲
		}
		Pass{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			struct appdata {
				float4 vertex:POSITION;
			};
			struct v2f {
				float4 pos:SV_POSITION;
			};
			v2f vert(appdata v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				return o;
			}
			half4 frag(v2f i) :COLOR{
				return half4(0,0,0,1);
			}
			ENDCG
		}
		
		
	}

}
