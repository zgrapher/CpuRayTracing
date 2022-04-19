using Unity.Mathematics;
using static Unity.Mathematics.math;
using UnityEngine;
using float3 = Unity.Mathematics.float3;

namespace RayTracer
{
    public class AreaLight : LightBase
    {
        [SerializeField] private Geomtric light_object;

        [SerializeField] private Material material;
        
        public override float3 L(ShadeRec sr)
        {
            var ndotd = dot(light_normal, -wi);
            return ndotd > 0.0f ? material.GetLe(sr) : float3.zero;
        }

        public override float G(ShadeRec sr)
        {
            var ndotd = dot(light_normal, -wi);
            var d2 = distancesq(sample_point, sr.hit_point);
            return (ndotd / d2);
        }

        public override float Pdf(ShadeRec sr)
        {
            return light_object.geometric.Pdf(sr);
        }

        public override float3 GetDirection(ShadeRec sr)
        {
            sample_point = light_object.geometric.Sample();
            light_normal = light_object.geometric.GetNormal();
            wi = normalize(sample_point - sr.hit_point);
            return wi;
        }

        public override bool InShadow(TraceRay ray, ShadeRec sr)
        {
            var ts = dot(sample_point - ray.o, ray.d);
            foreach (var obj in sr.w.objects)
            {
                if (obj.ShadowHit(ray, out var t) && t < ts)
                    return true;
            }

            return false;
        }

        private float3 sample_point;
        private float3 light_normal;
        private float3 wi;
    }
}