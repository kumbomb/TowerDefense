// Toony Colors Pro+Mobile 2
// (c) 2014-2021 Jean Moreno

Shader "Custom/Hologram"
{
	Properties
	{

		[TCP2HeaderHelp(Emission)]
		[NoScaleOffset] _MainTex ("Emission Texture", 2D) = "white" {}
		 _EmissionColor ("Emission Color", Float) = 1
		[TCP2Separator]
		
		[TCP2HeaderHelp(Silhouette Pass)]
		_SilhouetteColor ("Silhouette Color", Color) = (0,0,0,0.33)
		[TCP2Separator]
		_NDVMinFrag ("NDV Min", Range(0,2)) = 0.5
		_NDVMaxFrag ("NDV Max", Range(0,2)) = 1
		[TCP2Separator]
		// Custom Material Properties
		[TCP2ColorNoAlpha] [HDR] _HologramColor ("Hologram Color", Color) = (0,0.502,1,1)
		 _ScanlinesTex ("Scanlines Texture", 2D) = "white" {}
		[TCP2UVScrolling] _ScanlinesTex_SC ("Scanlines Texture UV Scrolling", Vector) = (1,1,0,0)

		[ToggleOff(_RECEIVE_SHADOWS_OFF)] _ReceiveShadowsOff ("Receive Shadows", Float) = 1

		// Avoid compile error if the properties are ending with a drawer
		[HideInInspector] __dummy__ ("unused", Float) = 0
	}

	SubShader
	{
		Tags
		{
			"RenderPipeline" = "UniversalPipeline"
			"RenderType"="Opaque"
			"Queue"="Geometry+10" // Make sure that the objects are rendered later to avoid sorting issues with the transparent silhouette
		}

		HLSLINCLUDE
		#define fixed half
		#define fixed2 half2
		#define fixed3 half3
		#define fixed4 half4

		#if UNITY_VERSION >= 202020
			#define URP_10_OR_NEWER
		#endif

		// Texture/Sampler abstraction
		#define TCP2_TEX2D_WITH_SAMPLER(tex)						TEXTURE2D(tex); SAMPLER(sampler##tex)
		#define TCP2_TEX2D_NO_SAMPLER(tex)							TEXTURE2D(tex)
		#define TCP2_TEX2D_SAMPLE(tex, samplertex, coord)			SAMPLE_TEXTURE2D(tex, sampler##samplertex, coord)
		#define TCP2_TEX2D_SAMPLE_LOD(tex, samplertex, coord, lod)	SAMPLE_TEXTURE2D_LOD(tex, sampler##samplertex, coord, lod)

		#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
		#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

		// Uniforms

		// Custom Material Properties

		TCP2_TEX2D_WITH_SAMPLER(_ScanlinesTex);

		// Shader Properties
		TCP2_TEX2D_WITH_SAMPLER(_MainTex);

		CBUFFER_START(UnityPerMaterial)
			
			// Custom Material Properties
			half4 _HologramColor;
			float4 _ScanlinesTex_ST;
			float4 _ScanlinesTex_TexelSize;
			half4 _ScanlinesTex_SC;

			// Shader Properties
			fixed4 _SilhouetteColor;
			float _NDVMinFrag;
			float _NDVMaxFrag;
			float _EmissionColor;
		CBUFFER_END

		// Built-in renderer (CG) to SRP (HLSL) bindings
		#define UnityObjectToClipPos TransformObjectToHClip
		#define _WorldSpaceLightPos0 _MainLightPosition
		
		ENDHLSL

		// Silhouette Pass
		Pass
		{
			Name "Silhouette"
			Tags { "LightMode" = "Silhouette" }
			Tags
			{
			}
			Blend SrcAlpha OneMinusSrcAlpha
			ZTest Greater
			ZWrite Off

			Stencil
			{
				Ref 1
				Comp NotEqual
				Pass Replace
				ReadMask 1
				WriteMask 1
			}

			HLSLPROGRAM
			#pragma vertex vertex_silhouette
			#pragma fragment fragment_silhouette
			#pragma multi_compile_instancing
			#pragma target 3.0

			struct appdata_sil
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct v2f_sil
			{
				float4 vertex : SV_POSITION;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			v2f_sil vertex_silhouette (appdata_sil v)
			{
				v2f_sil output = (v2f_sil)0;

				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, output);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

				float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				worldPos.xyz = ( worldPos.xyz + float3(-0.05,0,0) * saturate((0.0333 - (sin(_Time.z - worldPos.y*5)+1)*0.5)*30) );
				v.vertex.xyz = mul(unity_WorldToObject, float4(worldPos, 1)).xyz;
				output.vertex = TransformObjectToHClip(v.vertex.xyz);

				return output;
			}

			half4 fragment_silhouette (v2f_sil input) : SV_Target
			{

				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

				// Shader Properties Sampling
				float4 __silhouetteColor = ( _SilhouetteColor.rgba );

				return __silhouetteColor;
			}
			ENDHLSL
		}

		Pass
		{
			Name "Main"
			Tags
			{
				"LightMode"="UniversalForward"
			}
			BlendOp Add

			// Stencil value used for Silhouette Pass to make sure we don't see a
			// silhouette when the same mesh occludes parts of itself
			Stencil
			{
				Ref 1
				Pass Replace
				ReadMask 1
				WriteMask 1
			}

			HLSLPROGRAM
			// Required to compile gles 2.0 with standard SRP library
			// All shaders must be compiled with HLSLcc and currently only gles is not using HLSLcc by default
			#pragma prefer_hlslcc gles
			#pragma exclude_renderers d3d11_9x
			#pragma target 3.0

			// -------------------------------------
			// Material keywords
			#pragma shader_feature_local _ _RECEIVE_SHADOWS_OFF

			// -------------------------------------
			// Universal Render Pipeline keywords
			#pragma multi_compile _ _MAIN_LIGHT_SHADOWS
			#pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
			#pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
			#pragma multi_compile_fragment _ _ADDITIONAL_LIGHT_SHADOWS
			#pragma multi_compile_fragment _ _SHADOWS_SOFT
			#pragma multi_compile _ LIGHTMAP_SHADOW_MIXING
			#pragma multi_compile _ SHADOWS_SHADOWMASK

			// -------------------------------------

			//--------------------------------------
			// GPU Instancing
			#pragma multi_compile_instancing

			#pragma vertex Vertex
			#pragma fragment Fragment

			// vertex input
			struct Attributes
			{
				float4 vertex       : POSITION;
				float3 normal       : NORMAL;
				float4 texcoord0 : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			// vertex output / fragment input
			struct Varyings
			{
				float4 positionCS     : SV_POSITION;
				float3 normal         : NORMAL;
				float4 worldPosAndFog : TEXCOORD0;
			#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
				float4 shadowCoord    : TEXCOORD1; // compute shadow coord per-vertex for the main light
			#endif
			#ifdef _ADDITIONAL_LIGHTS_VERTEX
				half3 vertexLights : TEXCOORD2;
			#endif
				float4 screenPosition : TEXCOORD3;
				float2 pack1 : TEXCOORD4; /* pack1.xy = texcoord0 */
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			Varyings Vertex(Attributes input)
			{
				Varyings output = (Varyings)0;

				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_TRANSFER_INSTANCE_ID(input, output);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

				// Texture Coordinates
				output.pack1.xy = input.texcoord0.xy;

				float3 worldPos = mul(unity_ObjectToWorld, input.vertex).xyz;
				worldPos.xyz = ( worldPos.xyz + float3(-0.05,0,0) * saturate((0.0333 - (sin(_Time.z - worldPos.y*5)+1)*0.5)*30) );
				input.vertex.xyz = mul(unity_WorldToObject, float4(worldPos, 1)).xyz;
				VertexPositionInputs vertexInput = GetVertexPositionInputs(input.vertex.xyz);
			#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
				output.shadowCoord = GetShadowCoord(vertexInput);
			#endif
				float4 clipPos = vertexInput.positionCS;

				float4 screenPos = ComputeScreenPos(clipPos);
				output.screenPosition.xyzw = screenPos;

				VertexNormalInputs vertexNormalInput = GetVertexNormalInputs(input.normal);
			#ifdef _ADDITIONAL_LIGHTS_VERTEX
				// Vertex lighting
				output.vertexLights = VertexLighting(vertexInput.positionWS, vertexNormalInput.normalWS);
			#endif

				// world position
				output.worldPosAndFog = float4(vertexInput.positionWS.xyz, 0);

				// normal
				output.normal = normalize(vertexNormalInput.normalWS);

				// clip position
				output.positionCS = vertexInput.positionCS;

				return output;
			}

			half4 Fragment(Varyings input) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

				float3 positionWS = input.worldPosAndFog.xyz;
				float3 normalWS = normalize(input.normal);
				half3 viewDirWS = SafeNormalize(GetCameraPositionWS() - positionWS);

				//Screen Space UV
				float2 screenUV = input.screenPosition.xyzw.xy / input.screenPosition.xyzw.w;
				
				// Custom Material Properties Sampling
				half4 value__ScanlinesTex = TCP2_TEX2D_SAMPLE(_ScanlinesTex, _ScanlinesTex, screenUV * _ScanlinesTex_TexelSize.xy * _ScreenParams.xy * _ScanlinesTex_ST.xy + frac(_Time.yy * _ScanlinesTex_SC.xy) + _ScanlinesTex_ST.zw).rgba;

				// Shader Properties Sampling
				float __ndvMinFrag = ( _NDVMinFrag );
				float __ndvMaxFrag = ( _NDVMaxFrag );
				float4 __albedo = ( float4(0,0,0,1) );
				float4 __mainColor = ( float4(0,0,0,1) );
				float __alpha = ( __albedo.a * __mainColor.a );
				float __ambientIntensity = ( 1.0 );
				float3 __emission = ( TCP2_TEX2D_SAMPLE(_MainTex, _MainTex, input.pack1.xy).rgb * _HologramColor.rgb * _EmissionColor * value__ScanlinesTex.aaa );
				float3 __shadowColor = ( float3(0,0,0) );
				float3 __highlightColor = ( float3(0,0,0) );

				half ndv = abs(dot(viewDirWS, normalWS));
				half ndvRaw = ndv;
				ndv = 1 - ndv;
				ndv = smoothstep(__ndvMinFrag, __ndvMaxFrag, ndv);

				// main texture
				half3 albedo = __albedo.rgb;
				half alpha = __alpha;

				half3 emission = half3(0,0,0);
				
				albedo *= __mainColor.rgb;

				// main light: direction, color, distanceAttenuation, shadowAttenuation
			#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
				float4 shadowCoord = input.shadowCoord;
			#elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)
				float4 shadowCoord = TransformWorldToShadowCoord(positionWS);
			#else
				float4 shadowCoord = float4(0, 0, 0, 0);
			#endif

			#if defined(URP_10_OR_NEWER)
				#if defined(SHADOWS_SHADOWMASK) && defined(LIGHTMAP_ON)
					half4 shadowMask = SAMPLE_SHADOWMASK(input.uvLM);
				#elif !defined (LIGHTMAP_ON)
					half4 shadowMask = unity_ProbesOcclusion;
				#else
					half4 shadowMask = half4(1, 1, 1, 1);
				#endif

				Light mainLight = GetMainLight(shadowCoord, positionWS, shadowMask);
			#else
				Light mainLight = GetMainLight(shadowCoord);
			#endif

				// ambient or lightmap
				// Samples SH fully per-pixel. SampleSHVertex and SampleSHPixel functions
				// are also defined in case you want to sample some terms per-vertex.
				half3 bakedGI = SampleSH(normalWS);
				half occlusion = 1;

				half3 indirectDiffuse = bakedGI;
				indirectDiffuse *= occlusion * albedo * __ambientIntensity;
				emission += ( __emission * ndv.xxx );

				half3 lightDir = mainLight.direction;
				half3 lightColor = mainLight.color.rgb;

				half atten = mainLight.shadowAttenuation * mainLight.distanceAttenuation;

				half ndl = dot(normalWS, lightDir);
				half3 ramp;
				
				ndl = saturate(ndl);
				ramp = ndl.xxx;

				// apply attenuation
				ramp *= atten;

				half3 color = half3(0,0,0);
				half3 accumulatedRamp = ramp * max(lightColor.r, max(lightColor.g, lightColor.b));
				half3 accumulatedColors = ramp * lightColor.rgb;

				// Additional lights loop
			#ifdef _ADDITIONAL_LIGHTS
				uint additionalLightsCount = GetAdditionalLightsCount();
				for (uint lightIndex = 0u; lightIndex < additionalLightsCount; ++lightIndex)
				{
					#if defined(URP_10_OR_NEWER)
						Light light = GetAdditionalLight(lightIndex, positionWS, shadowMask);
					#else
						Light light = GetAdditionalLight(lightIndex, positionWS);
					#endif
					half atten = light.shadowAttenuation * light.distanceAttenuation;
					half3 lightDir = light.direction;
					half3 lightColor = light.color.rgb;

					half ndl = dot(normalWS, lightDir);
					half3 ramp;
					
					ndl = saturate(ndl);
					ramp = ndl.xxx;

					// apply attenuation (shadowmaps & point/spot lights attenuation)
					ramp *= atten;

					accumulatedRamp += ramp * max(lightColor.r, max(lightColor.g, lightColor.b));
					accumulatedColors += ramp * lightColor.rgb;

				}
			#endif
			#ifdef _ADDITIONAL_LIGHTS_VERTEX
				color += input.vertexLights * albedo;
			#endif

				accumulatedRamp = saturate(accumulatedRamp);
				half3 shadowColor = (1 - accumulatedRamp.rgb) * __shadowColor;
				accumulatedRamp = accumulatedColors.rgb * __highlightColor + shadowColor;
				color += albedo * accumulatedRamp;

				// apply ambient
				color += indirectDiffuse;

				color += emission;

				return half4(color, alpha);
			}
			ENDHLSL
		}

		// Depth & Shadow Caster Passes
		HLSLINCLUDE

		#if defined(SHADOW_CASTER_PASS) || defined(DEPTH_ONLY_PASS)

			#define fixed half
			#define fixed2 half2
			#define fixed3 half3
			#define fixed4 half4

			float3 _LightDirection;
			float3 _LightPosition;

			struct Attributes
			{
				float4 vertex   : POSITION;
				float3 normal   : NORMAL;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct Varyings
			{
				float4 positionCS     : SV_POSITION;
			#if defined(DEPTH_ONLY_PASS)
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			#endif
			};

			float4 GetShadowPositionHClip(Attributes input)
			{
				float3 positionWS = TransformObjectToWorld(input.vertex.xyz);
				float3 normalWS = TransformObjectToWorldNormal(input.normal);

				#if _CASTING_PUNCTUAL_LIGHT_SHADOW
					float3 lightDirectionWS = normalize(_LightPosition - positionWS);
				#else
					float3 lightDirectionWS = _LightDirection;
				#endif
				float4 positionCS = TransformWorldToHClip(ApplyShadowBias(positionWS, normalWS, lightDirectionWS));

				#if UNITY_REVERSED_Z
					positionCS.z = min(positionCS.z, UNITY_NEAR_CLIP_VALUE);
				#else
					positionCS.z = max(positionCS.z, UNITY_NEAR_CLIP_VALUE);
				#endif

				return positionCS;
			}

			Varyings ShadowDepthPassVertex(Attributes input)
			{
				Varyings output = (Varyings)0;
				UNITY_SETUP_INSTANCE_ID(input);
				#if defined(DEPTH_ONLY_PASS)
					UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);
				#endif

				float3 worldPos = mul(unity_ObjectToWorld, input.vertex).xyz;
				worldPos.xyz = ( worldPos.xyz + float3(-0.05,0,0) * saturate((0.0333 - (sin(_Time.z - worldPos.y*5)+1)*0.5)*30) );
				input.vertex.xyz = mul(unity_WorldToObject, float4(worldPos, 1)).xyz;

				#if defined(DEPTH_ONLY_PASS)
					output.positionCS = TransformObjectToHClip(input.vertex.xyz);
				#elif defined(SHADOW_CASTER_PASS)
					output.positionCS = GetShadowPositionHClip(input);
				#else
					output.positionCS = float4(0,0,0,0);
				#endif

				return output;
			}

			half4 ShadowDepthPassFragment(Varyings input) : SV_TARGET
			{
				#if defined(DEPTH_ONLY_PASS)
					UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
				#endif

				// Shader Properties Sampling
				float4 __albedo = ( float4(0,0,0,1) );
				float4 __mainColor = ( float4(0,0,0,1) );
				float __alpha = ( __albedo.a * __mainColor.a );

				half3 albedo = half3(1,1,1);
				half alpha = __alpha;
				half3 emission = half3(0,0,0);

				return 0;
			}

		#endif
		ENDHLSL

		Pass
		{
			Name "ShadowCaster"
			Tags
			{
				"LightMode" = "ShadowCaster"
			}

			ZWrite On
			ZTest LEqual

			HLSLPROGRAM
			// Required to compile gles 2.0 with standard srp library
			#pragma prefer_hlslcc gles
			#pragma exclude_renderers d3d11_9x
			#pragma target 2.0

			// using simple #define doesn't work, we have to use this instead
			#pragma multi_compile SHADOW_CASTER_PASS

			//--------------------------------------
			// GPU Instancing
			#pragma multi_compile_instancing
			#pragma multi_compile_vertex _ _CASTING_PUNCTUAL_LIGHT_SHADOW

			#pragma vertex ShadowDepthPassVertex
			#pragma fragment ShadowDepthPassFragment

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Shadows.hlsl"

			ENDHLSL
		}

		Pass
		{
			Name "DepthOnly"
			Tags
			{
				"LightMode" = "DepthOnly"
			}

			ZWrite On
			ColorMask 0

			HLSLPROGRAM

			// Required to compile gles 2.0 with standard srp library
			#pragma prefer_hlslcc gles
			#pragma exclude_renderers d3d11_9x
			#pragma target 2.0

			//--------------------------------------
			// GPU Instancing
			#pragma multi_compile_instancing

			// using simple #define doesn't work, we have to use this instead
			#pragma multi_compile DEPTH_ONLY_PASS

			#pragma vertex ShadowDepthPassVertex
			#pragma fragment ShadowDepthPassFragment

			ENDHLSL
		}

	}

	FallBack "Hidden/InternalErrorShader"
	CustomEditor "ToonyColorsPro.ShaderGenerator.MaterialInspector_SG2"
}

/* TCP_DATA u config(unity:"2022.3.24f1";ver:"2.4.0";tmplt:"SG2_Template_URP";features:list["UNITY_5_4","UNITY_5_5","EMISSION","OUTLINE_CONSTANT_SIZE","OUTLINE_PIXEL_PERFECT","NO_RAMP","UNITY_5_6","UNITY_2017_1","UNITY_2018_1","UNITY_2018_2","UNITY_2018_3","UNITY_2019_1","UNITY_2019_2","UNITY_2019_3","UNITY_2019_4","UNITY_2020_1","UNITY_2021_1","DEPTH_PREPASS","OUTLINE_URP_FEATURE","BLEND_OP","PASS_SILHOUETTE","SILHOUETTE_URP_FEATURE","SILHOUETTE_STENCIL","TEMPLATE_LWRP"];flags:list["noforwardadd","novertexlights"];flags_extra:dict[];keywords:dict[RENDER_TYPE="Opaque",RampTextureDrawer="[TCP2Gradient]",RampTextureLabel="Ramp Texture",SHADER_TARGET="3.0"];shaderProperties:list[sp(name:"Albedo";imps:list[imp_constant(type:color_rgba;fprc:float;fv:1;f2v:(1, 1);f3v:(1, 1, 1);f4v:(1, 1, 1, 1);cv:RGBA(0, 0, 0, 1);guid:"d2a2970d-367b-4e1c-be8d-0caeeb24fbc4";op:Multiply;lbl:"Albedo";gpu_inst:False;locked:False;impl_index:-1)];layers:list[];unlocked:list[];clones:dict[];isClone:False),sp(name:"Main Color";imps:list[imp_constant(type:color_rgba;fprc:float;fv:1;f2v:(1, 1);f3v:(1, 1, 1);f4v:(1, 1, 1, 1);cv:RGBA(0, 0, 0, 1);guid:"781390df-aa25-4555-9e99-f061a8d84a81";op:Multiply;lbl:"Color";gpu_inst:False;locked:False;impl_index:-1)];layers:list[];unlocked:list[];clones:dict[];isClone:False),,,sp(name:"Highlight Color";imps:list[imp_constant(type:color;fprc:float;fv:1;f2v:(1, 1);f3v:(1, 1, 1);f4v:(1, 1, 1, 1);cv:RGBA(0, 0, 0, 1);guid:"96873fb2-f1ff-4326-b398-169b31766775";op:Multiply;lbl:"Highlight Color";gpu_inst:False;locked:False;impl_index:-1)];layers:list[];unlocked:list[];clones:dict[];isClone:False),sp(name:"Shadow Color";imps:list[imp_constant(type:color;fprc:float;fv:1;f2v:(1, 1);f3v:(1, 1, 1);f4v:(1, 1, 1, 1);cv:RGBA(0, 0, 0, 1);guid:"8123499c-9c63-40ed-84ea-ab7f6d6e8260";op:Multiply;lbl:"Shadow Color";gpu_inst:False;locked:False;impl_index:-1)];layers:list[];unlocked:list[];clones:dict[];isClone:False),sp(name:"Emission";imps:list[imp_mp_texture(uto:False;tov:"";tov_lbl:"";gto:False;sbt:False;scr:False;scv:"";scv_lbl:"";gsc:False;roff:False;goff:False;sin_anm:False;sin_anmv:"";sin_anmv_lbl:"";gsin:False;notile:False;triplanar_local:False;def:"white";locked_uv:False;uv:0;cc:3;chan:"RGB";mip:-1;mipprop:False;ssuv_vert:False;ssuv_obj:False;uv_type:Texcoord;uv_chan:"XZ";tpln_scale:1;uv_shaderproperty:__NULL__;uv_cmp:__NULL__;sep_sampler:__NULL__;prop:"_MainTex";md:"";gbv:False;custom:False;refs:"";pnlock:False;guid:"7766d999-6f78-4fa8-9981-5f6f011189f5";op:Multiply;lbl:"Emission Texture";gpu_inst:False;locked:False;impl_index:-1),imp_ct(lct:"_HologramColor";cc:3;chan:"RGB";avchan:"RGBA";guid:"2897e734-c440-4185-aeed-9949183bc14e";op:Multiply;lbl:"Emission";gpu_inst:False;locked:False;impl_index:-1),imp_mp_float(def:1;prop:"_EmissionColor";md:"";gbv:False;custom:False;refs:"";pnlock:False;guid:"84e9a450-04a6-4998-bee7-b8f5dffbda4a";op:Multiply;lbl:"Emission Color";gpu_inst:False;locked:False;impl_index:-1),imp_ct(lct:"_ScanlinesTex";cc:3;chan:"AAA";avchan:"RGBA";guid:"ff5c6b86-d50b-4945-96e5-f4eba3c612cb";op:Multiply;lbl:"Emission";gpu_inst:False;locked:False;impl_index:-1),imp_generic(cc:3;chan:"XXX";source_id:"float ndv3fragment";needed_features:"USE_NDV_FRAGMENT";custom_code_compatible:False;options_v:dict[Use Min/Max Properties=True,Invert=True,Ignore Normal Map=False];guid:"d4833553-1f51-4b1f-a65b-a6a51888d5b5";op:Multiply;lbl:"Emission";gpu_inst:False;locked:False;impl_index:-1)];layers:list[];unlocked:list[];clones:dict[];isClone:False),,,,,,,sp(name:"Vertex Position World";imps:list[imp_hook(guid:"6f8169be-3773-4d71-bc9a-8c03dcdffc20";op:Multiply;lbl:"worldPos.xyz";gpu_inst:False;locked:False;impl_index:0),imp_customcode(prepend_type:Disabled;prepend_code:"";prepend_file:"";prepend_file_block:"";preprend_params:dict[];code:"+ float3(-0.05,0,0) * saturate((0.0333 - (sin(_Time.z - worldPos.y*5)+1)*0.5)*30)";guid:"aad47089-ac8e-4333-bc45-5141ba70f31d";op:Multiply;lbl:"Vertex Position World";gpu_inst:False;locked:False;impl_index:-1)];layers:list[];unlocked:list[];clones:dict[];isClone:False),,,,,,,,,,,,,sp(name:"Outline Color Vertex";imps:list[imp_ct(lct:"_HologramColor";cc:4;chan:"RGBA";avchan:"RGBA";guid:"fada5583-2db1-4fc8-9d77-20cf7240ec47";op:Multiply;lbl:"Color";gpu_inst:False;locked:False;impl_index:-1)];layers:list[];unlocked:list[];clones:dict[];isClone:False)];customTextures:list[ct(cimp:imp_mp_color(def:RGBA(0, 0.502, 1, 1);hdr:True;cc:4;chan:"RGBA";prop:"_HologramColor";md:"[TCP2ColorNoAlpha]";gbv:False;custom:True;refs:"Emission";pnlock:False;guid:"d98e1807-216f-481e-98ca-23550db36d24";op:Multiply;lbl:"Hologram Color";gpu_inst:False;locked:False;impl_index:-1);exp:True;uv_exp:False;imp_lbl:"Color"),ct(cimp:imp_mp_texture(uto:True;tov:"";tov_lbl:"";gto:False;sbt:True;scr:True;scv:"";scv_lbl:"";gsc:False;roff:False;goff:False;sin_anm:False;sin_anmv:"";sin_anmv_lbl:"";gsin:False;notile:False;triplanar_local:False;def:"white";locked_uv:False;uv:4;cc:4;chan:"RGBA";mip:0;mipprop:False;ssuv_vert:False;ssuv_obj:False;uv_type:ScreenSpace;uv_chan:"XZ";tpln_scale:1;uv_shaderproperty:__NULL__;uv_cmp:__NULL__;sep_sampler:__NULL__;prop:"_ScanlinesTex";md:"";gbv:False;custom:True;refs:"Emission";pnlock:False;guid:"e059c82d-333e-41a0-b42d-83d58469534e";op:Multiply;lbl:"Scanlines Texture";gpu_inst:False;locked:False;impl_index:-1);exp:True;uv_exp:False;imp_lbl:"Texture")];codeInjection:codeInjection(injectedFiles:list[];mark:False);matLayers:list[]) */
/* TCP_HASH c3f13d08843518b5c18b7e43843d5427 */
