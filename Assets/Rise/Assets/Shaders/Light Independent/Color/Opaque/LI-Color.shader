Shader "Observ3d/Light Independent/Color/Opaque/LI-Color" {
	Properties {
		_Color ("Main Color", Color) = (1,1,1,1)
		
		_AntiFlick ("AntiFlick", Range (0, 0.0001)) = 0
	}
	SubShader {
		Tags { "Queue"="Geometry" "RenderType"="Opaque" }
		
		Pass {
			ZWrite On
			Lighting Off
			Cull Back
			CGPROGRAM
			
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			
			struct VertInput {
				float4 vertex : POSITION;
			};
			
			struct FragInput {
				float4 vertex : POSITION;
			};
			
			fixed3 _Color;
			
			half _AntiFlick;
			
			FragInput vert (VertInput v) {
				FragInput o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.vertex.z -= _AntiFlick*o.vertex.w;
				return o;
			}
			
			half4 frag (FragInput IN) : COLOR{
				half3 albedo = _Color.rgb;
				return half4((albedo.rgb ) ,1.0);
			}
			
			ENDCG
		}
	}
		Fallback "VertexLit"
		CustomEditor "OBSMaterialInspector"
}
