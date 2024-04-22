// Copyright Elliot Bentine, 2018-
Shader "HLPixelizer/SRP/HLPixelizerTerrain"
{

    Properties
    {
        _BaseColor("BaseColor", Color) = (1, 1, 1, 1)
        _TransitionSmoothness("TransitionSmoothness", Range(0, 1)) = 0.5
        _AlphaClipThreshold("Alpha Clip Threshold", Float) = 0.5
        _Color_4("Color 1", Color) = (1, 1, 1, 1)
        _Albedo("Albedo 1", 2D) = "white" {}
        [Normal]_Normal_1("Normal 1", 2D) = "bump" {}
        _Normal_1_Strength("Normal 1 Strength", Range(0, 5)) = 1
        [NoScaleOffset]_Height_1("Height 1", 2D) = "white" {}
        _Height_1_Shift("Height 1 Shift", Range(-5, 5)) = 0
        _Tiling_1("Tiling 1", Vector) = (1, 1, 0, 0)
        _Offset_1("Offset 1", Vector) = (0, 0, 0, 0)
        _AmbientLight("Ambient Light 1", Color) = (0, 0, 0, 0.5019608)
        [NoScaleOffset]_LightingRamp("Lighting Ramp 1", 2D) = "white" {}
        _Color_1("Color 2", Color) = (1, 1, 1, 1)
        _Albedo_2("Albedo 2", 2D) = "white" {}
        [Normal][NoScaleOffset]_Normal_2("Normal 2", 2D) = "bump" {}
        _Normal_2_Strength("Normal 2 Strength", Range(0, 5)) = 1
        [NoScaleOffset]_Height_2("Height 2", 2D) = "white" {}
        _Height_2_Shift("Height 2 Shift", Range(-5, 5)) = 0
        _Tiling_2("Tiling 2", Vector) = (1, 1, 0, 0)
        _Offset_2("Offset 2", Vector) = (0, 0, 0, 0)
        _AmbientLight_1("Ambient Light 2", Color) = (0, 0, 0, 0.5019608)
        [NoScaleOffset]_LightingRamp_1("Lighting Ramp 2", 2D) = "white" {}
        _Color_2("Color 3", Color) = (1, 1, 1, 1)
        [NoScaleOffset]_Albedo_3("Albedo 3", 2D) = "white" {}
        [Normal]_Normal_3("Normal 3", 2D) = "bump" {}
        _Normal_3_Strength("Normal 3 Strength", Range(0, 5)) = 1
        [NoScaleOffset]_Height_3("Height 3", 2D) = "white" {}
        _Height_3_Shift("Height 3 Shift", Range(-5, 5)) = 0
        _Tiling_3("Tiling 3", Vector) = (1, 1, 0, 0)
        _Offset_3("Offset 3", Vector) = (0, 0, 0, 0)
        _AmbientLight_2("Ambient Light 3", Color) = (0, 0, 0, 0.5019608)
        [NoScaleOffset]_LightingRamp_2("Lighting Ramp 3", 2D) = "white" {}
        _Color_3("Color 4", Color) = (1, 1, 1, 1)
        _Albedo_4("Albedo 4", 2D) = "white" {}
        [Normal]_Normal_4("Normal 4", 2D) = "bump" {}
        _Normal_4_Strength("Normal 4 Strength", Range(0, 5)) = 1
        [NoScaleOffset]_Height_4("Height 4", 2D) = "white" {}
        _Height_4_Shift("Height 4 Shift", Range(-5, 5)) = 0
        _Tiling_4("Tiling 4", Vector) = (1, 1, 0, 0)
        _Offset_4("Offset 4", Vector) = (0, 0, 0, 0)
        _AmbientLight_3("Ambient Light 4", Color) = (0, 0, 0, 0.5019608)
        [NoScaleOffset]_LightingRamp_3("Lighting Ramp 4", 2D) = "white" {}
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
			Tags { "RenderType" = "ProPixelizer" "RenderPipeline" = "UniversalPipeline" }

			UsePass "HLPixelizer/Hidden/HLPixelizerTerrain/UNIVERSAL FORWARD"
			UsePass "HLPixelizer/Hidden/HLPixelizerTerrain/SHADOWCASTER"
			UsePass "HLPixelizer/Hidden/HLPixelizerTerrain/DEPTHONLY"
			UsePass "HLPixelizer/Hidden/HLPixelizerTerrain/DEPTHNORMALS"

		Pass
		{
			Name "ProPixelizerPass"
			Tags {
				"LightMode" = "ProPixelizer"
				"DisableBatching" = "True"
			}

			ZWrite On
			Cull Off
			Blend Off

			HLSLPROGRAM
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Assets/ProPixelizer/SRP/ShaderLibrary/PixelUtils.hlsl"
            #include "Assets/ProPixelizer/SRP/ShaderLibrary/PackingUtils.hlsl"
            #include "Assets/ProPixelizer/SRP/ShaderLibrary/ScreenUtils.hlsl"
			#pragma vertex outline_vert
			#pragma fragment outline_frag
			#pragma target 2.5
            #pragma multi_compile_local USE_OBJECT_POSITION_FOR_PIXEL_GRID_ON _
            #pragma shader_feature_local PROPIXELIZER_DITHERING_ON _
			
			// If you want to use the SRP Batcher:
			// The CBUFFER has to match that generated from ShaderGraph - otherwise all hell breaks loose.
			// In some cases, it might be easier to just break SRP Batching support for your outline shader.
			// Graph Properties
            CBUFFER_START(UnityPerMaterial)
            float4 _LightingRamp_1_TexelSize;
            float _TransitionSmoothness;
            float _Saturation;
            float _Contrast;
            float4 _LightingRamp_TexelSize;
            float _Normal_2_Strength;
            float _Normal_3_Strength;
            float _Normal_4_Strength;
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
            float _AlphaClipThreshold;
            float _ID;
            float4 _OutlineColor;
            float4 _EdgeHighlightColor;
            float _Hue;
            float _Normal_1_Strength;
            float _SpecularPower;
            float4 _SpecularColor;
            float _EnableDynamicLights;
            float _Specular;
            float _Show_Vertex_Color_Weight;
            float _Lighting_Step;
            float4 _Height_1_TexelSize;
            float4 _Height_2_TexelSize;
            float4 _AmbientLight_1;
            float4 _AmbientLight_2;
            float4 _AmbientLight_3;
            float4 _Height_3_TexelSize;
            float4 _LightingRamp_2_TexelSize;
            float4 _LightingRamp_3_TexelSize;
            float4 _Height_4_TexelSize;
            float _Height_1_Shift;
            float _Height_3_Shift;
            float _Height_4_Shift;
            float _Height_2_Shift;
            float2 _Tiling_1;
            float2 _Tiling_2;
            float2 _Tiling_3;
            float2 _Tiling_4;
            float2 _Offset_1;
            float2 _Offset_2;
            float2 _Offset_3;
            float2 _Offset_4;
            float4 _Color_4;
            float4 _Color_1;
            float4 _Color_2;
            float4 _Color_3;
            float4 _Albedo_2_TexelSize;
            float4 _Albedo_2_ST;
            float4 _Albedo_3_TexelSize;
            float4 _Albedo_4_TexelSize;
            float4 _Albedo_4_ST;
            float4 _Normal_2_TexelSize;
            float4 _Normal_3_TexelSize;
            float4 _Normal_3_ST;
            float4 _Normal_4_TexelSize;
            float4 _Normal_4_ST;
            float4 _Normal_1_TexelSize;
            float4 _Normal_1_ST;
            CBUFFER_END

            // Object and Global properties
            SAMPLER(SamplerState_Linear_Repeat);
            SAMPLER(SamplerState_Point_Clamp);
            TEXTURE2D(_LightingRamp_1);
            SAMPLER(sampler_LightingRamp_1);
            TEXTURE2D(_LightingRamp);
            SAMPLER(sampler_LightingRamp);
            TEXTURE2D(_PaletteLUT);
            SAMPLER(sampler_PaletteLUT);
            float4 _PaletteLUT_TexelSize;
            TEXTURE2D(_Albedo);
            SAMPLER(sampler_Albedo);
            float _DiffuseVertexColorWeight;
            float _EmissiveVertexColorWeight;
            SAMPLER(SamplerState_Point_Repeat);
            TEXTURE2D(_Height_1);
            SAMPLER(sampler_Height_1);
            TEXTURE2D(_Height_2);
            SAMPLER(sampler_Height_2);
            TEXTURE2D(_Height_3);
            SAMPLER(sampler_Height_3);
            TEXTURE2D(_LightingRamp_2);
            SAMPLER(sampler_LightingRamp_2);
            TEXTURE2D(_LightingRamp_3);
            SAMPLER(sampler_LightingRamp_3);
            TEXTURE2D(_Height_4);
            SAMPLER(sampler_Height_4);
            TEXTURE2D(_Albedo_2);
            SAMPLER(sampler_Albedo_2);
            TEXTURE2D(_Albedo_3);
            SAMPLER(sampler_Albedo_3);
            TEXTURE2D(_Albedo_4);
            SAMPLER(sampler_Albedo_4);
            TEXTURE2D(_Normal_2);
            SAMPLER(sampler_Normal_2);
            TEXTURE2D(_Normal_3);
            SAMPLER(sampler_Normal_3);
            TEXTURE2D(_Normal_4);
            SAMPLER(sampler_Normal_4);
            TEXTURE2D(_Normal_1);
            SAMPLER(sampler_Normal_1);

			#include "Assets/ProPixelizer/SRP/ShaderLibrary/OutlinePass.hlsl"
			ENDHLSL
		}
     }
	FallBack "ProPixelizer/Hidden/ProPixelizerBase"
}
