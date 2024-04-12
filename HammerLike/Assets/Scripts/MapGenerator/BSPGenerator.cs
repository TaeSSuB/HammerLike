using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RoomType
{
    None,
    StartRoom,
    BossRoom,
    EliteRoom,
    NormalRoom
}

public class BSPNode
{
    public BSPNode leftChild;
    public BSPNode rightChild;
    public Rect room;
    public RoomType roomType = RoomType.None;
    public GameObject roomObject; // 방 오브젝트
    public bool isConnectedNorth;
    public bool isConnectedEast;
    public bool isConnectedSouth;
    public bool isConnectedWest;
    // 기타 필요한 변수들
}

public class BSPGenerator : MonoBehaviour
{
    public GameObject startRoomPrefab;
    public GameObject bossRoomPrefab;
    public GameObject eliteRoomPrefab;
    public GameObject[] normalRoomPrefabs;
    public int normalRoomCount; // NormalRoom의 개수
    public int mapWidth;
    public int mapHeight;
    public int divideCount; // 분할 횟수를 정하는 변수

    void Start()
    {
        BSPNode root = new BSPNode();
        root.room = new Rect(0, 0, mapWidth, mapHeight);
        SplitNode(root, divideCount);
        PlaceSpecialRooms(root); // 특별한 방 배치
        PlaceNormalRooms(root, normalRoomCount); // 일반 방 배치
        ConnectRooms(root); // 길 생성 및 연결
        DrawLines(root);
    }

    public void ReGenerator()
    {
        StartCoroutine(ReGenerateCoroutine());
    }

    private IEnumerator ReGenerateCoroutine()
    {
        // 기존에 생성된 모든 자식 오브젝트 제거
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        // 모든 오브젝트 제거 후 다음 프레임까지 기다림
        yield return null;

        // 새로운 BSP 구조 생성
        BSPNode root = new BSPNode();
        root.room = new Rect(0, 0, mapWidth, mapHeight);
        SplitNode(root, divideCount);
        PlaceSpecialRooms(root); // 특별한 방 배치
        PlaceNormalRooms(root, normalRoomCount); // 일반 방 배치
        ConnectRooms(root); // 길 생성 및 연결
        DrawLines(root);
    }

    void DrawLines(BSPNode node)
    {
        if (node == null) return;

        // LineRenderer 생성 및 설정
        GameObject lineObj = new GameObject("Line");
        lineObj.transform.parent = this.transform; // BSPGenerator의 자식으로 설정
        LineRenderer lineRenderer = lineObj.AddComponent<LineRenderer>();
        lineRenderer.material = new Material(Shader.Find("Sprites/Default")); // 적절한 쉐이더 선택
        lineRenderer.widthMultiplier = 0.05f; // 선의 너비
        lineRenderer.positionCount = 5; // 사각형을 그리기 위해 5개의 점이 필요

        // 사각형의 모서리 좌표 계산
        Vector3[] corners = new Vector3[5];
        corners[0] = new Vector3(node.room.xMin, 0, node.room.yMin);
        corners[1] = new Vector3(node.room.xMax, 0, node.room.yMin);
        corners[2] = new Vector3(node.room.xMax, 0, node.room.yMax);
        corners[3] = new Vector3(node.room.xMin, 0, node.room.yMax);
        corners[4] = corners[0]; // 시작점으로 돌아옴

        // LineRenderer에 점들을 설정
        lineRenderer.SetPositions(corners);

        // 자식 노드에 대해서도 같은 작업 수행
        DrawLines(node.leftChild);
        DrawLines(node.rightChild);
    }
    void SplitNode(BSPNode node, int depth)
    {
        if (depth <= 0) return;

        // 랜덤 분할 비율 (0.4에서 0.6 사이)
        float splitRatio = Random.Range(0.4f, 0.6f);

        // 수평이 수직보다 길면 수평으로, 그렇지 않으면 수직으로 분할
        if (node.room.width > node.room.height)
        {
            // 수평 분할
            float split = node.room.width * splitRatio;
            node.leftChild = new BSPNode { room = new Rect(node.room.x, node.room.y, split, node.room.height) };
            node.rightChild = new BSPNode { room = new Rect(node.room.x + split, node.room.y, node.room.width - split, node.room.height) };
        }
        else
        {
            // 수직 분할
            float split = node.room.height * splitRatio;
            node.leftChild = new BSPNode { room = new Rect(node.room.x, node.room.y, node.room.width, split) };
            node.rightChild = new BSPNode { room = new Rect(node.room.x, node.room.y + split, node.room.width, node.room.height - split) };
        }

        // 재귀적으로 자식 노드들을 분할
        SplitNode(node.leftChild, depth - 1);
        SplitNode(node.rightChild, depth - 1);
    }

    

    void ConnectRooms(BSPNode node)
    {
        if (node == null || node.leftChild == null || node.rightChild == null) return;

        // 두 자식 노드의 중심점 찾기
        Vector3 centerA = new Vector3(
            node.leftChild.room.x + node.leftChild.room.width / 2,
            0, // y축 위치는 필요에 따라 조정
            node.leftChild.room.y + node.leftChild.room.height / 2
        );

        Vector3 centerB = new Vector3(
            node.rightChild.room.x + node.rightChild.room.width / 2,
            0, // y축 위치는 필요에 따라 조정
            node.rightChild.room.y + node.rightChild.room.height / 2
        );

        // L자 형태의 길 생성
        Vector3 midPoint = new Vector3(centerA.x, 0, centerB.z);
        CreatePath(centerA, midPoint, node.leftChild, node.rightChild);
        CreatePath(midPoint, centerB, node.leftChild, node.rightChild);

        // 자식 노드에 대해서도 길 생성
        ConnectRooms(node.leftChild);
        ConnectRooms(node.rightChild);


    }

    void CreatePath(Vector3 start, Vector3 end, BSPNode nodeA, BSPNode nodeB)
    {
        GameObject lineObj = new GameObject("PathLine");
        lineObj.transform.parent = this.transform;
        LineRenderer lineRenderer = lineObj.AddComponent<LineRenderer>();
        Rigidbody rb = lineObj.AddComponent<Rigidbody>();
        BoxCollider collider = lineObj.AddComponent<BoxCollider>();

        // LineRenderer에 빨간색 재질 설정
        Material redMaterial = new Material(Shader.Find("Sprites/Default"));
        redMaterial.color = Color.red; // 빨간색으로 설정
        lineRenderer.material = redMaterial;

        // Rigidbody 설정
        rb.isKinematic = true;
        rb.useGravity = false;

        // Collider 설정
        collider.isTrigger = true;

        lineRenderer.widthMultiplier = 0.1f;
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, end);
        // Collider의 위치, 크기, 방향 조정
        Vector3 midPoint = (start + end) / 2;
        lineObj.transform.position = midPoint;

        float lineLength = Vector3.Distance(start, end);
        collider.size = new Vector3(0.1f, 0.1f, lineLength); // 콜라이더의 두께와 높이는 적절히 조정

        // Collider를 라인의 방향으로 회전
        Vector3 lineDirection = (end - start).normalized;
        Quaternion rotation = Quaternion.LookRotation(lineDirection);
        lineObj.transform.rotation = rotation;

        lineObj.AddComponent<PathLineCollision>();
    }
    void DisableEntrance(BSPNode node, string direction)
    {
        if (node.roomObject != null)
        {
            Transform interactiveModules = node.roomObject.transform.Find("Interactive Modules");
            if (interactiveModules != null)
            {
                Transform entrance = interactiveModules.Find("Entrance_" + direction);
                if (entrance != null)
                    entrance.gameObject.SetActive(false);
            }
        }
    }


    void PlaceSpecialRooms(BSPNode node)
    {
        // StartRoom, BossRoom, EliteRoom의 위치를 결정하는 로직
        List<BSPNode> leafNodes = GetLeafNodes(node);
        if (leafNodes.Count < 3) return; // 적어도 세 개의 잎 노드가 필요

        // StartRoom 배치
        BSPNode startNode = leafNodes[Random.Range(0, leafNodes.Count)];
        InstantiateRoom(startRoomPrefab, startNode.room, startNode); // 여기 수정
        startNode.roomType = RoomType.StartRoom;

        // BossRoom 배치 - StartRoom에서 가장 먼 곳에 위치
        leafNodes.Remove(startNode);
        BSPNode bossNode = GetFurthestNode(startNode, leafNodes);
        InstantiateRoom(bossRoomPrefab, bossNode.room, bossNode); // 여기 수정
        bossNode.roomType = RoomType.BossRoom;

        // EliteRoom 배치 - 나머지 잎 노드 중 하나 선택
        leafNodes.Remove(bossNode);
        BSPNode eliteNode = leafNodes[Random.Range(0, leafNodes.Count)];
        InstantiateRoom(eliteRoomPrefab, eliteNode.room, eliteNode); // 여기 수정
        eliteNode.roomType = RoomType.EliteRoom;
    }

    void PlaceNormalRooms(BSPNode node, int count)
    {
        // 기존 로직에서 수정하여 이미 특별한 방이 배치된 노드는 건너뛰고 일반 방만 배치
        List<BSPNode> leafNodes = GetLeafNodes(node);
        foreach (BSPNode leafNode in leafNodes)
        {
            if (count <= 0 || leafNode.roomType != RoomType.None) continue;
            InstantiateRoom(normalRoomPrefabs[Random.Range(0, normalRoomPrefabs.Length)], leafNode.room, leafNode); // 여기 수정
            leafNode.roomType = RoomType.NormalRoom;
            count--;
        }
    }

    void InstantiateRoom(GameObject prefab, Rect room, BSPNode node)
    {
        Renderer prefabRenderer = prefab.GetComponent<Renderer>();
        if (prefabRenderer != null)
        {
            Vector3 prefabSize = prefabRenderer.bounds.size;
            if (prefabSize.x <= room.width && prefabSize.z <= room.height)
            {
                Vector3 position = new Vector3(
                    room.x + (room.width - prefabSize.x) / 2,
                    0,
                    room.y + (room.height - prefabSize.z) / 2
                );
                GameObject roomObject = Instantiate(prefab, position, Quaternion.identity);
                roomObject.transform.parent = this.transform;
                node.roomObject = roomObject; // 생성된 방 오브젝트 저장
            }
        }
    }




    List<BSPNode> GetLeafNodes(BSPNode node)
    {
        // 잎 노드들을 반환하는 로직
        List<BSPNode> leafNodes = new List<BSPNode>();
        if (node == null) return leafNodes;
        if (node.leftChild == null && node.rightChild == null)
        {
            leafNodes.Add(node);
        }
        else
        {
            leafNodes.AddRange(GetLeafNodes(node.leftChild));
            leafNodes.AddRange(GetLeafNodes(node.rightChild));
        }
        return leafNodes;
    }

    BSPNode GetFurthestNode(BSPNode startNode, List<BSPNode> nodes)
    {
        // StartNode로부터 가장 먼 노드를 찾는 로직
        BSPNode furthestNode = null;
        float maxDistance = 0;
        foreach (BSPNode node in nodes)
        {
            float distance = Vector3.Distance(
                new Vector3(startNode.room.center.x, 0, startNode.room.center.y),
                new Vector3(node.room.center.x, 0, node.room.center.y)
            );
            if (distance > maxDistance)
            {
                maxDistance = distance;
                furthestNode = node;
            }
        }
        return furthestNode;
    }

    // 기타 필요한 메서드들
}
