using System;
using Unity.Entities;
using Unity.Mathematics;

namespace com.binaryfeast.ECS
{
    [Serializable]
    public struct Movement2D : IComponentData
    {
        public float2 Value;

        public Movement2D(float2 Value)
        {
            this.Value = Value;
        }

        public Movement2D(float x, float y)
        {
            Value = new float2(x, y);
        }
    }

    public class Movement2DComponent : ComponentDataWrapper<Movement2D> { }
}