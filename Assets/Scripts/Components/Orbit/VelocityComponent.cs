using Unity.Entities;
using Unity.Mathematics;

namespace Components.Orbit
{
    [GenerateAuthoringComponent]
    public struct VelocityComponent : IComponentData
    {
        public double3 Velocity;
    }
}