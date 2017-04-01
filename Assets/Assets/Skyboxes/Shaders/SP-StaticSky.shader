// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Observ3d/Specific/SP-StaticSky" {

	Properties {
		_Color ("Main Color", Color) = (1,1,1,1)
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_OrientationOffset ("Orientation Offset", Range (0, 1)) = 0
	}
	SubShader {
		Tags { "Queue"="Background+1" "RenderType" = "Opaque" }
		Lighting off
		Cull Back
		ZWrite off
		Fog {Mode Off}
		Pass { 
		
			
			CGPROGRAM
			
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"
	
			
			sampler2D _MainTex;
			float4 _MainTex_ST;
			fixed4 _Color;
			
			half _OrientationOffset;
			
			struct appdata_t {
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};
			
			
			struct v2f {
				    float4  pos : SV_POSITION;
				    float2  uv : TEXCOORD0;
				};
			
			v2f vert (appdata_t v)
				{
				    v2f o;
				    o.pos = UnityObjectToClipPos (v.vertex);
				    o.pos.z = 0.9999999 * o.pos.w;
				    o.uv = TRANSFORM_TEX (float2(v.uv.x+_OrientationOffset,v.uv.y), _MainTex);
				    return o;
				}
			
			float4 frag (v2f IN) : COLOR{
		  		return tex2D(_MainTex, IN.uv)*_Color;
			}
				
			ENDCG
		}
	}
}
