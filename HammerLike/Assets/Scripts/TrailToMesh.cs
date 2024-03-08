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
        if (segments < 2) return; // 최소 2개 이상의 위치가 필요

        Vector3[] vertices = new Vector3[segments * 2];
        int[] triangles = new int[(segments - 1) * 6];
        Vector2[] uv = new Vector2[vertices.Length];

        for (int i = 0; i < segments; i++)
        {
            float width = trailRenderer.widthMultiplier * trailRenderer.widthCurve.Evaluate((float)i / (segments - 1));
            Vector3 position = transform.InverseTransformPoint(trailRenderer.GetPosition(i));

            Vector3 direction = Vector3.forward; // 기본 방향
            if (i < segments - 1)
            {
                Vector3 nextPosition = transform.InverseTransformPoint(trailRenderer.GetPosition(i + 1));
                direction = (nextPosition - position).normalized;
            }
            else if (i > 0)
            {
                // 마지막 세그먼트의 경우, 이전 세그먼트의 방향을 사용
                Vector3 prevPosition = transform.InverseTransformPoint(trailRenderer.GetPosition(i - 1));
                direction = (position - prevPosition).normalized;
            }

            Vector3 right = Vector3.Cross(direction, Vector3.up).normalized * width;
            vertices[i * 2] = position + right;
            vertices[i * 2 + 1] = position - right;

            uv[i * 2] = new Vector2(0, (float)i / (segments - 1));
            uv[i * 2 + 1] = new Vector2(1, (float)i / (segments - 1));

            if (i < segments - 1)
            {
                int baseIndex = i * 6;
                triangles[baseIndex] = i * 2;
                triangles[baseIndex + 1] = i * 2 + 3;
                triangles[baseIndex + 2] = i * 2 + 1;
                triangles[baseIndex + 3] = i * 2;
                triangles[baseIndex + 4] = i * 2 + 2;
                triangles[baseIndex + 5] = i * 2 + 3;
            }
        }

        mesh.Clear();
        mesh.vertices = vertices;
        mesh.uv = uv;
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
