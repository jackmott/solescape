Shader "Custom/TestShader" {
	Properties {
		_Z ("Z",Range (0,10)) = 0
	}
	SubShader {
	Tags {"Queue" = "Geometry"}

	
	

		Pass {
			
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			#include "UnityCG.cginc"			

			uniform sampler2D _MainTex;
			uniform float4 _MainTex_ST;
			uniform float _Z;

			#ifndef __noise_hlsl_
			#define __noise_hlsl_

 

			// hash based 3d value noise
			// function taken from [url]https://www.shadertoy.com/view/XslGRr[/url]
			// Created by inigo quilez - iq/2013
			// License Creative Commons Attribution-NonCommercial-ShareAlike 3.0 Unported License. 
			// ported from GLSL to HLSL

 

			float hash( float n )
			{
				return frac(sin(n)*43758.5453);
			}

 

			float noise( float3 x )
			{

			    // The noise function returns a value in the range -1.0f -> 1.0f 
				float3 p = floor(x);
				float3 f = frac(x);
 
				f = f*f*(3.0-2.0*f);
				float n = p.x + p.y*57.0 + 113.0*p.z;
 
				return lerp(lerp(lerp( hash(n+0.0), hash(n+1.0),f.x),
					lerp( hash(n+57.0), hash(n+58.0),f.x),f.y),
					lerp(lerp( hash(n+113.0), hash(n+114.0),f.x),
					lerp( hash(n+170.0), hash(n+171.0),f.x),f.y),f.z);
			}
 
			#endif

			struct vertexInput
			{
				float4 vertex : POSITION;
				//float4 tangent :TANGENT;
				//float3 normal : NORMAL;
				//float4 texcoord : TEXCOORD0;
				//float4 texcoord1 : TEXCOORD1;
				//fixed4 color : COLOR;
			};

			struct fragmentInput
			{
				float4 color : COLOR0;	
				float4 pos : SV_POSITION;				
				float4 v : TEXCOORD0;
			};

			fragmentInput vert (vertexInput i)
			{
			
				fragmentInput o;		
				o.pos = mul(UNITY_MATRIX_MVP,i.vertex);						
				o.v = i.vertex;
				return o;
			}

			half4 frag (fragmentInput i) : COLOR
			{
				float n = noise(float3(12*i.v.x+_Z,12*i.v.y+_Z,i.v.z*12+_Z));
				return half4(0.0,0.0,n,1.0);
				//return i.color;
			}
			
			

			
		

			ENDCG
		}
	} 
	FallBack "Diffuse"
}
