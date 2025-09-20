Shader "Gorgonize/Gorgonize Toon Shader"
{
    Properties
    {
        [Header(Base Properties)]
        _BaseColor ("Base Color", Color) = (1, 1, 1, 1)
        _BaseMap ("Base Texture", 2D) = "white" {}
        _Metallic("Metallic", Range(0.0, 1.0)) = 0.0
        _Smoothness("Smoothness", Range(0.0, 1.0)) = 0.5
        [Toggle(_ENABLESPECULARHIGHLIGHTS_ON)] _EnableSpecularHighlights ("Enable Toon Specular", Float) = 1
        [Toggle(_ENVIRONMENTREFLECTIONS_ON)] _EnableEnvironmentReflections ("Enable Environment Reflections", Float) = 0
        _EnvironmentReflections ("Environment Reflections", Range(0, 1)) = 1.0
        
        [Header(Advanced Lighting System)]
        [KeywordEnum(Single Cell, Dual Cell, Banded, Gradient Ramp, Custom Ramp)] _LightingMode ("Lighting Mode", Float) = 0
        
        [Header(Single Cell Mode)]
        _ShadowThreshold ("Shadow Threshold", Range(0, 1)) = 0.5
        _TransitionSoftness ("Transition Softness", Range(0.001, 1)) = 0.05
        _ShadowContrast("Shadow Contrast", Range(0, 2)) = 1.0
        _ShadowColor ("Shadow Color", Color) = (0.5, 0.5, 0.8, 1)
        [Toggle(_TINT_SHADOW_ON_BASE)] _TintShadowOnBase ("Tint On Full Object", Float) = 0

        [Header(Dual Cell Mode)]
        _PrimaryThreshold("Primary Threshold", Range(0,1)) = 0.6
        _SecondaryThreshold("Secondary Threshold", Range(0,1)) = 0.3
        _PrimaryShadowColor("Primary Shadow Color", Color) = (0.4, 0.4, 0.7, 1)
        _SecondaryShadowColor("Secondary Shadow Color", Color) = (0.2, 0.2, 0.5, 1)

        [Header(Enhanced Banded Mode)]
        [IntRange] _BandCount ("Band Count", Range(2, 8)) = 3
        _MidtoneThreshold("Mid-tone Threshold", Range(0, 1)) = 0.75
        _BandSoftness("Band Softness", Range(0.001, 1)) = 0.05
        _BandDistribution("Band Distribution", Range(0.1, 5)) = 1.0

        [Header(Ramp Modes)]
        _ShadowRamp ("Gradient Ramp Texture", 2D) = "white" {} [NoScaleOffset]
        _CustomRamp("Custom Ramp Texture", 2D) = "white" {} [NoScaleOffset]
        _RampIntensity("Ramp Intensity", Range(0, 2)) = 1.0
        
        [Header(General Lighting)]
        _OcclusionStrength ("Occlusion Strength", Range(0, 1)) = 1
        
        [Header(Advanced Specular System)]
        [KeywordEnum(Stepped, Soft, Anisotropic, Sparkle, Double Tone, Matcap, Hair Fur)] _SpecularMode ("Specular Mode", Float) = 0

        [Header(Stepped Mode)]
        _SpecularColor ("Specular Color", Color) = (1, 1, 1, 1)
        _SpecularSize ("Specular Size", Range(0, 1)) = 0.1
        _SpecularSmoothness ("Specular Smoothness", Range(0.001, 1)) = 0.5
        [IntRange] _SpecularSteps ("Specular Steps", Range(1, 8)) = 2
        _SteppedFalloff("Falloff", Range(0.1, 10)) = 2.0

        [Header(Soft Mode)]
        _SoftSpecularGlossiness("Glossiness", Range(0, 1)) = 0.5
        _SoftSpecularStrength("Strength", Range(0, 2)) = 1.0
        [NoScaleOffset] _SoftSpecularMask("Specular Mask", 2D) = "white" {}

        [Header(Enhanced Anisotropic Mode)]
        _AnisotropicDirection("Direction", Range(-1, 1)) = 0.5
        _AnisotropicSharpness("Sharpness", Range(0, 1)) = 0.5
        _AnisotropicIntensity("Intensity", Range(0, 2)) = 1.0
        _AnisotropicOffset("Offset", Range(-1, 1)) = 0.0
        [NoScaleOffset] _AnisotropicFlowMap("Flow Map", 2D) = "black" {}

        [Header(Animated Sparkle Mode)]
        _SparkleMap("Sparkle Pattern", 2D) = "white" {}
        _SparkleDensity("Density", Range(1, 20)) = 5.0
        _SparkleColor("Sparkle Color", Color) = (1,1,1,1)
        _SparkleSize("Size", Range(0.1, 2.0)) = 1.0
        _SparkleAnimSpeed("Animation Speed", Range(0, 10)) = 1.0

        [Header(Double Tone Mode)]
        _SpecularInnerColor("Inner Color", Color) = (1,1,1,1)
        _SpecularOuterColor("Outer Color", Color) = (0.8, 0.8, 0.8, 1)
        _SpecularInnerSize("Inner Size", Range(0, 1)) = 0.1
        _SpecularOuterSize("Outer Size", Range(0, 1)) = 0.3
        _SpecularDoubleToneSoftness("Softness", Range(0.001, 0.5)) = 0.05
        
        [Header(Matcap Mode)]
        [NoScaleOffset] _MatcapTex("Matcap Texture", 2D) = "gray" {}
        _MatcapIntensity("Intensity", Range(0, 5)) = 1.0
        [Toggle(_MATCAP_BLEND_WITH_LIGHTING)] _MatcapBlendWithLighting("Blend with Lighting", Float) = 1.0

        [Header(Hair Fur Mode)]
        _HairPrimaryColor("Primary Hair Color", Color) = (1,1,1,1)
        _HairSecondaryColor("Secondary Hair Color", Color) = (0.8, 0.8, 0.8, 1)
        _HairPrimaryShift("Primary Shift", Range(-1, 1)) = 0.0
        _HairSecondaryShift("Secondary Shift", Range(-1, 1)) = 0.1
        _HairPrimaryExponent("Primary Exponent", Range(1, 200)) = 80
        _HairSecondaryExponent("Secondary Exponent", Range(1, 200)) = 120

        [Header(Rim Lighting System)]
        [Toggle(_ENABLERIM_ON)] _EnableRim ("Enable Rim Lighting", Float) = 0
        [KeywordEnum(Standard, Stepped, LightBased, Textured)] _RimMode("Rim Mode", Float) = 0
        _RimColor ("Rim Color", Color) = (1, 1, 1, 1)
        _RimIntensity ("Rim Intensity", Range(0, 3)) = 1

        [Header(Standard Rim Mode)]
        _RimPower ("Rim Power", Range(0, 10)) = 2
        _RimOffset ("Rim Offset", Range(-1, 1)) = 0

        [Header(Stepped Rim Mode)]
        _RimThreshold("Threshold", Range(0, 1)) = 0.5
        _RimSoftness("Softness", Range(0.001, 1)) = 0.1

        [Header(Light Based Rim Mode)]
        _RimLightInfluence("Light Influence", Range(0, 5)) = 1.0

        [Header(Textured Rim Mode)]
        _RimTexture("Rim Texture", 2D) = "white" {}
        _RimScrollSpeed("Scroll Speed", Range(-2, 2)) = 0.5
        
        [Header(Advanced Features)]
        _NormalMap ("Normal Map", 2D) = "bump" {}
        _NormalStrength ("Normal Strength", Range(0, 2)) = 1
        _EmissionMap ("Emission", 2D) = "black" {}
        [HDR] _EmissionColor ("Emission Color", Color) = (0, 0, 0, 1)
        _EmissionIntensity ("Emission Intensity", Range(0, 10)) = 0
        
        [Header(Detail Textures)]
        _DetailMap ("Detail Albedo", 2D) = "gray" {}
        _DetailNormalMap ("Detail Normal", 2D) = "bump" {}
        _DetailStrength ("Detail Strength", Range(0, 2)) = 1
        
        [Header(Subsurface Scattering)]
        [Toggle(_ENABLESUBSURFACE_ON)] _EnableSubsurface ("Enable Subsurface", Float) = 0
        _SubsurfaceColor ("Subsurface Color", Color) = (1, 0.4, 0.25, 1)
        _SubsurfaceIntensity ("Subsurface Intensity", Range(0, 2)) = 0
        _SubsurfaceDistortion ("Subsurface Distortion", Range(0, 2)) = 1
        _SubsurfacePower ("Subsurface Power", Range(0.1, 10)) = 1
        
        [Header(Smart Outline System)]
        [Toggle(_ENABLEOUTLINE_ON)] _EnableOutline ("Enable Outline", Float) = 0
        _OutlineColor ("Outline Color", Color) = (0, 0, 0, 1)
        _OutlineWidth ("Outline Width", Range(0, 10)) = 1
        [KeywordEnum(Normal, Position, UV)] _OutlineMode("Extrusion Mode", Float) = 0

        [Header(Adaptive Animated Features)]
        [Toggle(_OUTLINE_DISTANCE_SCALING_ON)] _OutlineDistanceScaling ("Distance Scaling", Float) = 0
        _OutlineAdaptiveMinWidth("Min Width", Range(0, 10)) = 0.5
        _OutlineAdaptiveMaxWidth("Max Width", Range(0, 10)) = 2.0
        [Toggle(_OUTLINE_ANIMATED_COLOR_ON)] _OutlineAnimatedColor("Animate Color", Float) = 0
        _OutlineColorB("Color B", Color) = (1,0,0,1)
        _OutlineAnimationSpeed("Animation Speed", Range(0, 10)) = 1.0
        
        [Header(Wind Animation)]
        [Toggle(_ENABLEWIND_ON)] _EnableWind ("Enable Wind", Float) = 0
        _WindSpeed ("Wind Speed", Range(0, 5)) = 1
        _WindStrength ("Wind Strength", Range(0, 1)) = 0.1
        _WindDirection ("Wind Direction", Vector) = (1, 0, 1, 0)
        
        [Header(Performance)]
        [Toggle(_RECEIVESHADOWS_ON)] _ReceiveShadows ("Receive Shadows", Float) = 1
        [Toggle(_ENABLEADDITIONALLIGHTS_ON)] _EnableAdditionalLights ("Additional Lights", Float) = 1
        _LightmapInfluence ("Lightmap Influence", Range(0, 1)) = 1
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
            #pragma shader_feature_local _OUTLINE_DISTANCE_SCALING_ON
            #pragma shader_feature_local _OUTLINE_ANIMATED_COLOR_ON
            #pragma shader_feature_local _OUTLINEMODE_NORMAL _OUTLINEMODE_POSITION _OUTLINEMODE_UV
            #pragma shader_feature_local _ENABLEWIND_ON
            
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
            #pragma shader_feature_local _ENABLERIM_ON
            #pragma shader_feature_local_fragment _RIMMODE_STANDARD _RIMMODE_STEPPED _RIMMODE_LIGHTBASED _RIMMODE_TEXTURED
            #pragma shader_feature_local _ENABLESUBSURFACE_ON
            #pragma shader_feature_local_fragment _LIGHTINGMODE_SINGLE_CELL _LIGHTINGMODE_DUAL_CELL _LIGHTINGMODE_BANDED _LIGHTINGMODE_GRADIENT_RAMP _LIGHTINGMODE_CUSTOM_RAMP
            #pragma shader_feature_local_fragment _TINT_SHADOW_ON_BASE
            #pragma shader_feature_local_fragment _MATCAP_BLEND_WITH_LIGHTING
            #pragma shader_feature_local _ENABLEWIND_ON
            #pragma shader_feature_local _RECEIVESHADOWS_ON
            #pragma shader_feature_local _ENABLEADDITIONALLIGHTS_ON
            #pragma shader_feature_local _NORMALMAP
            #pragma shader_feature_local _SOFT_SPECULAR_MASK
            #pragma shader_feature_local _ANISOTROPIC_FLOW_MAP
            #pragma shader_feature_local _EMISSION
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

