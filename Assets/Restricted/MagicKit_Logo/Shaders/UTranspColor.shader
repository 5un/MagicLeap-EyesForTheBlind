// ---------------------------------------------------------------------
//
// Copyright (c) 2018 Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Creator Agreement, located
// here: https://id.magicleap.com/creator-terms
//
// ---------------------------------------------------------------------

 Shader "Magic Leap/Unlit/Transparent Color"
 {
	Properties
	{
		_Color ("Tint Color", Color) = (1,1,1,1)
    }

	SubShader
	{
		Tags { "RenderType" = "Transparent" "Queue"="Transparent"}
      
		Lighting Off
		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha

		CGPROGRAM
		#pragma surface surf Unlit alpha
		#pragma target 3.0

		struct Input
		{
			fixed4 _Color;
          INTERNAL_DATA
		};

		fixed4 _Color;

		half4 LightingUnlit(SurfaceOutput s, half3 lightDir, half atten) {
			half4 c;
			c.rgb = s.Albedo;
			c.a = s.Alpha;
			return c;
		}

		void surf (Input IN, inout SurfaceOutput o)
		{
			o.Albedo = _Color.rgb;
			o.Alpha = _Color.a;
		}
		ENDCG
	}

    Fallback "Diffuse"
}