Shader "Observ3d/Specific/SP-Texture-Alpha-Matrix" {
	Properties {
		_Color ("Main Color", COLOR) = (1,1,1,1)
		_MainTex ("Base (RGB)", 2D) = "white" {}
	}
	SubShader {
		Pass {
			Tags { "RenderType"="Transparent" }
			LOD 200
			ZWrite Off
	        Blend SrcAlpha OneMinusSrcAlpha
	
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"
			
			float4 _Color;
			sampler2D _MainTex;
			
			struct v2f {
			    float4  pos : SV_POSITION;
			    float2  uv : TEXCOORD0;
			};
			
			float4x4 _Rotation;
			
			float4 _MainTex_ST;
			
			v2f vert (appdata_base v)
			{
			    v2f o;
			    float2 texcoordMainTex = mul(_Rotation, v.texcoord);
			    o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
			    o.uv = TRANSFORM_TEX (texcoordMainTex, _MainTex);
			    return o;
			}
			
			half4 frag (v2f i) : COLOR
			{
				
				half4 tex = tex2D (_MainTex, i.uv);
				
				return half4(tex.r, tex.g, tex.b, tex.a) * _Color.a;
			}
			ENDCG
		}
    }
}
