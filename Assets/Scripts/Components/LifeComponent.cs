using Unity.Entities;

namespace Components
{
    [GenerateAuthoringComponent]
    public struct LifeComponent : IComponentData
    {
        public float Value;
        public float MaxValue;
        public float NormalizedValue => Value / MaxValue;
    }
}