Shader "Custom/LightVolume" {
    Properties{
        _Color("Color", Color) = (1,1,1,1)
        _Quantization("Quantization", Range(0,100)) = 50
        _ScreenspaceShadows("Screenspace Shadows", Float) = 0 // 0 false, 1 true
        _ShadowStrength("Shadow Strength", Range(0,1)) = 0.5
        _LightDir("Light Direction", Vector) = (-1,-1,-1)
        _CameraDepthTexture("_CameraDepthTexture", 2D) = "white" {}
    }
        SubShader{
            Tags { "RenderType" = "Opaque" }
            LOD 200

            CGPROGRAM
            #pragma surface surf Standard fullforwardshadows
            #pragma target 3.0

            sampler2D _CameraDepthTexture;
            sampler2D _DepthTexture;
            float4 _Color;
            float _Quantization;
            float _ScreenspaceShadows;
            float _ShadowStrength;
            float3 _LightDir;

            struct Input {
                float2 uv_MainTex;
                float3 worldPos;
                float3 worldNormal;
                float4 screenPos;
            };

            fixed4 QuantizeColor(fixed4 color, float quantizationLevels) {
                float q = 1.0 / quantizationLevels;
                color.rgb = floor(color.rgb / q + 0.5) * q;
                return color;
            }

            float ShadowCalculation(Input IN) {
                float shadow = 1.0; // 기본적으로 그림자가 없음을 가정
                float depth = tex2Dproj(_CameraDepthTexture, IN.screenPos).r;
                float3 lightDir = normalize(_LightDir);
                float3 pos = IN.worldPos + lightDir * 0.05; // 조명 방향으로 약간 이동
                float4 clipSpacePos = UnityObjectToClipPos(pos);
                float2 uv = clipSpacePos.xy / clipSpacePos.w;
                uv = uv * 0.5 + 0.5; // NDC로 변환
                float sampleDepth = tex2D(_CameraDepthTexture, uv).r;
                if (sampleDepth < depth) {
                    shadow = 1.0 - _ShadowStrength; // 그림자 영역
                }
                return shadow;
            }

            void surf(Input IN, inout SurfaceOutputStandard o) {
                fixed4 c = _Color;
                c = QuantizeColor(c, _Quantization);

                if (_ScreenspaceShadows > 0.5) {
                    float shadow = ShadowCalculation(IN);
                    c.rgb *= shadow;
                }

                o.Albedo = c.rgb;
                o.Alpha = c.a;
            }
            ENDCG
    }
        FallBack "Diffuse"
}
