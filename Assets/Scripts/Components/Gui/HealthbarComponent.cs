using Unity.Entities;

namespace Components.Gui
{
    /// <summary>
    /// Marks entity, which needs a healthbar ui
    /// </summary>
    [GenerateAuthoringComponent]
    public struct HealthbarComponent : IComponentData
    {
    }
}