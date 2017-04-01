// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Observ3d/Specific/SP-Overlay-Color" {
Properties {
	_Color ("Main Color", Color) = (1,1,1,1)

}
SubShader {


	LOD 200
	Tags { "Queue"="Transparent+2" "IgnoreProjector"="True" "RenderType"="Transparent" }
	
	Pass{
	 
	Blend One OneMinusSrcAlpha
	ZTest Off

	CGPROGRAM
	#pragma vertex vert
	#pragma fragment frag
	
	#include "UnityCG.cginc"
	
	
	fixed4 _Color;

	
	
	struct appdata_t {
				float4 vertex : POSITION;
				float3 normal : NORMAL;
			};
		
		
		struct v2f {
			float4 vertex : POSITION;
			float3 worldNormal:TEXCOORD0;
			float4 worldPosition:TEXCOORD1;
		};
		
		v2f vert (appdata_t v) {
			v2f o;
			o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
			o.worldNormal = mul((float3x3)unity_ObjectToWorld,v.normal);
			o.worldPosition = mul(unity_ObjectToWorld,v.vertex);
			return o;
		}
		
		half4 frag (v2f IN) : COLOR{

			half3 worldViewDir = normalize(_WorldSpaceCameraPos.xyz -IN.worldPosition);
			half dt = 1-dot(worldViewDir,IN.worldNormal);
			

	  		return half4(_Color.rgb*(0.5+0.5*dt)*_Color.a,0);
		}
	
	
	ENDCG
	}
}
    Fallback Off
}
