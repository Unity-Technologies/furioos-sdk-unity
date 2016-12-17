Shader "Observ3d/Light Responsive/Color/Opaque/LR-Color-DS" {
	Properties {
		_Color ("Main Color", Color) = (1,1,1,1)
		
		_SpecColor ("Specular color", color) = (0.5,0.5,0.5,0.5)
		_Specular ("Specular", Range (0.01, 1)) = 0.078125
		_Gloss ("Glossiness", Range (0.01, 1)) = 0.1
		_AntiFlick ("AntiFlick", Range (0, 0.0001)) = 0
	}
	SubShader {
		Tags { "Queue"="Geometry" "RenderType"="Opaque" }
		
			ZWrite On
			Cull Off
			CGPROGRAM
			
			#pragma surface surf BlinnPhong vertex:vert
			#include "UnityCG.cginc"
			
			struct Input {
				float2 uv_MainTex;
			};
			
			fixed3 _Color;
			
			half _Specular;
			half _Gloss;
			
			half _AntiFlick;
			
			void vert (inout appdata_full v) {
				v.vertex.z -= _AntiFlick*v.vertex.w;
			}
			
			void surf (Input IN, inout SurfaceOutput o) {
				half3 albedo = _Color.rgb;
				o.Gloss = _Gloss ;
				o.Specular = _Specular;
				o.Albedo = albedo.rgb ;
			}
			
			ENDCG
	}
		Fallback "VertexLit"
		CustomEditor "OBSMaterialInspector"
}
