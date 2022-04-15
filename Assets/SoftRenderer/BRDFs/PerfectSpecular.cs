using Unity.Mathematics;
using static Unity.Mathematics.math;
using float3 = Unity.Mathematics.float3;

namespace RayTracer
{
    public class PerfectSpecular : Brdf
    {
        public void SetKr(float k)
        {
            kr = k;
        }

        public void SetCr(float c)
        {
            cr = c;
        }

        public void SetCr(float r, float g, float b)
        {
            cr = float3(r, g, b);
        }
        
        public override float3 f(ShadeRec sr, float3 wi, float3 wo)
        {
            return float3(0);
        }

        public override float3 rho(ShadeRec sr, float3 wo)
        {
            return float3(0);
        }

        public override float3 SampleF(ShadeRec sr, float3 wo, out float3 wi)
        {
            wi = reflect(wo, sr.normal);
            return kr * cr / abs(dot(sr.normal, wi));
        }

        public override float3 SampleF(ShadeRec sr, float3 wo, out float3 wi, out float pdf)
        {
            wi = reflect(wo, sr.normal);
            pdf = abs(dot(sr.normal, wi));
            return kr * cr;
        }

        private float kr;
        private float3 cr;
    }
}