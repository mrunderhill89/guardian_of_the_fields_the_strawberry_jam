Shader "Custom/StrawberrySkin" {
	Properties {
		_Green ("Green Color", Color) = (1,1,1,1)
		_Pink ("Pink Color", Color) = (1,1,1,1)
		_Ripe ("Ripe Color", Color) = (1,1,1,1)
		_Overripe ("Overripe Color", Color) = (1,1,1,1)
		_Mold ("Mold Color", Color) = (1,1,1,1)
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
		fixed4 _Green;
		fixed4 _Pink;
		fixed4 _Ripe;
		fixed4 _Overripe;
		fixed4 _Mold;

		float gradient(float vertex, float value, float range){
			return max(min((1.0 - abs(value - vertex)/range),1.0),0.0);
		}

		void surf (Input IN, inout SurfaceOutputStandard o) {
			float green = gradient(0.0, _Quality, 0.4);
			float pink = gradient(0.4, _Quality, 0.3);
			float ripe = gradient(0.8, _Quality, 1.0);
			float over = gradient(1.2, _Quality, 0.5);
			float mold = gradient(2.0, _Quality, 0.5);
			fixed4 spot_color = 
				(green * _Green)
				+(pink * _Pink)
				+(ripe * _Ripe)
				+(over * _Overripe)
				+(mold * _Mold);
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
