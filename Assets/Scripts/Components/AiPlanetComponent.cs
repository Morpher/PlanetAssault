using Unity.Entities;
using Unity.Mathematics;

namespace Components
{
    [GenerateAuthoringComponent]
    public struct AiPlanetComponent : IComponentData
    {
        public float TimeTillNextShot;
        public float2 IntervalRangeSec;
    }
}