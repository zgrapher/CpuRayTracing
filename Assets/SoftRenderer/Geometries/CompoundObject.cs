using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace RayTracer
{
    public class CompoundObject : IGeometricObject
    {
        protected const float KEpsilon = 0.0001f;
        
        protected Material material;

        public virtual void Init()
        {
        }

        public void Clear()
        {
            objects.Clear();
        }

        public Material GetMaterial()
        {
            return material;
        }

        public void SetSampler(Sampler samp)
        {
            throw new System.NotImplementedException();
        }

        public void AddObject(IGeometricObject obj)
        {
            objects.Add(obj);
        }

        public BBox GetBoundingBox()
        {
            throw new System.NotImplementedException();
        }

        public virtual bool Hit(TraceRay ray, out float tMin, ref ShadeRec sr)
        {
            float3 normal = default;
            float3 localHitPoint = default;
            var hit = false;
            tMin = float.MaxValue;

            foreach (IGeometricObject obj in objects)
            {
                if (obj.Hit(ray, out var t, ref sr) && t < tMin)
                {
                    hit = true;
                    tMin = t;
                    material = obj.GetMaterial();
                    normal = sr.normal;
                    localHitPoint = sr.local_hit_point;
                }
            }

            if (!hit) return false;
            
            sr.t = tMin;
            sr.normal = normal;
            sr.local_hit_point = localHitPoint;

            return true;
        }

        public virtual bool ShadowHit(TraceRay ray, out float tMin)
        {
            var hit = false;
            tMin = float.MaxValue;

            foreach (var obj in objects)
            {
                if (obj.ShadowHit(ray, out var t) && t < tMin)
                {
                    hit = true;
                    tMin = t;
                }
            }

            return hit;
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

        protected List<IGeometricObject> objects = new List<IGeometricObject>();

        public float3 FindMinBounds()
        {
            float3 p0 = math.float3(float.MaxValue);

            foreach (IGeometricObject obj in objects)
            {
                BBox objectBox = obj.GetBoundingBox();

                if (objectBox.x0 < p0.x)
                    p0.x = objectBox.x0;
                if (objectBox.y0 < p0.y)
                    p0.y = objectBox.y0;
                if (objectBox.z0 < p0.z)
                    p0.z = objectBox.z0;
            }

            p0.x -= KEpsilon; p0.y -= KEpsilon; p0.z -= KEpsilon;

            return p0;
        }

        public float3 FindMaxBounds()
        {
            float3 p1 = math.float3(-float.MaxValue);

            foreach (IGeometricObject obj in objects)
            {
                BBox objectBox = obj.GetBoundingBox();

                if (objectBox.x1 > p1.x)
                    p1.x = objectBox.x1;
                if (objectBox.y1 > p1.y)
                    p1.y = objectBox.y1;
                if (objectBox.z1 > p1.z)
                    p1.z = objectBox.z1;
            }

            p1.x += KEpsilon; p1.y += KEpsilon; p1.z += KEpsilon;

            return p1;
        }
    }
}