// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Observ3d/Light Independent/Textured/Opaque/LI-Texture-Bump" {
	Properties {
		_Color ("Main Color", Color) = (1,1,1,1)
		
		_MainTex ("Diffuse (RGB)", 2D) = "white" {}
		
		_BumpMap ("Bump Map", 2D) = "bump" {}
		_BumpQuantity ("Bump Quantity", Range (0, 2)) = 1
		
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
				float2 texcoord : TEXCOORD0;
				float4 tangent : TANGENT;
				float3 normal : NORMAL;
			};
			
			struct FragInput {
				float4 vertex : POSITION;
				float2 uv_MainTex : TEXCOORD0;
				float3 worldNormal:TEXCOORD3;
				float2 uv_BumpMap : TEXCOORD1;
				float3 worldBinormal:TEXCOORD5;
				float3 worldTangent:TEXCOORD6;
			};
			
			fixed3 _Color;
			
			sampler2D _MainTex;
			float4 _MainTex_ST;
			
			sampler2D _BumpMap;
			float4 _BumpMap_ST;
			fixed _BumpQuantity;
			
			half _AntiFlick;
			
			FragInput vert (VertInput v) {
				FragInput o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.vertex.z -= _AntiFlick*o.vertex.w;
				o.uv_MainTex = TRANSFORM_TEX(v.texcoord, _MainTex);
				o.worldNormal = normalize( mul((float3x3)unity_ObjectToWorld,v.normal));
				o.uv_BumpMap = TRANSFORM_TEX(v.texcoord, _BumpMap);
				float3 binormal = cross( v.normal, v.tangent.xyz ) * v.tangent.w;
				o.worldBinormal = normalize( mul((float3x3)unity_ObjectToWorld,binormal));
				o.worldTangent = normalize( mul((float3x3)unity_ObjectToWorld,v.tangent.xyz));
				return o;
			}
			
			half4 frag (FragInput IN) : COLOR{
				half3 albedo = tex2D(_MainTex, IN.uv_MainTex).rgb*_Color.rgb;
				fixed3 bumpNormal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
				half3 worldNormal = normalize(IN.worldNormal * bumpNormal.z +
					IN.worldTangent * bumpNormal.x * _BumpQuantity +
					IN.worldBinormal * bumpNormal.y * _BumpQuantity );
				half bump = dot(IN.worldNormal,worldNormal);
				return half4((albedo.rgb * bump ) ,1.0);
			}
			
			ENDCG
		}
	}
		Fallback "VertexLit"
		CustomEditor "OBSMaterialInspector"
}
