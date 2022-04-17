using Unity.Mathematics;
using UnityEngine;

namespace RayTracer
{
    public class AreaLighting : Tracer
    {
        public AreaLighting(World world) : base(world)
        {
        }

        public override float3 TraceRay(TraceRay ray, int depth)
        {   
            if (depth > world.traceCamera.vp.maxDepth)
                return float3.zero;
            else
            {
                ShadeRec sr = world.HitObjects(ray);

                if (sr.hit_an_object)
                {
                    sr.depth = depth;
                    sr.ray = ray;
                    return sr.material.AreaLightShade(sr);
                }
                else
                {
                    return  (Vector3)(Vector4)world.backGroundColor;
                }
            }
        }

        public override float3 TraceRay(TraceRay ray, ref float tmin, int depth)
        {
            if (depth > world.traceCamera.vp.maxDepth)
                return (Vector3)(Vector4)world.backGroundColor;
            else
            {
                ShadeRec sr = world.HitObjects(ray);

                if (sr.hit_an_object)
                {
                    sr.depth = depth;
                    sr.ray = ray;
                    tmin = sr.t;
                    return sr.material.AreaLightShade(sr);
                }
                else
                {
                    return  (Vector3)(Vector4)world.backGroundColor;
                }
            }
        }
    }
}