Shader "Observ3d/Light Responsive/Textured/Transparent/LR-Texture-Alpha-DS" {
	Properties {
		_Color ("Main Color", Color) = (1,1,1,1)
		
		_MainTex ("Diffuse (RGB)", 2D) = "white" {}
		
		_SpecColor ("Specular color", color) = (0.5,0.5,0.5,0.5)
		_Specular ("Specular", Range (0.01, 1)) = 0.078125
		_Gloss ("Glossiness", Range (0.01, 1)) = 0.1
		_AntiFlick ("AntiFlick", Range (0, 0.0001)) = 0
	}
	SubShader {
		Tags { "Queue"="AlphaTest" "RenderType"="TransparentCutout" "IgnoreProjector"="True" }
		
			ZWrite Off
			Cull Front
			Blend SrcAlpha OneMinusSrcAlpha
			CGPROGRAM
			
			#pragma surface surf BlinnPhong vertex:vert
			#include "UnityCG.cginc"
			
			struct Input {
				float2 uv_MainTex;
			};
			
			fixed4 _Color;
			
			sampler2D _MainTex;
			
			half _Specular;
			half _Gloss;
			
			half _AntiFlick;
			
			void vert (inout appdata_full v) {
				v.vertex.z -= _AntiFlick*v.vertex.w;
			}
			
			void surf (Input IN, inout SurfaceOutput o) {
				half4 albedo = tex2D(_MainTex, IN.uv_MainTex) * half4(_Color.rgb,1);
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
			
			sampler2D _MainTex;
			
			half _Specular;
			half _Gloss;
			
			half _AntiFlick;
			
			void vert (inout appdata_full v) {
				v.vertex.z -= _AntiFlick*v.vertex.w;
			}
			
			void surf (Input IN, inout SurfaceOutput o) {
				half4 albedo = tex2D(_MainTex, IN.uv_MainTex) * half4(_Color.rgb,1);
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
