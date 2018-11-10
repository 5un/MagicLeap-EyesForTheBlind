// ---------------------------------------------------------------------
//
// Copyright (c) 2018 Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Creator Agreement, located
// here: https://id.magicleap.com/creator-terms
//
// ---------------------------------------------------------------------


Shader "Magic Leap/Mask/DepthMask" {
   
    SubShader {

       
        Tags {"Queue" = "Geometry-10" }
        Lighting Off
        ZTest LEqual
        ZWrite On
        ColorMask 0

        Pass {}
    }
}