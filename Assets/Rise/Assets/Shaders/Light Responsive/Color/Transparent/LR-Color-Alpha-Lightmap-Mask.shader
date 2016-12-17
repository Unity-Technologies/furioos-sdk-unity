Shader "Observ3d/Light Responsive/Color/Transparent/LR-Color-Alpha-Lightmap-Mask" {
	Properties {
		_Color ("Main Color", Color) = (1,1,1,1)
		
		_MaskMap ("Mask (Gray)", 2D) = "white" {}
		
		_LightMap ("Lightmap (RGB)", 2D) = "gray" { }
		_LightMapContrast ("LightMap Contrast", Range (0, 3)) = 1
		_LightMapOffset ("LightMap 0ffset", Range (-1, 1)) = 0
		
		_SpecColor ("Specular color", color) = (0.5,0.5,0.5,0.5)
		_Specular ("Specular", Range (0.01, 1)) = 0.078125
		_Gloss ("Glossiness", Range (0.01, 1)) = 0.1
		_AntiFlick ("AntiFlick", Range (0, 0.0001)) = 0
	}
	SubShader {
		Tags { "Queue"="Transparent+1" "RenderType"="Transparent" "IgnoreProjector"="True" }
		
			ZWrite Off
			Cull Back
			Blend SrcAlpha OneMinusSrcAlpha
			CGPROGRAM
			
			#pragma surface surf BlinnPhong vertex:vert
			#include "UnityCG.cginc"
			
			struct Input {
				float2 uv_MainTex;
				float2 uv2_LightMap;
				float2 uv_MaskMap;
			};
			
			fixed4 _Color;
			
			sampler2D _MaskMap;
		
			sampler2D _LightMap;
			fixed _LightMapContrast;
			fixed _LightMapOffset;
			
			half _Specular;
			half _Gloss;
			
			half _AntiFlick;
			
			void vert (inout appdata_full v) {
				v.vertex.z -= _AntiFlick*v.vertex.w;
			}
			
			void surf (Input IN, inout SurfaceOutput o) {
				half4 albedo = half4(_Color.rgb,1);
				half3 mask = tex2D(_MaskMap, IN.uv_MaskMap).rgb;
				albedo.a *= (mask.r + mask.g + mask.b) / 3 ;
				half alpha = _Color.a * albedo.a;
				half4 lmc = tex2D (_LightMap, IN.uv2_LightMap);
				half3 lm = (_LightMapOffset + (lmc.rgb * (8 * lmc.a)) - 1) * _LightMapContrast + 1.0;
				o.Gloss = _Gloss * albedo.a;
				o.Specular = _Specular;
				o.Albedo = ( albedo.rgb  ) * lm;
				o.Alpha = alpha;
			}
			
			ENDCG
	}
		Fallback "Transparent/VertexLit"
		CustomEditor "OBSMaterialInspector"
}
