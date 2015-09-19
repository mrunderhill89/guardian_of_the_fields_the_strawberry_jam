Shader "Custom/StrawberrySkin" {
	Properties {
		_Unripe ("Unripe Color", Color) = (1,1,1,1)
		_Ripe ("Ripe Color", Color) = (1,1,1,1)
		_Overripe ("Overripe Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
		_Quality ("Quality", Range(0,2))= 1.0
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;

		struct Input {
			float2 uv_MainTex;
		};

		half _Glossiness;
		half _Metallic;
		half _Quality;
		fixed4 _Unripe;
		fixed4 _Ripe;
		fixed4 _Overripe;

		void surf (Input IN, inout SurfaceOutputStandard o) {
			float ur_amount = max(min((1.0 - _Quality),1.0),0.0);
			float r_amount =  max(min((1.0 - abs(1.0 - _Quality)),1.0),0.0);
			float or_amount = max(min(_Quality - 1.0,1.0),0.0);
			fixed4 spot_color = (_Unripe * ur_amount)+(_Ripe * r_amount)+(_Overripe * or_amount);
			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * spot_color;
			o.Albedo = c.rgb;
			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = c.a;
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
