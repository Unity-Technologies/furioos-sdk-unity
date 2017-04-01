// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Observ3d/Light Independent/Color/Transparent/LI-Color-Alpha-Mask-DS" {
	Properties {
		_Color ("Main Color", Color) = (1,1,1,1)
		
		_MaskMap ("Mask (Gray)", 2D) = "white" {}
		
		_AntiFlick ("AntiFlick", Range (0, 0.0001)) = 0
	}
	SubShader {
		Tags { "Queue"="Transparent+1" "RenderType"="Transparent" "IgnoreProjector"="True" }
		
		Pass {
			ZWrite Off
			Lighting Off
			Cull Front
			Blend SrcAlpha OneMinusSrcAlpha
			CGPROGRAM
			
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			
			struct VertInput {
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
			};
			
			struct FragInput {
				float4 vertex : POSITION;
				float2 uv_MaskMap : TEXCOORD7;
			};
			
			fixed4 _Color;
			
			sampler2D _MaskMap;
			float4 _MaskMap_ST;
		
			half _AntiFlick;
			
			FragInput vert (VertInput v) {
				FragInput o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.vertex.z -= _AntiFlick*o.vertex.w;
				o.uv_MaskMap = TRANSFORM_TEX(v.texcoord, _MaskMap);
				return o;
			}
			
			half4 frag (FragInput IN) : COLOR{
				half4 albedo = half4(_Color.rgb,1);
				half3 mask = tex2D(_MaskMap, IN.uv_MaskMap).rgb;
				albedo.a *= (mask.r + mask.g + mask.b) / 3 ;
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
				float2 texcoord : TEXCOORD0;
			};
			
			struct FragInput {
				float4 vertex : POSITION;
				float2 uv_MaskMap : TEXCOORD7;
			};
			
			fixed4 _Color;
			
			sampler2D _MaskMap;
			float4 _MaskMap_ST;
		
			half _AntiFlick;
			
			FragInput vert (VertInput v) {
				FragInput o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.vertex.z -= _AntiFlick*o.vertex.w;
				o.uv_MaskMap = TRANSFORM_TEX(v.texcoord, _MaskMap);
				return o;
			}
			
			half4 frag (FragInput IN) : COLOR{
				half4 albedo = half4(_Color.rgb,1);
				half3 mask = tex2D(_MaskMap, IN.uv_MaskMap).rgb;
				albedo.a *= (mask.r + mask.g + mask.b) / 3 ;
				half alpha = _Color.a * albedo.a;
				return half4((albedo.rgb ) ,alpha);
			}
			
			ENDCG
		}
	}
		Fallback "Transparent/VertexLit"
		CustomEditor "OBSMaterialInspector"
}
