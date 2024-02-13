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
        segments = Mathf.Min(segments, 10000); // 성능상의 이유로 세그먼트 수 제한

        if (segments < 2) return;

        Vector3[] vertices = new Vector3[segments * 2];
        int[] triangles = new int[(segments - 1) * 6];
        Vector2[] uv = new Vector2[vertices.Length];

        for (int i = 0; i < segments; i++)
        {
            float width = trailRenderer.widthMultiplier * trailRenderer.widthCurve.Evaluate(i / (float)(segments - 1));
            Vector3 position = transform.InverseTransformPoint(trailRenderer.GetPosition(i));
            Vector3 direction;

            // 각 구간에 따른 방향 벡터 계산
            if (i < segments / 3)
            {
                // 우측 대각선 방향
                direction = Quaternion.Euler(0, 45, 0) * Vector3.forward;
            }
            else if (i < 2 * segments / 3)
            {
                // 정면 방향
                direction = Vector3.forward;
            }
            else
            {
                // 좌측 대각선 방향
                direction = Quaternion.Euler(0, -45, 0) * Vector3.forward;
            }

            Vector3 right = Vector3.Cross(direction.normalized, Vector3.up).normalized * width;

            vertices[i * 2] = position + right;
            vertices[i * 2 + 1] = position - right;

            uv[i * 2] = new Vector2(0, i / (float)segments);
            uv[i * 2 + 1] = new Vector2(1, i / (float)segments);

            if (i < segments - 1)
            {
                int baseIndex = i * 6;
                triangles[baseIndex] = i * 2;
                triangles[baseIndex + 1] = i * 2 + 1;
                triangles[baseIndex + 2] = i * 2 + 2;
                triangles[baseIndex + 3] = i * 2 + 2;
                triangles[baseIndex + 4] = i * 2 + 1;
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
