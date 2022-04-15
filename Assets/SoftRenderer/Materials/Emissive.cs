using Unity.Mathematics;
using static Unity.Mathematics.math;
using UnityEngine;
using float3 = Unity.Mathematics.float3;

namespace RayTracer
{
    [CreateAssetMenu(fileName = "Emmisive", menuName = "RayTracingMaterial/Emmisive")]
    public class Emissive : Material
    {
        [SerializeField] private float ls;

        [SerializeField] private Color color;

        private float3 ce;
        
        private void OnEnable()
        {
            ce = float3((Vector3)(Vector4) color);
        }

        public override float3 AreaLightShade(ShadeRec sr)
        {
            if (dot(sr.normal, -sr.ray.d) > 0.0f)
                return ls * ce;
            else
                return float3.zero;
        }

        public override float3 GetLe(ShadeRec sr)
        {
            return ls * ce;
        }
    }
}