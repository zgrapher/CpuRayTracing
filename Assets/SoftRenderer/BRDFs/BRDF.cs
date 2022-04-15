using Unity.Mathematics;
using static Unity.Mathematics.math;
using float3 = Unity.Mathematics.float3;

namespace RayTracer
{
    public abstract class Brdf
    {
        // ReSharper disable once InconsistentNaming
        public abstract float3 f(ShadeRec sr, float3 wi, float3 wo);

        // ReSharper disable once InconsistentNaming
        public abstract float3 rho(ShadeRec sr, float3 wo);

        public virtual float3 SampleF(ShadeRec sr, float3 wo, out float3 wi)
        {
            wi = float3.zero;
            return float3(0);
        }
        
        public virtual float3 SampleF(ShadeRec sr, float3 wo, out float3 wi, out float pdf)
        {
            wi = float3.zero;
            pdf = 0f;
            return float3(0);
        }
    }
}