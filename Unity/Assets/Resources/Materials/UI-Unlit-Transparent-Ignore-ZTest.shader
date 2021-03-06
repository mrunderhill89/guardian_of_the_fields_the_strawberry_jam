Shader "Custom/UI-Unlit-Transparent-Ignore-Z"
{
Properties {
	_Color ("Main Color", Color) = (1,1,1,1)
    _MainTex ("Base (RGB)", 2D) = "white" {}
}

SubShader {
    Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
    LOD 100
   
    ZWrite Off
    ZTest Always
    Blend SrcAlpha OneMinusSrcAlpha
   
    Pass {  
        CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
           
            #include "UnityCG.cginc"

            struct appdata_t {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f {
                float4 vertex : SV_POSITION;
                half2 texcoord : TEXCOORD0;
            };

            sampler2D _MainTex;
           
            float4 _MainTex_ST;
			fixed4 _Color;
           
            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
                o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
                return o;
            }
           
            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = _Color * tex2D(_MainTex, i.texcoord);
               
                return fixed4(col.r, col.g, col.b, col.a);
            }
        ENDCG
    }
}
	FallBack "UI/Default"
}
