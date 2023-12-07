// Copyright Elliot Bentine, 2018-
Shader "ProPixelizer/SRP/KMJPixelizedWithOutline"
{
	// 픽셀화된 오브젝트의 아웃라인 버퍼 데이터와 색상 모양을 렌더링하는 셰이더
	//
	// 픽셀화된 아웃라인 셰이더에 자체 프로퍼티를 추가하거나 모양을 수정하려면
	// 모양을 변경하려면 KMJProPixelizerBase 셰이더 그래프를 변경하자
	// 
	// 1. 새 프로퍼티를 추가하는 등 ProPixelizerBase 그래프를 변경
	// 연결을 변경합니다. 변경 사항은 Unity가 이 셰이더를 다시 로드할 때까지 표시x
	// 셰이더를 다시 로드할 때까지 변경 사항이 표시 X 리컴파일을 트리거하는 가장 쉬운 방법은 이 파일을 수정하고 다시 저장
	// 2. 아래 ProPixelizerPass의 UnityPerMaterial CBUFFER가 생성된 프로픽셀라이저 패스에서
	// 일치하는지 확인합니다(생성된 셰이더는 인스펙터 창에서 // 확인
	// 인스펙터 창에서 확인
	// 3. 에디터에서 새 프로퍼티를 편집하려면 하단에 있는
	// CustomEditor를 비활성화하면 도움됨

    Properties
    {
		_LightingRamp("LightingRamp", 2D) = "white" {}
		_PaletteLUT("PaletteLUT", 2D) = "white" {}
		[MainTex][NoScaleOffset]_Albedo("Albedo", 2D) = "white" {}
		_Albedo_ST("Albedo_ST", Vector) = (1, 1, 0, 0)
		[MainColor]_BaseColor("Color", Color) = (1, 1, 1, 1)
		_AmbientLight("AmbientLight", Color) = (0.2, 0.2, 0.2, 1.0)
		[IntRange] _PixelSize("PixelSize", Range(1, 5)) = 3
		_PixelGridOrigin("PixelGridOrigin", Vector) = (0, 0, 0, 0)
		[Normal][NoScaleOffset]_NormalMap("Normal Map", 2D) = "bump" {}
		_NormalMap_ST("Normal Map_ST", Vector) = (1, 1, 0, 0)
		[NoScaleOffset]_Emission("Emission", 2D) = "white" {}
		_Emission_ST("Emission_ST", Vector) = (1, 1, 0, 0)
		_EmissionColor("EmissionColor", Color) = (1, 1, 1, 0)
		_AlphaClipThreshold("Alpha Clip Threshold", Range(0, 1)) = 0.5
		[IntRange] _ID("ID", Range(0, 255)) = 1 // 윤곽선을 위해 객체를 구분하는 데 사용되는 고유 ID
		_OutlineColor("OutlineColor", Color) = (0.0, 0.0, 0.0, 0.5)
		_EdgeHighlightColor("Edge Highlight Color", Color) = (0.5, 0.5, 0.5, 0)
		_DiffuseVertexColorWeight("DiffuseVertexColorWeight", Range(0, 1)) = 1
		_EmissiveVertexColorWeight("EmissiveVertexColorWeight", Range(0, 1)) = 0
		[HideInInspector][NoScaleOffset]unity_Lightmaps("unity_Lightmaps", 2DArray) = "" {}
		[HideInInspector][NoScaleOffset]unity_LightmapsInd("unity_LightmapsInd", 2DArray) = "" {}
		[HideInInspector][NoScaleOffset]unity_ShadowMasks("unity_ShadowMasks", 2DArray) = "" {}
		[Toggle]COLOR_GRADING("Use Color Grading", Float) = 0
		[Toggle]USE_OBJECT_POSITION("Use Object Position", Float) = 1
		[Toggle]RECEIVE_SHADOWS("ReceiveShadows", Float) = 1
		[Toggle]PROPIXELIZER_DITHERING("Use Dithering", Float) = 1
	}

		SubShader
		{
			//Tags { "RenderType" = "Transparent" "RenderPipeline" = "UniversalPipeline"  }
			Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline"  }

			UsePass "ProPixelizer/Hidden/ProPixelizerBase/UNIVERSAL FORWARD"
			UsePass "ProPixelizer/Hidden/ProPixelizerBase/SHADOWCASTER"
			UsePass "ProPixelizer/Hidden/ProPixelizerBase/DEPTHONLY"
			UsePass "ProPixelizer/Hidden/ProPixelizerBase/DEPTHNORMALS"

		Pass
		{
			Name "ProPixelizerPass"
			Tags {
				"RenderPipeline" = "UniversalRenderPipeline"
				"LightMode" = "ProPixelizer"
				"DisableBatching" = "True"
			}

			ZWrite On	// 추후에 반투명 ( 논 디더링) 형태에서 Zwrite를 off 하고 Blend 는 On 해야되는거 아닐까?
			Cull Off
			Blend Off	//
			//Blend SrcAlpha OneMinusSrcAlpha

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
			
			// SRP 배처를 사용하려는 경우:
			// 버퍼가 셰이더그래프에서 생성된 것과 일치시키자
			// 경우에 따라서는 아웃라인 셰이더에 대한 SRP 배칭 지원을 중단하는 것이 더 쉬울 수도 있다 (큰 차이는 모르겠음)
			// 그래프 프로퍼티
			CBUFFER_START(UnityPerMaterial)
			float4 _LightingRamp_TexelSize;
			float4 _PaletteLUT_TexelSize;
			float4 _Albedo_TexelSize;
			float4 _Albedo_ST;
			float4 _BaseColor;
			float4 _AmbientLight;
			float _PixelSize;
			float4 _PixelGridOrigin;
			float4 _NormalMap_TexelSize;
			float4 _NormalMap_ST;
			float4 _Emission_TexelSize;
			float4 _Emission_ST;
			float _AlphaClipThreshold;
			float _ID;
			float4 _OutlineColor;
			float4 _EdgeHighlightColor;
			float4 _EmissionColor;
			float _DiffuseVertexColorWeight;
			float _EmissiveVertexColorWeight;
			CBUFFER_END
			
			// Object and Global properties
			SAMPLER(SamplerState_Linear_Repeat);
			SAMPLER(SamplerState_Point_Clamp);
			SAMPLER(SamplerState_Point_Repeat);
			TEXTURE2D(_LightingRamp);
			SAMPLER(sampler_LightingRamp);
			TEXTURE2D(_PaletteLUT);
			SAMPLER(sampler_PaletteLUT);
			TEXTURE2D(_Albedo);
			SAMPLER(sampler_Albedo);
			TEXTURE2D(_NormalMap);
			SAMPLER(sampler_NormalMap);
			TEXTURE2D(_Emission);
			SAMPLER(sampler_Emission);

			#include "OutlinePass.hlsl"
			ENDHLSL
		}
     }
	CustomEditor "KMJPixelizedWithOutlineShaderGUI"
	FallBack "ProPixelizer/Hidden/ProPixelizerBase"
}
