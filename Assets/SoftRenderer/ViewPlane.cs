using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace RayTracer
{
    [Serializable]
    public class ViewPlane
    {
        public int hRes = 400;

        public int vRes = 400;

        public float gamma = 2.2f;
        public float invGamma => 1 / gamma;
        
        public float maxDepth;

        [HideInInspector]
        public float pixelSize = 1;
        
        public void SetSampler(Sampler samp)
        {
            this.sampler = samp;
        }

        public Sampler sampler { get; private set; }
    }
}