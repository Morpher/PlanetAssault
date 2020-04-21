using Unity.Entities;

namespace Components
{
    public struct DamageComponent : IComponentData
    {
        public float Value;
        /// <summary>
        /// Flag, indication that damage was dealt by the player (for scores) 
        /// </summary>
        public bool DealtByPlayer;
    }
}