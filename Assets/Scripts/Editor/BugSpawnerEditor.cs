using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

#if UNITY_EDITOR
namespace Editor
{
    [CustomEditor(typeof(PlanetSpawner))]
    public class PlanetSpawnerEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            if (EditorApplication.isPlaying 
                && GUILayout.Button("Spawn Planet"))
            {
                (target as PlanetSpawner)?.SpawnAiPlanets(1);
            }
        }
    }
}
#endif