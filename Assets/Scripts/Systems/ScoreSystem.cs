using Components;
using Components.Orbit;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Systems
{
    public class ScoreSystem : SystemBase
    {
        private EntityCommandBufferSystem barrier;
        
        protected override void OnCreate()
        {
            base.OnCreate();
            barrier = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        }
        
        protected override void OnUpdate()
        {
            var manager = GameManager.Instance;
            var commandBuffer = barrier.CreateCommandBuffer();
            Entities.WithoutBurst().ForEach((Entity entity, ref ScoreComponent input) =>
                {
                    commandBuffer.RemoveComponent<ScoreComponent>(entity);
                    manager.IncrementScore();
                })
                .Run();
        }
    }
}