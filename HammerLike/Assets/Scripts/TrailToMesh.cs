using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TrailRenderer), typeof(MeshFilter), typeof(MeshCollider))]
public class TrailToMesh : MonoBehaviour
{
    private TrailRenderer trailRenderer;
    private Mesh mesh;
    private MeshCollider meshCollider;

    void Start()
    {
        trailRenderer = GetComponent<TrailRenderer>();
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        meshCollider = GetComponent<MeshCollider>();
        //meshCollider.convex = false; // 볼록 메쉬 설정 해제
    }

    void Update()
    {
        if (trailRenderer.enabled)
        {
            UpdateTrailMesh();
        }
        else
        {
            ClearMesh();
        }
    }

    void UpdateTrailMesh()
    {
        int segments = trailRenderer.positionCount;
        segments = Mathf.Min(segments, 10000); // 성능을 위해 세그먼트 제한

        if (segments < 2) return;

        Vector3[] vertices = new Vector3[segments * 2];
        int[] triangles = new int[(segments - 1) * 6];
        Vector2[] uv = new Vector2[vertices.Length];

        for (int i = 0; i < segments; i++)
        {
            float width = trailRenderer.widthMultiplier * trailRenderer.widthCurve.Evaluate(i / (float)(segments - 1)); // 너비 조정
            Vector3 position = transform.InverseTransformPoint(trailRenderer.GetPosition(i)); // 로컬 좌표 변환
            Vector3 direction = (i > 0) ? position - transform.InverseTransformPoint(trailRenderer.GetPosition(i - 1)) : Vector3.forward;

            // 카메라 방향 대신 고정된 방향 사용
            Vector3 right = Vector3.Cross(direction, Vector3.up).normalized;

            vertices[i * 2] = position - right * width;
            vertices[i * 2 + 1] = position + right * width;

            // UV 매핑
            float uvProgress = i / (float)(segments - 1);
            uv[i * 2] = new Vector2(0, uvProgress);
            uv[i * 2 + 1] = new Vector2(1, uvProgress);

            if (i < segments - 1)
            {
                int index = i * 6;
                triangles[index] = i * 2;
                triangles[index + 1] = (i + 1) * 2;
                triangles[index + 2] = i * 2 + 1;

                triangles[index + 3] = i * 2 + 1;
                triangles[index + 4] = (i + 1) * 2;
                triangles[index + 5] = (i + 1) * 2 + 1;
            }
        }

        mesh.Clear();
        mesh.vertices = vertices;
        mesh.uv = uv; // UV 할당
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        meshCollider.sharedMesh = mesh;
    }

    void ClearMesh()
    {
        mesh.Clear();
        meshCollider.sharedMesh = null;
    }
}
