using UnityEngine;
using System.Collections.Generic;

public class RandomObjectSpawner : MonoBehaviour
{
    [System.Serializable]
    public class PrefabProbability
    {
        public GameObject prefab;
        public float probability; // 이 비율은 전체 합계가 1이 되어야 함
    }

    public List<PrefabProbability> prefabsWithProbability;
    public bool destroyOriginal = false;
    public bool useParentScale = true;
    public bool useParentRotation = true;
    public bool useParentPosition = true;

    private void Start()
    {
        ReplaceObject();
        MeshBakerManager bakerManager = FindObjectOfType<MeshBakerManager>();
        if (bakerManager != null)
        {
            bakerManager.BakeMeshes(); // 메시 병합
        }
    }

    public void ReplaceObject()
    {
        GameObject selectedPrefab = SelectRandomPrefab();

        Quaternion rotation = useParentRotation ? transform.rotation : selectedPrefab.transform.rotation;
        Vector3 scale = useParentScale ? transform.localScale : selectedPrefab.transform.localScale;

        Vector3 position;
        if (useParentPosition)
        {
            position = transform.position;
        }
        else
        {
            position = transform.position + (selectedPrefab.transform.position);
        }

        // 새 오브젝트를 생성하고 부모 오브젝트에 연결
        GameObject spawnedObject = Instantiate(selectedPrefab, position, rotation, transform.parent);
        spawnedObject.transform.localScale = scale;

        // 원본 오브젝트 처리
        if (destroyOriginal)
        {
            Destroy(gameObject);
        }
        else
        {
            // 원본 오브젝트를 비활성화하지 않고 그대로 둠
        }
    }


    private GameObject SelectRandomPrefab()
    {
        float total = 0;
        foreach (var item in prefabsWithProbability)
        {
            total += item.probability; // 모든 확률의 합을 계산
        }

        float randomPoint = Random.value * total; // 0과 total 사이의 무작위 값을 선택

        foreach (var item in prefabsWithProbability)
        {
            if (randomPoint < item.probability)
                return item.prefab;
            else
                randomPoint -= item.probability;
        }

        return null; // 이 경우는 발생하지 않아야 함
    }

}
