using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class MiniMapGenerator : MonoBehaviour
{
    public RectTransform miniMapArea; // 미니맵을 표시할 영역
    public GameObject mapIconPrefab; // 미니맵에 표시될 아이콘 프리팹

    public Vector2 worldMapSize = new Vector2(100, 100); // 실제 월드 크기
    public Vector3 worldMapCenter = new Vector3(50, 0, 50); // 실제 월드 중심

    void Start()
    {
        List<RoomData> rooms = new List<RoomData>();
        // 여기서 각 방의 데이터를 추가합니다.
        // 예: rooms.Add(new RoomData{ Position = new Vector3(10, 0, 10), Size = new Vector3(5, 0, 5), Rotation = 0 });

        GenerateMiniMap(rooms);
    }

    public void GenerateMiniMap(List<RoomData> rooms)
    {
        foreach (RoomData room in rooms)
        {
            // 방의 각 부분에 대한 UI 요소 생성
            GameObject mapIcon = Instantiate(mapIconPrefab, miniMapArea);
            RectTransform rectTransform = mapIcon.GetComponent<RectTransform>();

            // 위치, 크기, 회전 설정
            rectTransform.anchoredPosition = CalculateMiniMapPosition(room.Position);
            rectTransform.sizeDelta = CalculateMiniMapSize(room.Size);
            rectTransform.localRotation = Quaternion.Euler(0, 0, room.Rotation);

            // 필요한 경우 추가 설정
            mapIcon.SetActive(true);
        }
    }

    Vector2 CalculateMiniMapPosition(Vector3 worldPosition)
    {
        // 미니맵 내에서의 위치 조정
        // 여기서 scale은 월드 좌표와 미니맵 좌표 간의 비율을 정의합니다.
        float scale = miniMapArea.rect.width / (float)worldMapSize.x;
        float xPosition = (worldPosition.x - worldMapCenter.x) * scale;
        float yPosition = (worldPosition.z - worldMapCenter.z) * scale;

        return new Vector2(xPosition, yPosition);
    }

    Vector2 CalculateMiniMapSize(Vector3 worldSize)
    {
        // 미니맵 내에서의 크기 조정
        float scale = miniMapArea.rect.width / (float)worldMapSize.x;
        float width = worldSize.x * scale;
        float height = worldSize.z * scale;

        return new Vector2(width, height);
    }

}

public class RoomData
{
    public Vector3 Position; // 방의 위치
    public Vector3 Size; // 방의 크기
    public float Rotation; // 방의 회전
}
