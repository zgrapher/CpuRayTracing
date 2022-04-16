using Unity.Mathematics;
using static Unity.Mathematics.math;
using float3 = Unity.Mathematics.float3;

namespace RayTracer
{
    public class FresnelTransmitter : IBtdf
    {
        public float etaIn { set; get; }
        
        public float etaOut { set; get; }
        
        public float3 F(ShadeRec sr, float3 wo, float3 wi)
        {
            return float3.zero;
        }

        public float3 SampleF(ShadeRec sr, float3 wo, out float3 wt)
        {
            var n = sr.normal;
            var cosThetai = dot(n, wo);
            var eta = etaIn / etaOut;

            if (cosThetai < 0.0) {			// transmitted ray is outside
                cosThetai = -cosThetai;
                n = -n;  					// reverse direction of normal
                eta = 1.0f / eta; 			// invert ior
            }

            var temp = 1.0f - (1.0f - cosThetai * cosThetai) / (eta * eta);
            var cosTheta2 = sqrt(temp);
            wt = -wo / eta - (cosTheta2 - cosThetai / eta) * n;

            return (Fresnel(sr) / (eta * eta) / abs(dot(sr.normal, wt)));
        }

        public float3 Rho(ShadeRec sr, float3 wo)
        {
            return float3.zero;
        }

        public bool Tir(ShadeRec sr)
        {
            var wo = -sr.ray.d;
            var cosThetai = dot(sr.normal, wo);
            var eta = etaIn / etaOut;

            if (cosThetai < 0.0)
                eta = 1.0f / eta;

            return (1.0f - (1.0f - cosThetai * cosThetai) / (eta * eta) < 0.0f);
        }

        public float Fresnel(ShadeRec sr)
        {
            var normal = sr.normal;
            var ndotd = - dot(normal, sr.ray.d);
            float eta;

            if (ndotd < 0.0) {
                normal = -normal;
                eta = etaOut / etaIn;
            }
            else
                eta = etaIn / etaOut;

            var cosThetaI 		= -dot(normal,sr.ray.d);
            var temp 			= 1.0f - (1.0f - cosThetaI * cosThetaI) / (eta * eta);
            var cosThetaT 		= sqrt(1.0f - (1.0f - cosThetaI * cosThetaI) / (eta * eta));
            var rParallel 		= (eta * cosThetaI - cosThetaT) / (eta * cosThetaI + cosThetaT);
            var rPerpendicular 	= (cosThetaI - eta * cosThetaT) / (cosThetaI + eta * cosThetaT);
            var kr 				= 0.5f * (rParallel * rParallel + rPerpendicular * rPerpendicular);

            return (1.0f - kr);
        }
    }
}