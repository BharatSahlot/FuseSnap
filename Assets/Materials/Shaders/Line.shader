Shader "Unlit/Line"
{
    Properties
    {
        _NoCurrentTexture ("No Current Texture", 2D) = "white" {}
        _CurrentTexture ("Current Texture", 2D) = "white" {}
		_Current ("Current", float) = 0
        _IsCurrentFlowing ("Is current flowing", int) = 0
	}
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
				fixed4 color : COLOR;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
				fixed4 color : COLOR;
            };

            sampler2D _NoCurrentTexture;
            float4 _NoCurrentTexture_ST;

            sampler2D _CurrentTexture;
            float4 _CurrentTexture_ST;

			float _Current;
            int _IsCurrentFlowing;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                // o.uv = TRANSFORM_TEX(v.uv, _NoCurrentTexture);
                o.color = v.color;
				UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                float2 uv = i.uv + float2(_Current, 0);
				fixed4 col = tex2D(_NoCurrentTexture, TRANSFORM_TEX(i.uv, _NoCurrentTexture));
				fixed4 col2 = tex2D(_CurrentTexture, TRANSFORM_TEX(i.uv, _CurrentTexture));
                return lerp(col, col2, _IsCurrentFlowing);
                // return col * (1 - _IsCurrentFlowing) + col2 * _IsCurrentFlowing;
            }
            ENDCG
        }
    }
}
