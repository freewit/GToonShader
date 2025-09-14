#ifndef GTOON_WIND_INCLUDED
#define GTOON_WIND_INCLUDED

// Bu dosya, rüzgar animasyonu hesaplamalarını içerir.
// Hem ana pass hem de outline pass tarafından kullanılır.

// Rüzgar animasyonunu vertex pozisyonuna uygular
void ApplyWind(inout float3 positionOS, float4 initialPositionOS)
{
    #if defined(_ENABLEWIND_ON)
        float time = _Time.y * _WindSpeed;
        // Basit bir sinüs dalgası ile rüzgar efekti oluşturulur
        float windFactor = sin(time + initialPositionOS.x * 2 + initialPositionOS.z * 2) * _WindStrength;
        // Rüzgarın etkisi, objenin y eksenindeki yüksekliğiyle orantılıdır (ağaçlar, çimenler için ideal)
        positionOS += _WindDirection.xyz * windFactor * initialPositionOS.y;
    #endif
}

#endif // GTOON_WIND_INCLUDED
