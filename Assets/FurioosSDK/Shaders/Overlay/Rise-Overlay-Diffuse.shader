// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "FurioosSDK/Overlay/Diffuse" {
Properties {
    _Color ("Main Color", Color) = (1,1,1,1)
    _MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
}

SubShader {

	Blend SrcAlpha OneMinusSrcAlpha
	ZTest Off
	
	LOD 200
	Tags { "Queue"="Transparent+2" "RenderType"="Transparent" }
	
	Pass { 
		CGPROGRAM
		
		#pragma vertex vert
		#pragma fragment frag
		
		#include "UnityCG.cginc"

		
		sampler2D _MainTex;
		float4 _MainTex_ST;
		fixed4 _Color;
		
		struct appdata_t {
			float4 vertex : POSITION;
			float3 normal : NORMAL;
			float2 texcoord : TEXCOORD0;
		};
		
		
		struct v2f {
			float4 vertex : POSITION;
			float2 texcoord : TEXCOORD0;
		};
		
		v2f vert (appdata_t v) {
			v2f o;
			o.vertex = UnityObjectToClipPos(v.vertex);
			o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
			return o;
		}
		
		half4 frag (v2f IN) : COLOR{
	  		return tex2D(_MainTex, IN.texcoord)*_Color;
		}
			
		ENDCG
	}
}

Fallback "Alpha/VertexLit", 1
}
