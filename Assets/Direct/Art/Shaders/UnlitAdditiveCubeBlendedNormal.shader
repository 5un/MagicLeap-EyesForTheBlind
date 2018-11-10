// ---------------------------------------------------------------------
//
// Copyright (c) 2018 Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Creator Agreement, located
// here: https://id.magicleap.com/creator-terms
//
// ---------------------------------------------------------------------

  Shader "Magic Leap/Unlit/AdditiveCubeBlendedNormal" {
    Properties {
      
 
      _MainTex ("Texture", 2D) = "black" {}
      _Color1 ("Color", Color) = (1,1,1)
      _Bump ("Normal", 2D) = "bump" {}
      _Bump2 ("Normal2", 2D) = "bump" {}
      _Reflect ("LightMulti", Range(0.0,5)) = 0.0
      _Brightness ("Brightness", Range(0.0,5)) = 0.0
      _RefCube ("LightCube", CUBE) = "" {}

      
      _ScrollSpeed ("Scroll",Range(-10,10)) = 2

      _Active ("Active",Range(0,1)) = 1


      
    }
    SubShader {
      Tags { "Queue"="geometry" "RenderType" = "Opaque" }
      LOD 200
   
      Lighting Off
      Cull Back
      Blend One One 
      CGPROGRAM
     #pragma surface surf Unlit  novertexlights exclude_path:prepass noambient noforwardadd nolightmap nodirlightmap

      half4 LightingUnlit (SurfaceOutput s, half3 lightDir, half3 viewDir) {
		half3 h = normalize (lightDir + viewDir);
		s.Normal =normalize(s.Normal);


          half4 c;
          c.rgb = s.Albedo;
          c.a = s.Alpha;
          return c;
      }
      struct Input {
          float2 uv_MainTex;
          float2 uv_Bump2;
          float2 uv_Bump;
          float3 worldNormal;
          float3 worldRefl;
          INTERNAL_DATA
      
      };
      
      sampler2D _MainTex;
      sampler2D _Bump2;
      sampler2D _Bump;

      samplerCUBE _RefCube;
      float3 _Color1;

      float _Brightness;
      float _Reflect;

      
      float _ScrollSpeed;
      
      float _Active;
   
      

      
      void surf (Input IN, inout SurfaceOutput o) {
          fixed2 scrolledUVR = IN.uv_Bump;
          fixed ScrollValueR = _ScrollSpeed * _Time;
          scrolledUVR += fixed2(ScrollValueR,ScrollValueR);
          
          fixed2 scrolledUVG = IN.uv_Bump2;
          fixed ScrollValueG = (_ScrollSpeed*.5)* _Time;
          scrolledUVG += fixed2(ScrollValueG,ScrollValueG);
          
          half3 normalcomp = (UnpackNormal (tex2D (_Bump, scrolledUVR))*.5)+(UnpackNormal (tex2D (_Bump2, scrolledUVG ))*.5);
          o.Normal =  normalcomp;
	      half4 maintex = tex2D (_MainTex, IN.uv_MainTex);
	      float3 refcube = texCUBE(_RefCube, WorldReflectionVector(IN, o.Normal)).rgb*_Reflect;
      	  float mask =maintex.a;
      	  float3 activelerp = lerp(refcube*_Color1,refcube*refcube,_Active);
      	  
          o.Albedo = lerp(0,(refcube*refcube)*_Reflect,mask);

      }
      ENDCG
    } 
    Fallback "Diffuse"
  }