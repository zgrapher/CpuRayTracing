using Unity.Mathematics;
using UnityEngine;
using static Unity.Mathematics.math;
using Random = UnityEngine.Random;

namespace RayTracer
{
    public class MultiJittered : Sampler
    {
        public MultiJittered(int ns) : base(ns)
        {
            GenerateSamples();
        }

        public MultiJittered(int ns, int nSets) : base(ns)
        {
            GenerateSamples();
        }

        public sealed override void GenerateSamples()
        {
            var n = (int) sqrt(numSamples);
            var subCellWidth = 1.0f / numSamples;

            for (var i = 0; i < numSamples * num_sets; i++)
            {
                samples.Add(Vector2.zero);
            }

            for (var p = 0; p < num_sets; p++)
                for (var i = 0; i < n; i++)
                    for (var j = 0; j < n; j++)
                    {
                        var item = new Vector2
                        {
                            x = (i * n + j) * subCellWidth + Random.Range(0, subCellWidth),
                            y = (j * n + i) * subCellWidth + Random.Range(0, subCellWidth)
                        };

                        samples[i * n + j + p * numSamples] = item;
                    }
            
            for (var p = 0; p < num_sets; p++)
                for (var i = 0; i < n; i++)
                    for (var j = 0; j < n; j++)
                    {
                        int k = Random.Range(j, n);

                        float2 sj = samples[i * n + j + p * numSamples];
                        float2 sk = samples[i * n + k + p * numSamples];

                        (sj.x, sk.x) = (sk.x, sj.x);

                        samples[i * n + j + p * numSamples] = sj;
                        samples[i * n + k + p * numSamples] = sk;
                    }
            
            for (var p = 0; p < num_sets; p++)
                for (var i = 0; i < n; i++)
                    for (var j = 0; j < n; j++)
                    {
                        int k = Random.Range(j, n);

                        float2 sj = samples[i * n + j + p * numSamples];
                        float2 sk = samples[i * n + k + p * numSamples];

                        (sj.y, sk.y) = (sk.y, sj.y);

                        samples[i * n + j + p * numSamples] = sj;
                        samples[i * n + k + p * numSamples] = sk;
                    }
        }
    }
}