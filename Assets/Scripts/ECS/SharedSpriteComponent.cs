using System;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace com.binaryfeast.ECS
{
    [Serializable]
    public struct SharedSprite : ISharedComponentData
    {
        public Sprite Value;

        public SharedSprite(Sprite Value)
        {
            this.Value = Value;
        }
    }

    public class SharedSpriteComponent : SharedComponentDataWrapper<SharedSprite> { }
}