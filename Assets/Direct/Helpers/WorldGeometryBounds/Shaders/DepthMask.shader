// ---------------------------------------------------------------------
//
// Copyright (c) 2018 Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Creator Agreement, located
// here: https://id.magicleap.com/creator-terms
//
// ---------------------------------------------------------------------

Shader "Magic Kit/Depth Mask"
{
	SubShader
	{
		Tags{ "Queue" = "Geometry-1" }

		ColorMask 0
		ZWrite On

		Pass{}
	}
}