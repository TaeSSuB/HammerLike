// 픽셀라이즈 핵심부분
#ifndef PIXELUTILS_INCLUDED
#define PIXELUTILS_INCLUDED
#define ROUNDING_PREC 0.49
#define SCRN_OFFSET 2000.0

#include "ShadowCoordFix.hlsl"
#pragma warning (disable : 3571) // 0으로 나눌 경우를 대비해서 3571 실행 
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Version.hlsl"
#if VERSION_GREATER_EQUAL(10, 0)
#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
#endif
#include "PackingUtils.hlsl"
#include "ShaderGraphUtils.hlsl"

inline float mod(float x, float y)
{
	return x - y * floor(x / y);
}

// 스케일을 전역 변수로 설정하여 모든 ProPixelizer 머티리얼의 크기를 한 번에 제어할 수 있어야 한다.
// 머티리얼의 크기를 한 번에 제어할 수 있어야함 예를 들어, 모든 머티리얼을 픽셀 크기 1로 설정하고 이 값을 사용하여
//  모든 머티리얼의 크기를 공통 매크로 픽셀 크기로 조정할 수 있습니다.

float _ProPixelizer_Pixel_Scale;

inline void PixelClipAlpha_float(float4x4 unity_MatrixVP, float3 objectCentreWS, float4 screenParams, float4 posCS, float macroPixelSize, float alpha_in, float alpha_clip_threshold, out float alpha_out, out float2 ditherUV) {

#if VERSION_GREATER_EQUAL(10,0)
#define SHADOWTEST ( SHADERPASS == SHADERPASS_SHADOWCASTER )
#else
#define SHADOWTEST defined(SHADERPASS_SHADOWCASTER)
#endif

	screenParams = screenParams;

#if SHADOWTEST
	alpha_out = alpha_in;
	ditherUV = float2(0.0, 0.0);
#else
	//posCS는 화면 픽셀 위치의 (int 형태의) 좌표로 된 조각의 위치

	//화면 픽셀 단위로 객체 위치 가져오기
	float4 objClipPos = mul(unity_MatrixVP, float4(objectCentreWS, 1));
	float2 objViewPos = objClipPos.xy / objClipPos.w;
	float2 objPixelPos = (0.5 * (objViewPos.xy + float2(1,1)) * screenParams.xy);
	float xfactor, yfactor;
#if SHADERGRAPH_PREVIEW_TEST || SHADERGRAPH_PREVIEW_WINDOW
	// 셰이더 그래프 미리보기에서 픽셀화를 비활성화 근데 작동하는 데 필요한 패스가 부족함...
	float pixelSize = 1;
#else
	float pixelSize = round(macroPixelSize * max(1, _ProPixelizer_Pixel_Scale));
#endif

	/// 20231107 
	/// 원근법의 경우 float의 정밀도에 오차가 생긴다
	/// 쉽게 말해 원근이 찢어지는 현상이 생김 이를 방지하고자 객체 픽셀 포인트를 가장 가까운 픽셀로
	/// 반올림 하자
	if (unity_OrthoParams.w < 0.5)
	{
		objPixelPos = round(objPixelPos);
	}

	// 다렉과 오픈GL의 인터페이스 유지
#if UNITY_UV_STARTS_AT_TOP
	objPixelPos.y = screenParams.y - objPixelPos.y;
#else

#endif

	// 객체 위치와 픽셀 위치에 따라 클립
	// posCS는 반 픽셀 좌표 objPixelPos도 반 픽셀 좌표
	// 바닥을 취해 정수로 반올림한 다음 반을 포함시켜 mod(x,y)가 일관되게 올바른 값을 반환하도록
	// 예를 들어 3.0+편향 mod 3 = 편향이지만, 3 mod 3 != 0이지만 일부 카드에서는 2.999가 됨
	float2 delta = floor(floor(posCS.xy) - floor(objPixelPos.xy+0.1)) + 0.01;
	xfactor = step(mod(delta.x, pixelSize), 0.1);
	yfactor = step(mod(delta.y, pixelSize), 0.1);
	float2 macroPixelDelta = delta / pixelSize;
	ditherUV = macroPixelDelta / screenParams.xy + float2(0.5, 0.5);

	// 투명성을 위한 정렬된 디더링
	float TRANSPARENCY_DITHER[16] =
	{
		1.0 / 17.0,  9.0 / 17.0,  3.0 / 17.0, 11.0 / 17.0,
		13.0 / 17.0,  5.0 / 17.0, 15.0 / 17.0,  7.0 / 17.0,
		4.0 / 17.0, 12.0 / 17.0,  2.0 / 17.0, 10.0 / 17.0,
		16.0 / 17.0,  8.0 / 17.0, 14.0 / 17.0,  6.0 / 17.0
	};
	#if PROPIXELIZER_DITHERING_ON
		// 알파 클립 임계값 전에 디더링을 적용
		int index = mod(macroPixelDelta.x, 4) * 4 + mod(macroPixelDelta.y, 4);
		index = clamp(index, 0, 15);
		alpha_in *= step(TRANSPARENCY_DITHER[index], alpha_in);
	#else
		// 디더링을 사용하지 말고 알파 클립 임계값만 사용
	#endif
	alpha_in = saturate(alpha_in);
	//alpha_in = step(alpha_clip_threshold, alpha_in);	// 논 디더링때 사용할 방법 하지만 이걸 사용하기 위해선 파이프라인을 수정해야함


	// 항상 가장자리 픽셀 그리기
	float x_edge = min(1.0, step(posCS.x, 1) + step(screenParams.x - 1, posCS.x));
	float y_edge = min(1.0, step(posCS.y, 1) + step(screenParams.y - 1, posCS.y));
	float draw = min(1.0, (x_edge + xfactor) * (y_edge + yfactor));
	alpha_out = alpha_in * draw;
#endif
}

#endif