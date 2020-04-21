using Components;
using Components.Orbit;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

namespace Systems
{
    [UpdateBefore(typeof(EntityRemovalSystem))]
    public class RocketAttractorSystem : JobComponentSystem
    {
        private EntityCommandBufferSystem barrier;
        
        protected override void OnCreate()
        {
            base.OnCreate();
            barrier = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var rocketQuery = GetEntityQuery(typeof(VelocityComponent), ComponentType.ReadOnly<RocketComponent>(), ComponentType.ReadOnly<Translation>());
            var rocketVelocity = rocketQuery.ToComponentDataArray<VelocityComponent>(Allocator.TempJob);
            var rocketPosition = rocketQuery.ToComponentDataArray<Translation>(Allocator.TempJob);
            var rocketEntity = rocketQuery.ToEntityArray(Allocator.TempJob);
            var commandBuffer = barrier.CreateCommandBuffer().ToConcurrent();

            inputDeps = Entities.WithBurst().ForEach((int nativeThreadIndex, in Translation position, in RocketAttractorComponent attractor) =>
                {
                    for (var i = 0; i < rocketVelocity.Length; i++)
                    {
                        var velocity = rocketVelocity[i];
                        var diff = position.Value - rocketPosition[i].Value;
                        var dist = math.length(diff);
                        if (dist < attractor.MaxDistance)
                        {
                            velocity.Velocity += attractor.Strength * (diff / dist);
                            commandBuffer.SetComponent(nativeThreadIndex, rocketEntity[i], new VelocityComponent
                            {
                                Velocity = velocity.Velocity
                            }); 
                        }
                    }
                })
                .Schedule(inputDeps);
            
            barrier.AddJobHandleForProducer(inputDeps);
            inputDeps.Complete();
            rocketVelocity.Dispose();
            rocketPosition.Dispose();
            rocketEntity.Dispose();
            return inputDeps;
        }
    }
}