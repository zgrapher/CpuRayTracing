using Unity.Mathematics;
using UnityEditor;
using UnityEngine;

namespace RayTracer
{
    public abstract class LightBase : MonoBehaviour
    {
        [SerializeField]
        public bool castShadow = false;
        
        public abstract float3 L(ShadeRec sr);

        public abstract float3 GetDirection(ShadeRec sr);

        public virtual bool InShadow(TraceRay ray, ShadeRec sr)
        {
            return false;
        }

        public virtual float G(ShadeRec sr)
        {
            return 1.0f;
        }

        public virtual float Pdf(ShadeRec sr)
        {
            return 1.0f;
        }
    }
}