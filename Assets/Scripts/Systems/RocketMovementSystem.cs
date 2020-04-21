using Components;
using Components.Orbit;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

namespace Systems
{
    public class RocketMovementSystem : JobComponentSystem
    {
        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var deltaTime = Time.DeltaTime;
            return Entities.WithBurst().ForEach((ref Rotation rotation, ref Translation position,
                in VelocityComponent velocity, in RocketComponent rocket) =>
                { 
                    position.Value += (float3) velocity.Velocity * deltaTime;
                    rotation.Value = quaternion.LookRotation((float3) velocity.Velocity, new float3(0, 0, 1));
                    rotation.Value = quaternion.Euler(0, 0, rotation.Value.ToEuler().z);
                })
                .Schedule(inputDeps);
        }
    }
}