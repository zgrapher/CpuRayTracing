using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using static Unity.Mathematics.math;

namespace RayTracer
{
    public class World : MonoBehaviour
    {
        [SerializeField] 
        public Color backGroundColor;
        public Tracer tracer { get; private set; }

        public TraceCamera traceCamera;

        public LightBase ambient;

        public List<IGeometricObject> objects = new List<IGeometricObject>();

        public List<LightBase> lights = new List<LightBase>();

        public bool useGrid;

        private Grid grid = new Grid();
        
        private void Start()
        {
            traceCamera.world = this;
        }

        public void Build()
        {
            objects.Clear();
            lights.Clear();
            grid.Clear();

            tracer = new AreaLighting(this);
            traceCamera.vp.SetSampler(new MultiJittered(traceCamera.sampleCount));

            (ambient as AmbientOccluder)?.SetSampler(new MultiJittered(traceCamera.sampleCount));
            
            foreach (Geomtric geo in transform.GetComponentsInChildren<Geomtric>())
            {
                geo.geometric.GetMaterial().Init(traceCamera.sampleCount);
                AddObject(geo.geometric);
            }
            
            if (useGrid)
            {
                grid.SetupCells();
                objects.Add(grid);
            }

            foreach (var lt in transform.GetComponentsInChildren<LightBase>())
            {
                switch (lt)
                {
                    case Ambient _:
                    case AmbientOccluder _:
                        continue;
                    case AreaLight area:
                        area.GetComponent<Geomtric>().geometric.SetSampler(new MultiJittered(traceCamera.sampleCount));
                        break;
                }

                AddLight(lt);
            }
        }

        private void AddObject(IGeometricObject obj)
        {
            if (!useGrid || obj is Plane)
                objects.Add(obj);
            else
                grid.AddObject(obj);
        }

        private void AddLight(LightBase lt)
        {
            lights.Add(lt);
        }

        public ShadeRec HitObjects(TraceRay ray)
        {
            var sr = new ShadeRec(this);
            float3 normal = default;
            float3 localHitPoint = default;
            var tmin = float.MaxValue;

            foreach (var obj in objects)
            {
                if (obj.Hit(ray, out var t, ref sr) && t < tmin)
                {
                    sr.hit_an_object = true;
                    tmin = t;
                    sr.material = obj.GetMaterial();
                    sr.hit_point = ray.o + t * ray.d;
                    normal = sr.normal;
                    localHitPoint = sr.local_hit_point;
                }
            }
            
            if (sr.hit_an_object)
            {
                sr.t = tmin;
                sr.normal = normal;
                sr.local_hit_point = localHitPoint;
            }

            return sr;
        }

        public void DisplayPixel(Outputter outputter, int c, int r, float3 color)
        {
            var vp = traceCamera.vp;
            if (abs(vp.gamma - 1.0f) > float.Epsilon)
            {
                color.x = pow(color.x, vp.invGamma);
                color.y = pow(color.y, vp.invGamma);
                color.z = pow(color.z, vp.invGamma);
            }

            outputter.SetPixel(c, r, color);
        }
    }
}