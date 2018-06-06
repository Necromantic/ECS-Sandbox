using System;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[Serializable]
public struct SpriteColor : IComponentData
{
    public Color Value;

    public SpriteColor(Color Value)
    {
        this.Value = Value;
    }
}

public class SpriteColorComponent : ComponentDataWrapper<SpriteColor> { }