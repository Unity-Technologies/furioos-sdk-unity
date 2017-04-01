// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Observ3d/Light Independent/Color/Opaque/LI-Color-Reflect-DS" {
	Properties {
		_Color ("Main Color", Color) = (1,1,1,1)
		
		_ReflectMap ("Reflect Map", Cube) = "gray" { TexGen CubeReflect }
		_ReflectColor ("Reflect Color", Color) = (1,1,1,1)
		_ReflectContrast ("Reflect Contrast", Range (0, 3)) = 1
		_ReflectOffset ("Reflect Offset", Range (-1, 0)) = -0.5
		_Normal_Reflect_Alpha ("Normal Reflect Alpha", Range (0, 1)) = 0.05
		_Fresnel_Curve ("Fresnel Curve", Range (1, 10)) = 4
		_Tangent_Reflect_Alpha ("Tangent Reflect Alpha", Range (0.01, 1)) = 0.5
		
		_AntiFlick ("AntiFlick", Range (0, 0.0001)) = 0
	}
	SubShader {
		Tags { "Queue"="Geometry" "RenderType"="Opaque" }
		
		Pass {
			ZWrite On
			Lighting Off
			Cull Off
			CGPROGRAM
			
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			
			struct VertInput {
				float4 vertex : POSITION;
				float3 normal : NORMAL;
			};
			
			struct FragInput {
				float4 vertex : POSITION;
				float4 worldPosition:TEXCOORD4;
				float3 worldNormal:TEXCOORD3;
			};
			
			fixed3 _Color;
			
			samplerCUBE _ReflectMap;
			fixed _Normal_Reflect_Alpha;
			fixed _Fresnel_Curve;
			fixed _Tangent_Reflect_Alpha;
			fixed _ReflectContrast;
			fixed _ReflectOffset;
			fixed3 _ReflectColor;
			
			half _AntiFlick;
			
			FragInput vert (VertInput v) {
				FragInput o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.vertex.z -= _AntiFlick*o.vertex.w;
				o.worldPosition = mul(unity_ObjectToWorld,v.vertex);
				o.worldNormal = normalize( mul((float3x3)unity_ObjectToWorld,v.normal));
				return o;
			}
			
			half4 frag (FragInput IN) : COLOR{
				half3 albedo = _Color.rgb;
				half3 worldViewDir = normalize(_WorldSpaceCameraPos.xyz - IN.worldPosition.xyz);
				half3 worldNormal = IN.worldNormal;
				half viewDotProduct = dot(worldViewDir,worldNormal);
				half3 reflecDir =  - worldViewDir + 2.0 * ( worldNormal * viewDotProduct );
				viewDotProduct = abs(viewDotProduct);
				half reflectQty = (_Tangent_Reflect_Alpha - _Normal_Reflect_Alpha) * pow(( 1-viewDotProduct),_Fresnel_Curve) + _Normal_Reflect_Alpha;
				half3 reflectCol = (_ReflectOffset + texCUBE(_ReflectMap, reflecDir ).rgb * _ReflectColor) * _ReflectContrast;
				return half4((albedo.rgb * (1.0-reflectQty)  + (albedo.rgb * (1-_ReflectColor) + reflectCol + 0.5) * reflectQty ) ,1.0);
			}
			
			ENDCG
		}
	}
		Fallback "VertexLit"
		CustomEditor "OBSMaterialInspector"
}
