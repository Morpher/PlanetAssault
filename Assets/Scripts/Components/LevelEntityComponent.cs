using Unity.Entities;

namespace Components
{
    /// <summary>
    /// Indicates entities which should be destroyed on game restart
    /// </summary>
    [GenerateAuthoringComponent]
    public struct LevelEntityComponent : IComponentData
    {
    }
}