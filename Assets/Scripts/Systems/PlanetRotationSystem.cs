using Components;
using Components.Orbit;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

namespace Systems
{
    public class PlanetRotationSystem : JobComponentSystem
    {
        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var deltaTime = Time.DeltaTime;
            return Entities.WithBurst().ForEach((ref Rotation rotation, 
                    in PlanetComponent planet, in AngularVelocityComponent angular) =>
                { 
                    rotation.Value = math.mul(rotation.Value, quaternion.RotateZ(angular.Value * deltaTime));
                })
                .Schedule(inputDeps);
        }
    }
}