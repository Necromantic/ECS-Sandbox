using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.Experimental.PlayerLoop;

namespace com.binaryfeast.ECS
{
    [UpdateAfter(typeof(PreLateUpdate.ParticleSystemBeginUpdateAll))]
    [ExecuteInEditMode]
    public class SharedSpriteRendererSystem : ComponentSystem
    {
        private readonly Dictionary<SharedSprite, Material> materialCache = new Dictionary<SharedSprite, Material>();
        private readonly Dictionary<SharedSprite, Mesh> meshCache = new Dictionary<SharedSprite, Mesh>();
        
        private readonly Matrix4x4[] matricesArray = new Matrix4x4[1023];
        private readonly Vector4[] colorArray = new Vector4[1023];
        private readonly List<SharedSprite> cachedUniqueSharedSprites = new List<SharedSprite>(12);
        private ComponentGroup componentGroup;

        protected override void OnCreateManager(int capacity)
        {
            componentGroup = GetComponentGroup(typeof(SharedSprite), typeof(TransformMatrix), typeof(Coloring));
        }

        protected override void OnUpdate()
        {
            EntityManager.GetAllUniqueSharedComponentDatas(cachedUniqueSharedSprites);
            for (int i = 0; i != cachedUniqueSharedSprites.Count; i++)
            {
                var sharedSprite = cachedUniqueSharedSprites[i];
                if (sharedSprite.Value == null)
                    continue;
                componentGroup.SetFilter(sharedSprite);
                var transforms = componentGroup.GetComponentDataArray<TransformMatrix>();
                var colors = componentGroup.GetComponentDataArray<Coloring>();

                Mesh mesh;
                Material material;
                Sprite sprite = sharedSprite.Value;

                float2 meshSize = new float2(sprite.rect.width / sprite.pixelsPerUnit, sprite.rect.height / sprite.pixelsPerUnit);
                float2 meshPivot = new float2(sprite.pivot.x / sprite.rect.width * meshSize.x, sprite.pivot.y / sprite.rect.height * meshSize.y);
                if (!meshCache.TryGetValue(sharedSprite, out mesh))
                {
                    mesh = generateQuad(meshSize, meshPivot);
                    meshCache.Add(sharedSprite, mesh);
                }

                if (!materialCache.TryGetValue(sharedSprite, out material))
                {
                    material = new Material(Shader.Find("ECS/Sprite"))
                    {
                        enableInstancing = true,
                        mainTexture = sprite.texture
                    };

                    float4 rect = new float4(sprite.textureRect.x / sprite.texture.width,
                        sprite.textureRect.y / sprite.texture.height,
                        sprite.textureRect.width / sprite.texture.width,
                        sprite.textureRect.height / sprite.texture.height);
                    material.SetVector("_Rect", rect);

                    materialCache.Add(sharedSprite, material);
                }

                int beginIndex = 0;
                while (beginIndex < transforms.Length)
                {
                    int length = math.min(matricesArray.Length, transforms.Length - beginIndex);
                    Unity.Rendering.MeshInstanceRendererSystem.CopyMatrices(transforms, beginIndex, length, matricesArray);
                    
                    int k = 0;
                    for (int j = beginIndex; j < beginIndex + length; ++j)
                    {
                        Color color = colors[j].Value;
                        colorArray[k++] = new Vector4(color.r, color.g, color.b, color.a);
                    }

                    MaterialPropertyBlock properties = new MaterialPropertyBlock();
                    properties.SetVectorArray("_Color", colorArray);

                    Graphics.DrawMeshInstanced(mesh, 0, material, matricesArray, length, properties);

                    beginIndex += length;
                }
            }
            cachedUniqueSharedSprites.Clear();
        }

        public static Mesh generateQuad(Vector2 size, Vector2 pivot)
        {
            Vector3[] vertices = new Vector3[] {
                new Vector3(size.x - pivot.x, 0, size.y - pivot.y),
                new Vector3(size.x - pivot.x, 0, 0 - pivot.y),
                new Vector3(0 - pivot.x, 0, 0 - pivot.y),
                new Vector3(0 - pivot.x, 0, size.y - pivot.y)
            };

            Vector2[] uv = new Vector2[] {
                new Vector2(1, 1),
                new Vector2(1, 0),
                new Vector2(0, 0),
                new Vector2(0, 1)
            };

            int[] triangles = new int[] {
                0, 1, 2,
                2, 3, 0
            };

            return new Mesh {
                vertices = vertices,
                uv = uv,
                triangles = triangles
            };
        }
    }
}