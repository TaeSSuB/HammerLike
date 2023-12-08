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
        public GameObject worldObject; // 실제 월드 오브젝트
        public Image mapIcon; // 미니맵에 표시될 아이콘
    }

    public List<MapObject> mapObjects; // 표시할 오브젝트 리스트

    public void Update()
    {
        foreach (MapObject mo in mapObjects)
        {
            Vector3 worldPosition = mo.worldObject.transform.position;
            Vector3 mapPosition = (worldPosition - player.position) * mapScale;

            // 미니맵 경계 내에 있는지 확인
            if (Mathf.Abs(mapPosition.x) <= miniMapRect.sizeDelta.x / 2 && Mathf.Abs(mapPosition.z) <= miniMapRect.sizeDelta.y / 2)
            {
                // 미니맵 경계 내에 있으면 아이콘 표시
                mo.mapIcon.rectTransform.anchoredPosition = new Vector2(mapPosition.x, mapPosition.z);
                mo.mapIcon.enabled = true; // 아이콘 활성화
            }
            else
            {
                // 미니맵 경계를 벗어나면 아이콘 비활성화
                mo.mapIcon.enabled = false;
            }
        }
    }



}
