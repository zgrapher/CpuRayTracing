using System;
using UnityEngine;

namespace RayTracer
{
    public enum GeometryType
    {
        Unknown,
        Cube,
        Rect,
        Sphere,
        Plane
    }
    
    public class Geomtric : MonoBehaviour
    {
        public GeometryType geometryType;

        public Material material;

        public bool enableShadow;

        private void OnValidate()
        {
            if (geometryType == GeometryType.Plane)
                return;
            
            if (GetComponent<BoxCollider>() != null)
            {
                geometryType = GeometryType.Cube;
            }
            else if (GetComponent<SphereCollider>() != null)
            {
                geometryType = GeometryType.Sphere;
            }
            else if (GetComponent<MeshCollider>() != null)
            {
                geometryType = GeometryType.Rect;
            }
        }

        private void Update()
        {
            Transform trans = transform;
            
            if (geometryType != prev_type)
            {
                geometric = null;
                if (geometryType == GeometryType.Plane)
                {
                    geometric = new Plane();
                }
                else if (GetComponent<BoxCollider>() != null)
                {
                    geometric = new Cube();
                    geometryType = GeometryType.Cube;
                }
                else if (GetComponent<SphereCollider>() != null)
                {
                    geometric = new Sphere();
                    geometryType = GeometryType.Sphere;
                }
                else if (GetComponent<MeshCollider>() != null)
                {
                    geometric = new Rect();
                    geometryType = GeometryType.Rect;
                }

                prev_type = geometryType;
            }
            
            if (geometric == null) 
                return;
            
            geometric.UpdateData(trans.position, trans.right, trans.up, trans.forward, trans.lossyScale, material);
            geometric.enableShadow = enableShadow;
        }
        
        public IGeometricObject geometric { private set; get; }
        private GeometryType prev_type;
    }
}