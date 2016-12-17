Shader "Observ3d/Light Independent/Color/Transparent/LI-Color-Alpha-Cutout" {
	Properties {
		_Color ("Main Color", Color) = (1,1,1,1)
		
		_Cutoff ("Cutout Offset", Range (0, 1)) = 0.95
		
		_AntiFlick ("AntiFlick", Range (0, 0.0001)) = 0
	}
	SubShader {
		Tags { "Queue"="Transparent" "RenderType"="TransparentCutout" "IgnoreProjector"="True" }
		
		Pass {
			ZWrite On
			Lighting Off
			Cull Back
			Blend SrcAlpha OneMinusSrcAlpha
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
			
			fixed4 _Color;
			
			float _Cutoff;
		
			half _AntiFlick;
			
			FragInput vert (VertInput v) {
				FragInput o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.vertex.z -= _AntiFlick*o.vertex.w;
				return o;
			}
			
			half4 frag (FragInput IN) : COLOR{
				half4 albedo = half4(_Color.rgb,1);
				clip(albedo.a-_Cutoff);
				half alpha = _Color.a * albedo.a;
				return half4((albedo.rgb ) ,alpha);
			}
			
			ENDCG
		}
		Pass {
			ZWrite Off
			Lighting Off
			Cull Back
			Blend SrcAlpha OneMinusSrcAlpha
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
			
			fixed4 _Color;
			
			float _Cutoff;
		
			half _AntiFlick;
			
			FragInput vert (VertInput v) {
				FragInput o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.vertex.z -= _AntiFlick*o.vertex.w;
				return o;
			}
			
			half4 frag (FragInput IN) : COLOR{
				half4 albedo = half4(_Color.rgb,1);
				clip(_Cutoff-albedo.a);
				clip(albedo.a-0.004);
				half alpha = _Color.a * albedo.a;
				return half4((albedo.rgb ) ,alpha);
			}
			
			ENDCG
		}
	}
		Fallback "Transparent/Cutout/VertexLit"
		CustomEditor "OBSMaterialInspector"
}
