using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
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
    public GameObject roomObject;
}

public class BSPGenerator : MonoBehaviour
{
    public GameObject startRoomPrefab;
    public GameObject bossRoomPrefab;
    public GameObject eliteRoomPrefab;
    public GameObject[] normalRoomPrefabs;
    public GameObject verticalTilePrefab;
    public GameObject horizontalTilePrefab;
    public GameObject topLeftCornerPrefab;
    public GameObject topRightCornerPrefab;
    public GameObject bottomLeftCornerPrefab;
    public GameObject bottomRightCornerPrefab;
    public int normalRoomCount;
    public int mapWidth;
    public int mapHeight;
    public int divideCount;
    private Vector3 tileSize;
    private Dictionary<BSPNode, List<BSPNode>> roomConnections;
    [SerializeField] private B_Player player;

    public MB3_MeshBaker meshbaker;
    private List<GameObject> objsToCombine = new List<GameObject>();

    BSPNode startRoomNode = null;
    BSPNode bossRoomNode = null;

    [SerializeField] private NavMeshSurface navMesh;
    void Start()
    {
        tileSize = GetPrefabSize(verticalTilePrefab);
        player.gameObject.SetActive(false);

        roomConnections = new Dictionary<BSPNode, List<BSPNode>>();
        GenerateBSPDungeon();
    }

    private Vector3 GetPrefabSize(GameObject prefab)
    {
        Renderer renderer = prefab.GetComponent<Renderer>();
        if (renderer != null)
        {
            return renderer.bounds.size;
        }
        return new Vector3(1, 0, 1);
    }

    public void ReGenerator()
    {
        roomConnections = new Dictionary<BSPNode, List<BSPNode>>();
        ClearNavMesh();
        StartCoroutine(ReGenerateCoroutine());
    }

    private IEnumerator ReGenerateCoroutine()
    {
        List<Transform> children = new List<Transform>();
        foreach (Transform child in transform)
        {
            children.Add(child);
        }

        foreach (Transform child in children)
        {
            DestroyImmediate(child.gameObject);
        }

        ResetNodes();

        yield return null;

        GenerateBSPDungeon();
    }

    private void ResetNodes()
    {
        void ClearNode(BSPNode node)
        {
            if (node == null) return;
            node.roomObject = null;
            ClearNode(node.leftChild);
            ClearNode(node.rightChild);
        }

        BSPNode rootNode = new BSPNode { room = new Rect(0, 0, mapWidth, mapHeight) };
        ClearNode(rootNode);
    }

    void GenerateBSPDungeon()
    {
        BSPNode root = new BSPNode();

        root.room = new Rect(0, 0, mapWidth, mapHeight);
        SplitNode(root, divideCount);
        PlaceSpecialRooms(root);
        PlaceNormalRooms(root, normalRoomCount);
        ConnectRooms(root);

        if (startRoomNode != null && bossRoomNode != null)
        {
            TraceAndLogPath(startRoomNode, bossRoomNode);
        }

        DeactivateMonsterParents();
        StartCoroutine(BakeNavMesh());
        StartCoroutine(BakeMeshes());

        if (startRoomNode != null)
        {
            StartCoroutine(MovePlayerToStartRoom());
        }
    }

    void DeactivateMonsterParents()
    {
        foreach (Transform child in transform)
        {
            RoomPrefab roomPrefab = child.GetComponent<RoomPrefab>();
            if (roomPrefab != null && roomPrefab.monsterParent != null)
            {
                roomPrefab.monsterParent.SetActive(false);
            }
        }
    }

    void SplitNode(BSPNode node, int depth)
    {
        if (depth <= 0) return;

        float splitRatio = Mathf.Round(Random.Range(0.4f, 0.6f) * 100f) / 100f;

        if (node.room.width > node.room.height)
        {
            float split = node.room.width * splitRatio;
            node.leftChild = new BSPNode { room = new Rect(node.room.x, node.room.y, split, node.room.height) };
            node.rightChild = new BSPNode { room = new Rect(node.room.x + split, node.room.y, node.room.width - split, node.room.height) };
        }
        else
        {
            float split = node.room.height * splitRatio;
            node.leftChild = new BSPNode { room = new Rect(node.room.x, node.room.y, node.room.width, split) };
            node.rightChild = new BSPNode { room = new Rect(node.room.x, node.room.y + split, node.room.width, node.room.height - split) };
        }

        SplitNode(node.leftChild, depth - 1);
        SplitNode(node.rightChild, depth - 1);
    }

    void ConnectRooms(BSPNode node)
    {
        List<BSPNode> leafNodes = GetLeafNodes(node);
        if (leafNodes.Count < 2)
            return;

        List<Edge> edges = new List<Edge>();

        for (int i = 0; i < leafNodes.Count; i++)
        {
            for (int j = i + 1; j < leafNodes.Count; j++)
            {
                if (leafNodes[i].roomObject == null || leafNodes[j].roomObject == null)
                    continue;

                float distance = Vector3.Distance(
                    new Vector3(leafNodes[i].room.center.x, 0, leafNodes[i].room.center.y),
                    new Vector3(leafNodes[j].room.center.x, 0, leafNodes[j].room.center.y)
                );
                edges.Add(new Edge(leafNodes[i], leafNodes[j], distance));
            }
        }

        edges.Sort((a, b) => a.distance.CompareTo(b.distance));

        Dictionary<BSPNode, BSPNode> parent = new Dictionary<BSPNode, BSPNode>();
        foreach (var leaf in leafNodes)
        {
            parent[leaf] = leaf;
        }

        foreach (var edge in edges)
        {
            BSPNode rootA = Find(parent, edge.nodeA);
            BSPNode rootB = Find(parent, edge.nodeB);
            if (rootA != rootB)
            {
                CreatePathBetweenRooms(edge.nodeA, edge.nodeB);
                if (!roomConnections.ContainsKey(edge.nodeA))
                    roomConnections[edge.nodeA] = new List<BSPNode>();
                if (!roomConnections.ContainsKey(edge.nodeB))
                    roomConnections[edge.nodeB] = new List<BSPNode>();

                roomConnections[edge.nodeA].Add(edge.nodeB);
                roomConnections[edge.nodeB].Add(edge.nodeA);
                parent[rootA] = rootB;
            }
        }
    }

    BSPNode Find(Dictionary<BSPNode, BSPNode> parent, BSPNode node)
    {
        if (parent[node] != node)
            parent[node] = Find(parent, parent[node]);
        return parent[node];
    }

    class Edge
    {
        public BSPNode nodeA, nodeB;
        public float distance;
        public Edge(BSPNode a, BSPNode b, float dist)
        {
            nodeA = a;
            nodeB = b;
            distance = dist;
        }
    }

    void CreatePathBetweenRooms(BSPNode nodeA, BSPNode nodeB)
    {
        if (nodeA == null || nodeB == null || nodeA.roomObject == null || nodeB.roomObject == null) return;

        RoomPrefab roomPrefabA = nodeA.roomObject.GetComponent<RoomPrefab>();
        RoomPrefab roomPrefabB = nodeB.roomObject.GetComponent<RoomPrefab>();

        if (roomPrefabA == null || roomPrefabB == null)
            return;

        GameObject[] entrancesA = { roomPrefabA.EntranceN, roomPrefabA.EntranceE, roomPrefabA.EntranceS, roomPrefabA.EntranceW };
        GameObject[] entrancesB = { roomPrefabB.EntranceN, roomPrefabB.EntranceE, roomPrefabB.EntranceS, roomPrefabB.EntranceW };

        float minDistance = float.MaxValue;
        Vector3 start = Vector3.zero;
        Vector3 end = Vector3.zero;
        GameObject startEntrance = null;
        GameObject endEntrance = null;

        Dictionary<GameObject, GameObject> symmetricEntrances = new Dictionary<GameObject, GameObject>
        {
            { roomPrefabA.EntranceN, roomPrefabB.EntranceS },
            { roomPrefabA.EntranceE, roomPrefabB.EntranceW },
            { roomPrefabA.EntranceS, roomPrefabB.EntranceN },
            { roomPrefabA.EntranceW, roomPrefabB.EntranceE }
        };

        foreach (var pair in symmetricEntrances)
        {
            if (pair.Key == null || pair.Value == null) continue;
            float distance = Vector3.Distance(pair.Key.transform.position, pair.Value.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                startEntrance = pair.Key;
                endEntrance = pair.Value;
            }
        }

        if (minDistance < float.MaxValue)
        {
            start = startEntrance.transform.position;
            end = endEntrance.transform.position;
            CreateLShapedPath(start, end, startEntrance, endEntrance);
            if (startEntrance != null || endEntrance != null)
            {
                startEntrance.SetActive(false);
                endEntrance.SetActive(false);
            }
        }
    }

    void CreateLShapedPath(Vector3 startPosition, Vector3 endPosition, GameObject startEntrance, GameObject endEntrance)
    {
        float deltaX = endPosition.x - startPosition.x;
        float deltaZ = endPosition.z - startPosition.z;

        if (deltaX == 0 || deltaZ == 0)
        {
            if (deltaX == 0)
                PlaceTilesAlongLineVertical(startPosition, endPosition, false);
            if (deltaZ == 0)
                PlaceTilesAlongLineHorizontal(startPosition, endPosition, false);
        }
        else
        {
            if (Mathf.Abs(deltaX) < 10 || Mathf.Abs(deltaZ) < 10)
            {
                ReGenerator();
                return;
            }

            Vector3 midPoint1, midPoint2;

            if (startEntrance.name.Contains("N") && endEntrance.name.Contains("S") || startEntrance.name.Contains("S") && endEntrance.name.Contains("N"))
            {
                midPoint1 = new Vector3(startPosition.x, startPosition.y, (startPosition.z + endPosition.z) / 2);
                midPoint2 = new Vector3(endPosition.x, endPosition.y, (startPosition.z + endPosition.z) / 2);
                DetermineAndPlaceCornerObjects(startEntrance, endEntrance, midPoint1, midPoint2);
            }
            else
            {
                midPoint1 = new Vector3((startPosition.x + endPosition.x) / 2, startPosition.y, startPosition.z);
                midPoint2 = new Vector3((startPosition.x + endPosition.x) / 2, endPosition.y, endPosition.z);
                DetermineAndPlaceCornerObjects(startEntrance, endEntrance, midPoint1, midPoint2);
            }
        }
    }

    private void DetermineAndPlaceCornerObjects(GameObject start, GameObject end, Vector3 intermediate1, Vector3 intermediate2)
    {
        string startName = start.name;
        string endName = end.name;
        if ((startName.Contains("N") && endName.Contains("S")))
        {
            if (start.transform.position.x > end.transform.position.x)
            {
                PlaceTopRightCornerObject(intermediate1, Quaternion.identity);
                PlaceBottomLeftCornerObject(intermediate2, Quaternion.identity);
            }
            else
            {
                PlaceTopLeftCornerObject(intermediate1, Quaternion.identity);
                PlaceBottomRightCornerObject(intermediate2, Quaternion.identity);
            }
            Vector3 startPosition = start.transform.position;
            Vector3 endPosition = end.transform.position;
            Vector3 midPoint1 = new Vector3(startPosition.x, startPosition.y, (startPosition.z + endPosition.z) / 2);
            Vector3 midPoint2 = new Vector3(endPosition.x, endPosition.y, (startPosition.z + endPosition.z) / 2);
            Vector3 cornerSize = GetPrefabSize(verticalTilePrefab);
            Vector3 direction1 = (midPoint2 - midPoint1).normalized;
            Vector3 offsetStart1 = midPoint1 + direction1 * cornerSize.x * 4;
            Vector3 direction0 = (midPoint1 - startPosition).normalized;
            Vector3 direction2 = (endPosition - midPoint2).normalized;
            Vector3 offsetStart2 = midPoint2 + direction2 * cornerSize.x * 8f;
            Vector3 offsetEnd1 = midPoint1 - direction0 * cornerSize.x * 10;
            Vector3 offsetEnd11 = offsetEnd1 - direction0 * cornerSize.z * 6;
            Vector3 offsetEnd2 = midPoint2 - direction1 * cornerSize.x * 2;
            Vector3 offsetEnd22 = offsetEnd2 - direction1 * cornerSize.z * 2;
            PlaceTilesAlongLineVertical(startPosition, offsetEnd11, false);
            PlaceTilesAlongLineHorizontal(offsetStart1, offsetEnd22, false);
            PlaceTilesAlongLineVertical(offsetStart2, endPosition, false);
        }
        else if ((startName.Contains("S") && endName.Contains("N")))
        {
            if (start.transform.position.x > end.transform.position.x)
            {
                PlaceBottomRightCornerObject(intermediate1, Quaternion.identity);
                PlaceTopLeftCornerObject(intermediate2, Quaternion.identity);
            }
            else
            {
                PlaceBottomLeftCornerObject(intermediate1, Quaternion.identity);
                PlaceTopRightCornerObject(intermediate2, Quaternion.identity);
            }
            Vector3 startPosition = start.transform.position;
            Vector3 endPosition = end.transform.position;
            Vector3 midPoint1 = new Vector3(startPosition.x, startPosition.y, (startPosition.z + endPosition.z) / 2);
            Vector3 midPoint2 = new Vector3(endPosition.x, endPosition.y, (startPosition.z + endPosition.z) / 2);
            Vector3 cornerSize = GetPrefabSize(verticalTilePrefab);
            Vector3 direction1 = (midPoint2 - midPoint1).normalized;
            Vector3 offsetStart1 = midPoint1 + direction1 * cornerSize.x * 4;
            Vector3 direction0 = (midPoint1 - startPosition).normalized;
            Vector3 direction2 = (endPosition - midPoint2).normalized;
            Vector3 offsetStart2 = midPoint2 + direction2 * cornerSize.x * 16;
            Vector3 offsetEnd1 = midPoint1 - direction0 * cornerSize.x * 10;
            Vector3 offsetEnd11 = offsetEnd1 - direction0 * cornerSize.z;
            Vector3 offsetEnd2 = midPoint2 - direction1 * cornerSize.x * 2;
            Vector3 offsetEnd22 = offsetEnd2 - direction1 * cornerSize.z * 2;
            PlaceTilesAlongLineVertical(startPosition, offsetEnd11, false);
            PlaceTilesAlongLineHorizontal(offsetStart1, offsetEnd22, false);
            PlaceTilesAlongLineVertical(offsetStart2, endPosition, false);
        }
        else if ((startName.Contains("E") && endName.Contains("W")))
        {
            if (start.transform.position.z > end.transform.position.z)
            {
                PlaceTopRightCornerObject(intermediate1, Quaternion.identity);
                PlaceBottomLeftCornerObject(intermediate2, Quaternion.identity);
                Debug.Log("5");
                Vector3 startPosition = start.transform.position;
                Vector3 endPosition = end.transform.position;
                Vector3 midPoint1 = new Vector3((startPosition.x + endPosition.x) / 2, startPosition.y, startPosition.z);
                Vector3 midPoint2 = new Vector3((startPosition.x + endPosition.x) / 2, endPosition.y, endPosition.z);
                Vector3 cornerSize = GetPrefabSize(verticalTilePrefab);
                Vector3 direction1 = (midPoint2 - midPoint1).normalized;
                Vector3 offsetStart1 = midPoint1 + direction1 * cornerSize.x * 16;
                Vector3 direction0 = (midPoint1 - startPosition).normalized;
                Vector3 direction2 = (endPosition - midPoint2).normalized;
                Vector3 offsetStart2 = midPoint2 + direction2 * cornerSize.x * 4f;
                Vector3 offsetEnd1 = midPoint1 - direction0 * cornerSize.x * 10;
                Vector3 offsetEnd11 = offsetEnd1 + direction0 * cornerSize.z * 4;
                Vector3 offsetEnd2 = midPoint2 - direction1 * cornerSize.x * 2;
                Vector3 offsetEnd22 = offsetEnd2 - direction1 * cornerSize.z * 6;
                PlaceTilesAlongLineHorizontal(startPosition, offsetEnd11, false);
                PlaceTilesAlongLineVertical(offsetStart1, offsetEnd22, false);
                PlaceTilesAlongLineHorizontal(offsetStart2, endPosition, false);
            }
            else
            {
                PlaceBottomRightCornerObject(intermediate1, Quaternion.identity); ;
                PlaceTopLeftCornerObject(intermediate2, Quaternion.identity);
                Debug.Log("6");
                Vector3 startPosition = start.transform.position;
                Vector3 endPosition = end.transform.position;
                Vector3 midPoint1 = new Vector3((startPosition.x + endPosition.x) / 2, startPosition.y, startPosition.z);
                Vector3 midPoint2 = new Vector3((startPosition.x + endPosition.x) / 2, endPosition.y, endPosition.z);
                Vector3 cornerSize = GetPrefabSize(verticalTilePrefab);
                Vector3 direction1 = (midPoint2 - midPoint1).normalized;
                Vector3 offsetStart1 = midPoint1 + direction1 * cornerSize.x * 12;
                Vector3 direction0 = (midPoint1 - startPosition).normalized;
                Vector3 direction2 = (endPosition - midPoint2).normalized;
                Vector3 offsetStart2 = midPoint2 + direction2 * cornerSize.x * 4f;
                Vector3 offsetEnd1 = midPoint1 - direction0 * cornerSize.x * 10;
                Vector3 offsetEnd11 = offsetEnd1 + direction0 * cornerSize.z * 4;
                Vector3 offsetEnd2 = midPoint2 - direction1 * cornerSize.x * 2;
                Vector3 offsetEnd22 = offsetEnd2 - direction1 * cornerSize.z * 14;
                PlaceTilesAlongLineHorizontal(startPosition, offsetEnd11, false);
                PlaceTilesAlongLineVertical(offsetStart1, offsetEnd22, false);
                PlaceTilesAlongLineHorizontal(offsetStart2, endPosition, false);
            }
        }
        else if ((startName.Contains("W") && endName.Contains("E")))
        {
            if (start.transform.position.z > end.transform.position.z)
            {
                PlaceTopLeftCornerObject(intermediate1, Quaternion.identity);
                PlaceBottomRightCornerObject(intermediate2, Quaternion.identity);
                Debug.Log("7");
                Vector3 startPosition = start.transform.position;
                Vector3 endPosition = end.transform.position;
                Vector3 midPoint1 = new Vector3((startPosition.x + endPosition.x) / 2, startPosition.y, startPosition.z);
                Vector3 midPoint2 = new Vector3((startPosition.x + endPosition.x) / 2, endPosition.y, endPosition.z);
                Vector3 cornerSize = GetPrefabSize(verticalTilePrefab);
                Vector3 direction1 = (midPoint2 - midPoint1).normalized;
                Vector3 offsetStart1 = midPoint1 + direction1 * cornerSize.x * 16;
                Vector3 direction0 = (midPoint1 - startPosition).normalized;
                Vector3 direction2 = (endPosition - midPoint2).normalized;
                Vector3 offsetStart2 = midPoint2 + direction2 * cornerSize.x * 4f;
                Vector3 offsetEnd1 = midPoint1 - direction0 * cornerSize.x * 10;
                Vector3 offsetEnd11 = offsetEnd1 + direction0 * cornerSize.z * 4;
                Vector3 offsetEnd2 = midPoint2 - direction1 * cornerSize.x * 2;
                Vector3 offsetEnd22 = offsetEnd2 - direction1 * cornerSize.z * 6;
                PlaceTilesAlongLineHorizontal(startPosition, offsetEnd11, false);
                PlaceTilesAlongLineVertical(offsetStart1, offsetEnd22, false);
                PlaceTilesAlongLineHorizontal(offsetStart2, endPosition, false);
            }
            else
            {
                PlaceBottomLeftCornerObject(intermediate1, Quaternion.identity); ;
                PlaceTopRightCornerObject(intermediate2, Quaternion.identity);
                Debug.Log("8");
                Vector3 startPosition = start.transform.position;
                Vector3 endPosition = end.transform.position;
                Vector3 midPoint1 = new Vector3((startPosition.x + endPosition.x) / 2, startPosition.y, startPosition.z);
                Vector3 midPoint2 = new Vector3((startPosition.x + endPosition.x) / 2, endPosition.y, endPosition.z);
                Vector3 cornerSize = GetPrefabSize(verticalTilePrefab);
                Vector3 direction1 = (midPoint2 - midPoint1).normalized;
                Vector3 offsetStart1 = midPoint1 + direction1 * cornerSize.x * 12;
                Vector3 direction0 = (midPoint1 - startPosition).normalized;
                Vector3 direction2 = (endPosition - midPoint2).normalized;
                Vector3 offsetStart2 = midPoint2 + direction2 * cornerSize.x * 4f;
                Vector3 offsetEnd1 = midPoint1 - direction0 * cornerSize.x * 10;
                Vector3 offsetEnd11 = offsetEnd1 + direction0 * cornerSize.z * 4;
                Vector3 offsetEnd2 = midPoint2 - direction1 * cornerSize.x * 2;
                Vector3 offsetEnd22 = offsetEnd2 - direction1 * cornerSize.z * 14;
                PlaceTilesAlongLineHorizontal(startPosition, offsetEnd11, false);
                PlaceTilesAlongLineVertical(offsetStart1, offsetEnd22, false);
                PlaceTilesAlongLineHorizontal(offsetStart2, endPosition, false);
            }
        }
    }

    private void PlaceTopLeftCornerObject(Vector3 position, Quaternion rotation)
    {
        GameObject parent = EnsureParentStructure().transform.Find("Corner").gameObject;
        GameObject corner = Instantiate(topLeftCornerPrefab, position, rotation);
        corner.transform.parent = parent.transform;
    }

    private void PlaceTopRightCornerObject(Vector3 position, Quaternion rotation)
    {
        GameObject parent = EnsureParentStructure().transform.Find("Corner").gameObject;
        GameObject corner = Instantiate(topRightCornerPrefab, position, rotation);
        corner.transform.parent = parent.transform;
    }

    private void PlaceBottomLeftCornerObject(Vector3 position, Quaternion rotation)
    {
        GameObject parent = EnsureParentStructure().transform.Find("Corner").gameObject;
        GameObject corner = Instantiate(bottomLeftCornerPrefab, position, rotation);
        corner.transform.parent = parent.transform;
    }

    private void PlaceBottomRightCornerObject(Vector3 position, Quaternion rotation)
    {
        GameObject parent = EnsureParentStructure().transform.Find("Corner").gameObject;
        GameObject corner = Instantiate(bottomRightCornerPrefab, position, rotation);
        corner.transform.parent = parent.transform;
    }

    void PlaceSpecialRooms(BSPNode node)
    {
        List<BSPNode> leafNodes = GetLeafNodes(node);
        if (leafNodes.Count < 3) return;

        startRoomNode = leafNodes[Random.Range(0, leafNodes.Count)];
        InstantiateRoom(startRoomPrefab, startRoomNode.room, startRoomNode);
        startRoomNode.roomType = RoomType.StartRoom;

        leafNodes.Remove(startRoomNode);
        bossRoomNode = GetFurthestNode(startRoomNode, leafNodes);
        InstantiateRoom(bossRoomPrefab, bossRoomNode.room, bossRoomNode);
        bossRoomNode.roomType = RoomType.BossRoom;

        leafNodes.Remove(bossRoomNode);
        BSPNode eliteNode = leafNodes[Random.Range(0, leafNodes.Count)];
        InstantiateRoom(eliteRoomPrefab, eliteNode.room, eliteNode);
        eliteNode.roomType = RoomType.EliteRoom;
    }

    void PlaceNormalRooms(BSPNode node, int count)
    {
        List<BSPNode> leafNodes = GetLeafNodes(node);
        foreach (BSPNode leafNode in leafNodes)
        {
            if (count <= 0 || leafNode.roomType != RoomType.None) continue;
            InstantiateRoom(normalRoomPrefabs[Random.Range(0, normalRoomPrefabs.Length)], leafNode.room, leafNode);
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
                node.roomObject = roomObject;
            }
        }
    }

    List<BSPNode> GetLeafNodes(BSPNode node)
    {
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

    void CheckRoomConnections(BSPNode node, List<KeyValuePair<BSPNode, BSPNode>> connectedRooms)
    {
        if (node == null) return;

        CheckRoomConnections(node.leftChild, connectedRooms);
        CheckRoomConnections(node.rightChild, connectedRooms);

        if (node.roomObject != null)
        {
            if (node.leftChild != null && node.leftChild.roomObject != null &&
                node.rightChild != null && node.rightChild.roomObject != null)
            {
                connectedRooms.Add(new KeyValuePair<BSPNode, BSPNode>(node.leftChild, node.rightChild));
            }
        }
    }
    private void PlaceTilesAlongLineVertical(Vector3 start, Vector3 end, bool skipLastTile)
    {
        GameObject parent = EnsureParentStructure().transform.Find("Tile").gameObject;
        Vector3 direction = (end - start).normalized;
        float distance = Vector3.Distance(start, end);
        float tileStep = tileSize.x;
        bool isDecimal = distance % tileStep != 0;
        int tileCount = Mathf.FloorToInt(distance / tileStep);

        if (skipLastTile)
            tileCount--;

        Quaternion rotation = Quaternion.Euler(0, 0, 0);

        for (int i = 0; i <= tileCount; i += 4)
        {
            Vector3 tilePosition = start + direction * tileStep * i;
            GameObject tile = Instantiate(verticalTilePrefab, tilePosition, rotation);
            tile.transform.parent = parent.transform;
            if (isDecimal || i == tileCount - 1 || i == tileCount - 2 || i == tileCount - 3)
            {
                GameObject lastTile = Instantiate(verticalTilePrefab, end, rotation);
                lastTile.transform.parent = parent.transform;
            }
        }
    }

    private void PlaceTilesAlongLineHorizontal(Vector3 start, Vector3 end, bool skipLastTile)
    {
        GameObject parent = EnsureParentStructure().transform.Find("Tile").gameObject;
        Vector3 direction = (end - start).normalized;
        float distance = Vector3.Distance(start, end);
        float tileStep = tileSize.x;
        bool isDecimal = distance % tileStep != 0;
        int tileCount = Mathf.FloorToInt(distance / tileStep);

        if (skipLastTile)
            tileCount--;

        Quaternion rotation = Quaternion.identity;

        for (int i = 0; i <= tileCount; i += 2)
        {
            Vector3 tilePosition = start + direction * tileStep * i;
            GameObject tile = Instantiate(horizontalTilePrefab, tilePosition, rotation);
            tile.transform.parent = parent.transform;
            if (isDecimal || i == tileCount - 1)
            {
                GameObject lastTile = Instantiate(horizontalTilePrefab, end, rotation);
                lastTile.transform.parent = parent.transform;
            }
        }
    }

    private GameObject EnsureParentStructure()
    {
        GameObject tileAndCornerParent = transform.Find("TileAndCorner")?.gameObject ?? new GameObject("TileAndCorner");

        if (tileAndCornerParent.transform.parent != transform)
        {
            tileAndCornerParent.transform.parent = transform;
        }

        GameObject tileParent = tileAndCornerParent.transform.Find("Tile")?.gameObject ?? new GameObject("Tile");
        GameObject cornerParent = tileAndCornerParent.transform.Find("Corner")?.gameObject ?? new GameObject("Corner");

        if (tileParent.transform.parent != tileAndCornerParent.transform)
        {
            tileParent.transform.parent = tileAndCornerParent.transform;
        }
        if (cornerParent.transform.parent != tileAndCornerParent.transform)
        {
            cornerParent.transform.parent = tileAndCornerParent.transform;
        }

        return tileAndCornerParent;
    }

    IEnumerator BakeMeshes()
    {
        objsToCombine.Clear();
        yield return new WaitForSeconds(1f);

        Transform tileParent = transform.Find("TileAndCorner/Tile");
        if (tileParent != null)
        {
            foreach (Transform tile in tileParent)
            {
                AddChildObjectsWithMeshRenderer(tile, objsToCombine);
            }
        }

        foreach (Transform child in transform)
        {
            RoomPrefab roomPrefab = child.GetComponent<RoomPrefab>();
            if (roomPrefab != null && roomPrefab.meshBakingTileGroup != null)
            {
                AddChildObjectsWithMeshRenderer(roomPrefab.meshBakingTileGroup.transform, objsToCombine);
            }
        }

        if (meshbaker.AddDeleteGameObjects(objsToCombine.ToArray(), null, true))
        {
            meshbaker.Apply();
        }
    }

    void AddChildObjectsWithMeshRenderer(Transform parent, List<GameObject> objsToCombine)
    {
        foreach (Transform child in parent)
        {
            if (child.GetComponent<MeshRenderer>() != null || child.GetComponent<SkinnedMeshRenderer>() != null)
            {
                objsToCombine.Add(child.gameObject);
            }
            AddChildObjectsWithMeshRenderer(child, objsToCombine);
        }
    }

    IEnumerator BakeNavMesh()
    {
        yield return new WaitForSeconds(1f);

        if (navMesh.navMeshData == null)
            navMesh.BuildNavMesh();
    }

    void ClearNavMesh()
    {
        if (navMesh.navMeshData != null)
            navMesh.RemoveData();
    }

    void TraceAndLogPath(BSPNode startNode, BSPNode endNode)
    {
        List<BSPNode> path = new List<BSPNode>();
        if (startNode == null || endNode == null) return;

        path.Clear();
        path.Add(startNode);
        BSPNode currentNode = startNode;

        while (currentNode != endNode && currentNode != null)
        {
            BSPNode nextNode = FindNextNodeInPath(currentNode, endNode);
            if (nextNode == null) break;
            path.Add(nextNode);
            currentNode = nextNode;
        }

        string pathDescription = "Path from StartRoom to BossRoom: ";
        foreach (BSPNode node in path)
        {
            pathDescription += $"{node.roomType} -> ";
        }
        Debug.Log(pathDescription.TrimEnd(' ', '-', '>'));
    }

    BSPNode FindNextNodeInPath(BSPNode currentNode, BSPNode targetNode)
    {
        if (!roomConnections.ContainsKey(currentNode))
            return null;

        List<BSPNode> connectedNodes = roomConnections[currentNode];
        BSPNode closestNode = null;
        float closestDistance = float.MaxValue;

        foreach (BSPNode node in connectedNodes)
        {
            float distance = Vector3.Distance(new Vector3(node.room.center.x, 0, node.room.center.y), new Vector3(targetNode.room.center.x, 0, targetNode.room.center.y));
            if (distance < closestDistance)
            {
                closestNode = node;
                closestDistance = distance;
            }
        }

        return closestNode;
    }

    IEnumerator MovePlayerToStartRoom()
    {
        yield return new WaitForSeconds(1f);

        if (player != null && startRoomNode != null && startRoomNode.roomObject != null)
        {
            Vector3 roomPosition = new Vector3(
                startRoomNode.room.x + startRoomNode.room.width / 2,
                0,
                startRoomNode.room.y + startRoomNode.room.height / 2
            );
            player.transform.position = roomPosition;
            player.gameObject.SetActive(true);
            Debug.Log("Player moved to StartRoom at: " + roomPosition);
        }
        else
        {
            Debug.LogError("Failed to move player: Start room or player not properly initialized.");
        }
    }
}
