using Unity.Mathematics;
using static Unity.Mathematics.math;
using UnityEngine;
using float3 = Unity.Mathematics.float3;

namespace RayTracer
{
    [CreateAssetMenu(fileName = "Dielectric", menuName = "RayTracingMaterial/Dielectric")]
    public class Dielectric : Phone
    {
        [SerializeField] private float3 cfIn;

        [SerializeField] private float3 cfOut;

        [SerializeField] private float etaIn;

        [SerializeField] private float etaOut;

        public override void Init(int sampleCount)
        {
            base.Init(sampleCount);

            fresnel_brdf.etaIn = etaIn;
            fresnel_btdf.etaIn = etaIn;
            fresnel_brdf.etaOut = etaOut;
            fresnel_btdf.etaOut = etaOut;
        }

        private float3 LocalShade(ShadeRec sr)
        {
            float3 radiance = float3.zero;
            
            float3 wo = -sr.ray.d;
            float3 fr = fresnel_brdf.SampleF(sr, wo, out float3 wi);
            var reflectedRay = new TraceRay(sr.hit_point, wi);
            var t = -1.0f;
            float3 radianceR, radianceT;
            var ndotwi = dot(sr.normal, wi);
            
            if (fresnel_btdf.TotalInterReflect(sr))
            {
                if (ndotwi < 0.0f)
                {
                    // 内部
                    radianceR = sr.w.tracer.TraceRay(reflectedRay, ref t, sr.depth + 1);
                    // zjtest radiance += pow(cfIn, t) * radianceR;
                }
                else
                {
                    // 外部
                    radianceR = sr.w.tracer.TraceRay(reflectedRay, ref t, sr.depth + 1);
                    // zjtest radiance += pow(cfOut, t) * radianceR;
                }
            }
            else
            {
                float3 ft = fresnel_btdf.SampleF(sr, wo, out float3 wt);
                var transmittedRay = new TraceRay(sr.hit_point, wt);
                var ndotwt = dot(sr.normal, wt);

                if (ndotwi < 0.0f)
                {
                    // 反射光线在里面
                    radianceR = fr * sr.w.tracer.TraceRay(reflectedRay, ref t, sr.depth + 1) * abs(ndotwi);
                    // zjtest radiance += pow(cfIn, t) * radianceR;
                    
                    // 折射光线在外面
                    radianceT = ft * sr.w.tracer.TraceRay(transmittedRay, sr.depth + 1) * abs(ndotwt);
                    radiance += pow(cfOut, t) * radianceT;
                }
                else
                {
                    // 反射光线在外面
                    radianceR = fr * sr.w.tracer.TraceRay(reflectedRay, ref t, sr.depth + 1) * abs(ndotwi);
                    // zjtest radiance += pow(cfOut, t) * radianceR;
                    
                    // 折射光线在里面
                    radianceT = ft * sr.w.tracer.TraceRay(transmittedRay, sr.depth + 1) * abs(ndotwt);
                    radiance += pow(cfIn, t) * radianceT;
                }
            }

            return radiance;
        }
        
        public override float3 Shade(ShadeRec sr)
        {
            float3 radiance = base.Shade(sr);
            radiance += LocalShade(sr);
            return radiance;
        }

        public override float3 AreaLightShade(ShadeRec sr)
        {
            //float3 radiance = base.AreaLightShade(sr);
            // zjtest radiance += LocalShade(sr);
            var radiance = LocalShade(sr);
            return radiance;
        }

        private FresnelReflector fresnel_brdf = new FresnelReflector();
        private FresnelTransmitter fresnel_btdf = new FresnelTransmitter();
    }
}