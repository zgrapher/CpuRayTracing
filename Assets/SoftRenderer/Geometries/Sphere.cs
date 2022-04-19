using Unity.Mathematics;
using UnityEngine;
using static Unity.Mathematics.math;
using float3 = Unity.Mathematics.float3;

namespace RayTracer
{
    public class Sphere : IGeometricObject
    {
        private const float KEpsilon = 0.001f;

        private Material material;

        public void UpdateData(float3 pos, float3 right, float3 up, float3 forward, float3 scale, Material mat)
        {
            material = mat;
            
            center = pos;
            radius = scale.x * 0.5f;
        }

        public bool enableShadow { get; set; }

        public bool Hit(TraceRay ray, out float tMin, ref ShadeRec sr)
        {
            float3 temp = (float3)ray.o - center;
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

        public bool ShadowHit(TraceRay ray, out float tMin)
        {
            tMin = -1;
            if (!enableShadow)
                return false;

            float3 temp = (float3)ray.o - center;
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

        public float Pdf(ShadeRec sr)
        {
            return 1.0f;
        }

        public float3 Sample()
        {
            return float3.zero;
        }

        public float3 GetNormal()
        {
            return float3.zero;
        }

        public void SetSampler(Sampler samp)
        {
        }

        public BBox GetBoundingBox()
        {
            const float delta = 0.000001f;
            return (new BBox(center.x - radius - delta, center.x + radius + delta,
                center.y - radius - delta, center.y + radius + delta,
                center.z - radius - delta, center.z + radius + delta));
        }

        public Material GetMaterial()
        {
            return material;
        }

        private float3 center;
        private float radius;
    }
}