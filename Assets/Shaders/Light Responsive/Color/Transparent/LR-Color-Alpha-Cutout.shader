Shader "Observ3d/Light Responsive/Color/Transparent/LR-Color-Alpha-Cutout" {
	Properties {
		_Color ("Main Color", Color) = (1,1,1,1)
		
		_Cutoff ("Cutout Offset", Range (0, 1)) = 0.95
		
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
			#include "UnityCG.cginc"
			
			struct Input {
				float2 uv_MainTex;
			};
			
			fixed4 _Color;
			
			float _Cutoff;
		
			half _Specular;
			half _Gloss;
			
			half _AntiFlick;
			
			void vert (inout appdata_full v) {
				v.vertex.z -= _AntiFlick*v.vertex.w;
			}
			
			void surf (Input IN, inout SurfaceOutput o) {
				half4 albedo = half4(_Color.rgb,1);
				clip(albedo.a-_Cutoff);
				half alpha = _Color.a * albedo.a;
				o.Gloss = _Gloss * albedo.a;
				o.Specular = _Specular;
				o.Albedo = albedo.rgb ;
				o.Alpha = alpha;
			}
			
			ENDCG
			ZWrite Off
			Cull Back
			Blend SrcAlpha OneMinusSrcAlpha
			CGPROGRAM
			
			#pragma surface surf BlinnPhong vertex:vert
			#include "UnityCG.cginc"
			
			struct Input {
				float2 uv_MainTex;
			};
			
			fixed4 _Color;
			
			float _Cutoff;
		
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
				half alpha = _Color.a * albedo.a;
				o.Gloss = _Gloss * albedo.a;
				o.Specular = _Specular;
				o.Albedo = albedo.rgb ;
				o.Alpha = alpha;
			}
			
			ENDCG
	}
		Fallback "Transparent/Cutout/VertexLit"
		CustomEditor "OBSMaterialInspector"
}
