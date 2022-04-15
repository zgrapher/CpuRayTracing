using Unity.Mathematics;

namespace RayTracer
{
    public class Lambert : Brdf
    {
        public override float3 f(ShadeRec sr, float3 wi, float3 wo)
        {
            return kd * cd * (1.0f / math.PI);
        }

        public override float3 rho(ShadeRec sr, float3 wo)
        {
            return kd * cd;
        }

        public void SetKa(float k)
        {
            kd = k;
        }

        public void SetKd(float k)
        {
            kd = k;
        }

        public void SetCd(float3 c)
        {
            cd = c;
        }
        
        private float kd;
        private float3 cd = float3.zero; 
            
    }
}