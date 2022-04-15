using Unity.Mathematics;
using static Unity.Mathematics.math;
using UnityEngine;

namespace RayTracer
{
    public class Directional : LightBase
    {
        [SerializeField] private float ls = 1.0f;

        private void Start()
        {
            var color = GetComponent<Light>().color;
            
            c = float3(color.r, color.g, color.b);

            dir = -transform.forward;
        }

        public override float3 L(ShadeRec sr)
        {
            return ls * c;
        }

        public override float3 GetDirection(ShadeRec sr)
        {
            return dir;
        }

        private float3 c;
        private float3 dir = up();
    }
}