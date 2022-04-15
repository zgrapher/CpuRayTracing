using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace RayTracer
{
    public class CompoundObject : IGeometricObject
    {
        protected Material material;
        
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

            foreach (var obj in objects)
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
    }
}