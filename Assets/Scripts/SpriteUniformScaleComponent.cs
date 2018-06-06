using System;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[Serializable]
public struct SpriteUniformScale : IComponentData
{
    public float Value;

    public SpriteUniformScale(float Value)
    {
        this.Value = Value;
    }
}

public class SpriteUniformScaleComponent : ComponentDataWrapper<SpriteUniformScale> { }