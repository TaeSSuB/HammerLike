using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TrailRenderer))]
public class TrailToMesh : MonoBehaviour
{
    private TrailRenderer trailRenderer;
    private Mesh mesh;
    private Vector3[] vertices;
    private int[] triangles;

    void Start()
    {
        trailRenderer = GetComponent<TrailRenderer>();
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh; // MeshFilter 컴포넌트에 메쉬 할당 일일이 하기 좀 그래서
    }

    void Update()
    {
        UpdateTrailMesh();
    }

    void UpdateTrailMesh()
    {
        int segments = trailRenderer.positionCount;
        segments = Mathf.Min(segments, 10000); // 예시로 10000으로 제한

        if (segments < 2) return; // 적어도 2개의 세그먼트가 필요

        vertices = new Vector3[segments * 2];
        triangles = new int[(segments - 1) * 6];

        Vector3 trailPosition = transform.position;
        for (int i = 0; i < segments; i++)
        {
            Vector3 position = trailRenderer.GetPosition(i) - trailPosition;
            vertices[i * 2] = position + Vector3.left * trailRenderer.startWidth * 0.5f;
            vertices[i * 2 + 1] = position + Vector3.right * trailRenderer.startWidth * 0.5f;

            if (i < segments - 1)
            {
                int index = i * 6;
                if (index + 5 < triangles.Length) // 인덱스 검증
                {
                    triangles[index] = i * 2;
                    triangles[index + 1] = triangles[index + 4] = i * 2 + 1;
                    triangles[index + 2] = triangles[index + 3] = (i + 1) * 2;
                    triangles[index + 5] = (i + 1) * 2 + 1;
                }
            }
        }

        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        MeshCollider meshCollider = GetComponent<MeshCollider>();
        if (meshCollider != null)
        {
            meshCollider.sharedMesh = null;
            meshCollider.sharedMesh = mesh;
        }
    }
}
