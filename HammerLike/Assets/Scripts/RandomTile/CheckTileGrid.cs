using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class CheckTileGrid : MonoBehaviour
{
    private void Start()
    {
        Mesh mesh = GetComponent<MeshFilter>().mesh;

        int triangleCount = mesh.triangles.Length / 3;
        int quadCount = triangleCount / 2; // 한 quad당 2개의 삼각형으로 구성됩니다.

        int vertexCount = mesh.vertices.Length;

        Debug.Log($"This mesh contains {vertexCount} vertices.");
        Debug.Log($"This mesh contains {quadCount} quads.");
    }
}
