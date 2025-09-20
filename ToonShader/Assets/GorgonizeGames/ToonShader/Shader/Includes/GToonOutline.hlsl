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
    float2 texcoord     : TEXCOORD0;
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

    // --- Final Width Calculation ---
    half finalWidth = _OutlineWidth;

    #if defined(_OUTLINE_DISTANCE_SCALING_ON)
        VertexPositionInputs vertexInputForDist = GetVertexPositionInputs(positionOS);
        float dist = distance(vertexInputForDist.positionWS, _WorldSpaceCameraPos);
        // Uzaklaştıkça 0'a, yaklaştıkça 1'e giden bir faktör hesapla
        float distFactor = 1.0 - saturate((dist - _OutlineMinMaxDistance.x) / (_OutlineMinMaxDistance.y - _OutlineMinMaxDistance.x));
        
        #if defined(_OUTLINE_ADAPTIVE_ON)
            // Min ve Max değerleri arasında geçiş yap
            finalWidth = lerp(_OutlineMinWidth, _OutlineWidth, distFactor);
        #else
            // Sadece ana kalınlığı mesafeye göre ölçekle
            finalWidth = _OutlineWidth * distFactor;
        #endif
    #endif

    // --- Expansion Logic ---
    float3 expansionDir = normalize(input.normalOS);
    
    #if defined(_OUTLINEEXPANSIONMODE_POSITION)
        expansionDir = normalize(positionOS);
    #elif defined(_OUTLINEEXPANSIONMODE_UV)
        // UV koordinatlarının merkezden dışarı doğru olan yönünü kullanarak 2D bir genişleme yönü oluştur
        float2 uvDir = normalize(input.texcoord - 0.5);
        expansionDir = float3(uvDir.x, uvDir.y, 0);
    #endif

    // --- Noise Logic ---
    #if defined(_OUTLINE_NOISE_MODE_ON)
        float4 noiseTex = SAMPLE_TEXTURE2D_LOD(_OutlineNoiseMap, sampler_OutlineNoiseMap, initialPositionOS.xy * _OutlineNoiseScale * 0.1, 0);
        float noise = (noiseTex.r - 0.5) * 2.0;
        float noiseFactor = noise * _OutlineNoiseStrength;
        positionOS.xy += expansionDir.xy * noiseFactor * (finalWidth * 0.01);
    #endif
    
    // --- Final Position Calculation ---
    positionOS += expansionDir * finalWidth * 0.005; 
    
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
        half4 finalColor = _OutlineColor;
        #if defined(_OUTLINE_ANIMATED_COLOR_ON)
            float sine = (sin(_Time.y * _OutlineAnimationSpeed) + 1.0) * 0.5;
            finalColor = lerp(_OutlineColor, _OutlineColorB, sine);
        #endif
        return finalColor;
    #else
    discard;
    return half4(0, 0, 0, 0);
    #endif
}

#endif // GTOON_OUTLINE_INCLUDED

