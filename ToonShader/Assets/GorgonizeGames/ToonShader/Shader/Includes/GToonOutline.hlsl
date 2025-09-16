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
    float4 initialPositionOS = input.positionOS; // Orijinal pozisyonu gürültü ve rüzgar için sakla
    
    // Rüzgar animasyonunu uygula
    ApplyWind(positionOS, initialPositionOS);

    float3 normalOS = normalize(input.normalOS);
    
    // Outline Noise
    #if defined(_OUTLINE_NOISE_ON)
        // Gürültü dokusundan örnekleme yap. Vertex shader'da doku okumak için _LOD versiyonunu kullanıyoruz.
        float4 noiseTex = SAMPLE_TEXTURE2D_LOD(_OutlineNoiseMap, sampler_OutlineNoiseMap, initialPositionOS.xy * _OutlineNoiseScale * 0.1, 0);
        float noise = (noiseTex.r - 0.5) * 2.0;
        float noiseFactor = noise * _OutlineNoiseStrength;
        // Gürültüyü normal yönünde değil, ekran alanında daha tutarlı olması için xy düzleminde uygula
        positionOS.xy += normalOS.xy * noiseFactor * (_OutlineWidth * 0.01);
    #endif
    
    // Outline için vertex'leri normal yönünde genişlet
    positionOS += normalOS * _OutlineWidth * 0.005; 
    
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

