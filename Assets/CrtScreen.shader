Shader "Hidden/CrtScreen"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _PixelTex ("Pixel Tex", 2D) = "white" {}
        _SquareTex ("Square Tex", 2D) = "white" {}
        _Gradient ("Gradient Tex", 2D) = "white" {}
        _Gradient1 ("Gradient1 Color", Color) = (1, 0, 0, 1)
        _Gradient2 ("Gradient2 Color", Color) = (0, 0, 1, 1)
		

		_Distortion ("Distortion", Range(-3,3)) = -1
		_Y_Dist ("Y Dist", float) = 1
		_X_Dist ("X Dist", float) = 1
		_Scale ("Scale", float) = 1
	}
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

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

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            sampler2D _MainTex;
            sampler2D _PixelTex;
			sampler2D _SquareTex;
			sampler2D _Gradient;
			float4 _PixelTex_ST;
			float4 _SquareTex_ST;
			float4 _Gradient_ST;
			

			float _Distortion;
			float _Y_Dist;
			float _X_Dist;
			float _Scale;
			float _Offset;
			float _Flicker;
			float _Gradient1_Offset;
			float _Gradient2_Offset;
			float4 _Gradient1;
			float4 _Gradient2;

			float2 fisheye (float2 uv)
			{
                // lens distortion coefficient
                float k = -0.15;
                float r = (uv.x - 0.5) * (uv.x - 0.5) + (uv.y - 0.5) * (uv.y - 0.5);
				float f = 1 + r * (k + _Distortion * sqrt(r));
				// get the right pixel for the current position
                float x = f * (uv.x - 0.5) + 0.5;
                float y = f * (uv.y - 0.5) + 0.5;
                return float2(x,y);
			}

            fixed4 frag (v2f i) : SV_Target
            {
				// x glitch
				float dist = tex2D(_SquareTex, TRANSFORM_TEX(i.uv, _SquareTex) + float2(0, _Offset)).r;
				float2 uv = i.uv + float2(dist * _X_Dist, 0);	
				
				uv = fisheye(uv);

				float2 guv = TRANSFORM_TEX(uv, _Gradient);
				float3 col = tex2D(_MainTex, uv) + tex2D(_Gradient, guv + float2(0, _Gradient1_Offset)) * _Gradient1
												 + tex2D(_Gradient, guv + float2(0, _Gradient2_Offset)) * _Gradient2;

				// move rows up
				uv.y += _Y_Dist * round(i.uv.x * _Scale);
            	col *= tex2D(_PixelTex, TRANSFORM_TEX(uv, _PixelTex));

				// blend colors and control brightness
				col *= _Flicker;
				return float4(col,1);
			}
            ENDCG
        }
    }
}
