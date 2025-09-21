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
        _EnvironmentReflections ("Environment Reflection Strength", Range(0, 1)) = 1.0

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

        
        [Header(Advanced Specular System)]
        [KeywordEnum(Stepped, Soft, Enhanced Anisotropic, Animated Sparkle, Double Tone, Matcap, Hair Fur)] _SpecularMode ("Specular Mode", Float) = 0

        // --- Stepped ---
        _SpecularColor ("Specular Color", Color) = (1, 1, 1, 1)
        _SpecularSize ("Specular Size", Range(0, 1)) = 0.1
        _SpecularSmoothness ("Specular Smoothness", Range(0, 1)) = 0.5
        [IntRange] _SpecularSteps ("Specular Steps", Range(1, 8)) = 2
        _SteppedFalloff("Falloff", Range(1, 10)) = 2.0

        // --- Soft ---
        _SoftSpecularGlossiness("Glossiness", Range(0, 1)) = 0.5
        _SoftSpecularStrength("Strength", Range(0, 2)) = 1.0
        _SoftSpecularMask("Specular Mask", 2D) = "white" {}

        // --- Enhanced Anisotropic ---
        _AnisotropicDirection("Direction", Range(-1, 1)) = 0.5
        _AnisotropicSharpness("Sharpness", Range(0, 1)) = 0.5
        _AnisotropicIntensity("Intensity", Range(0, 2)) = 1.0
        _AnisotropicOffset("Offset", Range(-1, 1)) = 0.0
        _AnisotropicFlowMap("Flow Map", 2D) = "black" {}

        // --- Animated Sparkle ---
        _SparkleMap("Sparkle Pattern", 2D) = "white" {}
        _SparkleDensity("Density", Range(1, 20)) = 5.0
        _SparkleSize("Size", Range(0.1, 2.0)) = 1.0
        _SparkleAnimSpeed("Animation Speed", Range(0, 5)) = 1.0
        _SparkleColor("Sparkle Color", Color) = (1,1,1,1)

        // --- Double Tone ---
        _SpecularInnerColor("Inner Color", Color) = (1,1,1,1)
        _SpecularOuterColor("Outer Color", Color) = (0.8, 0.8, 0.8, 1)
        _SpecularInnerSize("Inner Size", Range(0, 1)) = 0.1
        _SpecularOuterSize("Outer Size", Range(0, 1)) = 0.3
        _SpecularDoubleToneSoftness("Softness", Range(0.001, 0.5)) = 0.05
        
        // --- Matcap ---
        _MatcapTex("Matcap Texture", 2D) = "gray" {}
        _MatcapIntensity("Intensity", Range(0, 5)) = 1.0
        [Toggle(_MATCAP_BLEND_ON)] _MatcapBlendWithLighting("Blend with Lighting", Float) = 1

        // --- Hair/Fur ---
        _HairPrimaryColor("Primary Hair Color", Color) = (1, 1, 1, 1)
        _HairPrimaryExponent("Primary Exponent", Range(1, 200)) = 80
        _HairPrimaryShift("Primary Shift", Range(-1, 1)) = 0.1
        _HairSecondaryColor("Secondary Hair Color", Color) = (1, 0.8, 0.5, 1)
        _HairSecondaryExponent("Secondary Exponent", Range(1, 200)) = 150
        _HairSecondaryShift("Secondary Shift", Range(-1, 1)) = -0.1

        [Header(Advanced Rim Lighting System)]
        [Toggle(_ENABLERIM_ON)] _EnableRim ("Enable Rim Lighting", Float) = 1
        [KeywordEnum(Standard, Stepped, LightBased, Textured, Fresnel Enhanced, Color Gradient)] _RimMode("Rim Mode", Float) = 0
        
        // Shared
        _RimColor ("Rim Color", Color) = (1, 1, 1, 1)
        _RimIntensity ("Rim Intensity", Range(0, 3)) = 1

        // Standard
        _RimPower ("Rim Power", Range(0, 10)) = 2
        _RimOffset ("Rim Offset", Range(-1, 1)) = 0

        // Stepped
        _RimThreshold("Threshold", Range(0, 1)) = 0.5
        _RimSoftness("Softness", Range(0.001, 1)) = 0.1

        // Light Based
        _RimLightInfluence("Light Influence", Range(0, 5)) = 1.0

        // Textured
        _RimTexture("Rim Texture", 2D) = "white" {}
        _RimScrollSpeed("Scroll Speed", Range(-2, 2)) = 0.5

        // Fresnel Enhanced
        _FresnelPower("Fresnel Power", Range(0.1, 20)) = 5.0
        _FresnelBias("Fresnel Bias", Range(-1, 1)) = 0.0
        
        // Color Gradient
        _RimColorTop("Top Color", Color) = (1, 0, 0, 1)
        _RimColorBottom("Bottom Color", Color) = (0, 0, 1, 1)
        _RimGradientPower("Gradient Power", Range(0.1, 10)) = 1.0
        
        [Header(Advanced Surface Details)]
        _NormalMap ("Normal Map", 2D) = "bump" {}
        _NormalStrength ("Normal Strength", Range(0, 3)) = 1
        [Toggle(_ENABLEPARALLAX_ON)] _EnableParallax("Enable Parallax Mapping", Float) = 0
        _HeightMap("Height Map", 2D) = "gray" {}
        _HeightScale("Height Scale", Range(0, 0.1)) = 0.02
        
        [Header(Emission)]
        _EmissionMap ("Emission", 2D) = "black" {}
        [HDR] _EmissionColor ("Emission Color", Color) = (0, 0, 0, 1)
        _EmissionIntensity ("Emission Intensity", Range(0, 10)) = 0
        [Toggle(_ENABLEEMISSIONPULSE_ON)] _EnableEmissionPulse("Enable Pulse Animation", Float) = 0
        _PulseSpeed("Pulse Speed", Range(0, 20)) = 5.0
        
        [Header(Detail Textures)]
        _DetailMap ("Detail Albedo", 2D) = "gray" {}
        _DetailNormalMap ("Detail Normal", 2D) = "bump" {}
        _DetailNormalScale("Detail Normal Scale", Range(0, 2)) = 1
        _DetailStrength ("Detail Strength", Range(0, 2)) = 1
        
        [Header(Subsurface Scattering)]
        [Toggle(_ENABLESUBSURFACE_ON)] _EnableSubsurface ("Enable Subsurface", Float) = 0
        [KeywordEnum(Basic, Advanced)] _SubsurfaceMode("Scattering Mode", Float) = 0
        _SubsurfaceColor ("Subsurface Color", Color) = (1, 0.4, 0.25, 1)
        _SubsurfaceIntensity ("Subsurface Intensity", Range(0, 2)) = 1.0
        _SubsurfaceDistortion ("Subsurface Distortion", Range(0, 2)) = 1.0
        _SubsurfacePower ("Subsurface Power", Range(0.1, 10)) = 1.0
        _SubsurfaceMap("Subsurface Mask", 2D) = "white" {}
        _ThicknessMap("Thickness Map", 2D) = "white" {}
        
        [Header(Smart Outline System)]
        [Toggle(_ENABLEOUTLINE_ON)] _EnableOutline ("Enable Outline", Float) = 0
        _OutlineColor ("Outline Color A", Color) = (0, 0, 0, 1)
        _OutlineWidth ("Base Thickness", Range(0, 10)) = 1
        [KeywordEnum(Normal, Position, UV)] _OutlineMode("Extrusion Mode", Float) = 0

        [Toggle(_OUTLINE_DISTANCE_SCALING_ON)] _OutlineDistanceScaling("Distance Scaling", Float) = 0
        _OutlineAdaptiveMinWidth("Min Thickness", Range(0, 10)) = 0.5
        _OutlineAdaptiveMaxWidth("Max Thickness", Range(0, 10)) = 2.0

        [Toggle(_OUTLINE_ANIMATED_COLOR_ON)] _OutlineAnimatedColor("Animate Color", Float) = 0
        _OutlineColorB("Outline Color B", Color) = (1, 1, 1, 1)
        _OutlineAnimationSpeed("Animation Speed", Range(0, 10)) = 1.0
        
        [Header(Advanced Wind System)]
        [Toggle(_ENABLEWIND_ON)] _EnableWind ("Enable Wind", Float) = 0
        [KeywordEnum(Basic, Advanced)] _WindMode("Wind Simulation Mode", Float) = 0
        
        // --- Basic Wind ---
        _WindSpeed ("Wind Speed", Range(0, 5)) = 1
        _WindStrength ("Wind Strength", Range(0, 1)) = 0.1
        _WindDirection ("Wind Direction", Vector) = (1, 0, 1, 0)
        
        // --- Advanced Wind ---
        _WindTurbulence("Turbulence", Range(0, 5)) = 1
        _WindNoiseScale("Noise Scale", Range(0.1, 10)) = 1
        _WindPhaseVariation("Phase Variation", Range(0, 1)) = 0.1
        _BranchBending("Branch Bending", Range(0, 2)) = 0.5
        [Toggle(_WIND_VERTEX_COLOR_MASK_ON)] _WindVertexColorMask("Vertex Color Mask (Alpha)", Float) = 0

        [Header(Performance)]
        [Toggle(_ADDITIONAL_LIGHTS_ON)] _EnableAdditionalLights ("Additional Lights", Float) = 1
    }

    SubShader
    {
        Tags 
        { 
            "RenderType" = "Opaque" 
            "RenderPipeline" = "UniversalPipeline"
            "Queue" = "Geometry"
        }
        
        // OUTLINE PASS - İlk olarak render edilir
        Pass
        {
            Name "Outline"
            Tags { "LightMode" = "SRPDefaultUnlit" }
            
            Cull Front
            ZWrite On
            ZTest LEqual
            
            HLSLPROGRAM
            #pragma target 4.5
            #pragma vertex OutlineVert
            #pragma fragment OutlineFrag
            
            #pragma shader_feature_local _ENABLEOUTLINE_ON
            #pragma shader_feature_local _OUTLINEMODE_NORMAL _OUTLINEMODE_POSITION _OUTLINEMODE_UV
            #pragma shader_feature_local _OUTLINE_DISTANCE_SCALING_ON
            #pragma shader_feature_local _OUTLINE_ANIMATED_COLOR_ON
            #pragma shader_feature_local _ENABLEWIND_ON
            #pragma shader_feature_local _WINDMODE_BASIC _WINDMODE_ADVANCED
            #pragma shader_feature_local _WIND_VERTEX_COLOR_MASK_ON

            #include "Includes/GToonOutline.hlsl"
            
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
            
            // Feature keywords
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
            #pragma shader_feature_local_fragment _LIGHTINGMODE_SINGLE_CELL _LIGHTINGMODE_DUAL_CELL _LIGHTINGMODE_BANDED _LIGHTINGMODE_GRADIENT_RAMP _LIGHTINGMODE_CUSTOM_RAMP
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
            
            // URP keywords
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN
            #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
            #pragma multi_compile_fragment _ _ADDITIONAL_LIGHT_SHADOWS
            #pragma multi_compile_fragment _ _SHADOWS_SOFT
            #pragma multi_compile _ LIGHTMAP_SHADOW_MIXING
            #pragma multi_compile _ SHADOWS_SHADOWMASK
            #pragma multi_compile _ DIRLIGHTMAP_COMBINED
            #pragma multi_compile _ LIGHTMAP_ON
            #pragma multi_compile _ DYNAMICLIGHTMAP_ON
            #pragma multi_compile_fog
            #pragma multi_compile_instancing
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Includes/GToonForwardPass.hlsl"
            
            ENDHLSL
        }
        
        // Shadow Caster Pass
        Pass
        {
            Name "ShadowCaster"
            Tags{"LightMode" = "ShadowCaster"}

            ZWrite On
            ZTest LEqual
            ColorMask 0
            Cull[_Cull]

            HLSLPROGRAM
            #pragma target 4.5
            #pragma multi_compile_instancing
            #pragma multi_compile _ DOTS_INSTANCING_ON
            #pragma vertex ShadowPassVertex
            #pragma fragment ShadowPassFragment
            #include "Packages/com.unity.render-pipelines.universal/Shaders/LitInput.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/ShadowCasterPass.hlsl"
            ENDHLSL
        }

        // Depth Only Pass
        Pass
        {
            Name "DepthOnly"
            Tags{"LightMode" = "DepthOnly"}

            ZWrite On
            ColorMask 0
            Cull[_Cull]

            HLSLPROGRAM
            #pragma target 4.5
            #pragma multi_compile_instancing
            #pragma multi_compile _ DOTS_INSTANCING_ON
            #pragma vertex DepthOnlyVertex
            #pragma fragment DepthOnlyFragment
            #include "Packages/com.unity.render-pipelines.universal/Shaders/LitInput.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/DepthOnlyPass.hlsl"
            ENDHLSL
        }

        // Depth Normals Pass
        Pass
        {
            Name "DepthNormals"
            Tags{"LightMode" = "DepthNormals"}

            ZWrite On
            Cull[_Cull]

            HLSLPROGRAM
            #pragma target 4.5
            #pragma shader_feature_local _NORMALMAP
            #pragma multi_compile_instancing
            #pragma multi_compile _ DOTS_INSTANCING_ON
            #pragma vertex DepthNormalsVertex
            #pragma fragment DepthNormalsFragment
            #include "Packages/com.unity.render-pipelines.universal/Shaders/LitInput.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/LitDepthNormalsPass.hlsl"
            ENDHLSL
        }
    }

    CustomEditor "Gorgonize.ToonShader.Editor.GorgonizeToonShaderGUI"
    FallBack "Hidden/Universal Render Pipeline/FallbackError"
}

