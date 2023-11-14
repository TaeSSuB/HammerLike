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


    private void Start()
    {
        ReplaceObject();
    }

    // 에디터 버튼에 연결될 메서드
    public void ReplaceObject()
    {
        GameObject selectedPrefab = SelectRandomPrefab();
        GameObject spawnedObject = Instantiate(selectedPrefab, transform.position, transform.rotation);

        // 원본 오브젝트 처리
        if (destroyOriginal)
        {
            Destroy(gameObject);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    private GameObject SelectRandomPrefab()
    {
        float total = 0;
        foreach (var item in prefabsWithProbability)
        {
            total += item.probability;
        }

        float randomPoint = Random.value * total;

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
