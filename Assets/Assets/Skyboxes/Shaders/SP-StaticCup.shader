// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Observ3d/Specific/SP-StaticCup" {
	Properties {
		_Color ("Main Color", Color) = (1,1,1,1)
		_MainTex ("Diffuse (RGB)", 2D) = "white" {}
		_OrientationOffset ("Orientation Offset", Range (0, 1)) = 0
		
	}
	SubShader {
		
		Tags { "RenderType"="TransparentCutout" "Queue"="Background+1" "IgnoreProjector"="True" }
		Fog {Mode Off}
		
		Pass { 
			Lighting off
			Cull Back
			ZWrite On
								
			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
         
			fixed3 _Color;
				
			sampler2D _MainTex;
			float4 _MainTex_ST;
			
			half _OrientationOffset;
			
				struct appdata_t {
					float4 vertex : POSITION;
					float2 uv : TEXCOORD0;
				};
				
				struct v2f {
				    float4  pos : SV_POSITION;
				    float2  uv : TEXCOORD0;
				};
				
				v2f vert (appdata_t v)
				{
				    v2f o;
				    o.pos = UnityObjectToClipPos (v.vertex);
				    o.uv = TRANSFORM_TEX (float2(v.uv.x+_OrientationOffset,v.uv.y), _MainTex);
				    return o;
				}
				
				fixed4 frag (v2f i) : COLOR
				{
					half balance = min(max(0.5-i.uv.y,0) *4,1);
					half4 albedo = tex2D(_MainTex, i.uv);
					fixed4 col = fixed4((albedo.rgb * (1-balance) + _Color.rgb * (balance)),albedo.a);
					clip(col.a - 0.7);
					return col;
				}
				
			ENDCG
		}
		
		Pass { 
			Lighting off
			Cull Back
			ZWrite Off
			Blend SrcAlpha OneMinusSrcAlpha	
								
			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
         
			fixed3 _Color;
			fixed3 _AmbientColor;
				
			sampler2D _MainTex;
			float4 _MainTex_ST;
			
			half _OrientationOffset;
			
				struct appdata_t {
					float4 vertex : POSITION;
					float2 uv : TEXCOORD0;
				};
				
				struct v2f {
				    float4  pos : SV_POSITION;
				    float2  uv : TEXCOORD0;
				};
				
				v2f vert (appdata_t v)
				{
				    v2f o;
				    o.pos = UnityObjectToClipPos (v.vertex);
				    o.uv = o.uv = TRANSFORM_TEX (float2(v.uv.x+_OrientationOffset,v.uv.y), _MainTex);
				    return o;
				}
				
				fixed4 frag (v2f i) : COLOR
				{
					half balance = min(max(0.5-i.uv.y,0) *4,1);
					half4 albedo = tex2D(_MainTex, i.uv);
					fixed4 col = fixed4((albedo.rgb * (1-balance) + _Color.rgb * (balance)),albedo.a);
					clip(0.7 - col.a);
					return col;
				}
				
			ENDCG
		}
		
	}
}
