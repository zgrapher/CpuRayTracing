using Unity.Mathematics;
using static Unity.Mathematics.math;
using UnityEngine;
using float3 = Unity.Mathematics.float3;

namespace RayTracer
{
    [CreateAssetMenu(fileName = "PhoneMat", menuName = "RayTracingMaterial/Phone")]
    public class Phone : Material
    {
        [SerializeField] private Color cd;

        [SerializeField] protected Color cs;

        [SerializeField] private float ka;

        [SerializeField] private float kd;
        
        [SerializeField] protected float ks;
        
        [SerializeField] protected float Exp;

        public override void Init(int sampleCount)
        {
            ambient_brdf.SetKa(ka);
            ambient_brdf.SetCd(float3(cd.r, cd.g, cd.b));
            
            diffuse_brdf.SetKd(kd);
            diffuse_brdf.SetCd(float3(cd.r, cd.g, cd.b));
            
            specular_brdf.SetKs(ks);
            specular_brdf.SetExp(Exp);
        }

        public override float3 Shade(ShadeRec sr)
        {
            float3 wo = -sr.ray.d;
            float3 radiance = ambient_brdf.rho(sr, wo) * sr.w.ambient.L(sr);

            foreach (LightBase light in sr.w.lights)
            {
                float3 wi = light.GetDirection(sr);
                var ndotwi = dot(sr.normal, wi);

                if (ndotwi <= 0.0f)
                    continue;

                var inShadow = false;
                if (light.castShadow)
                {
                    var shadowRay = new TraceRay(sr.hit_point, wi);
                    if (light.InShadow(shadowRay, sr))
                        inShadow = true;
                }
                
                if (inShadow)
                    continue;
                
                radiance += (diffuse_brdf.f(sr, wo, wi) + specular_brdf.f(sr, wo, wi)) * light.L(sr) * ndotwi;
            }

            return radiance;
        }

        public override float3 AreaLightShade(ShadeRec sr)
        {
            float3 wo = -sr.ray.d;
            float3 radiance = float3.zero;// zjtest = ambient_brdf.rho(sr, wo) * sr.w.ambient.L(sr);

            foreach (LightBase light in sr.w.lights)
            {
                float3 wi = light.GetDirection(sr);
                var ndotwi = dot(sr.normal, wi);

                if (ndotwi <= 0.0f)
                    continue;

                var inShadow = false;
                if (light.castShadow)
                {
                    var shadowRay = new TraceRay(sr.hit_point, wi);
                    inShadow = light.InShadow(shadowRay, sr);
                }
                if (inShadow)
                    continue;
                
                radiance += (diffuse_brdf.f(sr, wo, wi) + specular_brdf.f(sr, wo, wi)) * light.L(sr) * light.G(sr) * ndotwi / light.Pdf(sr);
            }

            return radiance;
        }

        private Lambert ambient_brdf = new Lambert();
        private Lambert diffuse_brdf = new Lambert();
        private GlossySpecular specular_brdf = new GlossySpecular();
    }
}