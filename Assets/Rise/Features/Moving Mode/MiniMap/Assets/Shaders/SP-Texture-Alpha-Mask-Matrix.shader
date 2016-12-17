Shader "Observ3d/Specific/SP-Texture-Alpha-Mask-Matrix" {
	Properties {
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
			
			sampler2D _MainTex;
			sampler2D _Alpha;
			
			struct v2f {
			    float4  pos : SV_POSITION;
			    float2  uv : TEXCOORD0;
			    float2	uv2 : TEXCOORD1;
			};
			
			float4x4 _Rotation;
			float _Test;
			
			float4 _MainTex_ST;
			float4 _Alpha_ST;
			
			v2f vert (appdata_base v)
			{
			    v2f o;
			    o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
			    o.uv = mul(_Rotation, v.texcoord);
			    o.uv2 = TRANSFORM_TEX (v.texcoord, _Alpha);
			    return o;
			}
			
			half4 frag (v2f i) : COLOR
			{
				clip(min(i.uv.x, i.uv.y));
				clip(1-max(i.uv.x, i.uv.y));
				
				//i.uv.x = i.uv2.x;
				//i.uv.y = i.uv2.y;
				
				half4 tex = tex2D (_MainTex, i.uv);
			    half4 texAlpha = tex2D (_Alpha, i.uv2);
			    
			    if(texAlpha.a < 1)
			    	discard;
			    
			    return half4(tex.r, tex.g, tex.b, 1.0);
			    //return half4(_Rotation[0][0], 0, 0, 1);
			}
			ENDCG
		}
    }
}