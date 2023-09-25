Shader "Custom/EdgeDetection"
{
    Properties
    {
        _MainTex("Main Texture", 2D) = "white" {}
        _EdgeColor("Edge Color", Color) = (0,0,0,1)
        _EdgeAmount("Edge Amount", Range(0,5)) = 1
    }

        SubShader
        {
            Pass
            {
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #include "UnityCG.cginc"

                struct appdata
                {
                    float4 vertex : POSITION;
                    float2 uv : TEXCOORD0;
                };

                struct v2f
                {
                    float2 uv : TEXCOORD0;
                    float4 vertex : SV_POSITION;
                };

                sampler2D _MainTex;
                float4 _MainTex_ST;
                float4 _EdgeColor;
                float _EdgeAmount;

                v2f vert(appdata v)
                {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                    return o;
                }

                fixed4 frag(v2f i) : SV_Target
                {
                    float depth = tex2D(_MainTex, i.uv).r;
                    float depthUp = tex2D(_MainTex, i.uv + float2(0, _MainTex_ST.y)).r;
                    float depthDown = tex2D(_MainTex, i.uv - float2(0, _MainTex_ST.y)).r;
                    float depthRight = tex2D(_MainTex, i.uv + float2(_MainTex_ST.x, 0)).r;
                    float depthLeft = tex2D(_MainTex, i.uv - float2(_MainTex_ST.x, 0)).r;

                    float edge = abs(depth - depthUp) + abs(depth - depthDown) + abs(depth - depthRight) + abs(depth - depthLeft);

                    if (edge > _EdgeAmount)
                        return _EdgeColor;
                    else
                        return tex2D(_MainTex, i.uv);
                }
                ENDCG
            }
        }
}
