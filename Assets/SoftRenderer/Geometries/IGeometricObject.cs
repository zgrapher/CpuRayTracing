using System;
using Unity.Mathematics;
using UnityEngine;

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

        public void UpdateData(float3 pos, float3 right, float3 up, float3 forward, float3 scale, Material mat);
        
        public bool enableShadow { set; get; }
    }
    
}