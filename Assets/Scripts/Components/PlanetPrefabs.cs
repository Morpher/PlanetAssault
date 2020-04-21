using Unity.Entities;

namespace Components
{
    /// <summary>
    /// Stores prefabs for spawning planets
    /// </summary>
    [GenerateAuthoringComponent]
    struct PlanetPrefabs : IComponentData
    {
        public Entity Planet1;
        public Entity Planet2;
        public Entity Planet3;
        public Entity Planet4;
        public Entity Planet5;
        public Entity Planet6;
    }
}
