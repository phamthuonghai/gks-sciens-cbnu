Shader "Laser"
{
	Properties
	{
		_Color ("Main Color", Color) = (1,1,1,1)
		_MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
	}
	
	SubShader
	{
		Tags
		{
			"Queue" = "Transparent+100"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
		}

		ZWrite Off
		Lighting Off
		Fog { Color (0,0,0,0) }
		Blend SrcAlpha One

		LOD 200
	
		Pass
		{
CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#pragma fragmentoption ARB_precision_hint_fastest

#include "UnityCG.cginc"

float4 _Color;
sampler2D _MainTex;

struct appdata_t
{
	float4 vertex : POSITION;
	float4 color : COLOR;
	float2 texcoord : TEXCOORD0;
};

struct v2f
{
	float4 vertex : POSITION;
	float4 color : COLOR;
	float2 texcoord : TEXCOORD0;
};

float4 _MainTex_ST;

v2f vert (appdata_t v)
{
	v2f o;
	o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
	o.color = v.color;
	o.texcoord = TRANSFORM_TEX(v.texcoord,_MainTex);
	return o;
}

half4 frag (v2f i) : COLOR
{
	return _Color * i.color * tex2D(_MainTex, i.texcoord);
}
ENDCG
		}
	}

	// ---- Dual texture cards
	SubShader
	{
		Pass
		{
			SetTexture [_MainTex]
			{
				constantColor [_TintColor]
				combine constant * primary
			}
			SetTexture [_MainTex]
			{
				combine texture * previous DOUBLE
			}
		}
	}
	
	// ---- Single texture cards (does not do color tint)
	SubShader
	{
		Pass
		{
			SetTexture [_MainTex]
			{
				combine texture * primary
			}
		}
	}
}
