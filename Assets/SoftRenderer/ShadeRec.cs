using Unity.Mathematics;
using UnityEngine;

namespace RayTracer
{
    public struct ShadeRec
    {
        public bool hit_an_object;

        public float3 hit_point;
        
        public float3 local_hit_point;

        public float3 normal;

        public Color color;

        public TraceRay ray;

        public int depth;

        public float t;
        
        public World w;

        public Material material;
        
        public ShadeRec(World wd)
        {
            w = wd;
            hit_an_object = false;
            hit_point = default;
            local_hit_point = default;
            normal = default;
            color = default;
            ray = new TraceRay();
            material = null;
            depth = 0;
            t = 0;
        }
    }
}