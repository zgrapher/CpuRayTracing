using Unity.Mathematics;
using UnityEngine;

namespace RayTracer
{
    public class Cube : GeometricObject
    {
        [SerializeField] private Material material;

        [SerializeField] private bool enableShadow;
        
        public override void Init()
        {
            Transform trans = transform;
            var p = (float3)trans.position;
            
            Vector3 scale = trans.lossyScale;
            
            float3 up = trans.up * scale.y;
            float3 right = trans.right * scale.x;
            float3 forward = trans.forward * scale.z;
            
            float3 p1 = p + (up - right - forward) * 0.5f; 
            float3 p2 = p - (up - right - forward) * 0.5f;
            
            comp.Clear();
            var t = gameObject.AddComponent<Rect>();
            t.SetData(p1, right, forward, up);
            t.enableShadow = enableShadow;
            comp.AddObject(t);

            var b = gameObject.AddComponent<Rect>();
            b.SetData(p2, -right, -forward, -up);
            b.enableShadow = enableShadow;
            comp.AddObject(b);

            var f = gameObject.AddComponent<Rect>();
            f.SetData(p1, -up, right, -forward);
            f.enableShadow = enableShadow;
            comp.AddObject(f);

            var back = gameObject.AddComponent<Rect>();
            back.SetData(p2, up, -right, forward);
            back.enableShadow = enableShadow;
            comp.AddObject(back);

            var r = gameObject.AddComponent<Rect>();
            r.SetData(p2, -forward, up, right);
            r.enableShadow = enableShadow;
            comp.AddObject(r);

            var l = gameObject.AddComponent<Rect>();
            l.SetData(p1, forward, -up, -right);
            l.enableShadow = enableShadow;
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

        public override Material GetMaterial()
        {
            return material;
        }

        public override BBox GetBoundingBox()
        {
            return bbox;
        }

        public override bool Hit(TraceRay ray, out float tMin, ref ShadeRec sr)
        {
            return comp.Hit(ray, out tMin, ref sr);
        }

        public override bool ShadowHit(TraceRay ray, out float tMin)
        {
            return comp.ShadowHit(ray, out tMin);
        }

        private readonly CompoundObject comp = new CompoundObject();

        private BBox bbox = new BBox();
    }
}