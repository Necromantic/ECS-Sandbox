using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Transforms2D;

namespace com.binaryfeast.ECS
{
    public sealed class SpriteRendererSceneBootstrap
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        public static void InitializeAfterScene()
        {
            EntityManager entityManager = World.Active.GetOrCreateManager<EntityManager>();
            
            Sprite[] sprites = Resources.LoadAll<Sprite>("Sprites/modular_ships_mockup_alpha");
            SharedSprite[] sharedSprites = new SharedSprite[sprites.Length];
            for (int i = 0; i < sprites.Length; ++i)
                sharedSprites[i] = new SharedSprite(sprites[i]);
            
            for (int i = 0; i < 10000; i++)
            {
                var entity = entityManager.CreateEntity(ComponentType.Create<Position2D>(),
                                                        ComponentType.Create<Heading2D>(),
                                                        ComponentType.Create<TransformMatrix>(),
                                                        ComponentType.Create<Coloring>(),
                                                        ComponentType.Create<Scale2D>(),
                                                        ComponentType.Create<Movement2D>());

                entityManager.SetComponentData(entity, new Position2D
                {
                    Value = new float2(Random.value * 16, Random.value * 9)
                });

                entityManager.SetComponentData(entity, new Heading2D
                {
                    Value = new float2(Random.value, Random.value)
                });

                entityManager.SetComponentData(entity, new Coloring
                {
                    Value = new Color(Random.value, Random.value, Random.value)
                });

                float scale = Random.value;
                entityManager.SetComponentData(entity, new Scale2D
                {
                    Value = new float2(scale, scale)
                });

                entityManager.SetComponentData(entity, new Movement2D
                {
                    Value = new float2(Random.value * 2 - 1, Random.value * 2 - 1)
                });

                entityManager.AddSharedComponentData(entity, sharedSprites[i % sprites.Length]);
            }
        }
    }
}