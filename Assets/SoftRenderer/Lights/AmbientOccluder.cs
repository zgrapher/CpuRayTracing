using static Unity.Mathematics.math;
using UnityEngine;
using float3 = Unity.Mathematics.float3;

namespace RayTracer
{
    public class AmbientOccluder : LightBase
    {
        [SerializeField] private float ls = 1.0f;

        [SerializeField] private float minAmount;

        [SerializeField] private Color color;

        private void Start()
        {
            c = float3(color.r, color.g, color.b);
        }

        public void SetSampler(Sampler samp)
        {
            sampler = samp;
            sampler.MapSamplesToHemisphere(1);
        }

        public override float3 L(ShadeRec sr)
        {
            w = sr.normal;
            v = cross(w, float3(0.0072f, 1.0f, 0.0034f));
            v = normalize(v);
            u = cross(v, w);

            var shadowRay = new TraceRay()
            {
                o = sr.hit_point,
                d = GetDirection(sr)
            };

            if (InShadow(shadowRay, sr))
            {
                return (minAmount * ls * c);
            }
            else
            {
                return ls * c;
            }
        }

        public override float3 GetDirection(ShadeRec sr)
        {
            var sp = sampler.SampleHemisphere();
            return sp.x * u + sp.y * v + sp.z * w;    
        }

        public override bool InShadow(TraceRay ray, ShadeRec sr)
        {
            foreach (var obj in sr.w.objects)
            {
                if (obj.ShadowHit(ray, out _))
                    return true;
            }

            return false;
        }

        private float3 c;
        private float3 u = new float3(1, 0, 0);
        private float3 v = new float3(0, 1, 0);
        private float3 w = new float3(0, 0, 1);
        private Sampler sampler;
    }
}