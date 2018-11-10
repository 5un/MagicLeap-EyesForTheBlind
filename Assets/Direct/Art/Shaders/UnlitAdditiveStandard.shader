// ---------------------------------------------------------------------
//
// Copyright (c) 2018 Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Creator Agreement, located
// here: https://id.magicleap.com/creator-terms
//
// ---------------------------------------------------------------------

  Shader "Magic Leap/Unlit/AdditiveStandard" {
    Properties {
      
 
      _MainTex ("Texture", 2D) = "Black" {}
      _Multi ("Multiplier", Range(0.0,2)) = 0.0
      _Color ("Main Color",Color)= (1,1,1,1)


      
    }
    SubShader {
      Tags { "Queue"="Transparent" "RenderType" = "Transparent" }
      LOD 200
      ZWrite Off
      Lighting Off
      Cull off
      ZTest LEqual
      Blend One One 
      Fog { Mode Off}
      CGPROGRAM
     #pragma surface surf Additive  halfasview novertexlights exclude_path:prepass noambient noforwardadd nolightmap nodirlightmap

      half4 LightingAdditive (SurfaceOutput s, half3 lightDir, half3 viewDir) {
		half3 h = normalize (lightDir + viewDir);


          half4 c;
          c.rgb = s.Albedo;
          c.a = s.Alpha;
          return c;
      }
      struct Input {
          float2 uv_MainTex;
      
      };
      
      sampler2D _MainTex;
      float _Multi;
      float3 _Color;




      
      void surf (Input IN, inout SurfaceOutput o) {
	      half4 maintex = tex2D (_MainTex, IN.uv_MainTex);
      	  float mask =maintex.a;

          o.Emission = (saturate(maintex*_Multi))*_Color;
 
      }
      ENDCG
    } 
    Fallback "Diffuse"
  }