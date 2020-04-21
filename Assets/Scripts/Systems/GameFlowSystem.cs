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
    public class GameFlowSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            var manager = GameManager.Instance;
            if (manager.State != GameState.Started)
            {
                return;
            }
            
            var playerPlanetQuery = GetEntityQuery(ComponentType.ReadOnly<PlayerPlanetComponent>());
            var aiPlanetQuery = GetEntityQuery(ComponentType.ReadOnly<AiPlanetComponent>());
            var player = playerPlanetQuery.ToComponentDataArray<PlayerPlanetComponent>(Allocator.TempJob);    
            var ai = aiPlanetQuery.ToComponentDataArray<AiPlanetComponent>(Allocator.TempJob);

            if (player.Length == 0)
            {
                manager.OnLevelFailed();
            }
            else if(ai.Length == 0)
            {
                manager.OnLevelCompleted();
            }

            player.Dispose();
            ai.Dispose();
        }
    }
}