Shader "Hidden/Observ3d/PostProduction" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "" {}
		//_Strength ("Strength", Range (1, 100)) = 100
		//_Radius ("Radius", Range (0.00001, 1)) = 0.00001
		_WadadaTex ("Wadada (RGB)", 2D) = "" {}
		_WadadaAlpha ("Wadada Alpha", Range (0, 1)) = 0.1
		_Saturation ("Saturation", Range (0.1, 3)) = 1
		_Black ("Black", Color) = (0.0,0.0,0.0,1)
		_Gamma ("Gamma", Color) = (0.1,0.1,0.1,1)
		_White ("White", Color) = (1.0,1.0,1.0,1)
	}
	
	// Shader code pasted into all further CGPROGRAM blocks
	CGINCLUDE

	#pragma fragmentoption ARB_precision_hint_fastest
	
	#include "UnityCG.cginc"
	
	struct v2f {
		float4 pos : POSITION;
		float2 uv : TEXCOORD0;
	};
	
	sampler2D _MainTex;
	sampler2D _WadadaTex;
	float _WadadaAlpha;
	float4 _Black;
	float4 _Gamma;
	float4 _White;
	fixed _Saturation;
	//fixed _Strength;
	//fixed _Radius;
	
	v2f vert( appdata_img v ) 
	{
		v2f o;
		o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
		o.uv = v.texcoord.xy;
		return o;
	} 
	
	float4 frag(v2f i) : COLOR 
	{
		float4 colorBase = tex2D(_MainTex, i.uv);
		//colorBase -= tex2D(_MainTex, i.uv + _Radius)*_Strength;
		//colorBase += tex2D(_MainTex, i.uv - _Radius)*_Strength;
		//colorBase += tex2D(_MainTex, half2(i.uv.x + _Radius , i.uv.y - _Radius))*_Strength;
		//colorBase -= tex2D(_MainTex, half2(i.uv.x - _Radius , i.uv.y + _Radius))*_Strength;
		
		float3 wadada = tex2D(_WadadaTex, i.uv).rgb;
		
		float3 leveled = pow((colorBase.rgb - _Black.rgb) / (_White.rgb - _Black.rgb),_Gamma.rgb*10);
		
		float3 wadaded = leveled * (_WadadaAlpha*(leveled + 2*wadada * (1-leveled)) + (1-_WadadaAlpha));

		float grayScale = (wadaded.r + wadaded.g + wadaded.b)/3;
		
		float3 saturated = saturate((wadaded-grayScale)*_Saturation+grayScale);
		
		if((saturated.r + saturated.g + saturated.b) == 0)
			return float4(colorBase);
		else
			return float4(saturated,colorBase.a);
	}

	ENDCG 
	
Subshader {
 Pass {
	  ZTest Off Cull Off ZWrite Off
	  Fog { Mode off }      

      CGPROGRAM
      #pragma vertex vert
      #pragma fragment frag
      ENDCG
  }
}

Fallback off
	
} // shader

