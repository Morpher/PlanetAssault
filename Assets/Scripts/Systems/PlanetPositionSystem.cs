using Components;
using Components.Orbit;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

namespace Systems
{
    public class PlanetPositionSystem : JobComponentSystem
    {
        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            return Entities.WithBurst().ForEach((ref Translation position, in OrbitalPositionComponent orbit, in PlanetComponent planet) =>
                    {
                        position.Value = (float3)orbit.Position;
                    })
                .Schedule(inputDeps);
        }
    }
}