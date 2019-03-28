// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "HiAR/TransparentVideo_LeftAndRight"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_FlipX("FlipX", Float) = 0.0
	}
	SubShader
	{
		Tags { "RenderType"="Transparent" "Quene"="Transparent" }
		LOD 100
		Blend SrcAlpha OneMinusSrcAlpha 
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
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float2 uv1 : TEXCOORD1;
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;

			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float _FlipX;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				o.uv.x = o.uv.x/2 + 0.5;
				o.uv1 = o.uv - float2(0.5,0);
				o.uv = TRANSFORM_TEX(o.uv, _MainTex);
				o.uv1 = TRANSFORM_TEX(o.uv1, _MainTex);
				o.uv1.x = lerp(o.uv1.x, (1 - o.uv1.x), _FlipX);
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				fixed4 col = tex2D(_MainTex, float2(lerp(i.uv.x, (1 - i.uv.x), _FlipX), i.uv.y));
				col.a = Luminance(tex2D(_MainTex,i.uv1).rgb);
				// apply fog
				UNITY_APPLY_FOG(i.fogCoord, col);
				return col;
			}
			ENDCG
		}
	}
}
