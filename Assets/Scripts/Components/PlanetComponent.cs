using Unity.Entities;

namespace Components
{
    [GenerateAuthoringComponent]
    public struct PlanetComponent : IComponentData
    {
        public float Radius;
    }
}