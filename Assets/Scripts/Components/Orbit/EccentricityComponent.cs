using Unity.Entities;

namespace Components.Orbit
{
    [GenerateAuthoringComponent]
    public struct EccentricityComponent : IComponentData
    {
        public double Eccentricity;
        public double EccentricitySqr;
    }
}