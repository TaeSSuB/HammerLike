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
