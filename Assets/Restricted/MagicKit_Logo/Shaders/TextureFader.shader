// ---------------------------------------------------------------------
//
// Copyright (c) 2018 Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Creator Agreement, located
// here: https://id.magicleap.com/creator-terms
//
// ---------------------------------------------------------------------

Shader "TextureFader"
{
	Properties
	{
		_MainTexture("Main Texture", 2D) = "white" {}
		_MaskTexture("Mask Texture", 2D) = "white" {}
		_BlendAmount("BlendAmount", Range( 0 , 1)) = 1
		_EmitA("Emit A", Range( 0 , 1)) = 0
		_EmitB("Emit B", Range( 0 , 1)) = 0
		_EmitC("Emit C", Range( 0 , 1)) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		Cull Back
		CGPROGRAM
		#pragma target 3.0
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows exclude_path:deferred nolightmap  nodynlightmap nodirlightmap nofog nometa noforwardadd 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform float _BlendAmount;
		uniform sampler2D _MainTexture;
		uniform float4 _MainTexture_ST;
		uniform sampler2D _MaskTexture;
		uniform float4 _MaskTexture_ST;
		uniform float _EmitA;
		uniform float _EmitB;
		uniform float _EmitC;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_MainTexture = i.uv_texcoord * _MainTexture_ST.xy + _MainTexture_ST.zw;
			float4 tex2DNode1 = tex2D( _MainTexture, uv_MainTexture );
			float layeredBlendVar5 = _BlendAmount;
			float4 layeredBlend5 = ( lerp( float4(1,1,1,1),tex2DNode1 , layeredBlendVar5 ) );
			o.Albedo = layeredBlend5.rgb;
			float2 uv_MaskTexture = i.uv_texcoord * _MaskTexture_ST.xy + _MaskTexture_ST.zw;
			float4 tex2DNode7 = tex2D( _MaskTexture, uv_MaskTexture );
			o.Emission = ( tex2DNode1 * ( ( tex2DNode7.r * _EmitA ) + ( tex2DNode7.g * _EmitB ) + ( _EmitC * tex2DNode7.b ) ) ).rgb;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=15401
3447;152;1906;1044;1090.968;548.0937;1.3;True;True
Node;AmplifyShaderEditor.RangedFloatNode;9;-539,452;Float;False;Property;_EmitA;Emit A;3;0;Create;True;0;0;False;0;0;0.661;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;7;-563.9,257.3;Float;True;Property;_MaskTexture;Mask Texture;1;0;Create;True;0;0;False;0;8a5c9a11f17c2334388fabc956c83ec4;6307ef07febf6a5449553a92cf0145c3;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;15;-537,624;Float;False;Property;_EmitC;Emit C;5;0;Create;True;0;0;False;0;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;14;-543,540;Float;False;Property;_EmitB;Emit B;4;0;Create;True;0;0;False;0;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;13;-183.6,393.8001;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;16;-184.2,497.6001;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;8;-179.4,288.7001;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;6;-335.5999,-250.7001;Float;False;Property;_BlendAmount;BlendAmount;2;0;Create;True;0;0;False;0;1;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;17;18.9,344.1;Float;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;1;-351.4,51.8;Float;True;Property;_MainTexture;Main Texture;0;0;Create;True;0;0;False;0;8a5c9a11f17c2334388fabc956c83ec4;3850e90231cbd034cb78b000eb7feca1;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;2;-268.8002,-127.3999;Float;False;Constant;_White;White;1;0;Create;True;0;0;False;0;1,1,1,1;0,0,0,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LayeredBlendNode;5;39.7,-78.99997;Float;True;6;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;18;193.4321,173.4062;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;515.2998,-9.000107;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;TextureFader;False;False;False;False;False;False;True;True;True;True;True;True;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;ForwardOnly;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;-1;False;-1;-1;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;13;0;7;2
WireConnection;13;1;14;0
WireConnection;16;0;15;0
WireConnection;16;1;7;3
WireConnection;8;0;7;1
WireConnection;8;1;9;0
WireConnection;17;0;8;0
WireConnection;17;1;13;0
WireConnection;17;2;16;0
WireConnection;5;0;6;0
WireConnection;5;1;2;0
WireConnection;5;2;1;0
WireConnection;18;0;1;0
WireConnection;18;1;17;0
WireConnection;0;0;5;0
WireConnection;0;2;18;0
ASEEND*/
//CHKSM=D5E0EFA06C6E890FE40B7F51029335740342A13E