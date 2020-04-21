using Unity.Entities;
using Unity.Mathematics;

namespace Components.Orbit
{
	[GenerateAuthoringComponent]
	public struct OrbitalPositionComponent : IComponentData
	{
		public double3 Position;
	}
}