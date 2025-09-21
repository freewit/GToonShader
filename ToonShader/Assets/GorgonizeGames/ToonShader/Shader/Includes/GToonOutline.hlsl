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
    float4 color        : COLOR;
    UNITY_VERTEX_INPUT_INSTANCE_ID
};

struct OutlineVaryings
{
    float4 positionCS   : SV_POSITION;
    float3 positionWS   : TEXCOORD0;
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
    ApplyWind(positionOS, input.positionOS, input.color);
    
    float width = _OutlineWidth;
    
    // Distance Scaling
    #if defined(_OUTLINE_DISTANCE_SCALING_ON)
        float3 positionWS = TransformObjectToWorld(positionOS);
        float distanceToCamera = length(positionWS - _WorldSpaceCameraPos.xyz);
        // Uzaklığa göre 0-1 arası bir değer hesapla, min/max ile kelepçele
        float distanceFactor = saturate(distanceToCamera / 50.0); // 50 birim varsayılan max mesafe
        width = lerp(_OutlineAdaptiveMinWidth, _OutlineAdaptiveMaxWidth, distanceFactor);
    #endif

    // Extrusion Mode
    #if defined(_OUTLINEMODE_POSITION)
        // --- DÜZELTME ---
        // Kameradan uzaklaşmak yerine objenin merkezinden (pivot) uzaklaş. 
        // Bu, sert kenarlı modellerde (küp gibi) daha stabil ve tutarlı sonuçlar verir.
        float3 direction = normalize(input.positionOS.xyz);
        positionOS += direction * width * 0.005;
    #elif defined(_OUTLINEMODE_UV)
        // Bu mod için özel bir UV genişletme tekniği gerekebilir.
        // Şimdilik normal mod gibi davranması için bırakıldı.
        positionOS += input.normalOS * width * 0.005;
    #else // _OUTLINEMODE_NORMAL
        positionOS += input.normalOS * width * 0.005;
    #endif
    
    output.positionWS = TransformObjectToWorld(positionOS);
    output.positionCS = TransformWorldToHClip(output.positionWS);
    
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
            float sine = sin(_Time.y * _OutlineAnimationSpeed + input.positionWS.y) * 0.5 + 0.5;
            finalColor = lerp(_OutlineColor, _OutlineColorB, sine);
        #endif
        return finalColor;
    #else
        discard;
        return half4(0, 0, 0, 0);
    #endif
}

#endif // GTOON_OUTLINE_INCLUDED

