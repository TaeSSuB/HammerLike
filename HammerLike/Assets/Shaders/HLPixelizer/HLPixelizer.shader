// Copyright Elliot Bentine, 2018-
Shader "HLPixelizer/SRP/HLPixelizer"
{

    Properties
    {
        _BaseColor("BaseColor", Color) = (1, 1, 1, 1)
        _AlphaClipThreshold("Alpha Clip Threshold", Float) = 0.5
        _Albedo("Albedo", 2D) = "white" {}
        _Albedo_ST("Albedo_ST", Vector) = (1, 1, 0, 0)
        _Normal_Strength("Normal Strength", Range(0, 5)) = 1
        [Normal]_NormalMap("Normal Map", 2D) = "bump" {}
        _NormalMap_ST("Normal Map_ST", Vector) = (1, 1, 0, 0)
        [HDR]_EmissionColor("Emission Color", Color) = (0, 0, 0, 0)
        _Emission("Emission", 2D) = "black" {}
        _Emission_ST("Emission_ST", Vector) = (1, 1, 0, 0)
        _AmbientLight("Ambient Light", Color) = (0, 0, 0, 0.5019608)
        [NoScaleOffset]_LightingRamp("Lighting Ramp", 2D) = "white" {}
        _Lighting_Step("Lighting Step", Range(0, 100)) = 5
        [ToggleUI]_EnableDynamicLights("Enable Dynamic Lights", Float) = 0
        [ToggleUI]_Specular("Specular", Float) = 0
        _SpecularColor("Specular Color", Color) = (1, 1, 1, 1)
        _SpecularPower("Specular Power", Range(0, 50)) = 3
        [ToggleUI]_ReflectionLight("Reflection Light", Float) = 0
        _ReflectionLightColor("Reflection Light Color", Color) = (0, 0, 1, 1)
        _ReflectionLightPower("Reflection Light Power", Range(0, 50)) = 3
        [ToggleUI]_RimLight("Rim Light", Float) = 0
        _RimLightColor("Rim Light Color", Color) = (1, 1, 1, 1)
        _RimLightPower("Rim Light Power", Range(0, 50)) = 3
        [Toggle]RECEIVE_SHADOWS("Receive Shadows", Float) = 1
        [Toggle]COLOR_GRADING("Use Color Grading", Float) = 0
        [ToggleUI]_Show_Vertex_Color_Weight("Show Vertex Color Weight", Float) = 0
        _Hue("Hue", Float) = 0
        _Saturation("Saturation", Float) = 1
        _Contrast("Contrast", Float) = 1
        _PixelSize("Pixel Size", Range(1, 5)) = 2
        [Toggle]USE_OBJECT_POSITION("Use Object Position", Float) = 1
        _PixelGridOrigin("Pixel Grid Origin", Vector) = (0, 0, 0, 0)
        _ID("ID", Float) = 1
        _OutlineColor("Inline Color", Color) = (0, 0, 0, 0.5019608)
        _EdgeHighlightColor("Edge Highlight Color", Color) = (1, 1, 1, 0.5019608)
        [HideInInspector]_QueueOffset("_QueueOffset", Float) = 0
        [HideInInspector]_QueueControl("_QueueControl", Float) = -1
        [HideInInspector][NoScaleOffset]unity_Lightmaps("unity_Lightmaps", 2DArray) = "" {}
        [HideInInspector][NoScaleOffset]unity_LightmapsInd("unity_LightmapsInd", 2DArray) = "" {}
        [HideInInspector][NoScaleOffset]unity_ShadowMasks("unity_ShadowMasks", 2DArray) = "" {}
    }
		SubShader
		{
			Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" }

			UsePass "HLPixelizer/Hidden/HLPixelizer/UNIVERSAL FORWARD"
			UsePass "HLPixelizer/Hidden/HLPixelizer/SHADOWCASTER"
			UsePass "HLPixelizer/Hidden/HLPixelizer/DEPTHONLY"
			UsePass "HLPixelizer/Hidden/HLPixelizer/DEPTHNORMALS"

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
            #include "Assets/ProPixelizer/SRP/PixelUtils.hlsl"
            #include "Assets/ProPixelizer/SRP/PackingUtils.hlsl"
            #include "Assets/ProPixelizer/SRP/ScreenUtils.hlsl"
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
            float _Saturation;
            float _Contrast;
            float4 _LightingRamp_TexelSize;
            float4 _Albedo_TexelSize;
            float4 _Albedo_ST;
            float _RimLightPower;
            float4 _RimLightColor;
            float _RimLight;
            float _ReflectionLightPower;
            float4 _ReflectionLightColor;
            float _ReflectionLight;
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
            float _Hue;
            float _Normal_Strength;
            float _SpecularPower;
            float4 _SpecularColor;
            float _EnableDynamicLights;
            float _Specular;
            float _Show_Vertex_Color_Weight;
            float _Lighting_Step;
            CBUFFER_END

            // Object and Global properties
            SAMPLER(SamplerState_Point_Clamp);
            TEXTURE2D(_LightingRamp);
            SAMPLER(sampler_LightingRamp);
            TEXTURE2D(_PaletteLUT);
            SAMPLER(sampler_PaletteLUT);
            float4 _PaletteLUT_TexelSize;
            TEXTURE2D(_Albedo);
            SAMPLER(sampler_Albedo);
            TEXTURE2D(_NormalMap);
            SAMPLER(sampler_NormalMap);
            TEXTURE2D(_Emission);
            SAMPLER(sampler_Emission);
            float _DiffuseVertexColorWeight;
            float _EmissiveVertexColorWeight;
            SAMPLER(SamplerState_Point_Repeat);

			#include "Assets/ProPixelizer/SRP/OutlinePass.hlsl"
			ENDHLSL
		}
     }
	FallBack "ProPixelizer/Hidden/ProPixelizerBase"
}
