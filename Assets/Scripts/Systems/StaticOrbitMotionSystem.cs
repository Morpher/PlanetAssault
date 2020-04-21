using Components.Orbit;
using SimpleKeplerOrbits;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

namespace Systems.Orbit
{
	/// <summary>
	/// Modified part of Kepler Orbits simulation from https://github.com/Karth42/SimpleKeplerOrbits
	/// </summary>
	public class StaticOrbitMotionSystem : JobComponentSystem
	{
		public double GravConstant = 0.1d;
		
		[BurstCompile]
		private struct EllipticAnomaliesTimeProgressionJob : IJobForEach<EccentricityComponent, OrbitPeriodComponent, AnomalyComponent>
		{
			public double DeltaTime;

			public void Execute([ReadOnly] ref EccentricityComponent eccComponent, [ReadOnly] ref OrbitPeriodComponent periodComponent, ref AnomalyComponent anomaliesComponent)
			{
				double eccentricity = eccComponent.Eccentricity;
				if (eccentricity < 1.0)
				{
					double period = periodComponent.Period;
					double meanAnomaly = anomaliesComponent.MeanAnomaly += KeplerOrbitUtils.PI_2 * DeltaTime / period;
					meanAnomaly %= KeplerOrbitUtils.PI_2;
					if (meanAnomaly < 0)
					{
						meanAnomaly = KeplerOrbitUtils.PI_2 - meanAnomaly;
					}
					double eccentricAnomaly = KeplerOrbitUtils.KeplerSolver(meanAnomaly, eccentricity);
					double cosE = math.cos(eccentricAnomaly);
					double trueAnomaly = math.acos((cosE - eccentricity) / (1.0 - eccentricity * cosE));
					if (meanAnomaly > math.PI)
					{
						trueAnomaly = KeplerOrbitUtils.PI_2 - trueAnomaly;
					}
					anomaliesComponent.MeanAnomaly = meanAnomaly;
					anomaliesComponent.TrueAnomaly = trueAnomaly;
					anomaliesComponent.EccentricAnomaly = eccentricAnomaly;
				}
			}
		}

		[BurstCompile]
		private struct HyperbolicAnomaliesTimeProgressionJob : IJobForEach<EccentricityComponent, AttractorMassComponent, SemiMinorMajorAxisComponent, AnomalyComponent>
		{
			public float DeltaTime;
			public double GravitationalConstant;

			public void Execute([ReadOnly]ref EccentricityComponent eccComponent, [ReadOnly]ref AttractorMassComponent attractorMassComponent, [ReadOnly]ref SemiMinorMajorAxisComponent axisComponent, ref AnomalyComponent anomaliesComponent)
			{
				double eccentricity = eccComponent.Eccentricity;
				if (eccentricity >= 1.0)
				{
					double n = math.sqrt(attractorMassComponent.AttractorMass * GravitationalConstant / axisComponent.SemiMajorAxisPow3);
					double meanAnomaly = anomaliesComponent.MeanAnomaly + n * DeltaTime;
					double eccentricAnomaly = KeplerOrbitUtils.KeplerSolverHyperbolicCase(meanAnomaly, eccentricity);
					double trueAnomaly = math.atan2(math.sqrt(eccComponent.EccentricitySqr - 1.0) * math.sinh(eccentricAnomaly), eccentricity - math.cosh(eccentricAnomaly));
					anomaliesComponent.MeanAnomaly = meanAnomaly;
					anomaliesComponent.TrueAnomaly = trueAnomaly;
					anomaliesComponent.EccentricAnomaly = eccentricAnomaly;
				}
			}
		}

		[BurstCompile]
		private struct UpdateFocalPositionsJob : IJobForEach<EccentricityComponent, SemiMinorMajorAxisComponent, AnomalyComponent, OrbitalPositionComponent>
		{
			public void Execute([ReadOnly]ref EccentricityComponent eccComponent, [ReadOnly]ref SemiMinorMajorAxisComponent axisComponent, [ReadOnly]ref AnomalyComponent anomComponent, [WriteOnly]ref OrbitalPositionComponent posComponent)
			{
				double dirX;
				double dirY;
				if (eccComponent.Eccentricity < 1.0)
				{
					dirX = math.sin(anomComponent.EccentricAnomaly) * axisComponent.SemiMinorAxis;
					dirY = math.cos(anomComponent.EccentricAnomaly) * axisComponent.SemiMajorAxis;
				}
				else
				{
					dirX = math.sinh(anomComponent.EccentricAnomaly) * axisComponent.SemiMinorAxis;
					dirY = math.cosh(anomComponent.EccentricAnomaly) * axisComponent.SemiMajorAxis;
				}
				double3 pos = axisComponent.SemiMinorAxisBasis * dirX + axisComponent.SemiMajorAxisBasis * dirY;
				pos += axisComponent.CenterPoint;
				posComponent.Position = pos;
			}
		}
		
		protected override JobHandle OnUpdate(JobHandle inputDeps)
		{
			var gravConstant = GravConstant;
			var deltaTime = Time.DeltaTime;
			var ellipticJob = new EllipticAnomaliesTimeProgressionJob { DeltaTime = deltaTime };
			inputDeps = ellipticJob.Schedule(this, inputDeps);
			var hyperbolicJob = new HyperbolicAnomaliesTimeProgressionJob { DeltaTime = deltaTime, GravitationalConstant = gravConstant};
			inputDeps = hyperbolicJob.Schedule(this, inputDeps);
			//TODO: Figure out how to make elliptic and hyperbolic jobs execute in parallel, not in sequence.
			var moveJob = new UpdateFocalPositionsJob();
			inputDeps = moveJob.Schedule(this, inputDeps);

			return inputDeps;
		}
	}
}