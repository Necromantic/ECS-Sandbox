using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms2D;
using Unity.Transforms;
using Unity.Jobs;
using UnityEngine;

namespace com.binaryfeast.ECS
{
    [UnityEngine.ExecuteInEditMode]
    [UpdateBefore(typeof(Transform2DSystem))]
    public class Movement2DSystem : JobComponentSystem
    {
        struct TransformGroup
        {
            [ReadOnly]
            public ComponentDataArray<Movement2D> movements;

            public ComponentDataArray<Position2D> positions;

            public int Length;
        }

        [Inject]
        TransformGroup transformGroup;

        [ComputeJobOptimization]
        struct TransformGroupJob : IJobParallelFor
        {
            [ReadOnly]
            public ComponentDataArray<Movement2D> movements;

            public ComponentDataArray<Position2D> positions;

            [ReadOnly]
            public float dt;

            public void Execute(int i)
            {
                float2 movement = movements[i].Value;
                positions[i] = new Position2D() {
                    Value = positions[i].Value + movement * dt
                };
            }
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var transformJob = new TransformGroupJob
            {
                positions = transformGroup.positions,
                movements = transformGroup.movements,
                dt = Time.deltaTime
            };

            return transformJob.Schedule(transformGroup.Length, 64, inputDeps);
        }
    }
}