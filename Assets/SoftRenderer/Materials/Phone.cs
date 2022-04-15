using Unity.Mathematics;
using static Unity.Mathematics.math;
using UnityEngine;

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

        protected void OnEnable()
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
            var wo = -sr.ray.d;
            var radiance = ambient_brdf.rho(sr, wo) * sr.w.ambient.L(sr);

            foreach (var light in sr.w.lights)
            {
                var wi = light.GetDirection(sr);
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
            var wo = -sr.ray.d;
            var radiance = ambient_brdf.rho(sr, wo) * sr.w.ambient.L(sr);

            foreach (var light in sr.w.lights)
            {
                var wi = light.GetDirection(sr);
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