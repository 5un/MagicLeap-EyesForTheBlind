// ---------------------------------------------------------------------
//
// Copyright (c) 2018 Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Creator Agreement, located
// here: https://id.magicleap.com/creator-terms
//
// ---------------------------------------------------------------------

 Shader "Magic Leap/BRDF/TextureLumaTintFade"
 {
	Properties
	{
		_Color ("Tint Color", Color) = (1,1,1,1)
		_MainTex ("Texture", 2D) = "white" {}
		_RampLight ("Ramp Light", 2D) = "Black" {}
		_Fade ("Fade", Range(0.0,1.0)) = 1.0
		_LightVector ("Light Vector", Vector) = (0,0,0,0)
    }

	SubShader
	{
		Tags { "RenderType" = "Transparent" "IgnoreProjector"="True" "Queue"="Transparent"}
      
		Lighting Off
		Blend SrcAlpha OneMinusSrcAlpha

		Pass {
        ZWrite On
        ColorMask 0
    }
		CGPROGRAM
		#pragma surface surf RampLight novertexlights exclude_path:prepass noambient noforwardadd nolightmap nodirlightmap alpha
		#pragma target 3.0

		struct Input
		{
			float2 uv_MainTex;
			float2 uv_Ramp;
		};

		sampler2D _MainTex;
		sampler2D _RampLight;
		half _Fade;
		float4 _Color;
		float4 _LightVector;

		half4 LightingRampLight (SurfaceOutput s, half3 lightDir, half3 viewDir, fixed atten)
		{
			float light = dot(s.Normal,_LightVector);
			float rim = dot(s.Normal,viewDir);
			float diff = (light*.5)+.5;
				
			float2 brdfdiff = float2(rim, diff);
			float3 BRDFLight = tex2D(_RampLight, brdfdiff.xy).rgb;
			half3 BRDFColor = (s.Albedo);
			
      
			half4 c;
			c.rgb =BRDFColor*BRDFLight*_Fade;
			c.a = s.Alpha;
			return c;
		}

		void surf (Input IN, inout SurfaceOutput o)
		{
			half4 maintex = tex2D (_MainTex, IN.uv_MainTex);
			o.Albedo = maintex*_Color.rgb;
			o.Alpha = _Color.a;
		}
		ENDCG
	}

    Fallback "Unlit/Transparent"
}