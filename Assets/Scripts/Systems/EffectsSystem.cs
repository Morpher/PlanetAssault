using Components;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace Systems
{
    [UpdateBefore(typeof(EntityRemovalSystem)), UpdateAfter(typeof(RocketCollisionSystem)), UpdateAfter(typeof(DamageSystem))]
    public class EffectsSystem : SystemBase
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
            
            Entities.WithoutBurst().ForEach((Entity entity, in RemoveMarkComponent removeMark, in Translation position, 
                    in PlanetComponent planet, in EffectComponent effect) =>
                {
                    effectsManager.GenerateEntityEffect(entity, position.Value.x, position.Value.y, EffectType.PlanetExplosion);
                    commandBuffer.RemoveComponent<EffectComponent>(entity);
                })
                .Run();
            
            Entities.WithoutBurst().ForEach((Entity entity, in RemoveMarkComponent removeMark, in Translation position, 
                    in RocketComponent rocket, in EffectComponent effect) =>
                {
                    effectsManager.GenerateEntityEffect(entity, position.Value.x, position.Value.y, EffectType.RocketExplosion);
                    commandBuffer.RemoveComponent<EffectComponent>(entity);
                })
                .Run();
        }
    }
}