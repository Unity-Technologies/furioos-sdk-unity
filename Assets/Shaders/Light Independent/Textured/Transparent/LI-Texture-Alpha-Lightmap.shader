Shader "Observ3d/Light Independent/Textured/Transparent/LI-Texture-Alpha-Lightmap" {
	Properties {
		_Color ("Main Color", Color) = (1,1,1,1)
		
		_MainTex ("Diffuse (RGB)", 2D) = "white" {}
		
		_LightMap ("Lightmap (RGB)", 2D) = "gray" { }
		_LightMapContrast ("LightMap Contrast", Range (0, 3)) = 1
		_LightMapOffset ("LightMap 0ffset", Range (-1, 1)) = 0
		
		_AntiFlick ("AntiFlick", Range (0, 0.0001)) = 0
	}
	SubShader {
		Tags { "Queue"="Transparent+1" "RenderType"="Transparent" "IgnoreProjector"="True" }
		
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
			};
			
			fixed4 _Color;
			
			sampler2D _MainTex;
			float4 _MainTex_ST;
			
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
				o.uv2_LightMap = TRANSFORM_TEX(v.texcoord1, _LightMap);
				return o;
			}
			
			half4 frag (FragInput IN) : COLOR{
				half4 albedo = tex2D(_MainTex, IN.uv_MainTex) * half4(_Color.rgb,1);
				half alpha = _Color.a * albedo.a;
				half4 lmc = tex2D (_LightMap, IN.uv2_LightMap);
				half3 lm = (_LightMapOffset + (lmc.rgb * (8 * lmc.a)) - 1) * _LightMapContrast + 1.0;
				return half4((albedo.rgb ) * lm,alpha);
			}
			
			ENDCG
		}
	}
		Fallback "Transparent/VertexLit"
		CustomEditor "OBSMaterialInspector"
}
