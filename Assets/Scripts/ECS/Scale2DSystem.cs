using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms2D;
using Unity.Transforms;
using Unity.Jobs;
using Unity.Burst;

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

            [ReadOnly]
            public readonly int Length;
        }

        [Inject]
        TransformGroup transformGroup;

        [BurstCompile]
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
                    Value = math.mul(transforms[i].Value, float4x4.scale(new float3(scale.x, 1, scale.y)))
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