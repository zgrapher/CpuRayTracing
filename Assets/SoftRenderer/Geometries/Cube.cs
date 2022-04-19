using Unity.Mathematics;
using UnityEngine;

namespace RayTracer
{
    public class Cube : IGeometricObject
    {
        private Material material;

        public void UpdateData(float3 pos, float3 right, float3 up, float3 forward, float3 scale, Material mat)
        {
            material = mat;
            
            up = up * scale.y;
            right = right * scale.x;
            forward = forward * scale.z;
            
            float3 p1 = pos + (up - right - forward) * 0.5f; 
            float3 p2 = pos - (up - right - forward) * 0.5f;
            
            comp.ClearObjects();
            var t = new Rect();
            t.SetData(p1, right, forward, up);
            t.enableShadow = enableShadow;
            t.material = mat;
            comp.AddObject(t);

            var b = new Rect();
            b.SetData(p2, -right, -forward, -up);
            b.enableShadow = enableShadow;
            b.material = mat;
            comp.AddObject(b);

            var f = new Rect();
            f.SetData(p1, -up, right, -forward);
            f.enableShadow = enableShadow;
            f.material = mat;
            comp.AddObject(f);

            var back = new Rect();
            back.SetData(p2, up, -right, forward);
            back.enableShadow = enableShadow;
            back.material = mat;
            comp.AddObject(back);

            var r = new Rect();
            r.SetData(p2, -forward, up, right);
            r.enableShadow = enableShadow;
            r.material = mat;
            comp.AddObject(r);

            var l = new Rect();
            l.SetData(p1, forward, -up, -right);
            l.enableShadow = enableShadow;
            l.material = mat;
            comp.AddObject(l);
            
            float3 pb0 = comp.FindMinBounds();
            float3 pb1 = comp.FindMaxBounds();

            bbox.x0 = pb0.x;
            bbox.y0 = pb0.y;
            bbox.z0 = pb0.z;
            bbox.x1 = pb1.x;
            bbox.y1 = pb1.y;
            bbox.z1 = pb1.z;
        }

        public bool enableShadow { get; set; }

        public Material GetMaterial()
        {
            return material;
        }

        public void SetSampler(Sampler samp)
        {
            throw new System.NotImplementedException();
        }

        public BBox GetBoundingBox()
        {
            return bbox;
        }

        public bool Hit(TraceRay ray, out float tMin, ref ShadeRec sr)
        {
            return comp.Hit(ray, out tMin, ref sr);
        }

        public bool ShadowHit(TraceRay ray, out float tMin)
        {
            return comp.ShadowHit(ray, out tMin);
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

        private readonly CompoundObject comp = new CompoundObject();

        private BBox bbox;
    }
}