#ifndef GTOON_OUTLINE_INCLUDED
#define GTOON_OUTLINE_INCLUDED

// Bu dosya, outline pass'inin vertex ve fragment shader'larını içerir.

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
#include "Includes/GToonInput.hlsl"
#include "Includes/GToonWind.hlsl"

struct OutlineAttributes
{
    float4 positionOS   : POSITION;
    float3 normalOS     : NORMAL;
    UNITY_VERTEX_INPUT_INSTANCE_ID
};

struct OutlineVaryings
{
    float4 positionCS   : SV_POSITION;
    UNITY_VERTEX_INPUT_INSTANCE_ID
    UNITY_VERTEX_OUTPUT_STEREO
};

// Outline Vertex Shader
OutlineVaryings OutlineVert(OutlineAttributes input)
{
    OutlineVaryings output = (OutlineVaryings)0;
    
    UNITY_SETUP_INSTANCE_ID(input);
    UNITY_TRANSFER_INSTANCE_ID(input, output);
    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);
    
    #if defined(_ENABLEOUTLINE_ON)
    
    float3 positionOS = input.positionOS.xyz;
    
    // Rüzgar animasyonunu uygula
    ApplyWind(positionOS, input.positionOS);
    
    // Outline için vertex'leri normal yönünde genişlet
    float3 normalOS = normalize(input.normalOS);
    positionOS += normalOS * _OutlineWidth * 0.005; // Değeri makul seviyelere ölçekle
    
    output.positionCS = TransformObjectToHClip(positionOS);
    
    #else
    // Outline kapalıysa, vertex'i kırpma alanının dışına taşıyarak görünmez yap
    output.positionCS = float4(0, 0, 0, -1);
    #endif
    
    return output;
}

// Outline Fragment Shader
half4 OutlineFrag(OutlineVaryings input) : SV_Target
{
    UNITY_SETUP_INSTANCE_ID(input);
    UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
    
    #if defined(_ENABLEOUTLINE_ON)
    return _OutlineColor;
    #else
    discard;
    return half4(0, 0, 0, 0);
    #endif
}

#endif // GTOON_OUTLINE_INCLUDED
