using Unity.Entities;

namespace Components
{
    [GenerateAuthoringComponent]
    public struct AngularVelocityComponent : IComponentData
    {
        public float Value;
    }
}