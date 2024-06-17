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
    }

    public void ReplaceObject()
    {
        GameObject selectedPrefab = SelectRandomPrefab();

        if (selectedPrefab == null)
        {
            Debug.LogError("No valid prefab selected.");
            return;
        }

        Quaternion rotation = useParentRotation ? transform.rotation : selectedPrefab.transform.rotation;
        Vector3 scale = useParentScale ? transform.localScale : selectedPrefab.transform.localScale;
        Vector3 position = useParentPosition ? transform.position : transform.position + (selectedPrefab.transform.position);

        GameObject spawnedObject = Instantiate(selectedPrefab, position, rotation, transform.parent);
        spawnedObject.transform.localScale = scale;

        if (destroyOriginal)
        {
            Destroy(gameObject);
        }
    }

    private GameObject SelectRandomPrefab()
    {
        List<PrefabProbability> validPrefabs = new List<PrefabProbability>();
        float totalProbability = 0;

        // 유효한 프리팹과 그 확률을 수집합니다.
        foreach (var item in prefabsWithProbability)
        {
            if (item.prefab != null)
            {
                validPrefabs.Add(item);
                totalProbability += item.probability;
            }
        }

        if (validPrefabs.Count == 0)
        {
            Debug.LogError("No valid prefabs available for spawning.");
            return null;
        }

        // 랜덤 값을 통해 프리팹을 선택합니다.
        float randomPoint = Random.value * totalProbability;

        foreach (var item in validPrefabs)
        {
            if (randomPoint < item.probability)
                return item.prefab;
            else
                randomPoint -= item.probability;
        }

        return null; // 이 경우는 발생하지 않아야 함
    }
}
