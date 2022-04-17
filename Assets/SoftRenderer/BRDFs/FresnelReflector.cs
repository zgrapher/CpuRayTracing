using Unity.Mathematics;
using static Unity.Mathematics.math;
using float3 = Unity.Mathematics.float3;

namespace RayTracer
{
    public class FresnelReflector : Brdf
    {
        public float etaIn { set; get; }
        
        public float etaOut { set; get; }
        
        public override float3 f(ShadeRec sr, float3 wi, float3 wo)
        {
            return float3.zero;
        }

        public override float3 rho(ShadeRec sr, float3 wo)
        {
            return float3.zero;
        }

        public override float3 SampleF(ShadeRec sr, float3 wo, out float3 wi)
        {
            wi = reflect(-wo, sr.normal);
            return (Fresnel(sr) / abs(dot(sr.normal, wi)));
        }

        private float Fresnel(ShadeRec sr)
        {
            float3 normal = sr.normal;
            var ndotd = -dot(normal, sr.ray.d);
            float eta;

            if (ndotd < 0.0f) {
                normal = -normal;
                eta = etaOut / etaIn;
            }
            else
                eta = etaIn / etaOut;

            var cosThetaI 		= -dot(normal, sr.ray.d);
            var cosThetaT 		= sqrt (1.0f - (1.0f - cosThetaI * cosThetaI) / (eta * eta));
            var rParallel 		= (eta * cosThetaI - cosThetaT) / (eta * cosThetaI + cosThetaT);
            var rPerpendicular 	= (cosThetaI - eta * cosThetaT) / (cosThetaI + eta * cosThetaT);
            var kr 				= 0.5f * (rParallel * rParallel + rPerpendicular * rPerpendicular);

            return kr;
        }
    }
}