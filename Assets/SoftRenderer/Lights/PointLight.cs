using Unity.Mathematics;
using static Unity.Mathematics.math;
using UnityEngine;

namespace RayTracer
{
    public class PointLight : LightBase
    {
        [SerializeField] private float ls = 1.0f;
        
        private void Start()
        {
            var c = GetComponent<Light>().color;
            
            color = float3(c.r, c.g, c.b);
            location = transform.position;
        }

        public override float3 L(ShadeRec sr)
        {
            return ls * color;
        }

        public override float3 GetDirection(ShadeRec sr)
        {
            return normalize(location - sr.hit_point);
        }

        public override bool InShadow(TraceRay ray, ShadeRec sr)
        {
            var d = distance(location, ray.o);
            foreach (var obj in sr.w.objects)
            {
                if (obj.ShadowHit(ray, out var t) && t < d)
                {
                    return true;
                }
            }

            return false;
        }

        private float3 location;
        private float3 color;
    }
}