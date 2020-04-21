using Unity.Entities;

namespace Components
{
    public struct DamageComponent : IComponentData
    {
        public float Value;
        public bool DealtByPlayer;
    }
}