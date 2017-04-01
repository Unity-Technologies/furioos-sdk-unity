Shader "Observ3d/Light Responsive/Color/Transparent/LR-Color-Alpha-Bump" {
	Properties {
		_Color ("Main Color", Color) = (1,1,1,1)
		
		_BumpMap ("Bump Map", 2D) = "bump" {}
		_BumpQuantity ("Bump Quantity", Range (0, 2)) = 1
		
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
				float2 uv_BumpMap;
			};
			
			fixed4 _Color;
			
			sampler2D _BumpMap;
			fixed _BumpQuantity;
			
			half _Specular;
			half _Gloss;
			
			half _AntiFlick;
			
			void vert (inout appdata_full v) {
				v.vertex.z -= _AntiFlick*v.vertex.w;
			}
			
			void surf (Input IN, inout SurfaceOutput o) {
				half4 albedo = half4(_Color.rgb,1);
				half3 bumpNormal = UnpackNormal(tex2D (_BumpMap, IN.uv_BumpMap));
				bumpNormal.xy *= _BumpQuantity;
				bumpNormal = normalize(bumpNormal);
				o.Normal = bumpNormal;
				half alpha = _Color.a * albedo.a;
				o.Gloss = _Gloss * albedo.a;
				o.Specular = _Specular;
				o.Albedo = albedo.rgb ;
				o.Alpha = alpha;
			}
			
			ENDCG
	}
		Fallback "Transparent/VertexLit"
		CustomEditor "OBSMaterialInspector"
}
