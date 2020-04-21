using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

#if UNITY_EDITOR
namespace Editor
{
    [CustomEditor(typeof(GameManager))]
    public class GameManagerEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            var manager = target as GameManager;
            if (EditorApplication.isPlaying)
            {
                if(GUILayout.Button("Restart Game"))
                {
                    manager.RestartGame();
                } 
                else if (GUILayout.Button("Next Level"))
                {
                    manager.OnLevelCompleted();
                }
            }
        }
    }
}
#endif