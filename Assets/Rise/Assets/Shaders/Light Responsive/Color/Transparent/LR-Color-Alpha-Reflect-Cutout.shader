Shader "Observ3d/Light Responsive/Color/Transparent/LR-Color-Alpha-Reflect-Cutout" {
	Properties {
		_Color ("Main Color", Color) = (1,1,1,1)
		
		_Cutoff ("Cutout Offset", Range (0, 1)) = 0.95
		
		_ReflectMap ("Reflect Map", Cube) = "gray" { TexGen CubeReflect }
		_ReflectColor ("Reflect Color", Color) = (1,1,1,1)
		_ReflectContrast ("Reflect Contrast", Range (0, 3)) = 1
		_ReflectOffset ("Reflect Offset", Range (-1, 0)) = -0.5
		_Normal_Reflect_Alpha ("Normal Reflect Alpha", Range (0, 1)) = 0.05
		_Fresnel_Curve ("Fresnel Curve", Range (1, 10)) = 4
		_Tangent_Reflect_Alpha ("Tangent Reflect Alpha", Range (0.01, 1)) = 0.5
		
		_SpecColor ("Specular color", color) = (0.5,0.5,0.5,0.5)
		_Specular ("Specular", Range (0.01, 1)) = 0.078125
		_Gloss ("Glossiness", Range (0.01, 1)) = 0.1
		_AntiFlick ("AntiFlick", Range (0, 0.0001)) = 0
	}
	SubShader {
		Tags { "Queue"="Transparent" "RenderType"="TransparentCutout" "IgnoreProjector"="True" }
		
			ZWrite On
			Cull Back
			Blend SrcAlpha OneMinusSrcAlpha
			CGPROGRAM
			
			#pragma surface surf BlinnPhong vertex:vert
			#pragma target 3.0
			#include "UnityCG.cginc"
			
			struct Input {
				float2 uv_MainTex;
				float3 viewDir;
				float3 worldRefl; INTERNAL_DATA
			};
			
			fixed4 _Color;
			
			float _Cutoff;
		
			samplerCUBE _ReflectMap;
			fixed _Normal_Reflect_Alpha;
			fixed _Fresnel_Curve;
			fixed _Tangent_Reflect_Alpha;
			fixed _ReflectContrast;
			fixed _ReflectOffset;
			fixed3 _ReflectColor;
			
			half _Specular;
			half _Gloss;
			
			half _AntiFlick;
			
			void vert (inout appdata_full v) {
				v.vertex.z -= _AntiFlick*v.vertex.w;
			}
			
			void surf (Input IN, inout SurfaceOutput o) {
				half4 albedo = half4(_Color.rgb,1);
				clip(albedo.a-_Cutoff);
				half viewDotProduct = abs(dot(normalize(IN.viewDir), o.Normal));
				half reflectQty = (_Tangent_Reflect_Alpha - _Normal_Reflect_Alpha) * pow(( 1-viewDotProduct),_Fresnel_Curve) + _Normal_Reflect_Alpha;
				half3 reflectCol = (_ReflectOffset + texCUBE(_ReflectMap, WorldReflectionVector(IN, o.Normal) ).rgb * _ReflectColor) * _ReflectContrast;
				half reflectLum = (reflectCol.r + reflectCol.g + reflectCol.b)/12;
				half alpha = (_Color.a + reflectQty + reflectLum) * albedo.a;
				o.Gloss = _Gloss * albedo.a;
				o.Specular = _Specular;
				o.Albedo = ( ( albedo.rgb * (1-_ReflectColor) + reflectCol + 0.5) * reflectQty );
				o.Alpha = alpha;
			}
			
			ENDCG
			ZWrite Off
			Cull Back
			Blend SrcAlpha OneMinusSrcAlpha
			CGPROGRAM
			
			#pragma surface surf BlinnPhong vertex:vert
			#pragma target 3.0
			#include "UnityCG.cginc"
			
			struct Input {
				float2 uv_MainTex;
				float3 viewDir;
				float3 worldRefl; INTERNAL_DATA
			};
			
			fixed4 _Color;
			
			float _Cutoff;
		
			samplerCUBE _ReflectMap;
			fixed _Normal_Reflect_Alpha;
			fixed _Fresnel_Curve;
			fixed _Tangent_Reflect_Alpha;
			fixed _ReflectContrast;
			fixed _ReflectOffset;
			fixed3 _ReflectColor;
			
			half _Specular;
			half _Gloss;
			
			half _AntiFlick;
			
			void vert (inout appdata_full v) {
				v.vertex.z -= _AntiFlick*v.vertex.w;
			}
			
			void surf (Input IN, inout SurfaceOutput o) {
				half4 albedo = half4(_Color.rgb,1);
				clip(_Cutoff-albedo.a);
				clip(albedo.a-0.004);
				half viewDotProduct = abs(dot(normalize(IN.viewDir), o.Normal));
				half reflectQty = (_Tangent_Reflect_Alpha - _Normal_Reflect_Alpha) * pow(( 1-viewDotProduct),_Fresnel_Curve) + _Normal_Reflect_Alpha;
				half3 reflectCol = (_ReflectOffset + texCUBE(_ReflectMap, WorldReflectionVector(IN, o.Normal) ).rgb * _ReflectColor) * _ReflectContrast;
				half reflectLum = (reflectCol.r + reflectCol.g + reflectCol.b)/12;
				half alpha = (_Color.a + reflectQty + reflectLum) * albedo.a;
				o.Gloss = _Gloss * albedo.a;
				o.Specular = _Specular;
				o.Albedo = ( ( albedo.rgb * (1-_ReflectColor) + reflectCol + 0.5) * reflectQty );
				o.Alpha = alpha;
			}
			
			ENDCG
	}
		Fallback "Transparent/Cutout/VertexLit"
		CustomEditor "OBSMaterialInspector"
}
