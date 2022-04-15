using Unity.Mathematics;
using static Unity.Mathematics.math;
using UnityEngine;

namespace RayTracer
{
    [CreateAssetMenu(fileName = "Glossy", menuName = "RayTracingMaterial/Glossy")]
    public class GlossyReflector : Phone
    {
        protected new void OnEnable()
        {
            base.OnEnable();
        }

        public override void SetSamples(Sampler samp)
        {
            samp.MapSamplesToHemisphere(Exp);
            glossy_brdf.SetExp(Exp);
            glossy_brdf.SetSampler(samp);
            glossy_brdf.SetCs(float3(cs.r, cs.g, cs.b));
        }

        public override float3 AreaLightShade(ShadeRec sr)
        {
            var radiance = base.AreaLightShade(sr);
            var wo = -sr.ray.d;
            var fr = glossy_brdf.SampleF(sr, wo, out var wi, out var pdf);
            var reflectedRay = new TraceRay(sr.hit_point, wi);

            radiance += fr * sr.w.tracer.TraceRay(reflectedRay, sr.depth + 1) * dot(sr.normal, wi) / pdf;
            return radiance;
        }

        private GlossySpecular glossy_brdf = new GlossySpecular();
    }
}