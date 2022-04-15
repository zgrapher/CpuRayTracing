using System.Collections.Generic;
using Unity.Mathematics;
using static Unity.Mathematics.math;

namespace RayTracer
{
    public class Grid : CompoundObject
    {
        private const float KEpsilon = 0.0001f;
        
        public void SetupCells()
        {
	        var p0 = FindMinBounds();
	        var p1 = FindMaxBounds();

	        bbox.x0 = p0.x;
	        bbox.y0 = p0.y;
	        bbox.z0 = p0.z;
	        bbox.x1 = p1.x;
	        bbox.y1 = p1.y;
	        bbox.z1 = p1.z;

	        // x y z三个边的长度
	        var wx = p1.x - p0.x;
	        var wy = p1.y - p0.y;
	        var wz = p1.z - p0.z;

	        // 计算三个轴向的切分数量，multiplier用于缩放切分密度
	        const float multiplier = 2.0f;
	        var s = pow(wx * wy * wz / objects.Count, 0.3333333);
	        nx = (int) (multiplier * wx / s + 1);
	        ny = (int) (multiplier * wy / s + 1);
	        nz = (int) (multiplier * wz / s + 1);


	        // 初始化Cells
	        var numCells = nx * ny * nz;
	        cells.Capacity = objects.Count;
	        for (var j = 0; j < numCells; j++)
		        cells.Add(null);

	        var counts = new List<int>
	        {
		        Capacity = numCells
	        };
	        for (var j = 0; j < numCells; j++)
		        counts.Add(0);

	        // 把父类Compounds中objects里的物体添加到格子里
	        foreach (var obj in objects)
	        {
		        var objBBox = obj.GetBoundingBox();

		        // 计算物体包围盒覆盖的三个轴向的各自范围

		        var ixmin = clamp((int)((objBBox.x0 - p0.x) * nx / (p1.x - p0.x)), 0, nx - 1);
		        var iymin = clamp((int)((objBBox.y0 - p0.y) * ny / (p1.y - p0.y)), 0, ny - 1);
		        var izmin = clamp((int)((objBBox.z0 - p0.z) * nz / (p1.z - p0.z)), 0, nz - 1);
		        var ixmax = clamp((int)((objBBox.x1 - p0.x) * nx / (p1.x - p0.x)), 0, nx - 1);
		        var iymax = clamp((int)((objBBox.y1 - p0.y) * ny / (p1.y - p0.y)), 0, ny - 1);
		        var izmax = clamp((int)((objBBox.z1 - p0.z) * nz / (p1.z - p0.z)), 0, nz - 1);

		        // 向格子里添加物体

		        for (var iz = izmin; iz <= izmax; iz++) 
		        for (var iy = iymin; iy <= iymax; iy++) 
		        for (var ix = ixmin; ix <= ixmax; ix++)
		        {
			        var index = ix + nx * iy + nx * ny * iz;

			        if (counts[index] == 0)
			        {
				        cells[index] = obj;
				        counts[index] += 1; // now = 1
			        }
			        else
			        {
				        if (counts[index] == 1)
				        {
					        var comp = new CompoundObject();
					        comp.AddObject(cells[index]);
					        comp.AddObject(obj);
					        cells[index] = comp;
					        counts[index] += 1; // now = 2
				        }
				        else
				        {
					        // counts[index] > 1
					        (cells[index] as CompoundObject)?.AddObject(obj);
					        counts[index] += 1;
				        }
			        }
		        }
	        } 

	        objects.Clear();

	        var num_zeroes = 0;
	        var num_ones = 0;
	        var num_twos = 0;
	        var num_threes = 0;
	        var num_greater = 0;

	        for (int j = 0; j < numCells; j++)
	        {
		        if (counts[j] == 0)
			        num_zeroes += 1;
		        if (counts[j] == 1)
			        num_ones += 1;
		        if (counts[j] == 2)
			        num_twos += 1;
		        if (counts[j] == 3)
			        num_threes += 1;
		        if (counts[j] > 3)
			        num_greater += 1;
	        }
        }

        public override bool Hit(TraceRay ray, out float tMin, ref ShadeRec sr)
        {
	        tMin = -1;
	        
	        var ox = ray.o.x;
	        var oy = ray.o.y;
	        var oz = ray.o.z;
	        var dx = ray.d.x;
	        var dy = ray.d.y;
	        var dz = ray.d.z;

	        var x0 = bbox.x0;
	        var y0 = bbox.y0;
	        var z0 = bbox.z0;
	        var x1 = bbox.x1;
	        var y1 = bbox.y1;
	        var z1 = bbox.z1;

	        float txMin, tyMin, tzMin;
	        float txMax, tyMax, tzMax;

	        var a = 1.0f / dx;
	        if (a >= 0)
	        {
		        txMin = (x0 - ox) * a;
		        txMax = (x1 - ox) * a;
	        }
	        else
	        {
		        txMin = (x1 - ox) * a;
		        txMax = (x0 - ox) * a;
	        }

	        var b = 1.0f / dy;
	        if (b >= 0)
	        {
		        tyMin = (y0 - oy) * b;
		        tyMax = (y1 - oy) * b;
	        }
	        else
	        {
		        tyMin = (y1 - oy) * b;
		        tyMax = (y0 - oy) * b;
	        }

	        var c = 1.0f / dz;
	        if (c >= 0)
	        {
		        tzMin = (z0 - oz) * c;
		        tzMax = (z1 - oz) * c;
	        }
	        else
	        {
		        tzMin = (z1 - oz) * c;
		        tzMax = (z0 - oz) * c;
	        }

	        var t0 = txMin > tyMin ? txMin : tyMin;

	        if (tzMin > t0)
		        t0 = tzMin;

	        var t1 = txMax < tyMax ? txMax : tyMax;

	        if (tzMax < t1)
		        t1 = tzMax;

	        if (t0 > t1)
		        return (false);

	        int ix, iy, iz;

	        if (bbox.Inside(ray.o))
	        {
		        ix = clamp((int)((ox - x0) * nx / (x1 - x0)), 0, nx - 1);
		        iy = clamp((int)((oy - y0) * ny / (y1 - y0)), 0, ny - 1);
		        iz = clamp((int)((oz - z0) * nz / (z1 - z0)), 0, nz - 1);
	        }
	        else
	        {
		        var p = ray.o + t0 * ray.d;
		        ix = clamp((int)((p.x - x0) * nx / (x1 - x0)), 0, nx - 1);
		        iy = clamp((int)((p.y - y0) * ny / (y1 - y0)), 0, ny - 1);
		        iz = clamp((int)((p.z - z0) * nz / (z1 - z0)), 0, nz - 1);
	        }

	        double dtx = (txMax - txMin) / nx;
	        double dty = (tyMax - tyMin) / ny;
	        double dtz = (tzMax - tzMin) / nz;

	        double txNext, tyNext, tzNext;
	        int ixStep, iyStep, izStep;
	        int ixStop, iyStop, izStop;

	        if (dx > 0)
	        {
		        txNext = txMin + (ix + 1) * dtx;
		        ixStep = +1;
		        ixStop = nx;
	        }
	        else
	        {
		        txNext = txMin + (nx - ix) * dtx;
		        ixStep = -1;
		        ixStop = -1;
	        }

	        if (dx == 0.0)
	        {
		        txNext = float.MaxValue;
		        ixStep = -1;
		        ixStop = -1;
	        }

	        if (dy > 0)
	        {
		        tyNext = tyMin + (iy + 1) * dty;
		        iyStep = +1;
		        iyStop = ny;
	        }
	        else
	        {
		        tyNext = tyMin + (ny - iy) * dty;
		        iyStep = -1;
		        iyStop = -1;
	        }

	        if (dy == 0.0)
	        {
		        tyNext = float.MaxValue;
		        iyStep = -1;
		        iyStop = -1;
	        }

	        if (dz > 0)
	        {
		        tzNext = tzMin + (iz + 1) * dtz;
		        izStep = +1;
		        izStop = nz;
	        }
	        else
	        {
		        tzNext = tzMin + (nz - iz) * dtz;
		        izStep = -1;
		        izStop = -1;
	        }

	        if (dz == 0.0)
	        {
		        tzNext = float.MaxValue;
		        izStep = -1;
		        izStop = -1;
	        }

	        while (true)
	        {
		        var obj = cells[ix + nx * iy + nx * ny * iz];

		        if (txNext < tyNext && txNext < tzNext)
		        {
			        if (obj != null && obj.Hit(ray, out tMin, ref sr) && tMin < txNext)
			        {
				        material = obj.GetMaterial();
				        return true;
			        }

			        txNext += dtx;
			        ix += ixStep;

			        if (ix == ixStop)
				        return false;
		        }
		        else
		        {
			        if (tyNext < tzNext)
			        {
				        if (obj != null && obj.Hit(ray, out tMin, ref sr) && tMin < tyNext)
				        {
					        material = obj.GetMaterial();
					        return true;
				        }

				        tyNext += dty;
				        iy += iyStep;

				        if (iy == iyStop)
					        return (false);
			        }
			        else
			        {
				        if (obj != null && obj.Hit(ray, out tMin, ref sr) && tMin < tzNext)
				        {
					        material = obj.GetMaterial();
					        return true;
				        }

				        tzNext += dtz;
				        iz += izStep;

				        if (iz == izStop)
					        return false;
			        }
		        }
	        }
        }

        public override bool ShadowHit(TraceRay ray, out float tMin)
        {
             tMin = -1;
	        
	        var ox = ray.o.x;
	        var oy = ray.o.y;
	        var oz = ray.o.z;
	        var dx = ray.d.x;
	        var dy = ray.d.y;
	        var dz = ray.d.z;

	        var x0 = bbox.x0;
	        var y0 = bbox.y0;
	        var z0 = bbox.z0;
	        var x1 = bbox.x1;
	        var y1 = bbox.y1;
	        var z1 = bbox.z1;

	        float txMin, tyMin, tzMin;
	        float txMax, tyMax, tzMax;

	        var a = 1.0f / dx;
	        if (a >= 0)
	        {
		        txMin = (x0 - ox) * a;
		        txMax = (x1 - ox) * a;
	        }
	        else
	        {
		        txMin = (x1 - ox) * a;
		        txMax = (x0 - ox) * a;
	        }

	        var b = 1.0f / dy;
	        if (b >= 0)
	        {
		        tyMin = (y0 - oy) * b;
		        tyMax = (y1 - oy) * b;
	        }
	        else
	        {
		        tyMin = (y1 - oy) * b;
		        tyMax = (y0 - oy) * b;
	        }

	        var c = 1.0f / dz;
	        if (c >= 0)
	        {
		        tzMin = (z0 - oz) * c;
		        tzMax = (z1 - oz) * c;
	        }
	        else
	        {
		        tzMin = (z1 - oz) * c;
		        tzMax = (z0 - oz) * c;
	        }

	        var t0 = txMin > tyMin ? txMin : tyMin;

	        if (tzMin > t0)
		        t0 = tzMin;

	        var t1 = txMax < tyMax ? txMax : tyMax;

	        if (tzMax < t1)
		        t1 = tzMax;

	        if (t0 > t1)
		        return (false);

	        int ix, iy, iz;

	        if (bbox.Inside(ray.o))
	        {
		        ix = clamp((int)((ox - x0) * nx / (x1 - x0)), 0, nx - 1);
		        iy = clamp((int)((oy - y0) * ny / (y1 - y0)), 0, ny - 1);
		        iz = clamp((int)((oz - z0) * nz / (z1 - z0)), 0, nz - 1);
	        }
	        else
	        {
		        var p = ray.o + t0 * ray.d;
		        ix = clamp((int)((p.x - x0) * nx / (x1 - x0)), 0, nx - 1);
		        iy = clamp((int)((p.y - y0) * ny / (y1 - y0)), 0, ny - 1);
		        iz = clamp((int)((p.z - z0) * nz / (z1 - z0)), 0, nz - 1);
	        }

	        double dtx = (txMax - txMin) / nx;
	        double dty = (tyMax - tyMin) / ny;
	        double dtz = (tzMax - tzMin) / nz;

	        double txNext, tyNext, tzNext;
	        int ixStep, iyStep, izStep;
	        int ixStop, iyStop, izStop;

	        if (dx > 0)
	        {
		        txNext = txMin + (ix + 1) * dtx;
		        ixStep = +1;
		        ixStop = nx;
	        }
	        else
	        {
		        txNext = txMin + (nx - ix) * dtx;
		        ixStep = -1;
		        ixStop = -1;
	        }

	        if (dx == 0.0)
	        {
		        txNext = float.MaxValue;
		        ixStep = -1;
		        ixStop = -1;
	        }

	        if (dy > 0)
	        {
		        tyNext = tyMin + (iy + 1) * dty;
		        iyStep = +1;
		        iyStop = ny;
	        }
	        else
	        {
		        tyNext = tyMin + (ny - iy) * dty;
		        iyStep = -1;
		        iyStop = -1;
	        }

	        if (dy == 0.0)
	        {
		        tyNext = float.MaxValue;
		        iyStep = -1;
		        iyStop = -1;
	        }

	        if (dz > 0)
	        {
		        tzNext = tzMin + (iz + 1) * dtz;
		        izStep = +1;
		        izStop = nz;
	        }
	        else
	        {
		        tzNext = tzMin + (nz - iz) * dtz;
		        izStep = -1;
		        izStop = -1;
	        }

	        if (dz == 0.0)
	        {
		        tzNext = float.MaxValue;
		        izStep = -1;
		        izStop = -1;
	        }

	        while (true)
	        {
		        var obj = cells[ix + nx * iy + nx * ny * iz];

		        if (txNext < tyNext && txNext < tzNext)
		        {
			        if (obj != null && obj.ShadowHit(ray, out tMin) && tMin < txNext)
				        return true;

			        txNext += dtx;
			        ix += ixStep;

			        if (ix == ixStop)
				        return false;
		        }
		        else
		        {
			        if (tyNext < tzNext)
			        {
				        if (obj != null && obj.ShadowHit(ray, out tMin) && tMin < tyNext)
					        return true;

				        tyNext += dty;
				        iy += iyStep;

				        if (iy == iyStop)
					        return (false);
			        }
			        else
			        {
				        if (obj != null && obj.ShadowHit(ray, out tMin) && tMin < tzNext)
					        return true;

				        tzNext += dtz;
				        iz += izStep;

				        if (iz == izStop)
					        return false;
			        }
		        }
	        }
        }

        private float3 FindMinBounds()
        {
            var p0 = float3(float.MaxValue);

            foreach (var obj in objects)
            {
                var objectBox = obj.GetBoundingBox();

                if (objectBox.x0 < p0.x)
                    p0.x = objectBox.x0;
                if (objectBox.y0 < p0.y)
                    p0.y = objectBox.y0;
                if (objectBox.z0 < p0.z)
                    p0.z = objectBox.z0;
            }

            p0.x -= KEpsilon; p0.y -= KEpsilon; p0.z -= KEpsilon;

            return p0;
        }

        private float3 FindMaxBounds()
        {
            var p1 = float3(-float.MaxValue);

            foreach (var obj in objects)
            {
                var objectBox = obj.GetBoundingBox();

                if (objectBox.x1 > p1.x)
                    p1.x = objectBox.x1;
                if (objectBox.y1 > p1.y)
                    p1.y = objectBox.y1;
                if (objectBox.z1 > p1.z)
                    p1.z = objectBox.z1;
            }

            p1.x += KEpsilon; p1.y += KEpsilon; p1.z += KEpsilon;

            return p1;
        }

        public void Clear()
        {
	        cells.Clear();
        }
        
        private List<IGeometricObject> cells = new List<IGeometricObject>();
        private int nx, ny, nz;
        private BBox bbox;
    }
}