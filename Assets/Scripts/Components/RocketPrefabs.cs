using Unity.Entities;

namespace Components
{
    [GenerateAuthoringComponent]
    struct RocketPrefabs : IComponentData
    {
        public Entity FastRocket;
    }
}
