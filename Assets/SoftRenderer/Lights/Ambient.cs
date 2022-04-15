using Unity.Mathematics;
using static Unity.Mathematics.math;
using UnityEngine;

namespace RayTracer
{
    public class Ambient : LightBase
    {
        [SerializeField] private Color ambientColor;

        [SerializeField] private float ls;
        
        private void Start()
        {
            color = float3(ambientColor.r, ambientColor.g, ambientColor.b);
        }

        public override float3 L(ShadeRec sr)
        {
            return ls * color;
        }

        public override float3 GetDirection(ShadeRec sr)
        {
            return new float3(0);
        }

        private float3 color = new float3(1, 1, 1);
    }
}