Shader "Hidden/Edge Detection"
{
    Properties
    {
        _OutlineThickness ("Fırça Kalınlığı", Float) = 8.0
        _OutlineColor ("Fırça Rengi", Color) = (0,0,0,1)
        _DepthThreshold ("Derinlik Hassasiyeti", Float) = 0.01
        _NormalsThreshold ("Normal Hassasiyeti", Range(0, 1)) = 0.4
        _BrushTex ("Fırça Dokusu", 2D) = "white" {}
        _BrushTiling ("Fırça Deseni Boyutu", Float) = 1.0
    }

    SubShader
    {
        Tags
        {
            "RenderPipeline" = "UniversalPipeline"
            "RenderType"="Opaque"
        }

        ZWrite Off
        Cull Off
        Pass
        {
            // Alpha blend ile outline'ı sahne üzerine yerleştir
            Blend SrcAlpha OneMinusSrcAlpha
            Name "TEXTURED BRUSH OUTLINE"

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"

            // Değişkenler
            float _OutlineThickness;
            float4 _OutlineColor;
            float _DepthThreshold;
            float _NormalsThreshold;
            sampler2D _BrushTex;
            float _BrushTiling;

            // Manual normal hesaplama fonksiyonu
            float3 GetNormalFromDepth(float2 uv, float2 texel_size)
            {
                float depth_center = SampleSceneDepth(uv);
                float depth_left = SampleSceneDepth(uv + float2(-texel_size.x, 0));
                float depth_right = SampleSceneDepth(uv + float2(texel_size.x, 0));
                float depth_up = SampleSceneDepth(uv + float2(0, texel_size.y));
                float depth_down = SampleSceneDepth(uv + float2(0, -texel_size.y));

                float3 normal;
                normal.x = depth_left - depth_right;
                normal.y = depth_down - depth_up;
                normal.z = 1.0;
                return normalize(normal);
            }

            float4 frag(Varyings IN) : SV_TARGET
            {
                float2 uv = IN.texcoord;
                float2 texel_size = _ScreenParams.zw - 1.0;
                
                float center_depth = Linear01Depth(SampleSceneDepth(uv), _ZBufferParams);
                float3 center_normal = GetNormalFromDepth(uv, texel_size);

                float edge = 0.0;
                const int sample_count = 16;

                [unroll]
                for (int i = 0; i < sample_count; i++)
                {
                    float angle = (float)i / sample_count * 6.28318;
                    float2 offset = float2(cos(angle), sin(angle)) * _OutlineThickness * texel_size;
                    float2 sample_uv = uv + offset;
                    
                    float sample_depth = Linear01Depth(SampleSceneDepth(sample_uv), _ZBufferParams);
                    float3 sample_normal = GetNormalFromDepth(sample_uv, texel_size);

                    float depth_diff = abs(center_depth - sample_depth);
                    float depth_edge = step(_DepthThreshold, depth_diff);

                    float normal_diff = dot(center_normal, sample_normal);
                    float normal_edge = step(normal_diff, 1.0 - _NormalsThreshold);

                    edge += max(depth_edge, normal_edge);
                }
                
                edge = saturate(edge / (sample_count * 0.25));

                // Fırça Dokusunu Örnekleme
                float2 brush_uv = uv * _BrushTiling;
                float brush_sample = tex2D(_BrushTex, brush_uv).r;
                
                // Kenar yoğunluğunu fırça dokusuyla modüle et
                edge *= brush_sample;
                
                // Sadece outline'ı döndür, arka planı GPU blend ile halledelim
                float final_alpha = edge * _OutlineColor.a;
                
                return float4(_OutlineColor.rgb, final_alpha);
            }
            ENDHLSL
        }
    }
}