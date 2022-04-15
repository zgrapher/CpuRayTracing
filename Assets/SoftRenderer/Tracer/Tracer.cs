using Unity.Mathematics;

namespace RayTracer
{
    public class Tracer
    {
        protected Tracer(World world)
        {
            this.world = world;
        }

        public virtual float3 TraceRay(TraceRay ray, int depth)
        {
            return float3.zero;
        }
        
        protected World world;
    }
}