using Unity.Mathematics;
using UnityEngine;
using static Unity.Mathematics.math;
using float3 = Unity.Mathematics.float3;

namespace RayTracer
{
    public class Rect : IGeometricObject
    {
        private const float KEpsilon = 0.001f;

        public Material material;

        public void SetData(float3 p0, float3 a, float3 b, float3 n)
        {
            this.p0 = p0;
            this.a = a;
            this.b = b;
            normal = normalize(n);
            
            a_len_squared = lengthsq(a);
            b_len_squared = lengthsq(b);

            area = length(a) * length(b);
            inv_area = 1.0f / area;
        }

        public void UpdateData(float3 pos, float3 right, float3 up, float3 forward, float3 scale, Material mat)
        {
            material = mat;
            
            var scaleX = scale.x;
            var scaleY = scale.y;

            a = scaleX * right;
            b = scaleY * up;
            p0 = pos - 0.5f * (a + b);
            normal = -forward;

            a_len_squared = lengthsq(a);
            b_len_squared = lengthsq(b);

            area = length(a) * length(b);
            inv_area = 1.0f / area;
        }

        public bool enableShadow { get; set; }

        public void SetSampler(Sampler samp)
        {
            sampler = samp;
        }
        
        public bool Hit(TraceRay ray, out float tMin, ref ShadeRec sr)
        {
            tMin = -1.0f;
            var t = dot((p0 - ray.o), normal) / dot(ray.d, normal);
            if (t <= KEpsilon)
                return false;
            
            float3 p = ray.o + t * ray.d;
            float3 d = p - p0;
            
            var ddota = dot(d, a);
            if (ddota < 0.0f || ddota > a_len_squared)
                return false;
            
            var ddotb = dot(d, b);
            if (ddotb < 0.0f || ddotb > b_len_squared)
                return false;
            
            sr.local_hit_point = p;
            sr.normal = normal;
            tMin = t;
            
            return true;
        }
        
        public bool ShadowHit(TraceRay ray, out float tMin)
        {
            tMin = -1.0f;
            if (!enableShadow)
                return false;

            var t = dot((p0 - ray.o), normal) / dot(ray.d, normal);
            if (t <= KEpsilon)
                return false;
            
            float3 p = ray.o + t * ray.d;
            float3 d = p - p0;
            
            var ddota = dot(d, a);
            if (ddota < 0.0f || ddota > a_len_squared)
                return false;
            
            var ddotb = dot(d, b);
            if (ddotb < 0.0f || ddotb > b_len_squared)
                return false;
            
            tMin = t;

            return true;
        }

        public float Pdf(ShadeRec sr)
        {
            return inv_area;
        }

        public float3 Sample()
        {
            float2 samplePoint = sampler.SampleUnitSquare();
            return p0 + samplePoint.x * a + samplePoint.y * b;
        }

        public float3 GetNormal()
        {
            return normal;
        }

        public BBox GetBoundingBox()
        {
            const float delta = 0.0001f;

            return(new BBox(min(p0.x, p0.x + a.x + b.x) - delta, max(p0.x, p0.x + a.x + b.x) + delta,
                min(p0.y, p0.y + a.y + b.y) - delta, max(p0.y, p0.y + a.y + b.y) + delta,
                min(p0.z, p0.z + a.z + b.z) - delta, max(p0.z, p0.z + a.z + b.z) + delta));
        }

        public Material GetMaterial()
        {
            return material;
        }

        private float3 p0;
        private float3 a;
        private float3 b;
        private float3 normal;
        private float a_len_squared;
        private float b_len_squared;
        private float area;
        private float inv_area;

        private Sampler sampler;
    }
}