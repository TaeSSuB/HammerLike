Shader "Custom/Pixelizer"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "red" {}
        _PixelSize ("Pixel Size", Range(1,100)) = 1
    }

    SubShader
    {
        // Draw after all opaque geometry
        Tags { "RenderType"="Opaque" }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #pragma target 3.0

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
float _PixelSize;

v2f vert(appdata v)
{
    v2f o;
    o.vertex = UnityObjectToClipPos(v.vertex);
    o.uv = v.uv;
    return o;
}

half4 frag(v2f i) : SV_Target
{
    i.uv.x = floor(i.uv.x * _ScreenParams.x / _PixelSize) * _PixelSize / _ScreenParams.x;
    i.uv.y = floor(i.uv.y * _ScreenParams.y / _PixelSize) * _PixelSize / _ScreenParams.y;

    half4 col = tex2D(_MainTex, i.uv);
    return col;
}
            ENDCG
        }
    }
}
