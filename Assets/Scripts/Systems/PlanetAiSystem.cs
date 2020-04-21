using Components;
using Components.Orbit;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;

namespace Systems
{
    /// <summary>
    /// Simple AI system
    /// </summary>
    [UpdateBefore(typeof(EntityRemovalSystem))]
    public class PlanetAiSystem : JobComponentSystem
    {
        private float timeSinceStart;
        private RocketSpawner spawner;
        
        protected override void OnCreate()
        {
            base.OnCreate();
            spawner = GameObject.FindObjectOfType<RocketSpawner>();
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var planetsQuery = GetEntityQuery(ComponentType.ReadOnly<PlanetComponent>(), 
                ComponentType.ReadOnly<Translation>(), ComponentType.ReadOnly<PlayerPlanetComponent>());
            var planetPosition = planetsQuery.ToComponentDataArray<Translation>(Allocator.TempJob);
            var planetEntity = planetsQuery.ToEntityArray(Allocator.TempJob);
            var time = Time.DeltaTime;
            timeSinceStart += time;
            var random = new Random((uint)(timeSinceStart * 1000f));
            
            Entities.WithoutBurst().ForEach((Entity entity, ref AiPlanetComponent ai,
                    in Translation position, in Rotation rotation, in PlanetComponent planet) =>
                {
                    ai.TimeTillNextShot -= time;
                    if (ai.TimeTillNextShot <= 0)
                    {
                        double3 directionToPlanet = float3.zero;
                        double minDist = float.MaxValue;
                        for (var i = 0; i < planetPosition.Length; i++)
                        {
                            //Do not commit suicide
                            if (entity == planetEntity[i])
                            {
                                continue;
                            }
                            var diff = planetPosition[i].Value - position.Value;
                            var dist = math.length(diff);
                            if (dist < minDist)
                            {
                                minDist = dist;
                                directionToPlanet = math.normalize(diff);
                            }
                        }
                        var direction = math.normalize(math.mul(rotation.Value, new float3(0, 1, 0)));
                        if (math.dot(direction, directionToPlanet) > 0.8d)
                        {
                            ai.TimeTillNextShot = random.NextFloat(ai.IntervalRangeSec.x, ai.IntervalRangeSec.y);
                            var rocketPos = position.Value + direction * planet.Radius * 1.2f;
                            spawner.Spawn(rocketPos, direction); 
                        }
                        
                    }
                })
                .Run();
            
            planetPosition.Dispose();
            planetEntity.Dispose();
            return inputDeps;
        }
    }
}