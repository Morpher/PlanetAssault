using Unity.Entities;

namespace Components.Orbit
{
	[GenerateAuthoringComponent]
	public struct AnomalyComponent : IComponentData
	{
		public double MeanAnomaly;
		public double EccentricAnomaly;
		public double TrueAnomaly;
	}
}