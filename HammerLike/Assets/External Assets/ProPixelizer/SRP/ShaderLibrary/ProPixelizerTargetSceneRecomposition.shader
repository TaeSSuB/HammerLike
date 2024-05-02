// Copyright Elliot Bentine, 2018-
// 
// Part of the ProPixelizer Package.
// 
// This shader is used to recomposite the low-resolution buffer back into the main screen.
//
// - Fullscreen mode: Only returns pixels from the low-resolution target.
// - Non-fullscreen modes: this shader merges the _MainTex and _Secondary targets based on _Depth 
//     and _SecondaryDepth, returning the values which are closest to the screen.
// - All modes: accounts for dissimilarity between the low-res and main scene camera projection,
//     which allows smooth sub-(low-res) pixel camera movement at the screen resolution.


Shader "Hidden/ProPixelizer/SRP/Internal/ProPixelizerTargetSceneRecomposition" {
		SubShader{
			Pass {
				ZTest Always Cull Off ZWrite On

				HLSLPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#pragma target 2.0
				#pragma multi_compile RECOMPOSITION_DEPTH_OUTPUT_ON _
				#pragma multi_compile PROPIXELIZER_FULL_SCENE _

				// 2022.2 & URP14+
				#define BLIT_API UNITY_VERSION >= 202220
				#if BLIT_API
					#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
					#include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

					sampler2D _MainTex;
					TEXTURE2D_X_FLOAT(_Depth);
					SAMPLER(sampler_Depth_point_clamp);
					uniform float4 _Depth_ST;
					

					sampler2D _Secondary;
					TEXTURE2D_X_FLOAT(_SecondaryDepth);
					SAMPLER(sampler_SecondaryDepth_point_clamp);
					uniform float4 _SecondaryDepth_ST;
					

					struct DBCVaryings {
						float4 vertex : SV_POSITION;
						float2 texcoord : TEXCOORD0;
						UNITY_VERTEX_OUTPUT_STEREO
					};

					DBCVaryings vert(Attributes v) {
						Varyings vars;
						vars = Vert(v);
						DBCVaryings o;
						o.vertex = vars.positionCS;
						o.texcoord = vars.texcoord;
						return o;
					}
				#else
					#include "UnityCG.cginc"

					sampler2D _MainTex;
					UNITY_DECLARE_DEPTH_TEXTURE(_Depth);
					uniform float4 _Depth_ST;

					sampler2D _Secondary;
					UNITY_DECLARE_DEPTH_TEXTURE(_SecondaryDepth);
					uniform float4 _SecondaryDepth_ST;
					

					struct DBCVaryings {
						float4 vertex : SV_POSITION;
						float2 texcoord : TEXCOORD0;
						UNITY_VERTEX_OUTPUT_STEREO
					};

					struct Attributes
					{
						float4 vertex : POSITION;
						float2 texcoord : TEXCOORD0;
						UNITY_VERTEX_INPUT_INSTANCE_ID
					};

					DBCVaryings vert(Attributes v) {
						DBCVaryings o;
						UNITY_SETUP_INSTANCE_ID(v);
						UNITY_INITIALIZE_OUTPUT(DBCVaryings, o);
						UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
						o.vertex = UnityObjectToClipPos(v.vertex);
						o.texcoord = TRANSFORM_TEX(v.texcoord.xy, _Depth);
						return o;
					}
				#endif

				#include "ScreenUtils.hlsl"

				#if RECOMPOSITION_DEPTH_OUTPUT_ON
				void frag(DBCVaryings i, out float4 outColor : SV_Target, out float outDepth : SV_Depth) {
				#else
				void frag(DBCVaryings i, out float4 outColor : SV_Target) {
					float outDepth;
				#endif
					float baseDepth;
					float secondaryDepth;

					float lowResolutionTargetMargin = GetProPixelizerLowResTargetMargin(); // defined in ProPixelizerUtils.cs
					float2 scaledCoord; 
					if (unity_OrthoParams.w < 0.5) // perspective projection
					{
						//The perspective projections for both the screen target and the low-res target are the same.
						// This means that taking only a section near the centre of the low-res target introduced visual errors,
						// because it presents a region of the target which has an effectively reduced field-of-view.
						// Thus, for perspective cameras (e.g. scene tab, preview, game when people use perspective) we need to use the full low res target.
						// Padding offers no advantage in these scenarios anyway, because creep cannot be eliminated.
						scaledCoord = i.texcoord;
					} else { // ortho projection
						// The low-res target is drawn with a border that over-hangs the primary target view area. This is to facilitate correct ID outlines
					    // at the edge, and smooth movement.
						scaledCoord = ConvertToLowResolutionTargetUV(i.texcoord);
					}

					#if !SHADERGRAPH_PREVIEW_TEST
					#if PROPIXELIZER_FULL_SCENE
					// We apply a upscaling filter that is suited to pixel art.
					// Based on the work of Cole Cecil, see here: https://colececil.io/blog/2017/scaling-pixel-art-without-destroying-it/
					// 
					// The pixels_per_texel is a measure of how many pixels ('screen pixels') there are per texel ('texture pixels') .
					// For upscaling, as most likely here, this number is always greater than one.
					float2 pixels_per_texel = _ProPixelizer_ScreenTargetInfo.xy / _ProPixelizer_RenderTargetInfo.xy;

					// There are two cases that occur when upscaling:
					//   1. A pixel is entirely within a texel. In this case, we just take the texel color (sample at texel centre).
					//   2. A pixel partially covers multiple texels. In this case, we take a weighted sample of the surrounding texels.
					// Both can be elegantly described by taking a transfer function for the low_res_coordinate:
					// 
					// ^  (uv)
					// |           ---
					// |          /
					// |      ----
					// |     /
					// |  ---
					// x--------------> (in)
					// 
					// This blends the uvs between texels for intermediate pixels, and snaps to texel centres for completely contained pixels.
					// The width of an 'edge' `/` in texel space is 'texels_per_pixel' / _RenderTargetInfo.xy. 
					// 
					// (Note that because we are only blitting two rectangles together, of same orientation, we don't need
					// to consider rotational effects that are required more generally).
					float2 low_res_coordinate = scaledCoord * _ProPixelizer_RenderTargetInfo.xy; // n+0.5 on texel centres.
					float2 low_res_texel_frac = frac(low_res_coordinate);
					float2 low_res_texel_base = floor(low_res_coordinate);
					float2 low_res_texel_offset = clamp(low_res_texel_frac * pixels_per_texel, 0, 0.5)
						+ clamp((low_res_texel_frac - 1) * pixels_per_texel + 0.5, 0.0, 0.5);
					scaledCoord = (low_res_texel_base + low_res_texel_offset) / _ProPixelizer_RenderTargetInfo.xy;

					// There is an issue with this approach for hybrid modes, see https://github.com/ProPixelizer/ProPixelizer/issues/180.
					#endif
					#endif

					#if BLIT_API
						baseDepth = SAMPLE_TEXTURE2D_X(_Depth, sampler_Depth_point_clamp, i.texcoord).r;
						secondaryDepth = SAMPLE_TEXTURE2D_X(_SecondaryDepth, sampler_SecondaryDepth_point_clamp, scaledCoord).r;
					#else
						baseDepth = SAMPLE_RAW_DEPTH_TEXTURE(_Depth, i.texcoord);
						secondaryDepth = SAMPLE_RAW_DEPTH_TEXTURE(_SecondaryDepth, scaledCoord);
					#endif
					UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);

					#if UNITY_REVERSED_Z
						float delta = baseDepth - secondaryDepth;
					#else
						float delta = secondaryDepth - baseDepth;
					#endif

					#if PROPIXELIZER_FULL_SCENE
						outDepth = secondaryDepth;
						outColor = tex2D(_Secondary, scaledCoord);
					#else
						if (delta >= 0) {
							outDepth = baseDepth;
							outColor = tex2D(_MainTex, i.texcoord);
						} else {
							outDepth = secondaryDepth;
							outColor = tex2D(_Secondary, scaledCoord);
						}
					#endif
				}
				ENDHLSL
			}
		}
	Fallback "Hidden/Universal Render Pipeline/Blit"
}