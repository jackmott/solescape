Shader "Custom/TestShader" {
	Properties {
		_MainTex("Main Texutre",2D) = "white" {}
	}
	SubShader {
	Tags {"Queue" = "Geometry"}

	Blend SrcAlpha OneMinusSrcAlpha
	

		Pass {
			Cull 
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"			

			uniform sampler2D _MainTex;
			uniform float4 _MainTex_ST;

			struct vertexInput
			{
				float4 vertex : POSITION;
				//float4 tangent :TANGENT;
				//float3 normal : NORMAL;
				float4 texcoord : TEXCOORD0;
				//float4 texcoord1 : TEXCOORD1;
				//fixed4 color : COLOR;
			};

			struct fragmentInput
			{
				float4 pos: SV_POSITION;	
				half2 uv : TEXCOORD0;			
			};

			fragmentInput vert (vertexInput i)
			{
			
				fragmentInput o;
				o.uv = TRANSFORM_TEX(i.texcoord,_MainTex);
				o.pos = mul( UNITY_MATRIX_MVP, i.vertex);
				return o;
			}

			half4 frag (fragmentInput i) : COLOR
			{
				
				return tex2D(_MainTex,i.uv);
			}

			ENDCG
		}
	} 
	FallBack "Diffuse"
}
