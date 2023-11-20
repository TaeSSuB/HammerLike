#if VL_CAST_DIRECT_LIGHT

void AddDirectLighting(float3 wpos, float2 uv, inout half4 sum) {
    GBufferData gbuffer;
	GetGBufferData(uv, gbuffer);
	half3 color = (gbuffer.albedo + gbuffer.specular) * _LightColor.rgb;
    half3 atten = GetShadowAttenWS_Soft(wpos) * DistanceAttenuation(wpos);
    half3 lambert = LightingLambert(_LightColor.rgb, _ToLightDir.xyz, gbuffer.normal);
    half4 directLight = half4(lambert * atten * DIRECT_LIGHT_MULTIPLIER, 1.0);
    sum += directLight * (1.0 - sum.a);
}

#endif