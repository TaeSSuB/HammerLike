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

    private void Awake()
    {
        Renderer rend = GetComponent<Renderer>();
        rend.material = new Material(rend.sharedMaterial);

        // 현재 오브젝트의 머터리얼이 materialsData에 있는지 확인
        // rend.material을 사용하여 일치하는 재질을 찾습니다.
        RandomTileMaterialData selectedData = materialsData.FirstOrDefault(md => md.material == rend.material);

        if (selectedData == null)
        {
            Debug.LogWarning("재질 데이터가 설정되지 않았습니다.");
            return;
        }

        // 랜덤 시드 초기화
        Random.InitState(System.DateTime.Now.Millisecond + this.GetInstanceID());

        MeshFilter mf = GetComponent<MeshFilter>();
        if (mf == null)
        {
            Debug.LogWarning("MeshFilter가 필요합니다.");
            return;
        }

        Vector2[] uvs = mf.mesh.uv;

        for (int i = 0; i < uvs.Length; i += 4) // 4개의 정점이 하나의 타일을 나타냅니다.
        {
            int randomX = Random.Range(0, selectedData.tilesX);
            int randomY = Random.Range(0, selectedData.tilesY);

            Vector2 scale = new Vector2(1.0f / selectedData.tilesX, 1.0f / selectedData.tilesY);
            Vector2 offset = new Vector2(randomX * scale.x, randomY * scale.y);

            uvs[i] = new Vector2(0 + offset.x, 0 + offset.y);
            uvs[i + 1] = new Vector2(scale.x + offset.x, 0 + offset.y);
            uvs[i + 2] = new Vector2(scale.x + offset.x, scale.y + offset.y);
            uvs[i + 3] = new Vector2(0 + offset.x, scale.y + offset.y);
        }

        mf.mesh.uv = uvs;
    }
}
