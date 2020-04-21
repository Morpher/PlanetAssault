using Unity.Entities;

namespace Components.Orbit
{
	[GenerateAuthoringComponent]
	public struct AttractorMassComponent : IComponentData
	{
		public double AttractorMass;
	}
}