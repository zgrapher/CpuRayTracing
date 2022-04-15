using UnityEngine;
using static Unity.Mathematics.math;
using float3 = Unity.Mathematics.float3;

namespace RayTracer
{
    public class Rect : GeometricObject
    {
        private const float KEpsilon = 0.001f;

        [SerializeField] private Material material;
        
        private void Start()
        {
            var trans = transform;
            var p = (float3)trans.position;
            var lossyScale = trans.lossyScale;
            var scaleX = lossyScale.x;
            var scaleY = lossyScale.y;

            a = scaleX * trans.right;
            b = scaleY * trans.up;
            p0 = p - 0.5f * (a + b);
            normal = -trans.forward;

            a_len_squared = lengthsq(a);
            b_len_squared = lengthsq(b);

            area = length(a) * length(b);
            inv_area = 1.0f / area;
        }

        public override void SetSampler(Sampler samp)
        {
            sampler = samp;
        }
        
        public override bool Hit(TraceRay ray, out float tMin, ref ShadeRec sr)
        {
            tMin = -1.0f;
            var t = dot((p0 - ray.o), normal) / dot(ray.d, normal);
            if (t <= KEpsilon)
                return false;
            
            var p = ray.o + t * ray.d;
            var d = p - p0;
            
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
        
        public override bool ShadowHit(TraceRay ray, out float tMin)
        {
            tMin = -1.0f;
            if (!enableShadow)
                return false;

            var t = dot((p0 - ray.o), normal) / dot(ray.d, normal);
            if (t <= KEpsilon)
                return false;
            
            var p = ray.o + t * ray.d;
            var d = p - p0;
            
            var ddota = dot(d, a);
            if (ddota < 0.0f || ddota > a_len_squared)
                return false;
            
            var ddotb = dot(d, b);
            if (ddotb < 0.0f || ddotb > b_len_squared)
                return false;
            
            tMin = t;

            return true;
        }

        public override float Pdf(ShadeRec sr)
        {
            return inv_area;
        }

        public override float3 Sample()
        {
            var samplePoint = sampler.SampleUnitSquare();
            return p0 + samplePoint.x * a + samplePoint.y * b;
        }

        public override float3 GetNormal()
        {
            return normal;
        }

        public override BBox GetBoundingBox()
        {
            const float delta = 0.0001f;

            return(new BBox(min(p0.x, p0.x + a.x + b.x) - delta, max(p0.x, p0.x + a.x + b.x) + delta,
                min(p0.y, p0.y + a.y + b.y) - delta, max(p0.y, p0.y + a.y + b.y) + delta,
                min(p0.z, p0.z + a.z + b.z) - delta, max(p0.z, p0.z + a.z + b.z) + delta));
        }

        public override Material GetMaterial()
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
        public bool enableShadow;
    }
}