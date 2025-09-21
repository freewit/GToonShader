#ifndef GTOON_WIND_INCLUDED
#define GTOON_WIND_INCLUDED

// Bu dosya, rüzgar animasyonu hesaplamalarını içerir.
// Hem ana pass hem de outline pass tarafından kullanılır.

// Basit 2D gürültü fonksiyonu
float N21(float2 p)
{
    return frac(sin(dot(p, float2(12.9898, 78.233))) * 43758.5453123);
}

// Daha karmaşık, çok katmanlı gürültü (FBM)
float FBM(float2 p, int octaves)
{
    float value = 0.0;
    float amplitude = 0.5;
    for (int i = 0; i < octaves; i++)
    {
        value += amplitude * N21(p);
        p *= 2.0;
        amplitude *= 0.5;
    }
    return value;
}


// Rüzgar animasyonunu vertex pozisyonuna uygular
void ApplyWind(inout float3 positionOS, float4 initialPositionOS, float4 vertexColor)
{
    #if defined(_ENABLEWIND_ON)
        half windMask = 1.0;
        #if defined(_WIND_VERTEX_COLOR_MASK_ON)
            windMask = vertexColor.a;
        #endif

        float3 worldPos = mul(unity_ObjectToWorld, initialPositionOS).xyz;
        float time = _Time.y * _WindSpeed;

        #if defined(_WINDMODE_ADVANCED)
            // Gelişmiş Rüzgar
            // Dal bükülmesi (düşük frekanslı ana hareket)
            float branchPhase = worldPos.x * 0.1 + worldPos.z * 0.1;
            float branchSway = sin(time + branchPhase) * _BranchBending;
            float3 branchOffset = float3(branchSway, 0, branchSway * 0.5);

            // Yaprak türbülansı (yüksek frekanslı detaylı hareket)
            float2 noiseCoord = worldPos.xz * _WindNoiseScale * 0.1;
            noiseCoord.x += time * (1.0 + _WindPhaseVariation);
            noiseCoord.y += time * (1.0 - _WindPhaseVariation);
            float turbulence = (FBM(noiseCoord, 4) - 0.5) * 2.0 * _WindTurbulence;
            
            // Toplam hareketi birleştir
            float3 totalOffset = (branchOffset + turbulence) * _WindStrength * initialPositionOS.y;
            positionOS += totalOffset * windMask;

        #else // Basic Wind
            float windFactor = sin(time + initialPositionOS.x * 2 + initialPositionOS.z * 2) * _WindStrength;
            positionOS += _WindDirection.xyz * windFactor * initialPositionOS.y * windMask;
        #endif
    #endif
}

#endif // GTOON_WIND_INCLUDED
