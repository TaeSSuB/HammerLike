using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class RandomTileUV : MonoBehaviour
{
    [Tooltip("머터리얼 인덱스 (대부분의 경우 0)")]
    public int materialIndex = 0;

    [Tooltip("타일맵의 가로 타일 수")]
    public int tilesX = 4;

    [Tooltip("타일맵의 세로 타일 수")]
    public int tilesY = 4;

    private void Start()
    {
        Renderer rend = GetComponent<Renderer>();
        if (rend.materials.Length > materialIndex)
        {
            Material mat = rend.materials[materialIndex];

            // 랜덤한 타일 선택
            int randomX = Random.Range(0, tilesX);
            int randomY = Random.Range(0, tilesY);

            // UV 스케일 및 오프셋 설정
            Vector2 scale = new Vector2(1.0f / tilesX, 1.0f / tilesY);
            Vector2 offset = new Vector2(randomX * scale.x, randomY * scale.y);

            mat.mainTextureScale = scale;
            mat.mainTextureOffset = offset;
        }
        else
        {
            Debug.LogWarning("지정한 머터리얼 인덱스가 유효하지 않습니다.");
        }
    }
}
