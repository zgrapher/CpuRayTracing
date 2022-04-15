using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace RayTracer
{
    public interface IGeometricObject
    {
        public Material GetMaterial();
        
        public bool Hit(TraceRay ray, out float tMin, ref ShadeRec sr);

        public bool ShadowHit(TraceRay ray, out float tMin);

        public float Pdf(ShadeRec sr);

        public float3 Sample();

        public float3 GetNormal();

        public void SetSampler(Sampler samp);

        public BBox GetBoundingBox();
    }
    
    public abstract class GeometricObject : MonoBehaviour, IGeometricObject
    {
        public abstract bool Hit(TraceRay ray, out float tMin, ref ShadeRec sr);

        public virtual bool ShadowHit(TraceRay ray, out float tMin)
        {
            tMin = 0.0f;
            return false;
        }

        public virtual float Pdf(ShadeRec sr)
        {
            return 1.0f;
        }

        public virtual float3 Sample()
        {
            return float3.zero;
        }

        public virtual float3 GetNormal()
        {
            return float3.zero;
        }

        public virtual void SetSampler(Sampler samp)
        {
        }

        public virtual BBox GetBoundingBox()
        {
            return new BBox();
        }

        public abstract Material GetMaterial();
    }
}