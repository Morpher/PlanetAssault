using System.Collections.Generic;
using SimpleKeplerOrbits;
using UnityEngine;

public class OrbitsRenderer : MonoBehaviour
{
    [SerializeField]
    private LineRenderer lineRenderer;

    private List<GameObject> orbitRenderers = new List<GameObject>();
    
    public void AddOrbit(KeplerOrbitData orbitData, Vector3[] orbitPoints)
    {
        var child = new GameObject();
        child.transform.parent = transform;
        var renderer = child.AddComponent<LineRenderer>();
        renderer.material = lineRenderer.material;
        //renderer.widthCurve = lineRenderer.widthCurve;
        renderer.startWidth = lineRenderer.startWidth;
        renderer.endWidth = lineRenderer.endWidth;
        renderer.positionCount = orbitPoints.Length;
        for (int i = 0; i < orbitPoints.Length; i++)
        {
            renderer.SetPosition(i, orbitPoints[i]);
        }
        renderer.loop = orbitData.Eccentricity < 1.0;
        orbitRenderers.Add(child);
    }

    public void ClearOrbits()
    {
        foreach (var orbitRenderer in orbitRenderers)
        {
            Destroy(orbitRenderer);
        }
        orbitRenderers.Clear();
    }
}