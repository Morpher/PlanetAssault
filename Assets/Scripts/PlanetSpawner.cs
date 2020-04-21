using Components;
using Components.Gui;
using Components.Orbit;
using Config;
using Reese.Spawning;
using SimpleKeplerOrbits;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// Monobehaviour responsible for spawning planets from entity prefabs
/// Based on this https://reeseschultz.com/spawning-prefabs-with-unity-ecs/
/// </summary>
public class PlanetSpawner : MonoBehaviour
{
    [SerializeField] 
    private Transform attractor;

    [SerializeField] 
    private StaticOrbitConfig aiPlanetsConfig;

    [SerializeField] 
    private StaticOrbitConfig playerPlanetsConfig;

    [SerializeField] 
    private PlanetSpawnerConfig config;
    
    [SerializeField] 
    private OrbitsRenderer orbitRenderer;
    
    [SerializeField] 
    private Camera cam;

    private PlanetPrefabs prefabs;
    private Entity[] entityPrefabs;

    private bool isInited;
    
    private EntityManager EntityManager => World
        .DefaultGameObjectInjectionWorld
        .EntityManager;
    
    public void SpawnPlayerPlanet()
    {
        Init();
        SpawnSystem.Enqueue(MakeRandomSpawn(entityPrefabs, true));
    }
    
    public void SpawnAiPlanets(int count)
    {
        Init();
        for (int i = 0; i < count; i++)
        {
            SpawnSystem.Enqueue(MakeRandomSpawn(entityPrefabs, false));            
        }
    }

    private void Init()
    {
        if (isInited)
        {
            return;
        }
        
        if (cam == null)
        {
            cam = Camera.main;
        }
        
        // Get the entity associated with the prefab:
        prefabs = EntityManager
            .CreateEntityQuery(typeof(PlanetPrefabs))
            .GetSingleton<PlanetPrefabs>();

        entityPrefabs = new[]
        {
            prefabs.Planet1,
            prefabs.Planet2,
            prefabs.Planet3,
            prefabs.Planet4,
            prefabs.Planet5,
            prefabs.Planet6,
        };

        isInited = true;
    }
    
    private Spawn MakeRandomSpawn(Entity[] entityPrefas, bool playerControlled)
    {
        var index = Random.Range(0, entityPrefas.Length);
        var position2d = Random.insideUnitCircle.normalized * Random.Range(config.SpawnDistanceRange.x, config.SpawnDistanceRange.y);
        var position = new Vector3(position2d.x, position2d.y, 0) + attractor.position;
        var directionToAttractor = position - attractor.position;
        var randomSign = Random.value > 0.5f ? 1f : -1f;
        var initialVelocity = Vector3.Cross(directionToAttractor, Vector3.forward).normalized 
                              * (Random.Range(config.VelocityRange.x, config.VelocityRange.y) * randomSign);

        var orbitData = new KeplerOrbitData
        {
            Position = new Vector3d(directionToAttractor),
            AttractorMass = 10,
            GravConst = 1.0,
            Velocity = new Vector3d(initialVelocity),
        };
        orbitData.CalculateNewOrbitData();
        orbitData.Velocity = KeplerOrbitUtils.CalcCircleOrbitVelocity(Vector3d.zero, orbitData.Position, orbitData.AttractorMass, 1f, orbitData.OrbitNormal, orbitData.GravConst);
        orbitData.CalculateNewOrbitData();
        orbitData = ModifyOrbitDataRandomly(orbitData, playerControlled ? playerPlanetsConfig : aiPlanetsConfig);
        
        orbitRenderer.AddOrbit(orbitData, orbitData.GetOrbitPoints(100, attractor.position));

        var spawn = new Spawn().WithPrefab(entityPrefas[index]);
        var translation = new Translation {Value = new float3(position.x, position.y, 0)};
        var angularVelocityComponent = new AngularVelocityComponent
            {Value = Random.Range(config.AngularVelocityRange.x, config.AngularVelocityRange.y) * randomSign};
        var eccentricityComponent = new EccentricityComponent {Eccentricity = orbitData.Eccentricity};
        var anomalyComponent = new AnomalyComponent
        {
            EccentricAnomaly = orbitData.EccentricAnomaly,
            TrueAnomaly = orbitData.TrueAnomaly,
            MeanAnomaly = orbitData.MeanAnomaly,
        };
        var attractorMassComponent = new AttractorMassComponent {AttractorMass = orbitData.AttractorMass};
        var orbitPeriodComponent = new OrbitPeriodComponent {Period = orbitData.Period};
        var semiMinorMajorAxisComponent = new SemiMinorMajorAxisComponent
        {
            SemiMajorAxisBasis = new double3(orbitData.SemiMajorAxisBasis.x, orbitData.SemiMajorAxisBasis.y,
                orbitData.SemiMajorAxisBasis.z),
            SemiMinorAxisBasis = new double3(orbitData.SemiMinorAxisBasis.x, orbitData.SemiMinorAxisBasis.y,
                orbitData.SemiMinorAxisBasis.z),
            SemiMinorAxis = orbitData.SemiMinorAxis,
            SemiMajorAxis = orbitData.SemiMajorAxis,
            CenterPoint = new double3(orbitData.CenterPoint.x, orbitData.CenterPoint.y, orbitData.CenterPoint.z),
            SemiMajorAxisPow3 = math.pow(orbitData.SemiMajorAxis, 3)
        };
        var orbitalPositionComponent = new OrbitalPositionComponent
            {Position = new double3(orbitData.Position.x, orbitData.Position.y, orbitData.Position.z)};
        var rocketAttractorComponent = new RocketAttractorComponent
        {
            Strength = config.PlanetAttractorStrength, 
            MaxDistance = config.PlanetAttractorRadius
        };
        var levelEntity = new LevelEntityComponent();
        var healthbar = new HealthbarComponent();

        if (playerControlled)
        {
            spawn.WithComponentList(translation, 
                angularVelocityComponent, eccentricityComponent, anomalyComponent, 
                attractorMassComponent, orbitPeriodComponent, semiMinorMajorAxisComponent, 
                orbitalPositionComponent, rocketAttractorComponent, levelEntity, healthbar,
                new PlayerPlanetComponent { ShootButton = KeyCode.Space, ShootDelay = 0.5f },
                new PlayerSelectorComponent()
            );
        }
        else
        {
            spawn.WithComponentList(translation, 
                angularVelocityComponent, eccentricityComponent, anomalyComponent, 
                attractorMassComponent, orbitPeriodComponent, semiMinorMajorAxisComponent, 
                orbitalPositionComponent, rocketAttractorComponent, levelEntity, healthbar,
                new AiPlanetComponent { IntervalRangeSec = new float2(config.AiShootIntervalRange.x, config.AiShootIntervalRange.y) }
            );
        }
        
        return spawn;
    }
    
    private static KeplerOrbitData ModifyOrbitDataRandomly(KeplerOrbitData orbitData, StaticOrbitConfig config)
    {
        bool isChanged = false;
        if (config.RandomPositionDeviationRange != Vector3.zero)
        {
            orbitData = orbitData.Clone();
            orbitData.Position += RandomVectorFromRange(config.RandomPositionDeviationRange);
            isChanged = true;
        }
        if (config.RandomVelocityDeviationRange != Vector3.zero)
        {
            if (!isChanged)
            {
                orbitData = orbitData.Clone();
            }
            orbitData.Velocity += RandomVectorFromRange(config.RandomVelocityDeviationRange);
            isChanged = true;
        }
        if (isChanged)
        {
            orbitData.CalculateNewOrbitData();
        }
        return orbitData;
    }

    private static Vector3d RandomVectorFromRange(Vector3 range)
    {
        var maxX = range.x;
        var minX = -range.x;

        var maxY = range.y;
        var minY = -range.y;

        var maxZ = range.z;
        var minZ = -range.z;

        return new Vector3d(
            Random.Range(minX, maxX),
            Random.Range(minY, maxY),
            Random.Range(minZ, maxZ));
    }
}
