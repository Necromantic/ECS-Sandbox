using System;
using Unity.Entities;
using UnityEngine;

namespace com.binaryfeast.ECS
{
    [Serializable]
    public struct Coloring : IComponentData
    {
        public Color Value;

        public Coloring(Color Value)
        {
            this.Value = Value;
        }

        public Coloring(float r, float g, float b, float a = 1)
        {
            Value = new Color(r, g, b, a);
        }
    }

    public class ColoringComponent : ComponentDataWrapper<Coloring> { }
}