using Unity.Mathematics;
using static Unity.Mathematics.math;
using UnityEngine;

namespace RayTracer
{
    [CreateAssetMenu(fileName = "Glossy", menuName = "RayTracingMaterial/Glossy")]
    public class GlossyReflector : Phone
    {
        public override void Init(int sampleCount)
        {
            base.Init(sampleCount);
            
            var samp =new MultiJittered(sampleCount);
            samp.MapSamplesToHemisphere(Exp);
            glossy_brdf.SetExp(Exp);
            glossy_brdf.SetSampler(samp);
            glossy_brdf.SetCs(float3(cs.r, cs.g, cs.b));
        }

        public override float3 AreaLightShade(ShadeRec sr)
        {
            float3 radiance = base.AreaLightShade(sr);
            float3 wo = -sr.ray.d;
            float3 fr = glossy_brdf.SampleF(sr, wo, out float3 wi, out var pdf);
            var reflectedRay = new TraceRay(sr.hit_point, wi);

            radiance += fr * sr.w.tracer.TraceRay(reflectedRay, sr.depth + 1) * dot(sr.normal, wi) / pdf;
            return radiance;
        }

        private GlossySpecular glossy_brdf = new GlossySpecular();
    }
}