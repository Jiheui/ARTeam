// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Hidden/EdgeCheck"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_WaveTex ("Wave Texture", 2D) = "black" {}
		_Color ("Edge Line Color", Color) = (0,1,1,0)
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
				float2 texcoord : TEXCOORD1;
				float2 texcoord1 : TEXCOORD2;
			};

			sampler2D _WaveTex;
			uniform half4 _WaveTex_ST;
			uniform half4 _WaveTex1_ST;
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				o.texcoord = TRANSFORM_TEX(v.uv,_WaveTex);
				o.texcoord1 = TRANSFORM_TEX(v.uv,_WaveTex1);
				return o;
			}
			
			sampler2D _MainTex;
			fixed4 _Color;
			uniform fixed _Offset;
			uniform fixed _delta;
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv);
				fixed4 colABlock = tex2D(_MainTex,i.uv+fixed2(_Offset,-_Offset));
				fixed col_L = Luminance(col.rgb);
				fixed colABlock_L = Luminance(colABlock.rgb);
				fixed4 mask = tex2D(_WaveTex,i.texcoord);
				fixed4 mask1 = tex2D(_WaveTex,i.texcoord1);
//				if(abs(col_L - colABlock_L) > _delta){
//					col = _Color * mask.a + col * (1-mask.a);
//				}
//				col = _Color * mask.r + col * (1-mask.r);

				fixed blockValue = 1 - (abs(col_L - colABlock_L) - _delta)*(1/_delta)*2;
				blockValue = clamp(blockValue ,0,1);
				col = lerp(col,_Color,(1-blockValue)*(mask.a+mask1.a));
				col = _Color * mask.r + col * (1-mask.r);

//				colABlock = tex2D(_MainTex,i.uv+fixed2(0.0015,0.0015));
//				colABlock_L = Luminance(colABlock.rgb);
//				if(abs(col_L - colABlock_L) > 0.1){
//					col = _Color;
//				}
//				col = fixed4(colNextBlock_L,colNextBlock_L,colNextBlock_L,0);
				// just invert the colors
				return col;
			}
			ENDCG
		}
	}
}
