// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Observ3d/Specific/SP-Texture-Mask" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Alpha ("Alpha (A)", 2D) = "white" {}
        _AmountAlpha ("Alpha Amount", Range(0.0, 1.0)) = 0.8
	}
	SubShader {
		Pass {
			Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
			LOD 200
			ZWrite Off
	        Blend SrcAlpha OneMinusSrcAlpha
	
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"
			
			sampler2D _MainTex;
			sampler2D _Alpha;
			
			struct v2f {
			    float4  pos : SV_POSITION;
			    float2  uv : TEXCOORD0;
			    float2	uv2 : TEXCOORD1;
			};
			
			float4x4 _Rotation;
			float _Test;
			
			fixed _AmountAlpha;
			
			float4 _MainTex_ST;
			float4 _Alpha_ST;
			
			v2f vert (appdata_base v)
			{
			    v2f o;
			    o.pos = UnityObjectToClipPos (v.vertex);
			    o.uv = TRANSFORM_TEX (v.texcoord, _MainTex);
			    return o;
			}
			
			half4 frag (v2f i) : COLOR
			{
				
			    half4 texAlpha = tex2D (_Alpha, i.uv);
				half4 tex = tex2D (_MainTex, float2(1 - i.uv.x, i.uv.y));
			    
			   	if(texAlpha.a < _AmountAlpha)
					return tex;
				else 
					return half4(0, 0, 0, 0);
			   			
			}
			ENDCG
		}
    }
}