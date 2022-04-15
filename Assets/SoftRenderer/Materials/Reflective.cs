using Unity.Mathematics;
using static Unity.Mathematics.math;
using UnityEngine;

namespace RayTracer
{
    [CreateAssetMenu(fileName = "Reflective", menuName = "RayTracingMaterial/Reflective")]
    public class Reflective : Phone
    {
        [SerializeField] private float kr;

        [SerializeField] private Color cr;
        
        private new void OnEnable()
        {
            base.OnEnable();
            
            reflective_brdf.SetKr(kr);
            reflective_brdf.SetCr(cr.r, cr.g, cr.b);
        }
        
        public override float3 Shade(ShadeRec sr)
        {
            var radiance = base.Shade(sr);

            var wo = sr.ray.d;
            var fr = reflective_brdf.SampleF(sr, wo, out var wi);
            var reflectRay = new TraceRay(sr.hit_point, wi);

            radiance += fr * sr.w.tracer.TraceRay(reflectRay, sr.depth + 1) * dot(sr.normal, wi);
            return radiance;
        }

        private PerfectSpecular reflective_brdf = new PerfectSpecular();
    }
}