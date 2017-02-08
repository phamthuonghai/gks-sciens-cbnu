Shader "MPGSK/Tank"
{
	Properties
	{
		_Color ("Main Color", Color) = (1,1,1,1)
		_SpecColor ("Specular Color", Color) = (0.5, 0.5, 0.5, 1)
		_Shininess ("Shininess", Range (0.01, 1)) = 0.078125
		_MainTex ("Base (RGB) Gloss (A)", 2D) = "white" {}
	}
	
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 300
		
CGPROGRAM
#pragma surface surf PPL

sampler2D _MainTex;
float4 _Color;
float _Shininess;

struct Input
{
	float2 uv_MainTex;
};

half4 LightingPPL (SurfaceOutput s, half3 lightDir, half3 viewDir, half atten)
{
	half3 h = normalize(lightDir + viewDir);
	float3 nm = normalize(s.Normal);
	
	half diff = max (0, dot (nm, lightDir));
	
	float nh = max (0, dot (nm, h));
	float spec = pow(nh, s.Specular * 128.0) * s.Gloss;
	
	half4 c;
	c.rgb = (s.Albedo * _LightColor0.rgb * diff + _LightColor0.rgb * _SpecColor * spec) * (atten * 2.0);
	c.a = s.Alpha;
	return c;
}

void surf (Input IN, inout SurfaceOutput o)
{
	half4 tex = tex2D(_MainTex, IN.uv_MainTex) * _Color;
	o.Albedo = tex.rgb;
	o.Alpha = _Color.a;
	o.Specular = _Shininess;
	o.Gloss = tex.a;
}
ENDCG
	}
	
	Fallback "VertexLit"
}
