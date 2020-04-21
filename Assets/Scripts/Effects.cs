using System;
using Cinemachine;
using Components;
using Gui;
using Unity.Entities;
using UnityEngine;

[RequireComponent(typeof(CinemachineImpulseSource))]
public class Effects : MonoBehaviour
{
    [SerializeField]
    private Transform planetExplosionPrefab;
    
    [SerializeField]
    private Transform rocketExplosionPrefab;
  
    [SerializeField]
    private Transform rocketTrailPrefab;   
    
    private CinemachineImpulseSource impulseSource;
    
    void Start()
    {
        impulseSource = GetComponent<CinemachineImpulseSource>();
    }

    public void GenerateEntityEffect(Entity entity, float x, float y, EffectType type)
    {
        switch (type)
        {
            case EffectType.PlanetExplosion:
                impulseSource.GenerateImpulse();
                Instantiate(planetExplosionPrefab, new Vector3(x, y, 0), Quaternion.identity); 
                break;
            case EffectType.RocketExplosion:
                impulseSource.GenerateImpulse();
                Instantiate(rocketExplosionPrefab, new Vector3(x, y, 0), Quaternion.identity);    
                break;
            case EffectType.RocketTrail:
                var effect = Instantiate(rocketTrailPrefab, new Vector3(x, y, 0), Quaternion.identity);
                var trail = effect.GetComponent<EntityEffect>();
                trail.Entity = entity;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }
    }
}

public enum EffectType
{
    PlanetExplosion,
    RocketExplosion,
    RocketTrail
}
