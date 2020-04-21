using UnityEngine;

namespace Config
{
	[CreateAssetMenu(fileName = "Planet Spawner Config", menuName = "CustomConfigs/Planet Spawner Config", order = 0)]
	public class PlanetSpawnerConfig : ScriptableObject
	{ 
		public Vector2 SpawnDistanceRange = new Vector2(2f, 5f);

		public Vector2 VelocityRange = new Vector2(0.1f, 2f);

		public Vector2 AngularVelocityRange = new Vector2(-10, 10);

		public float PlanetAttractorRadius = 2f;

		public float PlanetAttractorStrength = 0.02f;

		public Vector2 AiShootIntervalRange = new Vector2(2, 7);
	}
}