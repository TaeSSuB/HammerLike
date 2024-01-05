using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonMapGenerator : MonoBehaviour
{
    public GameObject startRoomPrefab;
    public GameObject eliteRoomPrefab;
    public GameObject bossRoomPrefab;

    public List<GameObject> normalRoomPrefabs; // NormalRoom 프리팹 리스트

    public int normalRoomCount = 5; // 생성할 NormalRoom의 수
    private int remainingNormalRooms; // 아직 생성하지 않은 NormalRoom의 수

    private List<GameObject> roomObjects; // 방 GameObject 리스트

    public LineRenderer lineRenderer; // LineRenderer 컴포넌트

    public int maxXSize = 1000;
    public int maxYSize = 1000;
    public int divisionCount = 5;
    private bool hasPlacedStartRoom = false;
    private bool hasPlacedEliteRoom = false;
    private bool hasPlacedBossRoom = false;
    private List<BSPNode> leafNodes;

    private class BSPNode
    {
        public BSPNode left;
        public BSPNode right;
        public Rect room;
    }

    void Start()
    {
        remainingNormalRooms = normalRoomCount;
        leafNodes = new List<BSPNode>();
        GenerateDungeon();
    }

    void GenerateDungeon()
    {
        BSPNode rootNode = new BSPNode
        {
            room = new Rect(0, 0, maxXSize, maxYSize) // 예시 크기, 실제 게임에 맞게 조정 필요
        };

        SplitNode(rootNode, divisionCount);
        PlaceRooms();
    }

    void SplitNode(BSPNode node, int depth)
    {
        if (depth <= 0)
        {
            leafNodes.Add(node); // 리프 노드를 리스트에 추가
            return;
        }

        // 수평 또는 수직 분할을 무작위로 결정
        bool splitH = Random.Range(0, 2) == 0;

        if (splitH)
        {
            // 수평 분할
            float splitPos = Random.Range(0.4f, 0.6f); // 분할 위치 비율
            node.left = new BSPNode
            {
                room = new Rect(node.room.x, node.room.y, node.room.width, node.room.height * splitPos)
            };
            node.right = new BSPNode
            {
                room = new Rect(node.room.x, node.room.y + node.room.height * splitPos, node.room.width, node.room.height * (1 - splitPos))
            };
        }
        else
        {
            // 수직 분할
            float splitPos = Random.Range(0.4f, 0.6f); // 분할 위치 비율
            node.left = new BSPNode
            {
                room = new Rect(node.room.x, node.room.y, node.room.width * splitPos, node.room.height)
            };
            node.right = new BSPNode
            {
                room = new Rect(node.room.x + node.room.width * splitPos, node.room.y, node.room.width * (1 - splitPos), node.room.height)
            };
        }

        SplitNode(node.left, depth - 1);
        SplitNode(node.right, depth - 1);
    }

    void PlaceRooms()
    {
        roomObjects = new List<GameObject>();

        GameObject startRoom = null;
        GameObject bossRoom = null;
        List<GameObject> otherRooms = new List<GameObject>();

        foreach (var node in leafNodes)
        {
            GameObject roomPrefab = ChooseRoomPrefab();
            if (roomPrefab != null) // null이 아닐 때만 방을 생성
            {
                GameObject room = Instantiate(roomPrefab, new Vector3(node.room.x, 0, node.room.y), Quaternion.identity);

                if (roomPrefab == startRoomPrefab)
                    startRoom = room;
                else if (roomPrefab == bossRoomPrefab)
                    bossRoom = room;
                else
                    otherRooms.Add(room);
            }
        }

        // 순서대로 방 배치: StartRoom -> 나머지 방들 -> BossRoom
        if (startRoom != null)
            roomObjects.Add(startRoom);
        roomObjects.AddRange(otherRooms);
        if (bossRoom != null)
            roomObjects.Add(bossRoom);

        CreateCorridors();
    }

    void CreateCorridors()
    {
        List<Vector3> points = new List<Vector3>();

        for (int i = 0; i < roomObjects.Count - 1; i++)
        {
            Vector3 start = roomObjects[i].transform.position;
            Vector3 end = roomObjects[i + 1].transform.position;

            // L자 모양 경로 생성
            points.Add(start);
            points.Add(new Vector3(start.x, start.y, end.z));
            points.Add(end);
        }

        lineRenderer.positionCount = points.Count;
        lineRenderer.SetPositions(points.ToArray());
    }


    GameObject ChooseRoomPrefab()
    {
        if (!hasPlacedStartRoom)
        {
            hasPlacedStartRoom = true;
            return startRoomPrefab;
        }
        else if (remainingNormalRooms > 0)
        {
            remainingNormalRooms--;
            return normalRoomPrefabs[Random.Range(0, normalRoomPrefabs.Count)];
        }
        else if (!hasPlacedEliteRoom)
        {
            hasPlacedEliteRoom = true;
            return eliteRoomPrefab;
        }
        else if (!hasPlacedBossRoom)
        {
            hasPlacedBossRoom = true;
            return bossRoomPrefab;
        }
        else
        {
            // 모든 특별 방과 필요한 수의 NormalRoom이 배치된 후
            // 여기서는 null을 반환하거나 다른 유형의 방을 반환할 수 있습니다.
            return null; // 또는 다른 방 유형의 프리팹 반환
        }
    }

}
