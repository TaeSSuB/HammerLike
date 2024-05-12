using UnityEngine;

public class RandomTiling : MonoBehaviour
{
    public Material material; // 사용할 Material
    public int textureColumns = 2; // 텍스쳐를 나눌 열의 수
    public int textureRows = 2; // 텍스쳐를 나눌 행의 수

    void Start()
    {
        UpdateMaterialProperties();
    }

    void UpdateMaterialProperties()
    {
        if (material == null)
        {
            Debug.LogError("Material is not assigned.");
            return;
        }

        // Tiling 값 설정
        material.mainTextureScale = new Vector2(1f / textureColumns, 1f / textureRows);

        // Offset 값 랜덤 설정
        float offsetX = Random.Range(0, textureColumns) * (1f / textureColumns);
        float offsetY = Random.Range(0, textureRows) * (1f / textureRows);
        material.mainTextureOffset = new Vector2(offsetX, offsetY);
    }
}
