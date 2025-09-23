#ifndef GTOON_OUTLINE_PASS_INCLUDED
#define GTOON_OUTLINE_PASS_INCLUDED

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
#include "Includes/GToonInput.hlsl"

struct Attributes
{
    float4 positionOS : POSITION;
    float3 normalOS : NORMAL;
    float2 uv : TEXCOORD0;
    half4 color : COLOR;
    UNITY_VERTEX_INPUT_INSTANCE_ID
};

struct Varyings
{
    float4 positionCS : SV_POSITION;
    float2 uv : TEXCOORD0;
    half4 color : COLOR;
    half fogFactor : TEXCOORD1;
    UNITY_VERTEX_INPUT_INSTANCE_ID
    UNITY_VERTEX_OUTPUT_STEREO
};

Varyings OutlineVertex(Attributes input)
{
    Varyings output = (Varyings)0;
    
    UNITY_SETUP_INSTANCE_ID(input);
    UNITY_TRANSFER_INSTANCE_ID(input, output);
    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

    #if defined(_ENABLEOUTLINE_ON)
        VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);
        VertexNormalInputs normalInput = GetVertexNormalInputs(input.normalOS);
        
        // Outline expansion
        float3 normalWS = normalInput.normalWS;
        
        #if defined(_OUTLINEMODE_NORMAL)
            // Use vertex normals for outline expansion
            float3 expandDir = normalWS;
        #elif defined(_OUTLINEMODE_STANDARD)
            // Use smoothed normals (if available)
            float3 expandDir = normalWS;
        #else // _OUTLINEMODE_TRADITIONAL
            // Traditional method using object space normals
            float3 expandDir = TransformObjectToWorldDir(input.normalOS);
        #endif
        
        // Apply outline width
        float outlineWidth = _OutlineWidth;
        
        #if defined(_OUTLINELOD_ON)
            // Distance-based LOD
            float distance = length(GetCameraPositionWS() - vertexInput.positionWS);
            outlineWidth *= saturate(1.0 / (distance * 0.1));
        #endif
        
        // Vertex color masking for outline width
        outlineWidth *= input.color.a;
        
        // Expand vertex position
        float3 expandedPositionWS = vertexInput.positionWS + expandDir * outlineWidth;
        output.positionCS = TransformWorldToHClip(expandedPositionWS);
        
        // Apply Z offset
        output.positionCS.z += _OutlineZOffset * output.positionCS.w;
        
    #else
        // If outline is disabled, render at far plane
        output.positionCS = float4(0, 0, 1, 1);
    #endif
    
    output.uv = TRANSFORM_TEX(input.uv, _BaseMap);
    output.color = input.color;
    output.fogFactor = ComputeFogFactor(output.positionCS.z);
    
    return output;
}

half4 OutlineFragment(Varyings input) : SV_Target
{
    UNITY_SETUP_INSTANCE_ID(input);
    UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

    #if defined(_ENABLEOUTLINE_ON)
        half4 color = _OutlineColor;
        
        // Apply base texture if needed
        half4 baseMap = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, input.uv);
        color.a *= baseMap.a;
        
        // Apply vertex color
        color *= input.color;
        
        // Apply fog
        color.rgb = MixFog(color.rgb, input.fogFactor);
        
        return color;
    #else
        // If outline is disabled, discard pixel
        discard;
        return 0;
    #endif
}

#endif