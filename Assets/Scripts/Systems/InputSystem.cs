using Components;
using Components.Orbit;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Systems
{
    public class InputSystem : SystemBase
    {
        private RocketSpawner spawner;
        
        protected override void OnCreate()
        {
            base.OnCreate();
            spawner = GameObject.FindObjectOfType<RocketSpawner>();
        }
        
        protected override void OnUpdate()
        {
            var time = Time.DeltaTime;
            var spawner = this.spawner;
            Entities.WithoutBurst().ForEach((ref PlayerPlanetComponent input, in PlanetComponent planet, in Translation position, in Rotation rotation) =>
                {
                    input.TimeSinceLastShoot += time;
                    if (input.TimeSinceLastShoot >= input.ShootDelay 
                        && Input.GetKeyDown(input.ShootButton))
                    {
                        var direction = math.mul(rotation.Value, new float3(0, 1, 0));
                        var rocketPos = position.Value + math.normalize(direction) * planet.Radius * 1.5f;
                        spawner.Spawn(rocketPos, direction, true);                                                                    
                    }
                })
                .Run();
        }
    }
}