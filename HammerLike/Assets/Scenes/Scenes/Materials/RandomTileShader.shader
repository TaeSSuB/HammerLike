Shader "Custom/RandomTileShader"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _TilesX("Tiles X", int) = 2
        _TilesY("Tiles Y", int) = 2
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
                int _TilesX;
                int _TilesY;

                v2f vert(appdata v)
                {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);

                    float tileWidth = 1.0 / _TilesX;
                    float tileHeight = 1.0 / _TilesY;

                    float randX = frac(sin(v.vertex.x * 12.9898 + v.vertex.y * 78.233) * 43758.5453);
                    float randY = frac(sin(v.vertex.y * 12.9898 + v.vertex.x * 78.233) * 43758.5453);

                    o.uv.x = v.uv.x * tileWidth + randX * tileWidth;
                    o.uv.y = v.uv.y * tileHeight + randY * tileHeight;

                    return o;
                }

                half4 frag(v2f i) : SV_Target
                {
                    return tex2D(_MainTex, i.uv);
                }
                ENDCG
            }
        }
}
