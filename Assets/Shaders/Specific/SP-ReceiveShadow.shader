// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Observ3d/Specific/SP-ReceiveShadow" { 
 
Properties 
{ 
 	
	_ShadowIntensity ("Shadow Intensity", Range (0, 1)) = 0.6
} 
 
 
SubShader 
{ 
	Tags { "Queue"="Background+2"  "IgnoreProjector"="True"  "RenderType"="Opaque" }
 
	LOD 300
 

 
 
 
		// Shadow Pass : Adding the shadows (from Directional Light)
		// by blending the light attenuation
		Pass {
			ZWrite On
			Blend SrcAlpha OneMinusSrcAlpha 
			Name "ShadowPass"
			Tags {"LightMode" = "ForwardBase"}
 
			CGPROGRAM 
// Upgrade NOTE: excluded shader from DX11 and Xbox360; has structs without semantics (struct v2f members lightDir)
#pragma exclude_renderers d3d11 xbox360
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fwdbase
			#pragma fragmentoption ARB_fog_exp2
			#pragma fragmentoption ARB_precision_hint_fastest
			#include "UnityCG.cginc"
			#include "AutoLight.cginc"
 
			struct v2f { 
				float2 uv_MainTex : TEXCOORD1;
				float4 pos : SV_POSITION;
				LIGHTING_COORDS(3,4)
				float3	lightDir;
			};
 
			
			float _ShadowIntensity;
 
			v2f vert (appdata_full v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos (v.vertex);
				o.lightDir = ObjSpaceLightDir( v.vertex );
				TRANSFER_VERTEX_TO_FRAGMENT(o);
				return o;
			}
 
			float4 frag (v2f i) : COLOR
			{
				float atten = LIGHT_ATTENUATION(i);
 
				half4 c;
				c.rgb =  0;
				c.a = (1-atten) * _ShadowIntensity; 
				return c;
			}
			ENDCG
		}
 
 
}
 
FallBack "VertexLit"
}


