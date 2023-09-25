Shader "Custom/PixelateWithOutline"
{
    Properties
    {
        _MainTex ("Main Texture", 2D) = "white" {}
        _PixelSizeX ("Pixel Size X", Range(1, 200)) = 100
        _PixelSizeY ("Pixel Size Y", Range(1, 200)) = 100
        _OutlineColor("Outline Color", Color) = (1, 0, 0, 1)
        _Outline("Outline", Range(0, 1)) = 0
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
float _PixelSizeX, _PixelSizeY;
float4 _OutlineColor;
float _Outline;

v2f vert(appdata v)
{
    v2f o;
    o.vertex = UnityObjectToClipPos(v.vertex);
    o.uv = v.uv;
    return o;
}

fixed4 frag(v2f i) : SV_Target
{
    float2 uv = i.uv;

    uv.x = floor(uv.x * _ScreenParams.x / _PixelSizeX) / (_ScreenParams.x / _PixelSizeX);
    uv.y = floor(uv.y * _ScreenParams.y / _PixelSizeY) / (_ScreenParams.y / _PixelSizeY);

    fixed4 col = tex2D(_MainTex, uv);

                // Outline
    if (_Outline > 0.5)
    {
        float dx = 1.0 / _ScreenParams.x;
        float dy = 1.0 / _ScreenParams.y;
        fixed4 left = tex2D(_MainTex, i.uv + float2(-dx, 0));
        fixed4 right = tex2D(_MainTex, i.uv + float2(dx, 0));
        fixed4 up = tex2D(_MainTex, i.uv + float2(0, dy));
        fixed4 down = tex2D(_MainTex, i.uv + float2(0, -dy));
                    
        if (left.a < 0.5 || right.a < 0.5 || up.a < 0.5 || down.a < 0.5)
        {
            col = _OutlineColor;
        }
    }
                
    return col;
}
            ENDCG
        }
    }
}
