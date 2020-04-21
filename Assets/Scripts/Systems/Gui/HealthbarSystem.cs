using Components;
using Components.Gui;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

namespace Systems.Gui
{
    public class HealthbarSystem : JobComponentSystem
    {
        private EntityCommandBufferSystem barrier;
        private GuiManager guiManager;
        
        protected override void OnCreate()
        {
            base.OnCreate();
            barrier = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
            //TODO: Inject with DI framework
            guiManager = GameObject.FindObjectOfType<GuiManager>();
        }
        
        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var commandBuffer = barrier.CreateCommandBuffer();
            var gui = guiManager;
            Entities.WithoutBurst().ForEach((Entity entity, int nativeThreadIndex, ref LifeComponent life, ref HealthbarComponent health) =>
                {
                    gui.AddHealthbar(entity);
                    commandBuffer.RemoveComponent<HealthbarComponent>(entity);
                })
                .Run();
            
            barrier.AddJobHandleForProducer(inputDeps);
            return inputDeps;
        }
    }
}