Shader "Observ3d/Light Independent/Textured/Transparent/LI-Texture-Alpha-Lightmap-Mask-Cutout" {
	Properties {
		_Color ("Main Color", Color) = (1,1,1,1)
		
		_MainTex ("Diffuse (RGB)", 2D) = "white" {}
		
		_MaskMap ("Mask (Gray)", 2D) = "white" {}
		
		_Cutoff ("Cutout Offset", Range (0, 1)) = 0.95
		
		_LightMap ("Lightmap (RGB)", 2D) = "gray" { }
		_LightMapContrast ("LightMap Contrast", Range (0, 3)) = 1
		_LightMapOffset ("LightMap 0ffset", Range (-1, 1)) = 0
		
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
				float2 texcoord : TEXCOORD0;
				float2 texcoord1 : TEXCOORD1;
			};
			
			struct FragInput {
				float4 vertex : POSITION;
				float2 uv_MainTex : TEXCOORD0;
				float2 uv2_LightMap : TEXCOORD2;
				float2 uv_MaskMap : TEXCOORD7;
			};
			
			fixed4 _Color;
			
			sampler2D _MainTex;
			float4 _MainTex_ST;
			
			sampler2D _MaskMap;
			float4 _MaskMap_ST;
		
			float _Cutoff;
		
			sampler2D _LightMap;
			float4 _LightMap_ST;
			fixed _LightMapContrast;
			fixed _LightMapOffset;
			
			half _AntiFlick;
			
			FragInput vert (VertInput v) {
				FragInput o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.vertex.z -= _AntiFlick*o.vertex.w;
				o.uv_MainTex = TRANSFORM_TEX(v.texcoord, _MainTex);
				o.uv_MaskMap = TRANSFORM_TEX(v.texcoord, _MaskMap);
				o.uv2_LightMap = TRANSFORM_TEX(v.texcoord1, _LightMap);
				return o;
			}
			
			half4 frag (FragInput IN) : COLOR{
				half4 albedo = tex2D(_MainTex, IN.uv_MainTex) * half4(_Color.rgb,1);
				half3 mask = tex2D(_MaskMap, IN.uv_MaskMap).rgb;
				albedo.a *= (mask.r + mask.g + mask.b) / 3 ;
				clip(albedo.a-_Cutoff);
				half alpha = _Color.a * albedo.a;
				half4 lmc = tex2D (_LightMap, IN.uv2_LightMap);
				half3 lm = (_LightMapOffset + (lmc.rgb * (8 * lmc.a)) - 1) * _LightMapContrast + 1.0;
				return half4((albedo.rgb ) * lm,alpha);
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
				float2 texcoord1 : TEXCOORD1;
			};
			
			struct FragInput {
				float4 vertex : POSITION;
				float2 uv_MainTex : TEXCOORD0;
				float2 uv2_LightMap : TEXCOORD2;
				float2 uv_MaskMap : TEXCOORD7;
			};
			
			fixed4 _Color;
			
			sampler2D _MainTex;
			float4 _MainTex_ST;
			
			sampler2D _MaskMap;
			float4 _MaskMap_ST;
		
			float _Cutoff;
		
			sampler2D _LightMap;
			float4 _LightMap_ST;
			fixed _LightMapContrast;
			fixed _LightMapOffset;
			
			half _AntiFlick;
			
			FragInput vert (VertInput v) {
				FragInput o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.vertex.z -= _AntiFlick*o.vertex.w;
				o.uv_MainTex = TRANSFORM_TEX(v.texcoord, _MainTex);
				o.uv_MaskMap = TRANSFORM_TEX(v.texcoord, _MaskMap);
				o.uv2_LightMap = TRANSFORM_TEX(v.texcoord1, _LightMap);
				return o;
			}
			
			half4 frag (FragInput IN) : COLOR{
				half4 albedo = tex2D(_MainTex, IN.uv_MainTex) * half4(_Color.rgb,1);
				half3 mask = tex2D(_MaskMap, IN.uv_MaskMap).rgb;
				albedo.a *= (mask.r + mask.g + mask.b) / 3 ;
				clip(_Cutoff-albedo.a);
				clip(albedo.a-0.004);
				half alpha = _Color.a * albedo.a;
				half4 lmc = tex2D (_LightMap, IN.uv2_LightMap);
				half3 lm = (_LightMapOffset + (lmc.rgb * (8 * lmc.a)) - 1) * _LightMapContrast + 1.0;
				return half4((albedo.rgb ) * lm,alpha);
			}
			
			ENDCG
		}
	}
		Fallback "Transparent/Cutout/VertexLit"
		CustomEditor "OBSMaterialInspector"
}
