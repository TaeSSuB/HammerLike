using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class RandomTileMapping : MonoBehaviour
{
    [Header("Material & Texture Settings")]
    public Material targetMaterial;
    public int textureColumns = 2;
    public int textureRows = 2;

    [Header("Attached Materials (Read-Only)")]
    public Material[] attachedMaterials;  // 현재 오브젝트의 렌더러에 연결된 모든 머터리얼을 표시합니다.

    private void Reset()
    {
        UpdateAttachedMaterials();
    }

    private void OnValidate()
    {
        UpdateAttachedMaterials();
    }

    private void Start()
    {
        UpdateAttachedMaterials();

        Mesh mesh = GetComponent<MeshFilter>().mesh;

        if (targetMaterial == null)
        {
            Debug.LogError("Target material is not set!");
            return;
        }

        // 현재 오브젝트의 렌더러에서 연결된 모든 머터리얼을 가져옵니다.
        Material[] currentMaterials = GetComponent<Renderer>().materials;

        if (currentMaterials.Length > 1 && currentMaterials[1] != null) // 1번 엘리멘탈만 변경하고자 합니다.
        {
            currentMaterials[1] = targetMaterial;
            GetComponent<Renderer>().materials = currentMaterials; // 변경된 머터리얼 배열을 다시 렌더러에 설정합니다.
        }

        Vector2[] uvs = mesh.uv;
        int[] triangles = mesh.triangles;
        float tileSizeX = 1f / textureColumns;
        float tileSizeY = 1f / textureRows;

        HashSet<int> processedVertices = new HashSet<int>();

        for (int i = 0; i < triangles.Length; i += 6)
        {
            Vector2 tileOffset = new Vector2(Random.Range(0, textureColumns) * tileSizeX, Random.Range(0, textureRows) * tileSizeY);

            for (int j = 0; j < 6; j++)
            {
                int vertexIndex = triangles[i + j];

                // 이미 처리한 정점을 다시 처리하지 않도록 합니다.
                if (processedVertices.Contains(vertexIndex))
                    continue;

                processedVertices.Add(vertexIndex);

                // UV 좌표를 정규화하여 (0,0)에서 시작하도록 합니다.
                Vector2 normalizedUV = new Vector2(uvs[vertexIndex].x % 1f, uvs[vertexIndex].y % 1f);

                uvs[vertexIndex] = Vector2.Scale(normalizedUV, new Vector2(tileSizeX, tileSizeY)) + tileOffset;
            }
        }

        mesh.uv = uvs;
    }




    private void UpdateAttachedMaterials()
    {
        Renderer renderer = GetComponent<Renderer>();
        if (renderer)
        {
            attachedMaterials = renderer.sharedMaterials;
        }
    }
}
