using Unity.Mathematics;
using UnityEngine;

namespace RayTracer
{
    public abstract class Material : ScriptableObject
    {
        public virtual float3 Shade(ShadeRec sr)
        {
            return float3.zero;
        }

        public virtual float3 AreaLightShade(ShadeRec sr)
        {
            return float3.zero;
        }

        public virtual float3 GetLe(ShadeRec sr)
        {
            return float3.zero;
        }

        public virtual void SetSamples(Sampler samp)
        {
        }
    }
}