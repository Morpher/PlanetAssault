using System.ComponentModel;
using Components;
using Unity.Entities;

namespace Systems
{
    /// <summary>
    /// System responsible for removing entities, which were marked with RemoveMarkComponent
    /// </summary>
    public class EntityRemovalSystem : SystemBase
    {
        private EntityCommandBufferSystem barrier;
        
        protected override void OnCreate()
        {
            base.OnCreate();
            barrier = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        }
        
        protected override void OnUpdate()
        {
            var deltaTimeMs = Time.DeltaTime * 1000f;
            var commandBuffer = barrier.CreateCommandBuffer().ToConcurrent();
            var effectComponent = GetComponentDataFromEntity<EffectComponent>();
            Dependency = Entities.WithBurst().ForEach((Entity entity, int nativeThreadIndex, ref RemoveMarkComponent removeMark) =>
                {
                    removeMark.TimeTillRemoveMs -= (int) deltaTimeMs;
                    if (removeMark.TimeTillRemoveMs <= 0 && !effectComponent.Exists(entity))
                    {
                        commandBuffer.DestroyEntity(nativeThreadIndex, entity);                        
                    }
                })
                .Schedule(Dependency);

            barrier.AddJobHandleForProducer(Dependency);
        }
    }
}