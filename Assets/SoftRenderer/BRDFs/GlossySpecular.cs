using UnityEngine;
using static Unity.Mathematics.math;
using float3 = Unity.Mathematics.float3;

namespace RayTracer
{
    public class GlossySpecular : Brdf
    {
        public void SetKs(float k)
        {
            ks = k;
        }
        public void SetExp(float e)
        {
            exp = e;
        }

        public void SetCs(float3 c)
        {
            cs = c;
        }

        public void SetSampler(Sampler samp)
        {
            sampler = samp;
        }
        
        public override float3 f(ShadeRec sr, float3 wi, float3 wo)
        {
            var r = reflect(-wi, sr.normal);
            var rdotwo = dot(r, wo);
            if (rdotwo > 0.0f)
            {
                return ks * pow(rdotwo, exp);
            }
            
            return float3.zero;
        }

        public override float3 SampleF(ShadeRec sr, float3 wo, out float3 wi, out float pdf)
        {
            var r = reflect(-wo, sr.normal);

            var u = cross(float3(0.00424f, 1f, 0.00764f), r);
            u = normalize(u);
            var v = cross(r, u);

            var sp = sampler.SampleHemisphere();
            wi = sp.x * u + sp.y * v + sp.z * r;
            if (dot(sr.normal, wi) < 0.0f)
                wi = -sp.x * u - sp.y * v + sp.z * r;

            var phoneLobe = pow(dot(r, wi), exp);
            pdf = phoneLobe * dot(sr.normal, wi);

            return ks * cs * phoneLobe;
        }

        public override float3 rho(ShadeRec sr, float3 wo)
        {
            return float3.zero;
        }
        
        private float ks = 1.0f;
        private float exp = 1.0f;
        private float3 cs;
        private Sampler sampler;
    }
}