using Components;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.UI;

namespace Gui
{
    public class Selector : MonoBehaviour 
    {
        public Entity Entity { get; set; }
        
        // Get the default world containing all entities:
        private EntityManager EntityManager => World
            .DefaultGameObjectInjectionWorld
            .EntityManager;
        
        void Awake() {
            GetComponent<Canvas>().worldCamera = Camera.main;
        }

        void Update ()
        {
            if (EntityManager.HasComponent<Translation>(Entity))
            {
                var translationComponent = EntityManager.GetComponentData<Translation>(Entity);
                transform.position = translationComponent.Value;
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}
