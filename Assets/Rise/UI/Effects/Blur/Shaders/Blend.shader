Shader "Rise/Blend" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_BlurTex ("Base (RGB)", 2D) = "white" {}
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		Pass {
			CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#include "UnityCG.cginc"
			
				struct v2f {
					float4 pos : SV_POSITION;
					float2 uv_MainTex : TEXCOORD0;
				};
			
				float4 _MainTex_ST;
				float4 _BlurTex_ST;
			
				v2f vert(appdata_base v) {
					v2f o;
					o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
					o.uv_MainTex = TRANSFORM_TEX(v.texcoord, _MainTex);
					return o;
				}
			
				sampler2D _MainTex;
				sampler2D _BlurTex;
			
				float4 frag(v2f IN) : COLOR {
					half4 ui = tex2D (_MainTex, IN.uv_MainTex);
					half4 blur = tex2D(_BlurTex, IN.uv_MainTex);
					
					half4 c = ui;
					
					if(ui.r + ui.g + ui.b == 0) {
						c = blur;
					}
					
					return c;
				}
			ENDCG
		}
	}
}