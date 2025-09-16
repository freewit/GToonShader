#ifndef GTOON_OUTLINE_INCLUDED
#define GTOON_OUTLINE_INCLUDED

// Bu dosya, outline pass'inin vertex ve fragment shader'larını içerir.

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
#include "Includes/GToonInput.hlsl"
#include "Includes/GToonWind.hlsl"

// Basit bir 2D noise fonksiyonu (Shadertoy'dan uyarlanmıştır)
// https://www.shadertoy.com/view/4dS3Wd
float2 random (float2 st) 
{
    return frac(sin(float2(dot(st, float2(12.9898,78.233)), dot(st, float2(12.9898,78.233)))) * 43758.5453123);
}

float noise (float2 st) 
{
    float2 i = floor(st);
    float2 f = frac(st);

    float a = random(i).x;
    float b = random(i + float2(1.0, 0.0)).x;
    float c = random(i + float2(0.0, 1.0)).x;
    float d = random(i + float2(1.0, 1.0)).x;

    float2 u = f * f * (3.0 - 2.0 * f);
    return lerp(a, b, u.x) + (c - a)* u.y * (1.0 - u.x) + (d - b) * u.y * u.x;
}


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
    float outlineOffset = _OutlineWidth * 0.005;

    #if defined(_OUTLINE_NOISE_ON)
        float2 noiseUV = positionOS.xz * _OutlineNoiseScale * 0.1;
        float time = _Time.y * _OutlineNoiseSpeed;
        // Gürültü değerini -1 ile 1 arasına getiriyoruz
        float noiseVal = noise(noiseUV + time) * 2.0 - 1.0; 
        float noiseStrength = _OutlineNoiseStrength * 0.1;
        outlineOffset += noiseVal * noiseStrength;
    #endif

    positionOS += normalOS * outlineOffset;
    
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
