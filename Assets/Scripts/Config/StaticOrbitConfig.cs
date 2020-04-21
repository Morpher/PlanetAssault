using UnityEngine;

namespace Config
{
	[CreateAssetMenu(fileName = "Static Orbit Config", menuName = "CustomConfigs/Static Orbit Config", order = 0)]
	public class StaticOrbitConfig : ScriptableObject
	{
		public double GravConstant = 0.1;
		public float TimeScale = 1;
		public Vector3 RandomPositionDeviationRange;
		public Vector3 RandomVelocityDeviationRange;
	}
}