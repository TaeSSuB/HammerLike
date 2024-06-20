using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class RandomTiling : MonoBehaviour
{
    public int textureColumns = 2; // 텍스쳐를 나눌 열의 수
    public int textureRows = 2; // 텍스쳐를 나눌 행의 수

    void Start()
    {
        RandomizeUVs();
    }

    void RandomizeUVs()
    {
        Mesh mesh = GetComponent<MeshFilter>().mesh;
        Vector2[] uvs = mesh.uv;

        // 랜덤한 타일 선택
        int tileX = Random.Range(0, textureColumns);
        int tileY = Random.Range(0, textureRows);

        // 새 UV 좌표 계산
        float tileSizeX = 1f / textureColumns;
        float tileSizeY = 1f / textureRows;
        float minX = tileX * tileSizeX;
        float minY = tileY * tileSizeY;

        for (int i = 0; i < uvs.Length; i++)
        {
            uvs[i] = new Vector2(minX + uvs[i].x * tileSizeX, minY + uvs[i].y * tileSizeY);
        }

        // 새로운 UV 좌표를 Mesh에 적용
        mesh.uv = uvs;
    }
}
