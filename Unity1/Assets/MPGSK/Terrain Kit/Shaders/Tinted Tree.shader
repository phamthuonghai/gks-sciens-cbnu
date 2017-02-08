Shader "Tasharen/Tinted Tree"
{
	Properties
	{
		_Color ("Main Color", Color) = (1, 1, 1, 1)
		_Secondary ("Secondary Color", Color) = (1, 1, 1, 1)
		_ColorTint ("Main Color Tint", Color) = (1, 1, 1, 1)
		_SecondaryTint ("Secondary Color Tint", Color) = (1, 1, 1, 1)
		_MainTex ("Base (RGB) Alpha (A)", 2D) = "white" {}
		_Cutoff ("Base Alpha cutoff", Range (0,.9)) = .5
	}

	// This subshader has smooth outlines around the edges of the tree leaves that improves the quality.
	// The downside is that this shader will not receive any shadows.
 	SubShader
	{
		LOD 600
		Cull Off
		ZWrite On

		Tags
		{
			"Queue"="AlphaTest"
			"IgnoreProjector"="True"
			"RenderType"="TransparentCutout"
		}

		Pass
		{
			// We actually get softer edges if we do write to RGB
			//ColorMask A

			AlphaTest GEqual [_Cutoff]
			Blend SrcAlpha OneMinusSrcAlpha

			SetTexture [_MainTex]
			{
				constantColor [_Color]
				Combine texture * constant, texture * constant 
			}
		}
		
		CGPROGRAM
		#pragma surface surf Lambert alpha
		sampler2D _MainTex;
		half4 _Color;
		half4 _Secondary;
		half4 _ColorTint;
		half4 _SecondaryTint;

		struct Input
		{
			half4 color : COLOR;
			half2 uv_MainTex : TEXCOORD0;
		};

		void surf (Input IN, inout SurfaceOutput o)
		{
			fixed4 tex = tex2D(_MainTex, IN.uv_MainTex);
			o.Albedo = lerp(tex.rgb, tex.rgb * lerp(_Color.rgb, _ColorTint, IN.color.b), IN.color.g + IN.color.r);
			o.Albedo = lerp(o.Albedo, tex.rgb * lerp(_SecondaryTint, _Secondary.rgb, IN.color.a), IN.color.r);
			o.Alpha = tex.a * _Color.a;
		}
		ENDCG
	}

	// This shader casts and receives shadows, but the edges around the leaves look very sharp.
	/*SubShader
	{
		LOD 500
		Cull Off
		ZWrite On

		Tags
		{
			"Queue"="AlphaTest"
			"IgnoreProjector"="True"
			"RenderType"="TransparentCutout"
		}

		CGPROGRAM
		#pragma surface surf Lambert alphatest:_Cutoff
		sampler2D _MainTex;
		half4 _Color;
		half4 _Secondary;
		half4 _ColorTint;
		half4 _SecondaryTint;

		struct Input
		{
			half4 color : COLOR;
			half2 uv_MainTex : TEXCOORD0;
		};

		void surf (Input IN, inout SurfaceOutput o)
		{
			fixed4 tex = tex2D(_MainTex, IN.uv_MainTex);
			o.Albedo = lerp(tex.rgb, tex.rgb * lerp(_Color.rgb, _ColorTint, IN.color.b), IN.color.g + IN.color.r);
			o.Albedo = lerp(o.Albedo, tex.rgb * lerp(_SecondaryTint, _Secondary.rgb, IN.color.a), IN.color.r);
			o.Alpha = tex.a * _Color.a;
		}
		ENDCG
	}*/
	Fallback "Transparent/Cutout/Diffuse"
}
