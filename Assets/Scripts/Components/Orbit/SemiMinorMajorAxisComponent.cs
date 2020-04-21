using Unity.Entities;
using Unity.Mathematics;

namespace Components.Orbit
{
    [GenerateAuthoringComponent]
    public struct SemiMinorMajorAxisComponent : IComponentData
    {
        public double3 SemiMinorAxisBasis;
        public double3 SemiMajorAxisBasis;
        public double SemiMinorAxis;
        public double SemiMajorAxis;
        /// <summary>
        /// Offset vector from focus point to center point.
        /// </summary>
        public double3 CenterPoint;

        /// <summary>
        /// Semimajor axis in power of 3. Used in calculations.
        /// </summary>
        public double SemiMajorAxisPow3;
    }
}