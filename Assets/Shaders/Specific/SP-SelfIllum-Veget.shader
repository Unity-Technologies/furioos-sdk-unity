// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Observ3d/Specific/SP-Selfillum-Veget" {
	Properties {
		_Color ("Main Color", Color) = (1, 1, 1, 1)
		_MainTex ("Base (RGB) Alpha (A)", 2D) = "white" {}
		_Cutoff ("Base Alpha cutoff", Range (0,1)) = 0.8
		_Exposure ("Exposure", Range (0.1, 10)) = 1
	}

SubShader {
	Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="TransparentCutout" }
	Lighting On
	
	// Render both front and back facing polygons.
	Cull Off
	
	// first pass:
	//   render any pixels that are more than [_Cutoff] opaque
	Pass {  
		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata_t {
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
				float3 normal : NORMAL;
			};

			struct v2f {
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
				float viewCoeff:TEXCOORD1;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			fixed _Exposure;
			float _Cutoff;
			
			v2f vert (appdata_t v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
				half3 worldNormal = mul((float3x3)unity_ObjectToWorld,v.normal);
				half4 worldPosition = mul(unity_ObjectToWorld,v.vertex);
				half3 worldViewDir = normalize(_WorldSpaceCameraPos.xyz - worldPosition);
				o.viewCoeff = abs(dot(worldViewDir,worldNormal));
				return o;
			}
			
			float4 _Color;
			half4 frag (v2f i) : COLOR
			{
				fixed4 albedo = tex2D(_MainTex, i.texcoord) * _Color;
				
				
				
				if(i.viewCoeff<0.5)albedo.a *= i.viewCoeff*2;
				
				half4 col = half4(albedo.rgb*_Exposure,albedo.a);
				clip(col.a - _Cutoff);
				return col;
			}
		ENDCG
	}

	// Second pass:
	//   render the semitransparent details.
	Pass {
		Tags { "RequireOption" = "SoftVegetation" }
		
		// Dont write to the depth buffer
		ZWrite off
		Cull off
		// Set up alpha blending
		Blend SrcAlpha OneMinusSrcAlpha
		
		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata_t {
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
				float3 normal : NORMAL;
			};

			struct v2f {
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
				float viewCoeff:TEXCOORD1;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			fixed _Exposure;
			float _Cutoff;
			
			v2f vert (appdata_t v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
				half3 worldNormal = mul((float3x3)unity_ObjectToWorld,v.normal);
				half4 worldPosition = mul(unity_ObjectToWorld,v.vertex);
				half3 worldViewDir = normalize(_WorldSpaceCameraPos.xyz - worldPosition);
				o.viewCoeff = abs(dot(worldViewDir,worldNormal));
				return o;
			}
			
			float4 _Color;
			half4 frag (v2f i) : COLOR
			{
				fixed4 albedo = tex2D(_MainTex, i.texcoord) * _Color;
				
				
				
				if(i.viewCoeff<0.5)albedo.a *= i.viewCoeff*2;
				
				half4 col = half4(albedo.rgb*_Exposure,albedo.a);
				clip(-(col.a - _Cutoff));
				return col;
			}
		ENDCG
	}
}
Fallback "Transparent/Cutout/VertexLit"
}
