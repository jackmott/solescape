// Shader created with Shader Forge Beta 0.23 
// Shader Forge (c) Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:0.23;sub:START;pass:START;ps:lgpr:1,nrmq:1,limd:1,blpr:0,bsrc:0,bdst:1,culm:0,dpts:2,wrdp:True,uamb:True,mssp:False,ufog:False,aust:True,igpj:False,qofs:0,lico:1,qpre:1,flbk:Diffuse,rntp:1,lmpd:False,lprd:False,enco:False,frtr:True,vitr:True,dbil:False,rmgx:True,hqsc:True,hqlp:False,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,ofsf:0,ofsu:0;n:type:ShaderForge.SFN_Final,id:1,x:32719,y:32712|diff-75-OUT,spec-148-OUT,normal-15-RGB,emission-164-OUT;n:type:ShaderForge.SFN_Tex2d,id:3,x:33406,y:32575,ptlb:MainTex,tex:157fae5bbddbdff47b5ab6aadcf967cc,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Tex2d,id:9,x:33342,y:33084,ptlb:Illumin (A),tex:42832027376e5684d85b8bcf751ada7a,ntxv:2,isnm:False;n:type:ShaderForge.SFN_Tex2d,id:15,x:33244,y:32699,ptlb:BumpMap,tex:ba877fc6b2d9f20439134af449b664a2,ntxv:3,isnm:True;n:type:ShaderForge.SFN_Color,id:21,x:33535,y:32748,ptlb:Specular Color,c1:1,c2:1,c3:1,c4:1;n:type:ShaderForge.SFN_Slider,id:27,x:33469,y:32919,ptlb:Shininess,min:0,cur:7.518797,max:100;n:type:ShaderForge.SFN_Color,id:74,x:33234,y:32445,ptlb:Color,c1:1,c2:1,c3:1,c4:1;n:type:ShaderForge.SFN_Multiply,id:75,x:33045,y:32583|A-74-RGB,B-3-RGB;n:type:ShaderForge.SFN_Multiply,id:102,x:33145,y:33000|A-127-RGB,B-9-RGB;n:type:ShaderForge.SFN_Color,id:127,x:33503,y:33028,ptlb:IllumColor,c1:1,c2:1,c3:1,c4:1;n:type:ShaderForge.SFN_Add,id:148,x:33275,y:32877|A-21-RGB,B-27-OUT;n:type:ShaderForge.SFN_Multiply,id:164,x:32965,y:32856|A-102-OUT,B-75-OUT;proporder:74-21-27-127-3-9-15;pass:END;sub:END;*/

Shader "Shader Forge/BuildingShaderOpaque" {
    Properties {
        _Color ("Color", Color) = (1,1,1,1)
        _SpecularColor ("Specular Color", Color) = (1,1,1,1)
        _Shininess ("Shininess", Range(0, 100)) = 0
        _IllumColor ("IllumColor", Color) = (1,1,1,1)
        _MainTex ("MainTex", 2D) = "white" {}
        _IlluminA ("Illumin (A)", 2D) = "black" {}
        _BumpMap ("BumpMap", 2D) = "bump" {}
    }
    SubShader {
        Tags {
            "RenderType"="Opaque"
        }
        Pass {
            Name "ForwardBase"
            Tags {
                "LightMode"="ForwardBase"
            }
            
            
            Fog {Mode Off}
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #pragma multi_compile_fwdbase_fullshadows
            #pragma exclude_renderers xbox360 ps3 flash 
            #pragma target 3.0
            uniform float4 _LightColor0;
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            uniform sampler2D _IlluminA; uniform float4 _IlluminA_ST;
            uniform sampler2D _BumpMap; uniform float4 _BumpMap_ST;
            uniform float4 _SpecularColor;
            uniform float _Shininess;
            uniform float4 _Color;
            uniform float4 _IllumColor;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float4 uv0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float4 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                float3 tangentDir : TEXCOORD3;
                float3 binormalDir : TEXCOORD4;
                LIGHTING_COORDS(5,6)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o;
                o.uv0 = v.uv0;
                o.normalDir = mul(float4(v.normal,0), _World2Object).xyz;
                o.tangentDir = normalize( mul( _Object2World, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.binormalDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                o.posWorld = mul(_Object2World, v.vertex);
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            fixed4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3x3 tangentTransform = float3x3( i.tangentDir, i.binormalDir, i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
/////// Normals:
                float2 node_212 = i.uv0;
                float3 normalLocal = UnpackNormal(tex2D(_BumpMap,TRANSFORM_TEX(node_212.rg, _BumpMap))).rgb;
                float3 normalDirection =  mul( normalLocal, tangentTransform ); // Perturbed normals
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 attenColor = attenuation * _LightColor0.xyz;
/////// Diffuse:
                float NdotL = dot( normalDirection, lightDirection );
                float3 diffuse = max( 0.0, NdotL) * attenColor + UNITY_LIGHTMODEL_AMBIENT.xyz;
////// Emissive:
                float3 node_75 = (_Color.rgb*tex2D(_MainTex,TRANSFORM_TEX(node_212.rg, _MainTex)).rgb);
                float3 emissive = ((_IllumColor.rgb*tex2D(_IlluminA,TRANSFORM_TEX(node_212.rg, _IlluminA)).rgb)*node_75);
///////// Gloss:
                float gloss = exp2(0.5*10.0+1.0);
////// Specular:
                NdotL = max(0.0, NdotL);
                float3 specularColor = (_SpecularColor.rgb+_Shininess);
                float3 specular = attenColor * pow(max(0,dot(halfDirection,normalDirection)),gloss) * specularColor;
                float3 finalColor = 0;
                float3 diffuseLight = diffuse;
                finalColor += diffuseLight * node_75;
                finalColor += specular;
                finalColor += emissive;
/// Final Color:
                return fixed4(finalColor,1);
            }
            ENDCG
        }
        Pass {
            Name "ForwardAdd"
            Tags {
                "LightMode"="ForwardAdd"
            }
            Blend One One
            
            
            Fog { Color (0,0,0,0) }
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDADD
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #pragma multi_compile_fwdadd_fullshadows
            #pragma exclude_renderers xbox360 ps3 flash 
            #pragma target 3.0
            uniform float4 _LightColor0;
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            uniform sampler2D _IlluminA; uniform float4 _IlluminA_ST;
            uniform sampler2D _BumpMap; uniform float4 _BumpMap_ST;
            uniform float4 _SpecularColor;
            uniform float _Shininess;
            uniform float4 _Color;
            uniform float4 _IllumColor;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float4 uv0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float4 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                float3 tangentDir : TEXCOORD3;
                float3 binormalDir : TEXCOORD4;
                LIGHTING_COORDS(5,6)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o;
                o.uv0 = v.uv0;
                o.normalDir = mul(float4(v.normal,0), _World2Object).xyz;
                o.tangentDir = normalize( mul( _Object2World, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.binormalDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                o.posWorld = mul(_Object2World, v.vertex);
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            fixed4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3x3 tangentTransform = float3x3( i.tangentDir, i.binormalDir, i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
/////// Normals:
                float2 node_213 = i.uv0;
                float3 normalLocal = UnpackNormal(tex2D(_BumpMap,TRANSFORM_TEX(node_213.rg, _BumpMap))).rgb;
                float3 normalDirection =  mul( normalLocal, tangentTransform ); // Perturbed normals
                float3 lightDirection = normalize(lerp(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz - i.posWorld.xyz,_WorldSpaceLightPos0.w));
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 attenColor = attenuation * _LightColor0.xyz;
/////// Diffuse:
                float NdotL = dot( normalDirection, lightDirection );
                float3 diffuse = max( 0.0, NdotL) * attenColor;
///////// Gloss:
                float gloss = exp2(0.5*10.0+1.0);
////// Specular:
                NdotL = max(0.0, NdotL);
                float3 specularColor = (_SpecularColor.rgb+_Shininess);
                float3 specular = attenColor * pow(max(0,dot(halfDirection,normalDirection)),gloss) * specularColor;
                float3 finalColor = 0;
                float3 diffuseLight = diffuse;
                float3 node_75 = (_Color.rgb*tex2D(_MainTex,TRANSFORM_TEX(node_213.rg, _MainTex)).rgb);
                finalColor += diffuseLight * node_75;
                finalColor += specular;
/// Final Color:
                return fixed4(finalColor * 1,0);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
