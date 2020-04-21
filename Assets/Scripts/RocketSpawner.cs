using Components;
using Components.Orbit;
using Reese.Spawning;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// Monobehaviour responsible for spawning planets from entity prefabs
/// Based on this https://reeseschultz.com/spawning-prefabs-with-unity-ecs/
/// </summary>
public class RocketSpawner : MonoBehaviour
{
    private RocketPrefabs prefabs;
    private Entity[] entityPrefabs;

    [SerializeField]
    private float initialVelocity = 10f;
    
    private bool isInited;

    // Get the default world containing all entities:
    private EntityManager EntityManager => World
        .DefaultGameObjectInjectionWorld
        .EntityManager;
    
    public void Spawn(Vector3 position, Vector3 direction, bool ownedByPlayer = false)
    {
        Init();
        SpawnSystem.Enqueue(RandomRocket(position, direction, entityPrefabs, ownedByPlayer));          
    }
 
    private Spawn RandomRocket(Vector3 position, Vector3 direction, Entity[] entityPrefas, bool ownedByPlayer)
    {
        var index = Random.Range(0, entityPrefas.Length);
        var spawn = new Spawn()
            .WithPrefab(entityPrefas[index]) //  Optional prefab entity.
            .WithComponentList(
                new LevelEntityComponent(),
                new RocketTrailComponent(),
                new RocketComponent { OwnedByPlayer = ownedByPlayer },
                new Translation { Value = new float3(position.x, position.y, 0) },
                new VelocityComponent { Velocity = new float3(direction) * initialVelocity},
                new RemoveMarkComponent { TimeTillRemoveMs = 5000 }
                );

        return spawn;
    }
    
    private void Init()
    {
        if (isInited)
        {
            return;
        }
        
        // Get the entity associated with the prefab:
        prefabs = EntityManager
            .CreateEntityQuery(typeof(RocketPrefabs))
            .GetSingleton<RocketPrefabs>();

        entityPrefabs = new[]
        {
            prefabs.FastRocket,
        };

        isInited = true;
    }
}
