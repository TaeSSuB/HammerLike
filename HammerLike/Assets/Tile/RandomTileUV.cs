using UnityEngine;
using System.Linq;

[RequireComponent(typeof(Renderer))]
public class RandomTileUV : MonoBehaviour
{
    [System.Serializable]
    public class RandomTileMaterialData
    {
        public Material material;
        public int tilesX = 4;
        public int tilesY = 4;
    }

    public RandomTileMaterialData[] materialsData;

    private void Start()
    {
        if (materialsData.Length == 0)
        {
            Debug.LogWarning("재질 데이터가 설정되지 않았습니다.");
            return;
        }

        RandomTileMaterialData selectedData = materialsData[Random.Range(0, materialsData.Length)];

        Renderer rend = GetComponent<Renderer>();
        if (rend.sharedMaterials.Contains(selectedData.material))
        {
            Material mat = selectedData.material;

            // 랜덤한 타일 선택
            int randomX = Random.Range(0, selectedData.tilesX);
            int randomY = Random.Range(0, selectedData.tilesY);

            // UV 스케일 및 오프셋 설정
            Vector2 scale = new Vector2(1.0f / selectedData.tilesX, 1.0f / selectedData.tilesY);
            Vector2 offset = new Vector2(randomX * scale.x, randomY * scale.y);

            mat.mainTextureScale = scale;
            mat.mainTextureOffset = offset;

            rend.material = mat;  // 선택된 재질로 렌더러의 재질 설정
        }
        else
        {
            Debug.LogWarning("지정한 머터리얼이 유효하지 않습니다.");
        }
    }
}