// Copyright Elliot Bentine, 2018-
Shader "ProPixelizer/SRP/TerrainPixleizedWithOutline"
{
	//A shader that renders outline buffer data and color appearance for pixelated objects.
	//
	// If you want to add your own properties to the PixelizedWithOutline shader, or modify
	// the appearance of it, you can make changes to the ProPixelizerBase shader graph.
	// Make sure you follow these steps:
	//   1. Make your changes to the ProPixelizerBase graph, e.g. adding new properties,
	//      changing connections. Your changes will not be visible until Unity reloads this
	//      shader. The easiest way to trigger recompilation is to modify and re-saving
	//      this file.
	//   2. Make sure the UnityPerMaterial CBUFFER in the ProPixelizerPass below matches that
	//      in the generated ProPixelizerBase shader (you can view the generated shader from
	//      the inspector window).
	//   3. If you want to edit your new properties in editor, it might help to disable the
	//      CustomEditor at the bottom of this file.

	Properties
	{
		_BaseColor("Color", Color) = (1, 1, 1, 1)
		_AlphaClipThreshold("Alpha Clip Threshold", Range(0, 1)) = 0.5
		[NoScaleOffset]_LightingRamp("LightingRamp", 2D) = "white" {}
		_AmbientLight("AmbientLight", Color) = (0.1, 0.1, 0.1, 0.5019608)
		_TransitionSmoothness("TransitionSmoothness", Range(0, 1)) = 0.5
		_PixelSize("PixelSize", Range(1, 5)) = 2
		_PixelGridOrigin("PixelGridOrigin", Vector) = (0, 0, 0, 0)
		_EmissionColor("EmissionColor", Color) = (1, 1, 1, 0)
		_Albedo_ST("Albedo_ST", Vector) = (1, 1, 0, 0)
		_Color_1("Color 1", Color) = (1, 1, 1, 1)
		_Albedo("Albedo", 2D) = "white" {}
		_Normal_1("Normal 1", 2D) = "white" {}
		_Height_1("Height 1", 2D) = "white" {}
		_Height_1_Shift("Height 1 Shift", Range(-5, 5)) = 0
		_Tiling_1("Tiling 1", Vector) = (1, 1, 0, 0)
		_Offset_1("Offset 1", Vector) = (0, 0, 0, 0)
		_Color_2("Color 2", Color) = (1, 1, 1, 1)
		_Albedo_2("Albedo 2", 2D) = "white" {}
		_Normal_2("Normal 2", 2D) = "white" {}
		_Height_2("Height 2", 2D) = "white" {}
		_Height_2_Shift("Height 2 Shift", Range(-5, 5)) = 0
		_Tiling_2("Tiling 2", Vector) = (1, 1, 0, 0)
		_Offset_2("Offset 2", Vector) = (0, 0, 0, 0)
		_Color_3("Color 3", Color) = (1, 1, 1, 1)
		_Albedo_3("Albedo 3", 2D) = "white" {}
		_Normal_3("Normal 3", 2D) = "white" {}
		_Height_3("Height 3", 2D) = "white" {}
		_Height_3_Shift("Height 3 Shift", Range(-5, 5)) = 0
		_Tiling_3("Tiling 3", Vector) = (1, 1, 0, 0)
		_Offset_3("Offset 3", Vector) = (0, 0, 0, 0)
		_Color_4("Color 4", Color) = (1, 1, 1, 1)
		_Albedo_4("Albedo 4", 2D) = "white" {}
		_Normal_4("Normal 4", 2D) = "white" {}
		_Height_4("Height 4", 2D) = "white" {}
		_Height_4_Shift("Height 4 Shift", Range(-5, 5)) = 0
		_Tiling_4("Tiling 4", Vector) = (1, 1, 0, 0)
		_Offset_4("Offset 4", Vector) = (0, 0, 0, 0)
		_OutlineColor("OutlineColor", Color) = (1, 1, 1, 0.5019608)
		_EdgeHighlightColor("Edge Highlight Color", Color) = (0.5, 0.5, 0.5, 0.5058824)
		_ID("ID", Float) = 1
		_DiffuseVertexColorWeight("DiffuseVertexColorWeight", Float) = 0
		_EmissiveVertexColorWeight("EmissiveVertexColorWeight", Float) = 0
		[Toggle]COLOR_GRADING("Use Color Grading", Float) = 0
		[Toggle]USE_OBJECT_POSITION("Use Object Position", Float) = 1
		[Toggle]RECEIVE_SHADOWS("Receive Shadows", Float) = 1
		[Toggle]PROPIXELIZER_DITHERING("Use Dithering", Float) = 0
		[HideInInspector]_QueueOffset("_QueueOffset", Float) = 0
		[HideInInspector]_QueueControl("_QueueControl", Float) = -1
		[HideInInspector][NoScaleOffset]unity_Lightmaps("unity_Lightmaps", 2DArray) = "" {}
		[HideInInspector][NoScaleOffset]unity_LightmapsInd("unity_LightmapsInd", 2DArray) = "" {}
		[HideInInspector][NoScaleOffset]unity_ShadowMasks("unity_ShadowMasks", 2DArray) = "" {}
	}
		SubShader
		{
			Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" }

			UsePass "ProPixelizer/Hidden/TerrainProPixelizerBase/UNIVERSAL FORWARD"
			UsePass "ProPixelizer/Hidden/TerrainProPixelizerBase/SHADOWCASTER"
			UsePass "ProPixelizer/Hidden/TerrainProPixelizerBase/DEPTHONLY"
			UsePass "ProPixelizer/Hidden/TerrainProPixelizerBase/DEPTHNORMALS"

		Pass
		{
			Name "ProPixelizerPass"
			Tags {
				"RenderPipeline" = "UniversalRenderPipeline"
				"LightMode" = "ProPixelizer"
				"DisableBatching" = "True"
			}

			ZWrite On
			Cull Off
			Blend Off

			HLSLPROGRAM
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "PixelUtils.hlsl"
			#include "PackingUtils.hlsl"
			#include "ScreenUtils.hlsl" 
			#pragma vertex outline_vert
			#pragma fragment outline_frag
			#pragma target 2.5
			#pragma multi_compile_local USE_OBJECT_POSITION_ON _
			#pragma multi_compile USE_ALPHA_ON _
			#pragma multi_compile NORMAL_EDGE_DETECTION_ON _
			#pragma multi_compile_local PROPIXELIZER_DITHERING_ON _
			
			// If you want to use the SRP Batcher:
			// The CBUFFER has to match that generated from ShaderGraph - otherwise all hell breaks loose.
			// In some cases, it might be easier to just break SRP Batching support for your outline shader.
			// Graph Properties
			CBUFFER_START(UnityPerMaterial)
			float4 _LightingRamp_TexelSize;
			float4 _Albedo_TexelSize;
			float4 _Albedo_ST;
			float4 _BaseColor;
			float _AlphaClipThreshold;
			float4 _Albedo_2_TexelSize;
			float4 _Albedo_2_ST;
			float4 _Normal_1_TexelSize;
			float4 _Normal_1_ST;
			float4 _Color_2;
			float2 _Tiling_2;
			float2 _Offset_2;
			float4 _Height_2_TexelSize;
			float4 _Height_2_ST;
			float _Height_2_Shift;
			float4 _AmbientLight;
			float4 _Normal_2_TexelSize;
			float4 _Normal_2_ST;
			float _PixelSize;
			float4 _PixelGridOrigin;
			float4 _Albedo_3_TexelSize;
			float4 _Albedo_3_ST;
			float4 _Color_3;
			float4 _Normal_3_TexelSize;
			float4 _Normal_3_ST;
			float2 _Tiling_3;
			float2 _Offset_3;
			float4 _Height_3_TexelSize;
			float4 _Height_3_ST;
			float _Height_3_Shift;
			float4 _Albedo_4_TexelSize;
			float4 _Albedo_4_ST;
			float4 _Color_4;
			float4 _Normal_4_TexelSize;
			float4 _Normal_4_ST;
			float2 _Tiling_4;
			float2 _Offset_4;
			float4 _Height_4_TexelSize;
			float4 _Height_4_ST;
			float _Height_4_Shift;
			float _ID;
			float4 _OutlineColor;
			float4 _EdgeHighlightColor;
			float4 _EmissionColor;
			float _DiffuseVertexColorWeight;
			float _EmissiveVertexColorWeight;
			float _TransitionSmoothness;
			float4 _Color_1;
			float2 _Tiling_1;
			float2 _Offset_1;
			float4 _Height_1_TexelSize;
			float4 _Height_1_ST;
			float _Height_1_Shift;
			CBUFFER_END

			// Object and Global properties
			SAMPLER(SamplerState_Linear_Repeat);
			SAMPLER(SamplerState_Point_Clamp);
			SAMPLER(SamplerState_Point_Repeat);
			TEXTURE2D(_LightingRamp);
			SAMPLER(sampler_LightingRamp);
			TEXTURE2D(_PaletteLUT);
			SAMPLER(sampler_PaletteLUT);
			float4 _PaletteLUT_TexelSize;
			TEXTURE2D(_Albedo);
			SAMPLER(sampler_Albedo);
			TEXTURE2D(_Albedo_2);
			SAMPLER(sampler_Albedo_2);
			TEXTURE2D(_Normal_1);
			SAMPLER(sampler_Normal_1);
			TEXTURE2D(_Height_2);
			SAMPLER(sampler_Height_2);
			TEXTURE2D(_Normal_2);
			SAMPLER(sampler_Normal_2);
			TEXTURE2D(_Emission);
			SAMPLER(sampler_Emission);
			float4 _Emission_TexelSize;
			TEXTURE2D(_Albedo_3);
			SAMPLER(sampler_Albedo_3);
			TEXTURE2D(_Normal_3);
			SAMPLER(sampler_Normal_3);
			TEXTURE2D(_Height_3);
			SAMPLER(sampler_Height_3);
			TEXTURE2D(_Albedo_4);
			SAMPLER(sampler_Albedo_4);
			TEXTURE2D(_Normal_4);
			SAMPLER(sampler_Normal_4);
			TEXTURE2D(_Height_4);
			SAMPLER(sampler_Height_4);
			TEXTURE2D(_Height_1);
			SAMPLER(sampler_Height_1);

			#include "OutlinePass.hlsl"
			ENDHLSL
		}
     }
	CustomEditor "TerrainPixelizedWithOutlineShaderGUI"
	FallBack "ProPixelizer/Hidden/TerrainProPixelizerBase"
}
