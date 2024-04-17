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
    public GameObject roomObject; // �� ������Ʈ
    public bool isConnectedNorth;
    public bool isConnectedEast;
    public bool isConnectedSouth;
    public bool isConnectedWest;
    // ��Ÿ �ʿ��� ������
}

public class BSPGenerator : MonoBehaviour
{
    public GameObject startRoomPrefab;
    public GameObject bossRoomPrefab;
    public GameObject eliteRoomPrefab;
    public GameObject[] normalRoomPrefabs;
    public int normalRoomCount; // NormalRoom�� ����
    public int mapWidth;
    public int mapHeight;
    public int divideCount; // ���� Ƚ���� ���ϴ� ����

    void Start()
    {
        BSPNode root = new BSPNode();
        root.room = new Rect(0, 0, mapWidth, mapHeight);
        SplitNode(root, divideCount);
        PlaceSpecialRooms(root); // Ư���� �� ��ġ
        PlaceNormalRooms(root, normalRoomCount); // �Ϲ� �� ��ġ
        ConnectRooms(root); // �� ���� �� ����
        DrawLines(root);
    }

    void DrawLines(BSPNode node)
    {
        if (node == null) return;

        // LineRenderer ���� �� ����
        GameObject lineObj = new GameObject("Line");
        LineRenderer lineRenderer = lineObj.AddComponent<LineRenderer>();
        lineRenderer.material = new Material(Shader.Find("Sprites/Default")); // ������ ���̴� ����
        lineRenderer.widthMultiplier = 0.05f; // ���� �ʺ�
        lineRenderer.positionCount = 5; // �簢���� �׸��� ���� 5���� ���� �ʿ�

        // �簢���� �𼭸� ��ǥ ���
        Vector3[] corners = new Vector3[5];
        corners[0] = new Vector3(node.room.xMin, 0, node.room.yMin);
        corners[1] = new Vector3(node.room.xMax, 0, node.room.yMin);
        corners[2] = new Vector3(node.room.xMax, 0, node.room.yMax);
        corners[3] = new Vector3(node.room.xMin, 0, node.room.yMax);
        corners[4] = corners[0]; // ���������� ���ƿ�

        // LineRenderer�� ������ ����
        lineRenderer.SetPositions(corners);

        // �ڽ� ��忡 ���ؼ��� ���� �۾� ����
        DrawLines(node.leftChild);
        DrawLines(node.rightChild);
    }
    void SplitNode(BSPNode node, int depth)
    {
        if (depth <= 0) return;

        // ���� ���� ���� (0.4���� 0.6 ����)
        float splitRatio = Random.Range(0.4f, 0.6f);

        // ������ �������� ��� ��������, �׷��� ������ �������� ����
        if (node.room.width > node.room.height)
        {
            // ���� ����
            float split = node.room.width * splitRatio;
            node.leftChild = new BSPNode { room = new Rect(node.room.x, node.room.y, split, node.room.height) };
            node.rightChild = new BSPNode { room = new Rect(node.room.x + split, node.room.y, node.room.width - split, node.room.height) };
        }
        else
        {
            // ���� ����
            float split = node.room.height * splitRatio;
            node.leftChild = new BSPNode { room = new Rect(node.room.x, node.room.y, node.room.width, split) };
            node.rightChild = new BSPNode { room = new Rect(node.room.x, node.room.y + split, node.room.width, node.room.height - split) };
        }

        // ��������� �ڽ� ������ ����
        SplitNode(node.leftChild, depth - 1);
        SplitNode(node.rightChild, depth - 1);
    }

    

    void ConnectRooms(BSPNode node)
    {
        if (node == null || node.leftChild == null || node.rightChild == null) return;

        // �� �ڽ� ����� �߽��� ã��
        Vector3 centerA = new Vector3(
            node.leftChild.room.x + node.leftChild.room.width / 2,
            0, // y�� ��ġ�� �ʿ信 ���� ����
            node.leftChild.room.y + node.leftChild.room.height / 2
        );

        Vector3 centerB = new Vector3(
            node.rightChild.room.x + node.rightChild.room.width / 2,
            0, // y�� ��ġ�� �ʿ信 ���� ����
            node.rightChild.room.y + node.rightChild.room.height / 2
        );

        // L�� ������ �� ����
        Vector3 midPoint = new Vector3(centerA.x, 0, centerB.z);
        CreatePath(centerA, midPoint, node.leftChild, node.rightChild);
        CreatePath(midPoint, centerB, node.leftChild, node.rightChild);

        // �ڽ� ��忡 ���ؼ��� �� ����
        ConnectRooms(node.leftChild);
        ConnectRooms(node.rightChild);


    }

    void CreatePath(Vector3 start, Vector3 end, BSPNode nodeA, BSPNode nodeB)
    {
        GameObject lineObj = new GameObject("PathLine");
        LineRenderer lineRenderer = lineObj.AddComponent<LineRenderer>();
        Rigidbody rb = lineObj.AddComponent<Rigidbody>();
        BoxCollider collider = lineObj.AddComponent<BoxCollider>();

        // LineRenderer�� ������ ���� ����
        Material redMaterial = new Material(Shader.Find("Sprites/Default"));
        redMaterial.color = Color.red; // ���������� ����
        lineRenderer.material = redMaterial;

        // Rigidbody ����
        rb.isKinematic = true;
        rb.useGravity = false;

        // Collider ����
        collider.isTrigger = true;

        lineRenderer.widthMultiplier = 0.1f;
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, end);
        // Collider�� ��ġ, ũ��, ���� ����
        Vector3 midPoint = (start + end) / 2;
        lineObj.transform.position = midPoint;

        float lineLength = Vector3.Distance(start, end);
        collider.size = new Vector3(0.1f, 0.1f, lineLength); // �ݶ��̴��� �β��� ���̴� ������ ����

        // Collider�� ������ �������� ȸ��
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
        // StartRoom, BossRoom, EliteRoom�� ��ġ�� �����ϴ� ����
        List<BSPNode> leafNodes = GetLeafNodes(node);
        if (leafNodes.Count < 3) return; // ��� �� ���� �� ��尡 �ʿ�

        // StartRoom ��ġ
        BSPNode startNode = leafNodes[Random.Range(0, leafNodes.Count)];
        InstantiateRoom(startRoomPrefab, startNode.room, startNode); // ���� ����
        startNode.roomType = RoomType.StartRoom;

        // BossRoom ��ġ - StartRoom���� ���� �� ���� ��ġ
        leafNodes.Remove(startNode);
        BSPNode bossNode = GetFurthestNode(startNode, leafNodes);
        InstantiateRoom(bossRoomPrefab, bossNode.room, bossNode); // ���� ����
        bossNode.roomType = RoomType.BossRoom;

        // EliteRoom ��ġ - ������ �� ��� �� �ϳ� ����
        leafNodes.Remove(bossNode);
        BSPNode eliteNode = leafNodes[Random.Range(0, leafNodes.Count)];
        InstantiateRoom(eliteRoomPrefab, eliteNode.room, eliteNode); // ���� ����
        eliteNode.roomType = RoomType.EliteRoom;
    }

    void PlaceNormalRooms(BSPNode node, int count)
    {
        // ���� �������� �����Ͽ� �̹� Ư���� ���� ��ġ�� ���� �ǳʶٰ� �Ϲ� �游 ��ġ
        List<BSPNode> leafNodes = GetLeafNodes(node);
        foreach (BSPNode leafNode in leafNodes)
        {
            if (count <= 0 || leafNode.roomType != RoomType.None) continue;
            InstantiateRoom(normalRoomPrefabs[Random.Range(0, normalRoomPrefabs.Length)], leafNode.room, leafNode); // ���� ����
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
                node.roomObject = roomObject; // ������ �� ������Ʈ ����
            }
        }
    }




    List<BSPNode> GetLeafNodes(BSPNode node)
    {
        // �� ������ ��ȯ�ϴ� ����
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
        // StartNode�κ��� ���� �� ��带 ã�� ����
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

    // ��Ÿ �ʿ��� �޼����
}
