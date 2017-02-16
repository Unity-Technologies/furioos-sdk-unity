// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Observ3d/Specific/SP-Water"
{
   Properties
   {
      _Cube   ("Cube Map", Cube)               = "" {}
      _Normal ("Normal map", 2D)               = "bump" {}
      _Color  ("Color", Color)                 = (1, 1, 1, 1)
      _MainTex("Main texture", 2D)             = "white" {}
      _Reflect("Reflection", Range(0.0, 1.0))  = 0.75
      _Refract("Refraction", Range(0.0, 1.0))  = 0.75
      _IOR("IOR", Range(0.01, 0.1))           = 0.1
      _Deform ("Deformation", Range(0.0, 0.4)) = 0.1
   }
   
   SubShader
   {
      Pass
      {   
         CGPROGRAM
 
         #pragma vertex   vert  
         #pragma fragment frag
		 #pragma target 3.0
		
		 #include "UnityCG.cginc"
 
         uniform samplerCUBE _Cube;
         uniform sampler2D   _Normal;
         uniform sampler2D   _MainTex;
         uniform float4      _Color;
         uniform float       _Reflect;
         uniform float       _Refract;
         uniform float       _IOR;
         uniform float       _Deform;
 
         struct vertexInput
         {
            float4 vertex   : POSITION;
            float3 normal   : NORMAL;
            float4 tangent  : TANGENT;
            float3 texcoord : TEXCOORD0;
         };
         
         struct vertexOutput
         {
            float4 pos       : POSITION;
            float3 texcoord  : TEXCOORD0;
            float3 viewDir   : TEXCOORD1;
            float3 tangentWorld  : TEXCOORD2;  
            float3 normalWorld   : TEXCOORD3;
            float3 binormalWorld : TEXCOORD4;
         };
 
         vertexOutput vert(vertexInput input) 
         {
            vertexOutput output;
 
            output.viewDir   = float3(mul(unity_ObjectToWorld, input.vertex) - float4(_WorldSpaceCameraPos, 1.0));
            output.pos       = mul(UNITY_MATRIX_MVP, input.vertex);
            output.texcoord  = input.texcoord;
            
            output.tangentWorld  = normalize(float3(mul(unity_ObjectToWorld, float4(float3(input.tangent), 0.0))));
            output.normalWorld   = normalize(mul(unity_ObjectToWorld, float4(input.normal, 0.0))).xyz;//normalize(mul(float4(input.normal, 0.0), _World2Object));
            output.binormalWorld = normalize(cross(output.normalWorld, output.tangentWorld) * input.tangent.w);
            
            return output;
         }
 
         float4 frag(vertexOutput input) : COLOR
         {
         	float3 orig_nrm = input.normalWorld;
         	float3 albedo = tex2D(_MainTex, input.texcoord.xy).rgb;
         	albedo *= _Color.rgb;
         	
         	float3x3 local2WorldTranspose = float3x3(input.tangentWorld, input.binormalWorld, input.normalWorld);
         	float3x3 local2World = transpose(local2WorldTranspose);
         	
         	float3 texnrm1 = tex2D(_Normal, input.texcoord.xy * 4.0 + float2(_Time.w * 0.06, _Time.w * 0.02)).xyz;
         	float3 texnrm2 = tex2D(_Normal, input.texcoord.xy * 4.0 + float2(_Time.w * 0.03, -_Time.w * 0.02 + 0.5)).xyz;
         	float3 texnrm  = normalize(texnrm1 + texnrm2);
         	texnrm.xy = texnrm.xy * 2.0 - float2(1.0);
         	
         	float3 bump_nrm = normalize(mul(local2World, texnrm).xyz);
         	       
         	float3 nrm = lerp(orig_nrm, bump_nrm, _Deform);
         
            float3 reflectedDir = reflect(input.viewDir, normalize(nrm));
            float4 reflected    = texCUBE(_Cube, reflectedDir);
            
            float3 refractedDir = refract(input.viewDir, normalize(nrm), _IOR);
            float4 refracted    = texCUBE(_Cube, refractedDir);
            
            float3 viewDir = input.viewDir;
            float fresnel = -dot(normalize(viewDir), normalize(nrm));
            
            float3 fresnelColor = lerp(reflected, refracted, fresnel).rgb;
               
            return lerp(float4(albedo, 1.0), float4(fresnelColor, 1.0), _Reflect);
         }
 
         ENDCG
      }
   }
}