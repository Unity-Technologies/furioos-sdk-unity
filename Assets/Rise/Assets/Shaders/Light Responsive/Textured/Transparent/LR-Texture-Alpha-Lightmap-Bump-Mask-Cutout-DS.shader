Shader "Observ3d/Light Responsive/Textured/Transparent/LR-Texture-Alpha-Lightmap-Bump-Mask-Cutout-DS" {
	Properties {
		_Color ("Main Color", Color) = (1,1,1,1)
		
		_MainTex ("Diffuse (RGB)", 2D) = "white" {}
		
		_MaskMap ("Mask (Gray)", 2D) = "white" {}
		
		_Cutoff ("Cutout Offset", Range (0, 1)) = 0.95
		
		_LightMap ("Lightmap (RGB)", 2D) = "gray" { }
		_LightMapContrast ("LightMap Contrast", Range (0, 3)) = 1
		_LightMapOffset ("LightMap 0ffset", Range (-1, 1)) = 0
		
		_BumpMap ("Bump Map", 2D) = "bump" {}
		_BumpQuantity ("Bump Quantity", Range (0, 2)) = 1
		
		_SpecColor ("Specular color", color) = (0.5,0.5,0.5,0.5)
		_Specular ("Specular", Range (0.01, 1)) = 0.078125
		_Gloss ("Glossiness", Range (0.01, 1)) = 0.1
		_AntiFlick ("AntiFlick", Range (0, 0.0001)) = 0
	}
	SubShader {
		Tags { "Queue"="Transparent" "RenderType"="TransparentCutout" "IgnoreProjector"="True" }
		
			ZWrite On
			Cull Off
			Blend SrcAlpha OneMinusSrcAlpha
			CGPROGRAM
			
			#pragma surface surf BlinnPhong vertex:vert
			#include "UnityCG.cginc"
			
			struct Input {
				float2 uv_MainTex;
				float2 uv_BumpMap;
				float2 uv2_LightMap;
				float2 uv_MaskMap;
			};
			
			fixed4 _Color;
			
			sampler2D _MainTex;
			
			sampler2D _MaskMap;
		
			float _Cutoff;
		
			sampler2D _LightMap;
			fixed _LightMapContrast;
			fixed _LightMapOffset;
			
			sampler2D _BumpMap;
			fixed _BumpQuantity;
			
			half _Specular;
			half _Gloss;
			
			half _AntiFlick;
			
			void vert (inout appdata_full v) {
				v.vertex.z -= _AntiFlick*v.vertex.w;
			}
			
			void surf (Input IN, inout SurfaceOutput o) {
				half4 albedo = tex2D(_MainTex, IN.uv_MainTex) * half4(_Color.rgb,1);
				half3 mask = tex2D(_MaskMap, IN.uv_MaskMap).rgb;
				albedo.a *= (mask.r + mask.g + mask.b) / 3 ;
				clip(albedo.a-_Cutoff);
				half3 bumpNormal = UnpackNormal(tex2D (_BumpMap, IN.uv_BumpMap));
				bumpNormal.xy *= _BumpQuantity;
				bumpNormal = normalize(bumpNormal);
				o.Normal = bumpNormal;
				half alpha = _Color.a * albedo.a;
				half4 lmc = tex2D (_LightMap, IN.uv2_LightMap);
				half3 lm = (_LightMapOffset + (lmc.rgb * (8 * lmc.a)) - 1) * _LightMapContrast + 1.0;
				o.Gloss = _Gloss * albedo.a;
				o.Specular = _Specular;
				o.Albedo = ( albedo.rgb  ) * lm;
				o.Alpha = alpha;
			}
			
			ENDCG
			ZWrite Off
			Cull Off
			Blend SrcAlpha OneMinusSrcAlpha
			CGPROGRAM
			
			#pragma surface surf BlinnPhong vertex:vert
			#include "UnityCG.cginc"
			
			struct Input {
				float2 uv_MainTex;
				float2 uv_BumpMap;
				float2 uv2_LightMap;
				float2 uv_MaskMap;
			};
			
			fixed4 _Color;
			
			sampler2D _MainTex;
			
			sampler2D _MaskMap;
		
			float _Cutoff;
		
			sampler2D _LightMap;
			fixed _LightMapContrast;
			fixed _LightMapOffset;
			
			sampler2D _BumpMap;
			fixed _BumpQuantity;
			
			half _Specular;
			half _Gloss;
			
			half _AntiFlick;
			
			void vert (inout appdata_full v) {
				v.vertex.z -= _AntiFlick*v.vertex.w;
			}
			
			void surf (Input IN, inout SurfaceOutput o) {
				half4 albedo = tex2D(_MainTex, IN.uv_MainTex) * half4(_Color.rgb,1);
				half3 mask = tex2D(_MaskMap, IN.uv_MaskMap).rgb;
				albedo.a *= (mask.r + mask.g + mask.b) / 3 ;
				clip(_Cutoff-albedo.a);
				clip(albedo.a-0.004);
				half3 bumpNormal = UnpackNormal(tex2D (_BumpMap, IN.uv_BumpMap));
				bumpNormal.xy *= _BumpQuantity;
				bumpNormal = normalize(bumpNormal);
				o.Normal = bumpNormal;
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
		Fallback "Transparent/Cutout/VertexLit"
		CustomEditor "OBSMaterialInspector"
}
