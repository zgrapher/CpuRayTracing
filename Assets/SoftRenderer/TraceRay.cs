using Unity.Mathematics;

namespace RayTracer
{
    public struct TraceRay
    {
        public float3 o;
        public float3 d;
        public TraceRay(float3 origin, float3 dir)
        {
            o = origin;
            d = dir;
        }
    }
}