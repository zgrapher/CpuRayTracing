using System;
using System.Diagnostics;
using Unity.Mathematics;
using UnityEngine;
using static Unity.Mathematics.math;

namespace RayTracer
{
    [CreateAssetMenu(fileName = "MatteMat", menuName = "RayTracingMaterial/Matte")]
    public class Matte : Material
    {
        [SerializeField] private Color cd;

        [SerializeField] private float ka;

        [SerializeField] private float kd;

        private void OnEnable()
        {
            ambient_brdf.SetKa(ka);
            ambient_brdf.SetCd(float3(cd.r, cd.g, cd.b));
            
            diffuse_brdf.SetKd(kd);
            diffuse_brdf.SetCd(float3(cd.r, cd.g, cd.b));
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
                    var shadowRay = new TraceRay()
                    {
                        o = sr.hit_point,
                        d = wi
                    };
                    inShadow = light.InShadow(shadowRay, sr);
                }
                
                if (inShadow)
                    continue;

                radiance += diffuse_brdf.f(sr, wo, wi) * light.L(sr) * ndotwi;
            }

            return radiance;
        }

        private Lambert ambient_brdf = new Lambert();
        private Lambert diffuse_brdf = new Lambert();
    }
}