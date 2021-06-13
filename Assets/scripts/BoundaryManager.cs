using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class BoundaryManager : MonoBehaviour
{

    // public List<Transform> m_posts;
    private LineRenderer m_lineRenderer;
    private MeshCollider m_collider;

    void Update()
    {
        if (null == m_lineRenderer)
            m_lineRenderer = GetComponent<LineRenderer>();

        if(null == m_collider)
        {
            m_collider = GetComponent<MeshCollider>();
        }

        //m_posts.Clear();
        int index = 0;
        Vector3[] positions = new Vector3[transform.childCount + 1];
        while (index < transform.childCount)
        {
            positions[index] = transform.GetChild(index).GetChild(0).position;
            index++;
        }
        positions[index] = transform.GetChild(0).GetChild(0).position;
        m_lineRenderer.positionCount = transform.childCount + 1;
        m_lineRenderer.SetPositions(positions);
        GenerateMesh(positions);
    }

    void GenerateMesh(Vector3[] positions)
    {
        Mesh mesh = new Mesh();
        List<Vector3> newPositions = new List<Vector3>(positions);
        List<int> tris = new List<int>();
        foreach (var pos in positions)
        {
            newPositions.Add(pos - Vector3.up);
        }

        for (int index = 0; index < positions.Length-1; index++)
        {
            int i0 = index;
            int i1 = index + 1;
            int i2 = index + positions.Length;
            int i3 = index + positions.Length + 1;

            tris.Add(i0);
            tris.Add(i1);
            tris.Add(i3);

            tris.Add(i3);
            tris.Add(i2);
            tris.Add(i0);
        }

        /*int i0f = positions.Length-1;
        int i1f = 0;
        int i2f = positions.Length-1 + positions.Length;
        int i3f = positions.Length-1 + positions.Length + 1;

        tris.Add(i0f);
        tris.Add(i1f);
        tris.Add(i3f);

        tris.Add(i3f);
        tris.Add(i2f);
        tris.Add(i0f);*/
        
        mesh.vertices = newPositions.ToArray();
        mesh.triangles = tris.ToArray();
        
        m_collider.sharedMesh = mesh;
    }
}
