using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using static Unity.Mathematics.math;

namespace RayTracer
{
    public abstract class Sampler
    {
        protected Sampler(int ns)
            : this(ns, 83)
        {
        }

        protected Sampler(int ns, int nSets)
        {
            rand = new Random(5);
            
            numSamples = ns;
            num_sets = nSets;
            samples.Capacity = numSamples * num_sets;
            SetupShuffledIndices();
        }

        public void SetNumSets(int np)
        {
            num_sets = np;
        }

        protected int numSamples { get; }
        
        public float2 SampleUnitSquare()
        {
            if (count % numSamples == 0)
            {
                jump = rand.NextInt(0, num_sets) * numSamples;
            }
            
            return samples[jump + shuffled_indices[(int) (jump + count++ % numSamples)]];
        }

        public float3 SampleHemisphere()
        {
            if (count % numSamples == 0)
            {
                jump = rand.NextInt(0, num_sets) * numSamples;
            }
            
            return hemisphere_samples[jump + shuffled_indices[(int) (jump + count++ % numSamples)]];
        }

        private void SetupShuffledIndices()
        {
            shuffled_indices.Capacity = numSamples * num_sets;
            var indices = new List<int>(numSamples);
            for (var i = 0; i < numSamples; i++)
            {
                indices.Add(i);
            }

            for (var p = 0; p < num_sets; p++)
            {
                var rnd = new System.Random();
                var randomized = indices.OrderBy(item => rnd.Next());
                                
                foreach (var v in randomized)
                {
                    shuffled_indices.Add(v);
                }
            }   
        }

        public void MapSamplesToHemisphere(float exp)
        {
            var size = samples.Count;
            hemisphere_samples.Capacity = numSamples * num_sets;

            for (var i = 0; i < size; i++)
            {
                var cosPhi = cos(2.0 * PI * samples[i].x);
                var sinPhi = sin(2.0 * PI * samples[i].x);
                var cosTheta = pow((1.0 - samples[i].y), 1.0 / (exp + 1.0));
                var sinTheta = sqrt(1.0 - cosTheta * cosTheta);
                var pu = sinTheta * cosPhi;
                var pv = sinTheta * sinPhi;
                var pw = cosTheta;
                hemisphere_samples.Add(float3((float)pu, (float)pv, (float)pw));
            }
        }

        public abstract void GenerateSamples();

        protected int num_sets;
        protected readonly List<float2> samples = new List<float2>();
        private List<int> shuffled_indices = new List<int>();
        private List<float3> hemisphere_samples = new List<float3>();
        private uint count = 0;
        private int jump = 0;
        private Random rand;
    }
}