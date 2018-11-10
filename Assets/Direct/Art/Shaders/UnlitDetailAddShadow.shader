// ---------------------------------------------------------------------
//
// Copyright (c) 2018 Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Creator Agreement, located
// here: https://id.magicleap.com/creator-terms
//
// ---------------------------------------------------------------------


Shader "Magic Leap/Unlit/DetailAddShadow" {
Properties {
	_MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
	_DetailTex ("Detail (RGB) Trans (A)", 2D) = "white" {}
	_PatternTex ("Pattern (RGB) Mask (A)", 2D) = "white" {}
	_ShadowColor ("Shadow Color", Color) = (1,1,1,1)
	_ShadowFade ("Shadow Fade", Range(0,1)) = 1
	_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
	_Fade ("Fade", Range(0,1)) = 1
	_PatFade ("Pattern Fade", Range(0,1)) = 1

	 //_Ramp ("Ramp", 2D) = "white" {}
     //_LightVector ("LightVector", Vector) = (0,0,0,0)
}
SubShader {
	Tags {"Queue"="geometry" "RenderType"="Opaque"}
	LOD 200

	//Lighting Off

	Pass {  

	        Name "FORWARD"
			Tags { "LightMode" = "ForwardBase" }

		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			#pragma multi_compile_fwdadd_fullshadows

			
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "AutoLight.cginc"




			struct v2f {
				float4 pos : SV_POSITION;
				half2 texcoord : TEXCOORD0;
				half2 texcoord2 : TEXCOORD1;
				half2 texcoord3 : TEXCOORD2;


			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			sampler2D _DetailTex;
			float4 _DetailTex_ST;
			sampler2D _PatternTex;
			float4 _PatternTex_ST;
			fixed _Cutoff;
			fixed _Fade;
			fixed _ShadowFade;
			fixed _PatFade;
			fixed4 _ShadowColor; 

			v2f vert (float4 pos : POSITION, float3 normal : NORMAL, float2 texcoord : TEXCOORD0,float2 texcoord2 : TEXCOORD1, float2 texcoord3 : TEXCOORD2)
			{
				v2f o;
				o.pos = UnityObjectToClipPos (pos);
				o.texcoord = TRANSFORM_TEX(texcoord, _MainTex);
				o.texcoord2 = TRANSFORM_TEX(texcoord, _DetailTex);
				o.texcoord3 = TRANSFORM_TEX(texcoord, _PatternTex);



				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{

				fixed4 col = tex2D(_MainTex, i.texcoord);
				fixed4 detail = tex2D(_DetailTex, i.texcoord2);
				fixed4 pattern = tex2D(_PatternTex, i.texcoord3);
				fixed4 c = 1;
				clip(col.a - _Cutoff);
				fixed4 colorcomp = lerp(col,col*saturate(pattern+_PatFade),pattern.a);
				fixed4 detailcomp=colorcomp*lerp(colorcomp,1,saturate((detail.r)+_Fade));

				c.rgb*=detailcomp.rgb;
				return (detailcomp);

			}
		ENDCG
	}
	Pass
		{
			Name "FORWARD"
			Tags { "LightMode" = "ForwardAdd" }
			ZWrite Off Blend DstColor Zero

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// Include shadowing support for point/spot
			#pragma multi_compile_fwdadd_fullshadows
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "AutoLight.cginc"

			struct v2f
			{
				float4 pos : SV_POSITION;
				float3 worldPos : TEXCOORD6;
				SHADOW_COORDS(1)
			};

			fixed _ShadowFade;
			fixed4 _ShadowColor;
			v2f vert (appdata_full v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos (v.vertex);
				o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				TRANSFER_SHADOW(o); // pass shadow coordinates to pixel shader
				return o;
			}

			fixed4 frag (v2f IN) : SV_Target
			{
				UNITY_LIGHT_ATTENUATION(atten, IN, IN.worldPos)
				fixed4 c = lerp(_ShadowColor,1,saturate(saturate(atten)+_ShadowFade));
				// might want to take light color into account?
				//c.rgb *= _LightColor0.rgb;
				return c;
			}

			ENDCG
		}
	UsePass "VertexLit/SHADOWCASTER"
}

}
