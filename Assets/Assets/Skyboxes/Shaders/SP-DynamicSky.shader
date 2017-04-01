// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Observ3d/Specific/SP-DynamicSky" {
	Properties {
		_MainTex   ("Stars (RGB)", 2D) = "black" {}
		_SunOrientation ("Sun Orientation", Vector) = (0.5773,0.5773,0.5773,1.0)
		_AmbianceCoefficients("Day Progress", Vector) = (0.0,0.0,1.0,0.0)
		_SunColor ("Sun Color", Color) =   (0.5,0.4,0.3,1.0)
		_MorningZenithColor ("Morning Zenith Color", Color) =   (0.15,0.32,0.53,1.00)
		_MorningMediumColor ("Morning Menium Color", Color) =   (0.58,0.63,0.76,1.00)
		_MorningHorizonColor ("Morning Horizon Color", Color) = (0.80,0.69,0.55,1.00)
		
		_NoonZenithColor ("Noon Zenith Color", Color) =         (0.29,0.54,0.82,1.00)
		_NoonMediumColor ("Noon Medium Color", Color) =         (0.63,0.83,1.00,1.00)
		_NoonHorizonColor ("Noon Horizon Color", Color) =       (0.83,0.94,1.00,1.00)
		
		_EveningZenithColor ("Evening Zenith Color", Color) =   (0.30,0.31,0.50,1.00)
		_EveningMediumColor ("Evening Medium Color", Color) =   (0.98,0.88,0.70,1.00)
		_EveningHorizonColor ("Evening Horizon Color", Color) = (0.84,0.43,0.24,1.00)
	}
	SubShader {
	
		Tags { "RenderType"="Opaque" "Queue"="Background" "IgnoreProjector"="True" }
		Fog {Mode Off}
		
		
         Pass { 
			Cull Back
			ZWrite Off
			Lighting Off
			
			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			//#pragma target   3.0
			
			
			float3 _SunColor; 
			fixed3 _MorningZenithColor;
			fixed3 _MorningMediumColor;
			fixed3 _MorningHorizonColor;
			fixed3 _NoonZenithColor;
			fixed3 _NoonMediumColor;
			fixed3 _NoonHorizonColor;
			fixed3 _EveningZenithColor;
			fixed3 _EveningMediumColor;
			fixed3 _EveningHorizonColor;
			
			float3 _SunOrientation;
			float4 _AmbianceCoefficients;
				
			sampler2D _MainTex;
			float4 _MainTex_ST;	
			
				struct appdata_t {
					float4 vertex : POSITION;
					float2 uv : TEXCOORD0;
				};
				
				
				struct v2f {
					    float4  pos : SV_POSITION;
					    float2  uv : TEXCOORD0;
					    float3 	orient : TEXCOORD1;
					    
					};
				
				v2f vert (appdata_t v)
				{
				    v2f o;
				    o.pos = UnityObjectToClipPos (v.vertex);
				    o.pos.z = 0.9999999 * o.pos.w;
				    o.uv = TRANSFORM_TEX (v.uv, _MainTex);
				    o.orient = float3(
									-sin(o.uv.x*6.283185307)*cos(o.uv.y*1.570796327),
									sin(o.uv.y*1.570796327),
									-cos(o.uv.x*6.283185307)*cos(o.uv.y*1.570796327)
							);
				    return o;
				}
				
				half4 frag (v2f IN) : COLOR
				{
					half3 stars = tex2D(_MainTex, IN.uv).rgb;
					
					half3 horizonColor = stars*_AmbianceCoefficients.x + _MorningHorizonColor*_AmbianceCoefficients.y + _NoonHorizonColor*_AmbianceCoefficients.z + _EveningHorizonColor*_AmbianceCoefficients.w;
					half3 mediumColor = stars*_AmbianceCoefficients.x + _MorningMediumColor*_AmbianceCoefficients.y + _NoonMediumColor*_AmbianceCoefficients.z + _EveningMediumColor*_AmbianceCoefficients.w;
					half3 zenithColor = stars*_AmbianceCoefficients.x + _MorningZenithColor*_AmbianceCoefficients.y + _NoonZenithColor*_AmbianceCoefficients.z + _EveningZenithColor*_AmbianceCoefficients.w;
					
					
					//float3 currentOrientation = float3(
							//-sin(IN.uv.x*6.283185307)*cos(IN.uv.y*1.570796327),
							//sin(IN.uv.y*1.570796327),
							//-cos(IN.uv.x*6.283185307)*cos(IN.uv.y*1.570796327)
					//);*/
							
					
					
					//float dotOrient = clamp(dot(currentOrientation,_SunOrientation),0,1);
					
					//float dotOrient = clamp(currentOrientation.x-_SunOrientation.x+currentOrientation.y*_SunOrientation.y+currentOrientation.z*_SunOrientation.z,0,1);
					
					float orientDistance = distance(IN.orient,_SunOrientation);
					
					float3 haloColor = _SunColor*(1.0-smoothstep(0.02,1.41,orientDistance));
					float3 sunColor = _SunColor * (1.0-smoothstep(0.005,0.03,orientDistance));
					
					float3 clearSkyColor = lerp(lerp(horizonColor,mediumColor,smoothstep(0.00, 0.15, IN.uv.y)),zenithColor,smoothstep(0.15, 0.8, IN.uv.y));
					
					///float4 res;
					//res.rgb = clearSkyColor+sunColor+haloColor;//lerp(clear_sky_color, clear_sun_color, sun_intensity);
					//res.a   = 1.0;
					//return half4(1.0f,0.0f,0.0f,1.0f);
					return fixed4(clearSkyColor+sunColor+haloColor,1.0f);
	
				}
				

			ENDCG
		}
		 
	}
	FallBack "Diffuse"
}
