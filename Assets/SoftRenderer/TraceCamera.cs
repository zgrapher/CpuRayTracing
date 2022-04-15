using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Unity.Mathematics;
using static Unity.Mathematics.math;
using UnityEngine;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;
using float3 = Unity.Mathematics.float3;

namespace RayTracer
{
    public class TraceCamera : MonoBehaviour
    {
        [SerializeField] 
        public RawImage rawImage;
        
        [SerializeField]
        public ViewPlane vp = new ViewPlane();
        
        [SerializeField]
        public int sampleSize = 1;
        
        [SerializeField]
        public float viewDistance;
        public int sampleCount => sampleSize * sampleSize;
        public World world { get; set; }

        private void Start()
        {
            var trans = transform;
            eye = trans.position;
            look_at = eye + (float3)trans.forward * viewDistance;

            var halfHeight = atan(radians(GetComponent<Camera>().fieldOfView * 0.5f));
            vp.pixelSize = halfHeight / (vp.vRes * 0.5f);

            ComputeUVW();
        }

        private void ComputeUVW()
        {
            var trans = transform;
            w = trans.forward;
            u = trans.right;
            v = trans.up;
        }

        public void SetExposureTime(float t)
        {
            exposure_time = t;
        }
        
        private void OnDrawGizmos()
        {

        }

        public void RenderScene()
        {
            var pixels = new float3[vp.hRes, vp.vRes];
            
            var depth = 0;
            var ray = new TraceRay()
            {
                o = eye
            };

            var sw = new Stopwatch();
            sw.Start();
            
            world.Build();
            
            for (var r = 0; r < vp.vRes; r++)
            {
                for (var c = 0; c < vp.hRes; c++)
                {
                    RenderPixel(pixels, c, r, depth, ray);
                }
            }
            Debug.Log(sw.ElapsedMilliseconds/1000.0f);
            
            var device = new Outputter();
            device.Create(vp.hRes, vp.vRes);
            for (var r = 0; r < vp.vRes; r++)
            {
                for (var c = 0; c < vp.hRes; c++)
                {
                    world.DisplayPixel(device, c, r, pixels[c, r]);
                }
            }
            device.Commit();
            rawImage.texture = device.GetResult();
        }

        private void RenderPixel(float3[,] pixels, int c, int r, int depth, TraceRay ray)
        {
            var radiance = float3.zero;
           
            for (var p = 0; p < sampleCount; p++)
            {
                var sp = vp.sampler.SampleUnitSquare();
                
                var x = vp.pixelSize * (c - 0.5f * vp.hRes + sp.x);
                var y = vp.pixelSize * (r - 0.5f * vp.vRes + sp.y);
                
                ray.d = RayDirection(x, y);
                radiance += world.tracer.TraceRay(ray, depth);
            }

            radiance /= sampleCount;
            radiance *= exposure_time;

            pixels[c, r] = radiance;
        }

        private float3 RayDirection(float x, float y)
        {
            var dir = x * u + y * v + w;
            dir = normalize(dir);
            return dir;
        }

        private float3 eye = new float3(0, 0, -500);
        private float3 look_at = new float3(0, 0, 0);
        private float3 up = new float3(0, 1, 0);
        private float3 u, v, w;
        private float exposure_time = 1.0f;
    }
}
