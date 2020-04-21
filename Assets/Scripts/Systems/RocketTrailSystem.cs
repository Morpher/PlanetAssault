using Components;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace Systems
{
    [UpdateBefore(typeof(EntityRemovalSystem))]
    public class RocketTrailSystem : SystemBase
    {
        private Effects effectsManager;
        private EntityCommandBufferSystem barrier;
        
        protected override void OnCreate()
        {
            base.OnCreate();
            //TODO: Inject the dependency with DI framework
            effectsManager = GameObject.FindObjectOfType<Effects>();
            barrier = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        }
        
        protected override void OnUpdate()
        {
            var effectsManager = this.effectsManager;
            var commandBuffer = barrier.CreateCommandBuffer();
            Entities.WithoutBurst().ForEach((Entity entity, in Translation position, 
                    in RocketComponent planet, in RocketTrailComponent trail) =>
                {
                    effectsManager.GenerateEntityEffect(entity, position.Value.x, position.Value.y, EffectType.RocketTrail);
                    commandBuffer.RemoveComponent<RocketTrailComponent>(entity);
                })
                .Run();
        }
    }
}