Shader "Gorgonize/Gorgonize Toon Shader"
{
    Properties
    {
        [Header(Base Properties)]
        _BaseColor ("Base Color", Color) = (1, 1, 1, 1)
        _BaseMap ("Base Texture", 2D) = "white" {}
        
        [Header(Shadow System)]
        [KeywordEnum(Single Cell, Banded, Ramp)] _LightingMode ("Lighting Mode", Float) = 0
        
        // Single Cell & Banded
        _ShadowThreshold ("Shadow Threshold", Range(0, 1)) = 0.5
        [Toggle(_TINT_SHADOW_ON_BASE)] _TintShadowOnBase ("Tint On Full Object", Float) = 0

        // Single Cell
        _TransitionSoftness ("Transition Softness", Range(0.001, 1)) = 0.05
        
        // Banded
        _MidtoneThreshold("Mid-tone Threshold", Range(0, 1)) = 0.75
        _BandSoftness("Band Softness", Range(0.001, 1)) = 0.05

        // General
        _ShadowRamp ("Shadow Ramp", 2D) = "white" {} [NoScaleOffset]
        _ShadowColor ("Shadow Color", Color) = (0.5, 0.5, 0.8, 1)
        _OcclusionStrength ("Occlusion Strength", Range(0, 1)) = 1
        
        [Header(Highlight System)]
        [Toggle(_ENABLEHIGHLIGHTS_ON)] _EnableHighlights ("Enable Highlights", Float) = 1
        _SpecularColor ("Specular Color", Color) = (1, 1, 1, 1)
        _SpecularSize ("Specular Size", Range(0, 1)) = 0.1
        _SpecularSmoothness ("Specular Smoothness", Range(0, 1)) = 0.5
        _SpecularSteps ("Specular Steps", Range(1, 8)) = 2
        
        [Header(Rim Lighting)]
        [Toggle(_ENABLERIM_ON)] _EnableRim ("Enable Rim Lighting", Float) = 1
        _RimColor ("Rim Color", Color) = (1, 1, 1, 1)
        _RimPower ("Rim Power", Range(0, 10)) = 2
        _RimIntensity ("Rim Intensity", Range(0, 3)) = 1
        _RimOffset ("Rim Offset", Range(-1, 1)) = 0
        
        [Header(Advanced Features)]
        [Toggle(_NORMALMAP)] _NormalMapToggle("Enable Normal Map", Float) = 0
        _NormalMap ("Normal Map", 2D) = "bump" {}
        _NormalStrength ("Normal Strength", Range(0, 2)) = 1
        [Toggle(_EMISSION)] _EmissionToggle("Enable Emission", Float) = 0
        _EmissionMap ("Emission", 2D) = "black" {}
        _EmissionColor ("Emission Color", Color) = (0, 0, 0, 1)
        _EmissionIntensity ("Emission Intensity", Range(0, 10)) = 0
        
        [Header(Detail Textures)]
        [Toggle(_DETAIL)] _DetailToggle("Enable Detail Textures", Float) = 0
        _DetailMap ("Detail Albedo", 2D) = "gray" {}
        _DetailNormalMap ("Detail Normal", 2D) = "bump" {}
        _DetailStrength ("Detail Strength", Range(0, 2)) = 1
        
        [Header(Subsurface Scattering)]
        [Toggle(_ENABLESUBSURFACE_ON)] _EnableSubsurface ("Enable Subsurface", Float) = 0
        _SubsurfaceColor ("Subsurface Color", Color) = (1, 0.4, 0.25, 1)
        _SubsurfaceIntensity ("Subsurface Intensity", Range(0, 2)) = 0
        _SubsurfaceDistortion ("Subsurface Distortion", Range(0, 2)) = 1
        _SubsurfacePower ("Subsurface Power", Range(0.1, 10)) = 1
        
        [Header(Outline)]
        [Toggle(_ENABLEOUTLINE_ON)] _EnableOutline ("Enable Outline", Float) = 0
        _OutlineColor ("Outline Color", Color) = (0, 0, 0, 1)
        _OutlineWidth ("Outline Width", Range(0, 10)) = 1
        
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
            #pragma shader_feature_local _ENABLEHIGHLIGHTS_ON
            #pragma shader_feature_local _ENABLERIM_ON
            #pragma shader_feature_local _ENABLESUBSURFACE_ON
            #pragma shader_feature_local _LIGHTINGMODE_SINGLE_CELL _LIGHTINGMODE_BANDED _LIGHTINGMODE_RAMP
            #pragma shader_feature_local _TINT_SHADOW_ON_BASE
            #pragma shader_feature_local _ENABLEWIND_ON
            #pragma shader_feature_local _RECEIVESHADOWS_ON
            #pragma shader_feature_local _ENABLEADDITIONALLIGHTS_ON
            #pragma shader_feature_local _NORMALMAP
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

