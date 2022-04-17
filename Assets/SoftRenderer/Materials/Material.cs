using Unity.Mathematics;
using UnityEngine;
using UnityEngine.PlayerLoop;

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

        public abstract void Init(int sampleCount);
    }
}