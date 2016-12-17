Shader "Observ3d/Specific/SP-Lightmap-Anim-Veget" {
	Properties {
		_Color ("Main Color", Color) = (1, 1, 1, 1)
		_MainTex ("Base (RGB) Alpha (A)", 2D) = "white" {}
		_Cutoff ("Base Alpha cutoff", Range (0,1)) = 0.8
		_Exposure ("Exposure", Range (0.1, 10)) = 1
		_Offset ("0ffset", Range (0, 1)) = 0
		_LightMap ("Lightmap (RGB)", 2D) = "black" {}
		
		_Breeze ("Breeze intensity", Range(0.0, 1.0)) = 0.75
		_WindForce("Wind force", Range(0.0, 1.0)) = 0.4
		_WindDir ("Wind direction", Vector) = (1.0, 0.5, 0.0, 0.0)
	}

	CGINCLUDE
	#include "UnityCG.cginc"
	
	struct appdata_t {
		float4 vertex : POSITION;
		float2 texcoord : TEXCOORD0;
		float2 texcoord1 : TEXCOORD1;
	};

	struct v2f {
		float4 vertex : POSITION;
		float2 texcoord : TEXCOORD0;
		float2 texcoord1 : TEXCOORD1;
	};
	
	sampler2D _MainTex;
	sampler2D _LightMap;
	float4 _MainTex_ST;
	float4 _LightMap_ST;
	fixed _Exposure;
	fixed _Offset;
	float _Cutoff;
	
	fixed  _Breeze;
	fixed  _WindForce;
	fixed4 _WindDir;
	
	fixed rand(fixed2 co)
	{
		fixed val = sin(dot(co.xy, fixed2(12.9898,78.233))) * 43758.5453;
	    return (val - floor(val));
	}
				
	v2f vert (appdata_t v)
	{
		v2f o;
		
		fixed brz   = clamp(_Breeze, 0.0, 1.0);
		fixed wndF  = clamp(_WindForce, 0.0, 1.0);
		fixed2 wndD = normalize(_WindDir.xy);
		
		fixed rv1 = (_Time.w * 0.25 + rand(v.vertex.xy) * 42.0);
		fixed rv2 = (_Time.w * 0.25 * 0.74 + rand(v.vertex.xz) * 404.0);
		fixed rv3 = (_Time.w * 0.25 * 1.13 + rand(v.vertex.yz) * 1337.0);
		
		fixed wind_force      = pow(sin(_Time.w * 0.02 +sin(_Time.w*0.2)), 8.0);
		
		fixed2 wind_offset    = wndD * wndF;
		fixed3 breeze_offset  = brz * fixed3(0.125 * sin(rv1), 0.125 * cos(rv2), 0.025 * sin(cos(rv3)*3.14159));
		
		fixed3 final_offset;
		final_offset.x = lerp(breeze_offset.x, wind_offset.x, wind_force);
		final_offset.y = lerp(breeze_offset.y, wind_offset.y, wind_force);
		final_offset.z = breeze_offset.z;
		
		o.vertex = mul(UNITY_MATRIX_MVP, v.vertex + fixed4(final_offset, 0.0));
		o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
		o.texcoord1 = TRANSFORM_TEX(v.texcoord1, _LightMap);
		return o;
	}
	
	float4 _Color;
	half4 frag_cutoff(v2f i) : COLOR
	{ 
		fixed4 albedo = tex2D(_MainTex, i.texcoord) * _Color;
		fixed3 lm = _Offset + DecodeLightmap(tex2D (_LightMap, i.texcoord1));
		half4 col = half4(albedo.rgb*lm*_Exposure,albedo.a);
		clip(col.a - _Cutoff);
		return col;
	}
	
	half4 frag_translucent(v2f i) : COLOR
	{
		fixed4 albedo = tex2D(_MainTex, i.texcoord) * _Color;
		fixed3 lm = _Offset + DecodeLightmap(tex2D (_LightMap, i.texcoord1));
		half4 col = half4(albedo.rgb*lm*_Exposure,albedo.a);
		
		//half4 col = _Color * tex2D(_MainTex, i.texcoord);
		clip(-(col.a - _Cutoff));
		return col;
	}
	ENDCG

SubShader {
	Tags { "Queue"="AlphaTest" "RenderType"="Opaque" }
	Lighting off
	
	// Render both front and back facing polygons.
	Cull Off
	
	// first pass:
	//   render any pixels that are more than [_Cutoff] opaque
	Pass {  
		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag_cutoff
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
			#pragma fragment frag_translucent
		ENDCG
	}
}
}
