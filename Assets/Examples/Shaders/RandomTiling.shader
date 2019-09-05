Shader "Custom/RandomTiling"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		[Toggle] 
		_UseRandMask("Use Random Mask", Int) = 0
	}
		
	SubShader
	{
		Tags { "RenderType" = "Opaque" }

		LOD 200
		CGPROGRAM

#pragma surface surf Lambert
#pragma shader_feature _USERANDMASK_ON

		sampler2D _MainTex;

		int _UseRandMask;

		struct Input
		{
			half2 uv_MainTex;
		};

		half rand2(half2 coords)
		{
			return frac(sin(dot(coords, half2(12.989,78.233))) * 43758.5453);
		}

		void surf(Input IN, inout SurfaceOutput o) 
		{
			half2 samp = IN.uv_MainTex;

#ifdef _USERANDMASK_ON
			half r = (round(rand2(floor(samp)) * 3));
			half m1 = ((r - 1)*(3 - r)) / min(r - 3, -1);
			half m2 = (r*(2 - r)) / max(r, 1);
			half m3 = (r*(r - 2)) / max(r, 1);
			half m4 = ((3 - r)*(r - 1)) / min(r - 3, -1);

			samp -= 0.5;
			samp = mul(samp, float2x2(m1, m2, m3, m4));
			samp.xy += 0.5;		
#endif
			half4 tex = tex2D(_MainTex, samp);
			o.Albedo = tex.rgb;
			o.Alpha = tex.a;
		}

		ENDCG
	}

	FallBack "Diffuse"
}