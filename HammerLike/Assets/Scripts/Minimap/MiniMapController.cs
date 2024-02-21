using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class MiniMapController : MonoBehaviour
{
    public Transform player; // 플레이어의 Transform
    public RectTransform miniMapRect; // 미니맵의 RectTransform
    public float mapScale = 5f; // 미니맵의 스케일

    [System.Serializable]
    public class MapObject
    {
        public string objectTag; // 월드 오브젝트의 태그
        public Image mapIconPrefab; // 미니맵 아이콘 프리팹
    }

    public List<MapObject> mapObjects; // 표시할 오브젝트 리스트
    private Dictionary<string, List<Image>> mapIcons = new Dictionary<string, List<Image>>(); // 생성된 아이콘들을 저장

    void Start()
    {
        // 각 태그에 대해 아이콘 리스트 초기화
        foreach (var mapObject in mapObjects)
        {
            mapIcons[mapObject.objectTag] = new List<Image>();
        }
    }

    public void Update()
    {
        foreach (MapObject mo in mapObjects)
        {
            GameObject[] worldObjects = GameObject.FindGameObjectsWithTag(mo.objectTag);

            // 필요한 만큼 아이콘 생성 또는 재사용
            while (mapIcons[mo.objectTag].Count < worldObjects.Length)
            {
                Image newIcon = Instantiate(mo.mapIconPrefab, miniMapRect);
                mapIcons[mo.objectTag].Add(newIcon);
            }

            // 각 오브젝트에 대해 아이콘 처리
            for (int i = 0; i < worldObjects.Length; i++)
            {
                GameObject worldObject = worldObjects[i];
                Image mapIcon = mapIcons[mo.objectTag][i];

                // Monster 컴포넌트가 있고, curHp가 0보다 큰 경우에만 아이콘을 표시
                Monster monster = worldObject.GetComponent<Monster>();
                if (monster != null && monster.stat.curHp > 0)
                {
                    Vector3 worldPosition = worldObject.transform.position;
                    Vector3 mapPosition = (worldPosition - player.position) * mapScale;

                    // 미니맵 경계 내에 있는지 확인
                    if (Mathf.Abs(mapPosition.x) <= miniMapRect.sizeDelta.x / 2 && Mathf.Abs(mapPosition.z) <= miniMapRect.sizeDelta.y / 2)
                    {
                        // 미니맵 경계 내에 있으면 아이콘 표시
                        mapIcon.rectTransform.anchoredPosition = new Vector2(mapPosition.x, mapPosition.z);
                        mapIcon.enabled = true;
                    }
                    else
                    {
                        // 미니맵 경계를 벗어나면 아이콘 비활성화
                        mapIcon.enabled = false;
                    }
                }
                else
                {
                    // Monster 컴포넌트가 없거나 curHp가 0 이하이면 아이콘을 비활성화
                    mapIcon.enabled = false;
                }
            }
        }
    }

}