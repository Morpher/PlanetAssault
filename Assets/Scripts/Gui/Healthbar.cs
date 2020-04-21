using Components;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.UI;

namespace Gui
{
    public class Healthbar : MonoBehaviour 
    {
        private Image healthbar;
        private Image back;
        private Color fullColor = Color.green;
        private Color emptyColor = Color.red;
        private float3 offset = new float3(0, 0.7f,  0);
        
        public Entity Entity { get; set; }
        
        // Get the default world containing all entities:
        private EntityManager EntityManager => World
            .DefaultGameObjectInjectionWorld
            .EntityManager;
        
        void Awake() {
            healthbar = transform.Find("Image").GetComponent<Image>();
            back = transform.Find("Back").GetComponent<Image>();
            GetComponent<Canvas>().worldCamera = Camera.main;
            healthbar.type = Image.Type.Filled;
            healthbar.enabled = true;
            back.enabled = healthbar.enabled;
        }

        void Update ()
        {
            if (EntityManager.HasComponent<LifeComponent>(Entity) 
                && EntityManager.HasComponent<Translation>(Entity))
            {
                var lifeComponent = EntityManager.GetComponentData<LifeComponent>(Entity);
                var translationComponent = EntityManager.GetComponentData<Translation>(Entity);
                transform.position = translationComponent.Value + offset;
                healthbar.fillAmount = lifeComponent.NormalizedValue;
                healthbar.color = Color.Lerp(emptyColor, fullColor, healthbar.fillAmount);
                healthbar.enabled = lifeComponent.NormalizedValue < 0.999f;
                back.enabled = healthbar.enabled;
            }
            else
            {
                Destroy(gameObject);
            };
        }
    }
}
