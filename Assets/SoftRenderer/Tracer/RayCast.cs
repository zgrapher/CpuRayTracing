using UnityEngine;
using float3 = Unity.Mathematics.float3;

namespace RayTracer
{
    public class RayCast : Tracer
    {
        public RayCast(World world) : base(world)
        {
        }

        public override float3 TraceRay(TraceRay ray, int depth)
        {
            ShadeRec sr = world.HitObjects(ray);

            if (sr.hit_an_object)
            {
                sr.ray = ray;
                if (sr.material)
                {
                    float3 c = sr.material.Shade(sr);
                    return new float3(c.x, c.y, c.z);
                }
                else
                {
                    return float3.zero;
                }
            }
            else
            {
                return  (Vector3)(Vector4)world.backGroundColor;
            }
        }
    }
}