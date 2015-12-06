Shader "Custom/StrawberryShader" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_SeedTex ("Seed Texture (RGBA)", 2D) = "white" {}
		_MoldTex ("Mold Texture (RGBA)", 2D) = "white" {}
		_BumpMap ("Normal Map (RGBA)", 2D) = "bump" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _SeedTex;
		sampler2D _BumpMap;

		struct Input {
			float2 uv_SeedTex;
			float2 uv_BumpMap;
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;

		void surf (Input IN, inout SurfaceOutputStandard o) {
			// Albedo comes from a texture tinted by color
			float a = tex2D(_SeedTex, IN.uv_SeedTex).a;
			fixed4 c =  (_Color * (1.0f- a)) + (tex2D(_SeedTex, IN.uv_SeedTex) * a);
			o.Albedo = c.rgb;
			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Normal = UnpackNormal (tex2D (_BumpMap, IN.uv_BumpMap));
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
