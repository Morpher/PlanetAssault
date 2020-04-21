using Unity.Entities;

namespace Components.Orbit
{
    [GenerateAuthoringComponent]
    public struct OrbitPeriodComponent : IComponentData
    {
        public double Period;
    }
}