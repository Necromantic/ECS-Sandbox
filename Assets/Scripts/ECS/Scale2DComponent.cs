using System;
using Unity.Entities;
using Unity.Mathematics;

namespace com.binaryfeast.ECS
{
    [Serializable]
    public struct Scale2D : IComponentData
    {
        public float2 Value;

        public Scale2D(float2 Value)
        {
            this.Value = Value;
        }

        public Scale2D(float x, float y)
        {
            Value = new float2(x, y);
        }
    }

    public class Scale2DComponent : ComponentDataWrapper<Scale2D> { }
}