// ---------------------------------------------------------------------
//
// Copyright (c) 2018 Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Creator Agreement, located
// here: https://id.magicleap.com/creator-terms
//
// ---------------------------------------------------------------------



Shader "Magic Leap/Unlit/AlphaDetailShadow" {
Properties {
	_MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
	_DetailTex ("Detail (RGB) Trans (A)", 2D) = "white" {}
	_PatternTex ("Pattern (RGB) Mask (A)", 2D) = "white" {}
	_ShadowColor ("Shadow Color", Color) = (1,1,1,1)
	_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
	_Fade ("Fade", Range(0,1)) = 1
	_PatFade ("Pattern Fade", Range(0,1)) = 1
}
SubShader {
	Tags {"Queue"="geometry" "RenderType"="Opaque"}
	LOD 200

	Lighting Off

	Pass {  

	        Name "FORWARD"
			Tags { "LightMode" = "ForwardBase" }
		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			//#pragma target 3.0
			#pragma multi_compile_fwdbase nolightmap nodynlightmap novertexlight

			
			#include "UnityCG.cginc"
			//#include "Lighting.cginc"
			#include "AutoLight.cginc"




			struct v2f {
				float4 pos : SV_POSITION;
				half2 texcoord : TEXCOORD3;
				half2 texcoord2 : TEXCOORD1;
				half2 texcoord3 : TEXCOORD2;
				SHADOW_COORDS(0)

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
			fixed4 _ShadowColor;

			v2f vert (appdata_full v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos (v.vertex);
				o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
				o.texcoord2 = TRANSFORM_TEX(v.texcoord2, _DetailTex);
				o.texcoord3 = TRANSFORM_TEX(v.texcoord3, _PatternTex);

				TRANSFER_SHADOW(o);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.texcoord);
				fixed4 detail = tex2D(_DetailTex, i.texcoord2);
				fixed4 pattern = tex2D(_PatternTex, i.texcoord3);
				UNITY_LIGHT_ATTENUATION(atten, i, 0)
				fixed4 c = 1;
				clip(col.a - _Cutoff);
				fixed4 colorcomp = lerp(col,col*saturate(pattern+_PatFade),pattern.a);
				//fixed shadowcomp = ceil(detail.g-1+saturate(atten));
				fixed shadowcomp = saturate(atten);
				fixed4 shadowcolor = lerp(_ShadowColor,1,(shadowcomp));
				fixed4 detailcomp=colorcomp*lerp(colorcomp,1,saturate((detail.r)+_Fade));

				c.rgb*=detailcomp.rgb;
				return detailcomp*shadowcolor;
			}
		ENDCG
	}
	UsePass "VertexLit/SHADOWCASTER"
}

}
