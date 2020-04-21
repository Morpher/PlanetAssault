using Unity.Entities;
using Unity.Mathematics;

namespace Components
{
    [GenerateAuthoringComponent]
    public struct EffectComponent : IComponentData
    {
        public bool EnableScreenshake;
    }
}