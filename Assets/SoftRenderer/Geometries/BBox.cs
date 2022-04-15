using Unity.Mathematics;

namespace RayTracer
{
    public struct BBox
    {
        public float x0, x1, y0, y1, z0, z1;

        private const float KEpsilon = 0.0001f;

        public BBox(float _x0, float _x1, float _y0, float _y1, float _z0, float _z1)
        {
            x0 = _x0;
            x1 = _x1;
            y0 = _y0;
            y1 = _y1;
            z0 = _z0;
            z1 = _z1;
        }
        
        public BBox(float3 p0, float3 p1)
        {
            x0 = p0.x;
            x1 = p1.x;
            y0 = p0.y;
            y1 = p1.y;
            z0 = p0.z;
            z1 = p1.z;
        }

        public bool Hit(TraceRay ray)
        {
            var ox = ray.o.x; var oy = ray.o.y; var oz = ray.o.z;
            var dx = ray.d.x; var dy = ray.d.y; var dz = ray.d.z;
	
            float txMin, tyMin, tzMin;
            float txMax, tyMax, tzMax; 

            var a = 1.0f / dx;
            if (a >= 0) {
                txMin = (x0 - ox) * a;
                txMax = (x1 - ox) * a;
            }
            else {
                txMin = (x1 - ox) * a;
                txMax = (x0 - ox) * a;
            }
	
            var b = 1.0f / dy;
            if (b >= 0) {
                tyMin = (y0 - oy) * b;
                tyMax = (y1 - oy) * b;
            }
            else {
                tyMin = (y1 - oy) * b;
                tyMax = (y0 - oy) * b;
            }
	
            var c = 1.0f / dz;
            if (c >= 0) {
                tzMin = (z0 - oz) * c;
                tzMax = (z1 - oz) * c;
            }
            else {
                tzMin = (z1 - oz) * c;
                tzMax = (z0 - oz) * c;
            }

            var t0 = txMin > tyMin ? txMin : tyMin;
		
            if (tzMin > t0)
                t0 = tzMin;	
		
            var t1 = txMax < tyMax ? txMax : tyMax;
		
            if (tzMax < t1)
                t1 = tzMax;
		
            return (t0 < t1 && t1 > KEpsilon);
        }

        public bool Inside(float3 p)
        {
            return ((p.x > x0 && p.x < x1) && (p.y > y0 && p.y < y1) && (p.z > z0 && p.z < z1));
        }
    }
}