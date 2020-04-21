using Components;
using Components.Orbit;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

namespace Systems
{
    /// <summary>
    /// Hit detection system, rockets with planets
    /// </summary>
    public class RocketCollisionSystem : JobComponentSystem
    {
        private EntityCommandBufferSystem barrier;
        
        protected override void OnCreate()
        {
            base.OnCreate();
            barrier = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        }
  
        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var planetQuery = GetEntityQuery(ComponentType.ReadOnly<PlanetComponent>(), ComponentType.ReadOnly<Translation>());
            var planetPosition = planetQuery.ToComponentDataArray<Translation>(Allocator.TempJob);
            var planetComponent = planetQuery.ToComponentDataArray<PlanetComponent>(Allocator.TempJob);
            var planetEntity = planetQuery.ToEntityArray(Allocator.TempJob);
            var commandBuffer = barrier.CreateCommandBuffer().ToConcurrent();
            var damagedPlanetQuery = GetEntityQuery(ComponentType.ReadOnly<PlanetComponent>(), ComponentType.ReadOnly<DamageComponent>());
            var damagedPlanetEntity = damagedPlanetQuery.ToEntityArray(Allocator.TempJob);
            
            inputDeps = Entities.WithBurst().ForEach((Entity entity, int nativeThreadIndex, in Translation position, 
                    in RocketComponent rocket, in VelocityComponent velocity) =>
                {
                    bool markForDestroy = false;
                    for (var i = 0; i < planetPosition.Length; i++)
                    {
                        if(damagedPlanetEntity.Contains(planetEntity[i])) continue;
                        var diff = position.Value - planetPosition[i].Value;
                        var dist = math.length(diff);
                        if (dist <= planetComponent[i].Radius)
                        {
                            commandBuffer.AddComponent<DamageComponent>(nativeThreadIndex, planetEntity[i]);
                            commandBuffer.SetComponent(nativeThreadIndex, planetEntity[i], new DamageComponent
                            {
                                Value = (float)math.length(velocity.Velocity) * 0.25f,
                                DealtByPlayer = rocket.OwnedByPlayer
                            });
                            markForDestroy = true;
                        }
                    }

                    if (markForDestroy)
                    {
                        commandBuffer.AddComponent<EffectComponent>(nativeThreadIndex, entity);
                        commandBuffer.AddComponent<RemoveMarkComponent>(nativeThreadIndex, entity);
                        commandBuffer.SetComponent(nativeThreadIndex, entity,
                            new RemoveMarkComponent {TimeTillRemoveMs = 0});
                    }
                })
                .Schedule(inputDeps);

            barrier.AddJobHandleForProducer(inputDeps);
            inputDeps.Complete();
            planetPosition.Dispose();
            planetComponent.Dispose();
            planetEntity.Dispose();
            damagedPlanetEntity.Dispose();
            return inputDeps;
        }
    }
}