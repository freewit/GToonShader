Shader "Gorgonize/Gorgonize Toon Shader"
{
    Properties
    {
        [Header(Base Properties)]
        _BaseColor ("Base Color", Color) = (1, 1, 1, 1)
        _BaseMap ("Base Texture", 2D) = "white" {}
        _Metallic ("Metallic", Range(0, 1)) = 0.0
        _Smoothness ("Smoothness", Range(0, 1)) = 0.5
        [Toggle(_ENABLESPECULARHIGHLIGHTS_ON)] _EnableSpecularHighlights("Enable Toon Specular", Float) = 1
        [Toggle(_ENVIRONMENTREFLECTIONS_ON)] _EnableEnvironmentReflections("Enable Environment Reflections", Float) = 1
        _ReflectionIntensity ("Environment Reflection Strength", Range(0, 1)) = 1.0

        [Header(Advanced Lighting System)]
        [KeywordEnum(Single Cell, Dual Cell, Enhanced Banded, Gradient Ramp, Custom Ramp)] _LightingMode ("Lighting Mode", Float) = 0
        
        // --- Single Cell ---
        _ShadowThreshold ("Shadow Threshold", Range(0, 1)) = 0.5
        _TransitionSoftness ("Transition Softness", Range(0.001, 1)) = 0.05
        _ShadowContrast("Shadow Contrast", Range(0, 2)) = 1.0
        _ShadowColor ("Shadow Color", Color) = (0.5, 0.5, 0.8, 1)
        [Toggle(_TINT_SHADOW_ON_BASE)] _TintShadowOnBase ("Tint On Full Object", Float) = 0

        // --- Dual Cell ---
        _PrimaryThreshold("Primary Threshold", Range(0, 1)) = 0.5
        _SecondaryThreshold("Secondary Threshold", Range(0, 1)) = 0.25
        _PrimaryShadowColor("Primary Shadow Color", Color) = (0.7, 0.7, 0.9, 1)
        _SecondaryShadowColor("Secondary Shadow Color", Color) = (0.4, 0.4, 0.6, 1)

        // --- Enhanced Banded ---
        [IntRange] _BandCount ("Band Count", Range(2, 8)) = 3
        _MidtoneThreshold("Mid-tone End", Range(0, 1)) = 0.75
        _BandSoftness("Band Softness", Range(0.001, 1)) = 0.05
        _BandDistribution("Band Distribution", Range(0.1, 2)) = 1.0

        // --- Ramps ---
        _ShadowRamp ("Gradient Ramp Texture", 2D) = "white" {} [NoScaleOffset]
        _CustomRamp ("Custom Ramp Texture", 2D) = "white" {} [NoScaleOffset]
        _RampIntensity("Ramp Intensity", Range(0, 2)) = 1.0

        // --- General ---
        _OcclusionStrength ("Occlusion Strength", Range(0, 1)) = 1
        _LightmapInfluence ("Lightmap Influence", Range(0, 1)) = 1
        [Toggle(_RECEIVESHADOWS_ON)] _ReceiveShadows ("Receive Shadows", Float) = 1

        [Header(Specular Settings)]
        [KeywordEnum(Stepped, Soft, Anisotropic, Sparkle, Double Tone, MatCap, Hair)] _SpecularMode ("Specular Mode", Float) = 0
        _SpecularColor ("Specular Color", Color) = (1, 1, 1, 1)
        _Glossiness ("Glossiness", Range(0, 1)) = 0.3
        _SpecularThreshold ("Specular Threshold", Range(0, 1)) = 0.5
        _SpecularSoftness ("Specular Softness", Range(0.001, 1)) = 0.05
        _SpecularSize ("Specular Size", Range(0, 1)) = 0.3
        _SpecularSmoothness ("Specular Smoothness", Range(0, 1)) = 0.5
        _SpecularSteps ("Specular Steps", Range(0, 1)) = 0.5
        _SteppedFalloff ("Stepped Falloff", Range(0.001, 1)) = 0.1
        _SoftSpecularGlossiness ("Soft Specular Glossiness", Range(0, 1)) = 0.3
        _SoftSpecularStrength ("Soft Specular Strength", Range(0, 2)) = 1.0
        [Toggle(_MATCAP_BLEND_ON)] _MatCapBlendWithLighting ("MatCap Blend With Lighting", Float) = 0
        _MatcapIntensity ("MatCap Intensity", Range(0, 2)) = 1.0
        _MatCapTexture ("MatCap Texture", 2D) = "white" {}
        _AnisotropicDirection ("Anisotropic Direction", Range(-1, 1)) = 0
        _AnisotropicOffset ("Anisotropic Offset", Range(-1, 1)) = 0
        _AnisotropicSharpness ("Anisotropic Sharpness", Range(0.1, 20)) = 5
        _AnisotropicIntensity ("Anisotropic Intensity", Range(0, 2)) = 1.0
        _AnisotropicFlowMap ("Anisotropic Flow Map", 2D) = "white" {}
        [Toggle(_ANISOTROPIC_FLOWMAP_ON)] _AnisotropicFlowMapToggle ("Use Flow Map", Float) = 0
        _SparkleDensity ("Sparkle Density", Range(1, 100)) = 10
        _SparkleSize ("Sparkle Size", Range(0, 1)) = 0.1
        _SparkleAnimSpeed ("Sparkle Animation Speed", Range(0, 5)) = 1
        _SparkleColor ("Sparkle Color", Color) = (1, 1, 1, 1)
        _SpecularInnerColor ("Inner Specular Color", Color) = (1, 1, 1, 1)
        _SpecularOuterColor ("Outer Specular Color", Color) = (0.5, 0.5, 0.5, 1)
        _SpecularInnerSize ("Inner Specular Size", Range(0, 1)) = 0.3
        _SpecularOuterSize ("Outer Specular Size", Range(0, 1)) = 0.6
        _SpecularDoubleToneSoftness ("Double Tone Softness", Range(0.001, 1)) = 0.1
        _HairPrimaryColor ("Hair Primary Color", Color) = (1, 1, 1, 1)
        _HairSecondaryColor ("Hair Secondary Color", Color) = (0.8, 0.8, 0.8, 1)
        _HairPrimaryShift ("Hair Primary Shift", Range(-1, 1)) = 0.1
        _HairSecondaryShift ("Hair Secondary Shift", Range(-1, 1)) = 0.3
        _HairPrimaryExponent ("Hair Primary Exponent", Range(1, 50)) = 20
        _HairSecondaryExponent ("Hair Secondary Exponent", Range(1, 50)) = 10
        _SoftSpecularMask ("Soft Specular Mask", 2D) = "white" {}
        [Toggle(_SOFT_SPECULAR_MASK_ON)] _SoftSpecularMaskToggle ("Use Soft Specular Mask", Float) = 0

        [Header(Rim Lighting)]
        [Toggle(_ENABLERIM_ON)] _EnableRim ("Enable Rim Lighting", Float) = 0
        [KeywordEnum(Standard, Stepped, Light Based, Textured, Fresnel Enhanced, Color Gradient)] _RimMode ("Rim Mode", Float) = 0
        _RimColor ("Rim Color", Color) = (1, 1, 1, 1)
        _RimThreshold ("Rim Threshold", Range(0, 1)) = 0.5
        _RimSoftness ("Rim Softness", Range(0.001, 1)) = 0.1
        _RimPower ("Rim Power", Range(0.1, 10)) = 2
        _RimIntensity ("Rim Intensity", Range(0, 5)) = 1
        _RimOffset ("Rim Offset", Range(-1, 1)) = 0
        _RimLightInfluence ("Rim Light Influence", Range(0, 1)) = 1
        _RimScrollSpeed ("Rim Scroll Speed", Range(0, 5)) = 1
        _FresnelPower ("Fresnel Power", Range(0.1, 10)) = 5
        _FresnelBias ("Fresnel Bias", Range(0, 1)) = 0
        _RimColorTop ("Rim Color Top", Color) = (1, 1, 1, 1)
        _RimColorBottom ("Rim Color Bottom", Color) = (0, 0, 1, 1)
        _RimGradientPower ("Rim Gradient Power", Range(0.1, 10)) = 1
        _RimGradient ("Rim Gradient", 2D) = "white" {}

        [Header(Subsurface Scattering)]
        [Toggle(_ENABLESUBSURFACE_ON)] _EnableSubsurface ("Enable Subsurface", Float) = 0
        [KeywordEnum(Basic, Advanced)] _SubsurfaceMode ("Subsurface Mode", Float) = 0
        _SubsurfaceColor ("Subsurface Color", Color) = (1, 0.4, 0.25, 1)
        _SubsurfaceIntensity ("Subsurface Intensity", Range(0, 2)) = 1
        _SubsurfacePower ("Subsurface Power", Range(0.1, 10)) = 3
        _SubsurfaceDistortion ("Subsurface Distortion", Range(0, 2)) = 0.2

        [Header(Normal Map)]
        [Toggle(_NORMALMAP)] _EnableNormalMap ("Enable Normal Map", Float) = 0
        _BumpMap ("Normal Map", 2D) = "bump" {}
        _BumpScale ("Normal Scale", Float) = 1

        [Header(Detail Maps)]
        [Toggle(_DETAIL)] _Detail ("Enable Detail", Float) = 0
        _DetailMap ("Detail Albedo x2", 2D) = "linearGrey" {}
        _DetailAlbedoMap ("Detail Albedo", 2D) = "linearGrey" {}
        _DetailNormalMap ("Detail Normal Map", 2D) = "bump" {}
        _DetailNormalScale ("Detail Normal Scale", Float) = 1
        _DetailMask ("Detail Mask", 2D) = "white" {}

        [Header(Height Mapping)]
        [Toggle(_ENABLEPARALLAX_ON)] _EnableParallax ("Enable Parallax", Float) = 0
        [Toggle(_HEIGHTMAP)] _HeightMap ("Height Map", 2D) = "black" {}
        _HeightScale ("Height Scale", Range(0.005, 0.08)) = 0.02

        [Header(Emission)]
        [Toggle(_EMISSION)] _Emission ("Enable Emission", Float) = 0
        _EmissionMap ("Emission", 2D) = "white" {}
        [HDR] _EmissionColor ("Emission Color", Color) = (0,0,0)
        [Toggle(_ENABLEEMISSIONPULSE_ON)] _EnableEmissionPulse ("Enable Emission Pulse", Float) = 0
        _EmissionPulseSpeed ("Emission Pulse Speed", Range(0.1, 10)) = 1
        _EmissionPulseAmount ("Emission Pulse Amount", Range(0, 1)) = 0.5

        [Header(Wind)]
        [Toggle(_ENABLEWIND_ON)] _EnableWind ("Enable Wind", Float) = 0
        [KeywordEnum(Basic, Advanced)] _WindMode ("Wind Mode", Float) = 0
        _WindDirection ("Wind Direction", Vector) = (1, 0, 0, 0)
        _WindSpeed ("Wind Speed", Range(0, 10)) = 1
        _WindStrength ("Wind Strength", Range(0, 1)) = 0.1
        _WindGustiness ("Wind Gustiness", Range(0, 1)) = 0.5
        [Toggle(_WIND_VERTEX_COLOR_MASK_ON)] _WindVertexColorMask("Vertex Color Mask (Alpha)", Float) = 0

        [Header(Outline)]
        [Toggle(_ENABLEOUTLINE_ON)] _EnableOutline ("Enable Outline", Float) = 0
        [KeywordEnum(Normal, Standard, Traditional)] _OutlineMode ("Outline Mode", Float) = 0
        _OutlineColor ("Outline Color", Color) = (0, 0, 0, 1)
        _OutlineWidth ("Outline Width", Range(0, 0.1)) = 0.01
        _OutlineZOffset ("Outline Z Offset", Range(0, 1)) = 0
        [Toggle(_OUTLINEANTIALIASING_ON)] _OutlineAntialiasing ("Outline Antialiasing", Float) = 0
        [Toggle(_OUTLINELOD_ON)] _OutlineLOD ("Outline LOD", Float) = 0

        [Header(Performance)]
        [Toggle(_ADDITIONAL_LIGHTS_ON)] _EnableAdditionalLights ("Additional Lights", Float) = 1
        _IndirectLightingMultiplier ("Indirect Lighting Multiplier", Range(0, 2)) = 1

        // Blending state
        [HideInInspector] _Surface("__surface", Float) = 0.0
        [HideInInspector] _Blend("__blend", Float) = 0.0
        [HideInInspector] _AlphaClip("__clip", Float) = 0.0
        [HideInInspector] _SrcBlend("__src", Float) = 1.0
        [HideInInspector] _DstBlend("__dst", Float) = 0.0
        [HideInInspector] _ZWrite("__zw", Float) = 1.0
        [HideInInspector] _Cull("__cull", Float) = 2.0
        [HideInInspector] _QueueOffset("Queue offset", Float) = 0.0
        [HideInInspector] _Cutoff("Alpha Cutoff", Range(0.0, 1.0)) = 0.5
    }

    SubShader
    {
        Tags 
        { 
            "RenderType" = "Opaque" 
            "RenderPipeline" = "UniversalPipeline"
            "Queue" = "Geometry"
        }
        
        // OUTLINE PASS
        Pass
        {
            Name "Outline"
            Tags { "LightMode" = "SRPDefaultUnlit" }
            
            Cull Front
            ZWrite On
            ZTest LEqual
            
            HLSLPROGRAM
            #pragma target 4.5
            #pragma vertex OutlineVertex
            #pragma fragment OutlineFragment
            
            #pragma shader_feature_local _ENABLEOUTLINE_ON
            #pragma shader_feature_local _OUTLINEMODE_NORMAL _OUTLINEMODE_STANDARD _OUTLINEMODE_TRADITIONAL
            #pragma shader_feature_local _OUTLINEANTIALIASING_ON
            #pragma shader_feature_local _OUTLINELOD_ON
            #pragma shader_feature_local _ENABLEWIND_ON
            #pragma shader_feature_local _WINDMODE_BASIC _WINDMODE_ADVANCED
            #pragma shader_feature_local _WIND_VERTEX_COLOR_MASK_ON

            #pragma multi_compile_instancing
            #pragma multi_compile_fog

            #include "Includes/GToonOutlinePass.hlsl"
            
            ENDHLSL
        }
        
        // MAIN FORWARD PASS
        Pass
        {
            Name "ToonForward"
            Tags { "LightMode" = "UniversalForward" }
            
            Cull Back
            ZWrite On
            ZTest LEqual
            
            HLSLPROGRAM
            #pragma target 4.5
            #pragma vertex vert
            #pragma fragment frag
            
            // Feature keywords - PRESERVE ALL ORIGINAL FEATURES
            #pragma shader_feature_local _ENABLESPECULARHIGHLIGHTS_ON
            #pragma shader_feature_local _ENVIRONMENTREFLECTIONS_ON
            #pragma shader_feature_local_fragment _SPECULARMODE_STEPPED _SPECULARMODE_SOFT _SPECULARMODE_ANISOTROPIC _SPECULARMODE_SPARKLE _SPECULARMODE_DOUBLE_TONE _SPECULARMODE_MATCAP _SPECULARMODE_HAIR
            #pragma shader_feature_local _MATCAP_BLEND_ON
            #pragma shader_feature_local _ANISOTROPIC_FLOWMAP_ON
            #pragma shader_feature_local _SOFT_SPECULAR_MASK_ON
            #pragma shader_feature_local _ENABLERIM_ON
            #pragma shader_feature_local_fragment _RIMMODE_STANDARD _RIMMODE_STEPPED _RIMMODE_LIGHTBASED _RIMMODE_TEXTURED _RIMMODE_FRESNEL_ENHANCED _RIMMODE_COLOR_GRADIENT
            #pragma shader_feature_local _ENABLESUBSURFACE_ON
            #pragma shader_feature_local_fragment _SUBSURFACE_BASIC _SUBSURFACE_ADVANCED
            #pragma shader_feature_local_fragment _LIGHTINGMODE_SINGLE_CELL _LIGHTINGMODE_DUAL_CELL _LIGHTINGMODE_ENHANCED_BANDED _LIGHTINGMODE_GRADIENT_RAMP _LIGHTINGMODE_CUSTOM_RAMP
            #pragma shader_feature_local_fragment _TINT_SHADOW_ON_BASE
            #pragma shader_feature_local _ENABLEWIND_ON
            #pragma shader_feature_local _WINDMODE_BASIC _WINDMODE_ADVANCED
            #pragma shader_feature_local _WIND_VERTEX_COLOR_MASK_ON
            #pragma shader_feature_local _RECEIVESHADOWS_ON
            #pragma shader_feature_local _ADDITIONAL_LIGHTS_ON
            #pragma shader_feature_local _NORMALMAP
            #pragma shader_feature_local _ENABLEPARALLAX_ON
            #pragma shader_feature_local _HEIGHTMAP
            #pragma shader_feature_local _EMISSION
            #pragma shader_feature_local _ENABLEEMISSIONPULSE_ON
            #pragma shader_feature_local _DETAIL
            
            // **CRITICAL: URP 17.0.4 compatible keywords for additional lights**
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN
            #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
            #pragma multi_compile_fragment _ _ADDITIONAL_LIGHT_SHADOWS
            #pragma multi_compile_fragment _ _SHADOWS_SOFT
            #pragma multi_compile _ LIGHTMAP_SHADOW_MIXING
            #pragma multi_compile _ SHADOWS_SHADOWMASK
            #pragma multi_compile _ DIRLIGHTMAP_COMBINED
            #pragma multi_compile _ LIGHTMAP_ON
            #pragma multi_compile _ DYNAMICLIGHTMAP_ON
            #pragma multi_compile_fragment _ _LIGHT_LAYERS
            #pragma multi_compile_fragment _ _LIGHT_COOKIES
            #pragma multi_compile _ _FORWARD_PLUS
            #pragma multi_compile_fog
            #pragma multi_compile_instancing
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Includes/GToonInput.hlsl"
            #include "Includes/GToonForwardPass.hlsl"

            ENDHLSL
        }

        // Shadow rendering pass
        Pass
        {
            Name "ShadowCaster"
            Tags{"LightMode" = "ShadowCaster"}

            ZWrite On
            ZTest LEqual
            ColorMask 0
            Cull Back

            HLSLPROGRAM
            #pragma target 4.5
            #pragma vertex ShadowPassVertex
            #pragma fragment ShadowPassFragment

            #pragma shader_feature_local_fragment _ALPHATEST_ON
            #pragma shader_feature_local_fragment _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A

            #pragma multi_compile_instancing
            #pragma multi_compile _ DOTS_INSTANCING_ON
            #pragma multi_compile_vertex _ _CASTING_PUNCTUAL_LIGHT_SHADOW

            #include "Packages/com.unity.render-pipelines.universal/Shaders/LitInput.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/ShadowCasterPass.hlsl"
            ENDHLSL
        }

        // Depth pass
        Pass
        {
            Name "DepthOnly"
            Tags{"LightMode" = "DepthOnly"}

            ZWrite On
            ColorMask 0
            Cull Back

            HLSLPROGRAM
            #pragma target 4.5
            #pragma vertex DepthOnlyVertex
            #pragma fragment DepthOnlyFragment

            #pragma shader_feature_local_fragment _ALPHATEST_ON
            #pragma shader_feature_local_fragment _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A

            #pragma multi_compile_instancing
            #pragma multi_compile _ DOTS_INSTANCING_ON

            #include "Packages/com.unity.render-pipelines.universal/Shaders/LitInput.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/DepthOnlyPass.hlsl"
            ENDHLSL
        }

        // Depth normals pass
        Pass
        {
            Name "DepthNormals"
            Tags{"LightMode" = "DepthNormals"}

            ZWrite On
            Cull Back

            HLSLPROGRAM
            #pragma target 4.5
            #pragma vertex DepthNormalsVertex
            #pragma fragment DepthNormalsFragment

            #pragma shader_feature_local _NORMALMAP
            #pragma shader_feature_local_fragment _ALPHATEST_ON
            #pragma shader_feature_local_fragment _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A

            #pragma multi_compile_instancing
            #pragma multi_compile _ DOTS_INSTANCING_ON

            #include "Packages/com.unity.render-pipelines.universal/Shaders/LitInput.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/LitDepthNormalsPass.hlsl"
            ENDHLSL
        }

        // Meta pass for lightmap baking
        Pass
        {
            Name "Meta"
            Tags{"LightMode" = "Meta"}

            Cull Off

            HLSLPROGRAM
            #pragma target 4.5
            #pragma vertex MetaPassVertex
            #pragma fragment MetaPassFragment

            #pragma shader_feature_local_fragment _SPECULAR_SETUP
            #pragma shader_feature_local_fragment _EMISSION
            #pragma shader_feature_local_fragment _METALLICSPECGLOSSMAP
            #pragma shader_feature_local_fragment _ALPHATEST_ON
            #pragma shader_feature_local_fragment _ _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
            #pragma shader_feature_local _ _DETAIL_MULX2 _DETAIL_SCALED
            #pragma shader_feature_local_fragment _SPECGLOSSMAP

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/MetaInput.hlsl"

            // Use the GToonInput for consistent properties
            #include "Includes/GToonInput.hlsl"

            struct Attributes_Meta
            {
                float4 positionOS : POSITION;
                float2 uv0 : TEXCOORD0;
                float2 uv1 : TEXCOORD1;
                float2 uv2 : TEXCOORD2;
            };

            struct Varyings_Meta
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            Varyings_Meta MetaPassVertex(Attributes_Meta input)
            {
                Varyings_Meta output;
                output.positionCS = MetaVertexPosition(input.positionOS, input.uv1, input.uv2, unity_LightmapST, unity_DynamicLightmapST);
                output.uv = TRANSFORM_TEX(input.uv0, _BaseMap);
                return output;
            }

            half4 MetaPassFragment(Varyings_Meta input) : SV_Target
            {
                MetaInput metaInput = (MetaInput)0;
                
                half4 albedo = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, input.uv) * _BaseColor;
                metaInput.Albedo = albedo.rgb;
                
                #ifdef _EMISSION
                    half3 emission = SAMPLE_TEXTURE2D(_EmissionMap, sampler_EmissionMap, input.uv).rgb * _EmissionColor.rgb;
                    metaInput.Emission = emission;
                #else
                    metaInput.Emission = 0;
                #endif

                return MetaFragment(metaInput);
            }

            ENDHLSL
        }

        // Universal2D pass
        Pass
        {
            Name "Universal2D"
            Tags{ "LightMode" = "Universal2D" }

            Blend[_SrcBlend][_DstBlend]
            ZWrite[_ZWrite]
            Cull[_Cull]

            HLSLPROGRAM
            #pragma target 4.5
            #pragma vertex vert
            #pragma fragment frag
            #pragma shader_feature_local_fragment _ALPHATEST_ON
            #pragma shader_feature_local_fragment _ALPHAPREMULTIPLY_ON

            #include "Packages/com.unity.render-pipelines.universal/Shaders/LitInput.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/Utils/Universal2D.hlsl"
            ENDHLSL
        }
    }

    Fallback "Hidden/Universal Render Pipeline/FallbackError"
    CustomEditor "Gorgonize.ToonShader.Editor.GorgonizeToonShaderGUI"
}