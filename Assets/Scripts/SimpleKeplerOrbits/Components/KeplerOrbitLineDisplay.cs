#region Copyright
/// Copyright © 2017-2018 Vlad Kirpichenko
/// 
/// Author: Vlad Kirpichenko 'itanksp@gmail.com'
/// Licensed under the MIT License.
/// License: http://opensource.org/licenses/MIT
#endregion

using UnityEngine;

namespace SimpleKeplerOrbits
{
    /// <summary>
    /// Component for displaying current orbit curve in editor and in game.
    /// </summary>
    /// <seealso cref="UnityEngine.MonoBehaviour" />
    [ExecuteInEditMode]
    public class KeplerOrbitLineDisplay : MonoBehaviour
    {
        /// <summary>
        /// The orbit curve precision.
        /// </summary>
        public int OrbitPointsCount = 50;

        /// <summary>
        /// The maximum orbit distance of orbit display in world units.
        /// </summary>
        public float MaxOrbitWorldUnitsDistance = 1000f;

        /// <summary>
        /// The line renderer reference.
        /// </summary>
        public LineRenderer LineRendererReference;

        [Header("Gizmo display options:")]
        public bool ShowOrbitGizmoInEditor = true;
        public bool ShowOrbitGizmoWhileInPlayMode = true;
        
        public Vector3[] OrbitPoints { get; set; }
        public KeplerOrbitData OrbitData { get; set; }
        
        private void LateUpdate()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                return;
            }
#endif
            if (LineRendererReference != null 
                && OrbitPoints != null 
                && OrbitData != null)
            {
                LineRendererReference.positionCount = OrbitPoints.Length;
                for (int i = 0; i < OrbitPoints.Length; i++)
                {
                    LineRendererReference.SetPosition(i, OrbitPoints[i]);
                }
                LineRendererReference.loop = OrbitData.Eccentricity < 1.0;
            }
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (ShowOrbitGizmoInEditor)
            {
                if (!Application.isPlaying || ShowOrbitGizmoWhileInPlayMode)
                {
                        ShowVelocity();
                        ShowOrbit();
                }
            }
        }

        private void ShowVelocity()
        {
            if (OrbitData != null)
            {
                var velocity = (Vector3)OrbitData.GetVelocityAtEccentricAnomaly(OrbitData.EccentricAnomaly);
                Gizmos.DrawLine(transform.position, transform.position + velocity);
            }
       }

        private void ShowOrbit()
        {
            if (OrbitPoints != null)
            {
                Gizmos.color = new Color(1, 1, 1, 0.3f);
                for (int i = 0; i < OrbitPoints.Length - 1; i++)
                {
                    Gizmos.DrawLine(OrbitPoints[i], OrbitPoints[i + 1]);
                }                
            }
        }

        [ContextMenu("AutoFind LineRenderer")]
        private void AutoFindLineRenderer()
        {
            if (LineRendererReference == null)
            {
                LineRendererReference = GetComponent<LineRenderer>();
            }
        }
#endif
    }
}