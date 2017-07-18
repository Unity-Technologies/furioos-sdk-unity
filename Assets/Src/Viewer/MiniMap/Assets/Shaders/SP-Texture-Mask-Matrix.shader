// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Observ3d/Specific/SP-Texture-Mask-Matrix" {
	Properties {
		_Color ("Main Color", COLOR) = (1,1,1,1)
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Alpha ("Alpha (A)", 2D) = "white" {}
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
			sampler2D _Alpha;
			
			struct v2f {
			    float4  pos : SV_POSITION;
			    float2  uv : TEXCOORD0;
			    float2	uv2 : TEXCOORD1;
			};
			
			float4x4 _Rotation;
			
			float4 _MainTex_ST;
			float4 _Alpha_ST;
			
			v2f vert (appdata_base v)
			{
			    v2f o;
			    float2 texcoordMainTex = mul(_Rotation, v.texcoord);
			    o.pos = UnityObjectToClipPos (v.vertex);
			    o.uv = TRANSFORM_TEX (v.texcoord, _MainTex);
			    o.uv2 = TRANSFORM_TEX (v.texcoord, _Alpha);
			    return o;
			}
			
			half4 frag (v2f i) : COLOR
			{
			    half4 tex = tex2D (_MainTex, i.uv);
			    half4 texAlpha = tex2D (_Alpha, i.uv2);
			    
			    return (half4(tex.r, tex.g, tex.b, tex.a));
			}
			ENDCG
		}
    }
}
