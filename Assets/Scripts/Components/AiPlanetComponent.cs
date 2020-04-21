using Unity.Entities;
using Unity.Mathematics;

namespace Components
{
    /// <summary>
    /// Simple AI component
    /// </summary>
    [GenerateAuthoringComponent]
    public struct AiPlanetComponent : IComponentData
    {
        public float TimeTillNextShot;
        public float2 IntervalRangeSec;
    }
}