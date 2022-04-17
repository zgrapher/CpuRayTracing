using UnityEngine;
using static Unity.Mathematics.math;
using float3 = Unity.Mathematics.float3;

namespace RayTracer
{
    public class Plane : GeometricObject
    {
        private const float KEpsilon = 0.001f;

        [SerializeField]
        private Material material;

        public override void Init()
        {
            Transform trans = transform;
            point = trans.position;
            normal = trans.up;
        }

        public override bool Hit(TraceRay ray, out float tMin, ref ShadeRec sr)
        {
            var t = dot(point - ray.o, normal) / dot(ray.d, normal);
            if (t > KEpsilon)
            {
                tMin = t;
                sr.normal = normal;
                sr.local_hit_point = ray.o + t * ray.d;
                return true;
            }
            else
            {
                tMin = -1.0f;
                return false;
            }
        }

        public override bool ShadowHit(TraceRay ray, out float tMin)
        {
            tMin = -1;
            if (!enableShadow)
                return false;
            
            var t = dot(point - ray.o, normal) / dot(ray.d, normal);
            if (t > KEpsilon)
            {
                tMin = t;
                return true;
            }

            return false;
        }

        public override Material GetMaterial()
        {
            return material;
        }

        private float3 point;
        private float3 normal;
        public bool enableShadow;
    }
}