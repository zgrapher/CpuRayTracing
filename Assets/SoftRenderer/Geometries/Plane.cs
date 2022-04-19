using static Unity.Mathematics.math;
using float3 = Unity.Mathematics.float3;

namespace RayTracer
{
    public class Plane : IGeometricObject
    {
        private const float KEpsilon = 0.001f;

        private Material material;

        public void Init()
        {
        }

        public BBox GetBoundingBox()
        {
            throw new System.NotImplementedException();
        }

        public void UpdateData(float3 pos, float3 right, float3 up, float3 forward, float3 scale, Material mat)
        {
            material = mat;
            
            point = pos;
            normal = up;
        }

        public bool enableShadow { get; set; }

        public bool Hit(TraceRay ray, out float tMin, ref ShadeRec sr)
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

        public bool ShadowHit(TraceRay ray, out float tMin)
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

        public Material GetMaterial()
        {
            return material;
        }

        private float3 point;
        private float3 normal;
    }
}