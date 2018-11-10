// ---------------------------------------------------------------------
//
// Copyright (c) 2018 Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Creator Agreement, located
// here: https://id.magicleap.com/creator-terms
//
// ---------------------------------------------------------------------


Shader "Magic Leap/Unlit/AdditiveCutoutDetail" {
Properties {
	_MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
	_DetailTex ("Detail (RGB) Trans (A)", 2D) = "white" {}
	_PatternTex ("Pattern (RGB) Mask (A)", 2D) = "white" {}
	_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
	_Fade ("Fade", Range(0,1)) = 1
	_PatFade ("Pattern Fade", Range(0,1)) = 1
}
SubShader {
	Tags {"Queue"="Transparent" "RenderType"="Transparent"}
	LOD 100

	Lighting Off
	zwrite off
	blend one one

	Pass {  
		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 2.0
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"

			struct appdata_t {
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
				float2 texcoord2 : TEXCOORD1;
				float2 texcoord3 : TEXCOORD2;
			};

			struct v2f {
				float4 vertex : SV_POSITION;
				half2 texcoord : TEXCOORD0;
				half2 texcoord2 : TEXCOORD1;
				half2 texcoord3 : TEXCOORD2;
				UNITY_FOG_COORDS(1)
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			sampler2D _DetailTex;
			float4 _DetailTex_ST;
			sampler2D _PatternTex;
			float4 _PatternTex_ST;
			fixed _Cutoff;
			fixed _Fade;
			fixed _PatFade;

			v2f vert (appdata_t v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
				o.texcoord2 = TRANSFORM_TEX(v.texcoord2, _DetailTex);
				o.texcoord3 = TRANSFORM_TEX(v.texcoord, _PatternTex);
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.texcoord)*_Cutoff;
				fixed4 detail = tex2D(_DetailTex, i.texcoord2);
				fixed4 pattern = tex2D(_PatternTex, i.texcoord3);
				clip(col.a - _Cutoff);
				fixed4 colorcomp = lerp(col,col*saturate(pattern+_PatFade),pattern.a);
				UNITY_APPLY_FOG(i.fogCoord, col);
				return colorcomp*lerp(colorcomp,1,saturate(detail.r+_Fade));
			}
		ENDCG
	}
}

}
