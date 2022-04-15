using UnityEngine;
using Unity.Mathematics;

namespace RayTracer
{
    public class Outputter
    {
        private Texture2D texture_2d;

        private int width { get; set; }
        private int height { get; set; }

        public void Create(int w, int h)
        {
            this.width = w;
            this.height = h;
            texture_2d = new Texture2D(w, h, TextureFormat.ARGB32, false, true);
        }

        public void Clear()
        {
            var black = float3.zero;
            for (var x = 0; x < width; x++)
            {
                for (var y = 0; y < height; y++)
                {
                    SetPixel(x, y, black);
                }
            }
        }

        public void Commit()
        {
            texture_2d.Apply();
        }

        public void SetPixel(int x, int y, float3 color)
        {
            var c = new Color(color.x, color.y, color.z);
            texture_2d.SetPixel(x, y, c);
        }

        public Texture2D GetResult()
        {
            return texture_2d;
        }
    }
}