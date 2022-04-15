using Unity.Mathematics;
using UnityEngine;
using static Unity.Mathematics.math;

namespace RayTracer
{
    public class Sphere : GeometricObject
    {
        private const float KEpsilon = 0.001f;

        [SerializeField] private Material material;

        private void Start()
        {
            var trans = transform;
            center = trans.position;
            radius = trans.localScale.x * 0.5f;
        }

        public override bool Hit(TraceRay ray, out float tMin, ref ShadeRec sr)
        {
            var temp = (float3)ray.o - center;
            var a = dot(ray.d, ray.d);
            var b = 2.0f * dot(temp, ray.d);
            var c = dot(temp, temp) - radius * radius;
            var disc = b * b - 4.0f * a * c;

            tMin = -1.0f;
            if (disc < 0.0f)
            {
                return false;
            }
            else
            {
                var e = sqrt(disc);
                var denom = 2.0f * a;
                var t = (-b - e) / denom;

                if (t > KEpsilon)
                {
                    tMin = t;
                    sr.normal = (temp + t * ray.d) / radius;
                    sr.local_hit_point = ray.o + t * ray.d;
                    return true;
                }

                t = (-b + e) / denom;

                if (t > KEpsilon)
                {
                    tMin = t;
                    sr.normal = (temp + t * ray.d) / radius;
                    sr.local_hit_point = ray.o + t * ray.d;
                    return true;
                }
            }

            return false;
        }

        public override bool ShadowHit(TraceRay ray, out float tMin)
        {
            tMin = -1;
            if (!enableShadow)
                return false;

            var temp = (float3)ray.o - center;
            var a = dot(ray.d, ray.d);
            var b = 2.0f * dot(temp, ray.d);
            var c = dot(temp, temp) - radius * radius;
            var disc = b * b - 4.0f * a * c;

            if (disc < 0.0f)
            {
                return false;
            }
            else
            {
                var e = sqrt(disc);
                var denom = 2.0f * a;
                var t = (-b - e) / denom;

                if (t > KEpsilon)
                {
                    tMin = t;
                    return true;
                }

                t = (-b + e) / denom;

                if (t > KEpsilon)
                {
                    tMin = t;
                    return true;
                }
            }

            return false;
        }

        public override BBox GetBoundingBox()
        {
            const float delta = 0.000001f;
            return (new BBox(center.x - radius - delta, center.x + radius + delta,
                center.y - radius - delta, center.y + radius + delta,
                center.z - radius - delta, center.z + radius + delta));
        }

        public override Material GetMaterial()
        {
            return material;
        }

        private float3 center;
        private float radius;
        public bool enableShadow;
    }
}