using Components;
using Components.Orbit;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

namespace Systems
{
    public class DamageSystem : JobComponentSystem
    {
        private EntityCommandBufferSystem barrier;
        
        protected override void OnCreate()
        {
            base.OnCreate();
            barrier = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        }
        
        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var commandBuffer = barrier.CreateCommandBuffer().ToConcurrent();
            inputDeps = Entities.WithBurst().ForEach((Entity entity, int nativeThreadIndex, ref LifeComponent life, in DamageComponent damage) =>
                {
                    life.Value -= damage.Value;
                    if (life.Value <= 0)
                    {
                        commandBuffer.AddComponent<RemoveMarkComponent>(nativeThreadIndex, entity);
                        commandBuffer.AddComponent<EffectComponent>(nativeThreadIndex, entity);
                        if (damage.DealtByPlayer)
                        {
                            commandBuffer.AddComponent<ScoreComponent>(nativeThreadIndex, entity);                            
                        }
                    }
                    commandBuffer.RemoveComponent<DamageComponent>(nativeThreadIndex, entity);
                    
                })
                .Schedule(inputDeps);
            
            
            //remove damage from entities without life component as well 
            inputDeps = Entities.WithBurst().ForEach((Entity entity, int nativeThreadIndex, in DamageComponent damage) =>
                {
                   commandBuffer.RemoveComponent<DamageComponent>(nativeThreadIndex, entity);
                    
                })
                .Schedule(inputDeps);
            
            barrier.AddJobHandleForProducer(inputDeps);
            return inputDeps;
        }
    }
}