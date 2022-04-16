using Unity.Mathematics;

namespace RayTracer
{
    public interface IBtdf
    {
        float3 F(ShadeRec sr, float3 wo, float3 wi);

        float3 SampleF(ShadeRec sr, float3 wo, out float3 wt);

        float3 Rho(ShadeRec sr, float3 wo);
    }
}