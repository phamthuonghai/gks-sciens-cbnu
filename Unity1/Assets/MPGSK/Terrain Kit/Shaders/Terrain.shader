Shader "Tasharen/Terrain"
{
	Properties
	{
		_Splat0 ("Layer 0 (R)", 2D) = "white" {}
		_Splat1 ("Layer 1 (G)", 2D) = "white" {}
		_Splat2 ("Layer 2 (B)", 2D) = "white" {}
		_Splat3 ("Layer 3 (A)", 2D) = "white" {}
	}
	
	SubShader
	{
		LOD 500
		Tags
		{
			"SplatCount" = "4"
			"Queue" = "Geometry-100"
			"RenderType" = "Opaque"
		}
		CGPROGRAM
		#pragma target 3.0
		#pragma exclude_renderers d3d11 d3d11_9x
		#pragma surface surf BlinnPhong vertex:vert
		#include "UnityCG.cginc"

		struct Input
		{
			float2 uv_Splat0 : TEXCOORD1;
			float2 uv_Splat1 : TEXCOORD2;
			float2 uv_Splat2 : TEXCOORD3;
			float2 uv_Splat3 : TEXCOORD4;
			float2 blenders;
		};
		
		sampler2D _Splat0,_Splat1,_Splat2,_Splat3;
		sampler2D terrainNormal;
		sampler2D terrainTint;

		// X = Y position offset
		// Y = Y scale
		// Z = -1.0 / Slope blend range
		// W = -Z * Slope blend start (degrees)
		float4 terrainData;

		// X = -1.0 / Blend 0 range
		// Y = -X * Blend 0 start (height)
		// Z = -1.0 / Blend 1 range
		// W = -Z * Blend 1 start (height)
		float4 terrainBlend;
		
		void vert (inout appdata_full v, out Input o)
		{
			float3 tan = float3(1.0, 0.0, 0.0);
			float3 bin = cross(tan, v.normal);
			tan = normalize(cross(v.normal, bin));

			v.tangent.xyz = tan;
			v.tangent.w = floor(dot(cross(v.normal, tan), bin)) * 2.0 + 1.0;
			
			o.blenders.x = v.vertex.y + terrainData.x;
			o.blenders.y = acos(v.normal.y) * 57.2958;
		}
		
		void surf (Input IN, inout SurfaceOutput o)
		{
			// With the slope texture only affecting the top 2 textures
			half slopeFactor = saturate(IN.blenders.y * terrainData.z + terrainData.w);
			half sandFactor = saturate(IN.blenders.x * terrainBlend.x + terrainBlend.y);

			half3 tint = tex2D(terrainTint, IN.uv_Splat1 * 0.1).rgb;
			half3 normal = UnpackNormal(tex2D(terrainNormal, IN.uv_Splat0));
			half3 col = lerp(
				tex2D(_Splat2, IN.uv_Splat2).rgb,
				tex2D(_Splat3, IN.uv_Splat3).rgb,
				saturate(IN.blenders.x * terrainBlend.z + terrainBlend.w));

			// With the slope texture only affecting the top 2 textures
			//col = lerp(col, tex2D(_Splat0, IN.uv_Splat0).rgb, slopeFactor);
			//col = lerp(tex2D(_Splat1, IN.uv_Splat1).rgb, col, sandFactor);
			//o.Albedo = lerp(col, col * tint, saturate(-IN.blenders.x * 0.25));
			//o.Normal = lerp(half3(0.0, 0.0, 1.0), normal, saturate(slopeFactor - (1.0 - sandFactor)));
			
			// With the slop texture affecting everything
			col = lerp(tex2D(_Splat1, IN.uv_Splat1).rgb, col, sandFactor);
			col = lerp(col, tex2D(_Splat0, IN.uv_Splat0).rgb, slopeFactor);
			o.Albedo = lerp(col, col * tint, saturate(-IN.blenders.x * 0.25));
			o.Normal = lerp(half3(0.0, 0.0, 1.0), normal, slopeFactor);
		}
		ENDCG
	}
	
	SubShader
	{
		LOD 200
		Tags
		{
			"SplatCount" = "4"
			"Queue" = "Geometry-100"
			"RenderType" = "Opaque"
		}
		CGPROGRAM
		#pragma exclude_renderers d3d11 d3d11_9x
		#pragma surface surf BlinnPhong vertex:vert
		#include "UnityCG.cginc"
		
		struct Input
		{
			float2 uv_Splat0 : TEXCOORD1;
			float2 uv_Splat1 : TEXCOORD2;
			float2 uv_Splat2 : TEXCOORD3;
			float2 uv_Splat3 : TEXCOORD4;
			float2 blenders;
		};
		
		sampler2D _Splat0,_Splat1,_Splat2,_Splat3;

		float4 terrainData;
		float4 terrainBlend;
		
		void vert (inout appdata_full v, out Input o)
		{
			float3 tan = float3(1.0, 0.0, 0.0);
			float3 bin = cross(tan, v.normal);
			tan = normalize(cross(v.normal, bin));

			v.tangent.xyz = tan;
			v.tangent.w = floor(dot(cross(v.normal, tan), bin)) * 2.0 + 1.0;
			
			o.blenders.x = v.vertex.y + terrainData.x;
			o.blenders.y = acos(v.normal.y) * 57.2958;
		}
		
		void surf (Input IN, inout SurfaceOutput o)
		{
			half3 col = tex2D(_Splat3, IN.uv_Splat3).rgb;
			col = lerp(tex2D(_Splat2, IN.uv_Splat2).rgb, col, saturate(IN.blenders.x * terrainBlend.z + terrainBlend.w));
			col = lerp(col, tex2D(_Splat0, IN.uv_Splat0).rgb, saturate(IN.blenders.y * terrainData.z + terrainData.w));
			col = lerp(tex2D(_Splat1, IN.uv_Splat1).rgb, col, saturate(IN.blenders.x * terrainBlend.x + terrainBlend.y));
			o.Albedo = col;
		}
		ENDCG
	}
	Fallback "Hidden/TerrainEngine/Splatmap/Lightmap-FirstPass"
}
