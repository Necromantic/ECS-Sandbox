using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms2D;
using Unity.Transforms;
using Unity.Jobs;

namespace com.binaryfeast.ECS
{
    [UnityEngine.ExecuteInEditMode]
    [UpdateAfter(typeof(Transform2DSystem))]
    public class Scale2DSystem : JobComponentSystem
    {
        struct TransformGroup
        {
            [ReadOnly]
            public ComponentDataArray<Scale2D> scales;

            public ComponentDataArray<TransformMatrix> transforms;
            public int Length;
        }

        [Inject]
        TransformGroup transformGroup;

        [ComputeJobOptimization]
        struct TransformGroupJob : IJobParallelFor
        {
            [ReadOnly]
            public ComponentDataArray<Scale2D> scales;

            public ComponentDataArray<TransformMatrix> transforms;

            public void Execute(int i)
            {
                float2 scale = scales[i].Value;
                transforms[i] = new TransformMatrix
                {
                    Value = math.mul(transforms[i].Value, math.scale(new float3(scale.x, 1, scale.y)))
                };
            }
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var transformJob = new TransformGroupJob
            {
                transforms = transformGroup.transforms,
                scales = transformGroup.scales
            };

            return transformJob.Schedule(transformGroup.Length, 64, inputDeps);
        }
    }
}