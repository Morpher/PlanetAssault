using Unity.Entities;
using Unity.Mathematics;

namespace Components
{
    [GenerateAuthoringComponent]
    public struct RocketComponent : IComponentData
    {
        public RocketType Type;
        public float Damage;
        public bool OwnedByPlayer;
    }

    public enum RocketType
    {
        Fast,
        Powerful,
    }
}