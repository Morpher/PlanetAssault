using Unity.Entities;
using UnityEngine;

namespace Components
{
    [GenerateAuthoringComponent]
    public struct PlayerPlanetComponent : IComponentData
    {
        public KeyCode ShootButton;
        public float ShootDelay;
        public float TimeSinceLastShoot;
    }
}