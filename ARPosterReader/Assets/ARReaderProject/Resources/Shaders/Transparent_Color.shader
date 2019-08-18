// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "HiAR/Transparent_Color"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_MaskColor ("Mask Color",Color) = (0,0,0,1)
		_DeltaColor ("Delta Color",Range(0.0,10.0)) = 10.0
		_FlipX ("FlipX", Float) = 0.0
	}
	SubShader
	{
		Tags { "RenderType"="Transparent" "Quene"="Transparent"}
		LOD 100
		//Blend One OneMinusSrcColor
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
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			half4 _MaskColor;
			float _DeltaColor;
			float _FlipX;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				fixed4 col = tex2D(_MainTex, float2(lerp(i.uv.x, (1-i.uv.x), _FlipX), i.uv.y));
				//fixed4 col = tex2D(_MainTex, i.uv);
				//col.a = (1 - (1-col.r)*(1-col.g)*(1-col.b));
				col.a = lerp(0,1,(abs(col.r-_MaskColor.r)+abs(col.g-_MaskColor.g)+abs(col.b-_MaskColor.b))*_DeltaColor);
				UNITY_APPLY_FOG(i.fogCoord, col);				
				return col;
			}
			ENDCG
		}
	}
}
