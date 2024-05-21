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
    public GameObject roomObject; // 


}

public class BSPGenerator : MonoBehaviour
{
    public GameObject startRoomPrefab;
    public GameObject bossRoomPrefab;
    public GameObject eliteRoomPrefab;
    public GameObject[] normalRoomPrefabs;
    public GameObject tilePrefab;
    public GameObject cornerPrefab;
    public int normalRoomCount; // 
    public int mapWidth;
    public int mapHeight;
    public int divideCount; // 
    private Vector3 tileSize;

    void Start()
    {
        tileSize = GetPrefabSize(tilePrefab);
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
        StartCoroutine(ReGenerateCoroutine());
    }

    private IEnumerator ReGenerateCoroutine()
    {
        // 모든 자식 객체 제거 (BSPGenerator 자신은 제외)
        List<Transform> children = new List<Transform>();
        foreach (Transform child in transform)
        {
            children.Add(child);
        }

        // 모든 자식 제거
        foreach (Transform child in children)
        {
            DestroyImmediate(child.gameObject);
        }

        ResetNodes();

        yield return null;

        // 던전 재생성
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

        // 노드 트리를 순회하여 각 노드의 `roomObject` 속성을 초기화합니다.
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


    }

    void DrawLines(BSPNode node)
    {
        if (node == null) return;

        // LineRenderer 
        GameObject lineObj = new GameObject("Line");
        lineObj.transform.parent = this.transform; // BSPGenerator
        LineRenderer lineRenderer = lineObj.AddComponent<LineRenderer>();
        lineRenderer.material = new Material(Shader.Find("Sprites/Default")); // 
        lineRenderer.widthMultiplier = 0.05f; // 
        lineRenderer.positionCount = 5; //

        // 
        Vector3[] corners = new Vector3[5];
        corners[0] = new Vector3(node.room.xMin, 0, node.room.yMin);
        corners[1] = new Vector3(node.room.xMax, 0, node.room.yMin);
        corners[2] = new Vector3(node.room.xMax, 0, node.room.yMax);
        corners[3] = new Vector3(node.room.xMin, 0, node.room.yMax);
        corners[4] = corners[0]; // 

        // 
        lineRenderer.SetPositions(corners);

        // 
        DrawLines(node.leftChild);
        DrawLines(node.rightChild);
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

        List<KeyValuePair<BSPNode, BSPNode>> roomConnections = new List<KeyValuePair<BSPNode, BSPNode>>();

        foreach (var edge in edges)
        {
            BSPNode rootA = Find(parent, edge.nodeA);
            BSPNode rootB = Find(parent, edge.nodeB);
            if (rootA != rootB)
            {
                CreatePathBetweenRooms(edge.nodeA, edge.nodeB);
                roomConnections.Add(new KeyValuePair<BSPNode, BSPNode>(edge.nodeA, edge.nodeB));
                parent[rootA] = rootB;
            }
        }

        foreach (var connection in roomConnections)
        {
            Debug.Log($"Connected: {connection.Key.roomType} to {connection.Value.roomType}");
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

        /*if (deltaX != 0 && deltaZ != 0 && Mathf.Abs(deltaX) < 9 && Mathf.Abs(deltaZ) < 9)
        {
            ReGenerator(); // x좌표 혹은 z좌표로 9이하면 다시 배치
            return;     // 더 이쁘게 배치할려면 시간이 좀 걸릴듯
        }*/

        if (deltaX == 0 || deltaZ == 0)
        {

            PlaceTilesAlongLine(startPosition, endPosition, false);
        }
        else
        {
            if(Mathf.Abs(deltaX) < 10 || Mathf.Abs(deltaZ) < 10)
            {

                ReGenerator(); 
                return;
            }

            Vector3 midPoint = new Vector3((startPosition.x + endPosition.x) / 2, startPosition.y, (startPosition.z + endPosition.z) / 2);
            Vector3 midPoint1, midPoint2;

            if (startEntrance.name.Contains("N") && endEntrance.name.Contains("S") || startEntrance.name.Contains("S") && endEntrance.name.Contains("N"))
            {
                midPoint1 = new Vector3(startPosition.x, startPosition.y, (startPosition.z + endPosition.z) / 2);
                midPoint2 = new Vector3(endPosition.x, endPosition.y, (startPosition.z + endPosition.z) / 2);
            }
            else
            {
                midPoint1 = new Vector3((startPosition.x + endPosition.x) / 2, startPosition.y, startPosition.z);
                midPoint2 = new Vector3((startPosition.x + endPosition.x) / 2, endPosition.y, endPosition.z);
            }

            DetermineAndPlaceCornerObjects(startEntrance, endEntrance, midPoint1, midPoint2);
            // Place corner objects
            //DetermineAndPlaceCornerObjects(deltaX, deltaZ, midPoint1, midPoint2);
            Vector3 cornerKMJ = new Vector3(2, 0, 6);
            Vector3 cornerSize = GetPrefabSize(cornerPrefab); // 
            Vector3 direction1 = (midPoint2 - midPoint1).normalized;
            Vector3 offsetStart1 = midPoint1 + direction1 * cornerSize.x * 4; // 
            Vector3 direction0 = (midPoint1 - startPosition).normalized;
            Vector3 direction2 = (endPosition - midPoint2).normalized;
            Vector3 offsetStart2 = midPoint2 + direction2 * cornerSize.x * 4f; // 
            Vector3 offsetEnd1 = midPoint1 - direction0 * cornerSize.x * 2;
            Vector3 offsetEnd11 = offsetEnd1 - direction0 * cornerSize.z * 2;
            Vector3 offsetEnd2 = midPoint2 - direction1 * cornerSize.x * 2;
            Vector3 offsetEnd22 = offsetEnd2 - direction1 * cornerSize.z * 2;
            PlaceTilesAlongLine(startPosition, offsetEnd11, false);
            PlaceTilesAlongLine(offsetStart1, offsetEnd22, false);
            PlaceTilesAlongLine(offsetStart2, endPosition, false);
        }
    }
    private void DetermineAndPlaceCornerObjects(float c, float d, Vector3 intermediate1, Vector3 intermediate2)
    {
        if (c > 0 && d > 0)
        {
            if (c > d)
            {
                PlaceCornerObject(intermediate1 - new Vector3(-2.0f, 0f, 2.0f), Quaternion.Euler(0, 270, 0));
                PlaceCornerObject(intermediate2 + new Vector3(-2.0f, 0f, 2.0f), Quaternion.Euler(0, 90, 0));
                Debug.Log("a");
            }
            else // c <= d
            {
                PlaceCornerObject(intermediate1 - new Vector3(2.0f, 0f, -2.0f), Quaternion.Euler(0, 90, 0));
                PlaceCornerObject(intermediate2 + new Vector3(2.0f, 0f, -2.0f), Quaternion.Euler(0, -90, 0));
                Debug.Log("b");
            }
        }
        else if (c > 0 && d < 0)
        {
            if (Mathf.Abs(c) > Mathf.Abs(d))
            {
                PlaceCornerObject(intermediate1 - new Vector3(-2.0f, 0f, -2.0f), Quaternion.Euler(0, 180, 0));
                PlaceCornerObject(intermediate2 + new Vector3(-2.0f, 0f, -2.0f), Quaternion.Euler(0, 0, 0));
                Debug.Log("c");
            }
            else // Abs(d) >= Abs(c)
            {
                PlaceCornerObject(intermediate1 - new Vector3(2.0f, 0f, 2.0f), Quaternion.Euler(0, 0, 0));
                PlaceCornerObject(intermediate2 + new Vector3(2.0f, 0f, 2.0f), Quaternion.Euler(0, 180, 0));
                Debug.Log("d");
            }
        }
        else if (c < 0 && d > 0)
        {
            if (Mathf.Abs(c) > d)
            {
                PlaceCornerObject(intermediate1 - new Vector3(-2.0f, 0f, -2.0f), Quaternion.Euler(0, 180, 0));
                PlaceCornerObject(intermediate2 + new Vector3(-2.0f, 0f, -2.0f), Quaternion.identity);
                Debug.Log("e");
            }
            else // d >= Abs(c)
            {
                PlaceCornerObject(intermediate1 - new Vector3(-2.0f, 0f, -2.0f), Quaternion.Euler(0, 180, 0));
                PlaceCornerObject(intermediate2 + new Vector3(-2.0f, 0f, -2.0f), Quaternion.identity);
                Debug.Log("f");
            }
        }
        else if (c < 0 && d < 0)
        {
            if (Mathf.Abs(c) > Mathf.Abs(d))
            {
                PlaceCornerObject(intermediate1 - new Vector3(2.0f, 0f, -2.0f), Quaternion.Euler(0, 90, 0));
                PlaceCornerObject(intermediate2 + new Vector3(2.0f, 0f, -2.0f), Quaternion.Euler(0, -90, 0));
                Debug.Log("g");
            }
            else // Abs(d) >= Abs(c)
            {
                PlaceCornerObject(intermediate1 - new Vector3(-2.0f, 0f, 2.0f), Quaternion.Euler(0, 270, 0));
                PlaceCornerObject(intermediate2 + new Vector3(-2.0f, 0f, 2.0f), Quaternion.Euler(0, 90, 0));
                Debug.Log("h");
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

                PlaceCornerObject(intermediate1 - new Vector3(-2.0f, 0f, 2.0f), Quaternion.Euler(0, 270, 0));
                PlaceCornerObject(intermediate2 + new Vector3(-2.0f, 0f, 2.0f), Quaternion.Euler(0, 90, 0));
            }
            else
            {
                PlaceCornerObject(intermediate1 - new Vector3(+2.0f, 0f, 2.0f), Quaternion.Euler(0, 0, 0));
                PlaceCornerObject(intermediate2 + new Vector3(2.0f, 0f, 2.0f), Quaternion.Euler(0, 180, 0));
            }
        }
        else if ((startName.Contains("S") && endName.Contains("N")))
        {
            if (start.transform.position.x > end.transform.position.x)
            {
                PlaceCornerObject(intermediate1 - new Vector3(-2.0f, 0f, -2.0f), Quaternion.Euler(0, 180, 0));
                PlaceCornerObject(intermediate2 + new Vector3(-2.0f, 0f, -2.0f), Quaternion.Euler(0, 0, 0));
            }
            else
            {
                PlaceCornerObject(intermediate1 - new Vector3(2.0f, 0f, -2.0f), Quaternion.Euler(0, 90, 0));
                PlaceCornerObject(intermediate2 + new Vector3(2.0f, 0f, -2.0f), Quaternion.Euler(0, 270, 0));
            }
        }
        else if ((startName.Contains("E") && endName.Contains("W")))
        {
            if (start.transform.position.z > end.transform.position.z)
            {
                PlaceCornerObject(intermediate2 - new Vector3(-2.0f, 0f, 2.0f), Quaternion.Euler(0, 270, 0));
                PlaceCornerObject(intermediate1 + new Vector3(-2.0f, 0f, 2.0f), Quaternion.Euler(0, 90, 0));
            }
            else
            {
                PlaceCornerObject(intermediate2 - new Vector3(-2.0f, 0f, -2.0f), Quaternion.Euler(0, 180, 0));
                PlaceCornerObject(intermediate1 + new Vector3(-2.0f, 0f, -2.0f), Quaternion.Euler(0, 0, 0));
            }
        }
        else if ((startName.Contains("W") && endName.Contains("E")))
        {
            if (start.transform.position.z > end.transform.position.z)
            {
                PlaceCornerObject(intermediate1 - new Vector3(-2.0f, 0f, -2.0f), Quaternion.Euler(0, 180, 0));
                PlaceCornerObject(intermediate2 + new Vector3(-2.0f, 0f, -2.0f), Quaternion.Euler(0, 0, 0));
            }
            else
            {
                PlaceCornerObject(intermediate1 - new Vector3(-2.0f, 0f, 2.0f), Quaternion.Euler(0, 270, 0));
                PlaceCornerObject(intermediate2 + new Vector3(-2.0f, 0f, 2.0f), Quaternion.Euler(0, 90, 0));
            }
        }

    }

    private void PlaceCornerObject(Vector3 position, Quaternion rotation)
    {
        GameObject tileAndCornerParent = EnsureParentStructure();
        GameObject cornerParent = tileAndCornerParent.transform.Find("Corner").gameObject;

        GameObject corner = Instantiate(cornerPrefab, position, rotation);
        corner.transform.parent = cornerParent.transform;
    }


    void PlaceSpecialRooms(BSPNode node)
    {
        // 
        List<BSPNode> leafNodes = GetLeafNodes(node);
        if (leafNodes.Count < 3) return; // 

        // StartRoom 
        BSPNode startNode = leafNodes[Random.Range(0, leafNodes.Count)];
        InstantiateRoom(startRoomPrefab, startNode.room, startNode); //
        startNode.roomType = RoomType.StartRoom;

        // BossRoom 
        leafNodes.Remove(startNode);
        BSPNode bossNode = GetFurthestNode(startNode, leafNodes);
        InstantiateRoom(bossRoomPrefab, bossNode.room, bossNode); // 
        bossNode.roomType = RoomType.BossRoom;

        // EliteRoom 
        leafNodes.Remove(bossNode);
        BSPNode eliteNode = leafNodes[Random.Range(0, leafNodes.Count)];
        InstantiateRoom(eliteRoomPrefab, eliteNode.room, eliteNode); //
        eliteNode.roomType = RoomType.EliteRoom;
    }

    void PlaceNormalRooms(BSPNode node, int count)
    {
        // 
        List<BSPNode> leafNodes = GetLeafNodes(node);
        foreach (BSPNode leafNode in leafNodes)
        {
            if (count <= 0 || leafNode.roomType != RoomType.None) continue;
            InstantiateRoom(normalRoomPrefabs[Random.Range(0, normalRoomPrefabs.Length)], leafNode.room, leafNode); // 
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
                node.roomObject = roomObject; //
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

        // Recursively check connections in child nodes
        CheckRoomConnections(node.leftChild, connectedRooms);
        CheckRoomConnections(node.rightChild, connectedRooms);

        // Check if this node has a room object
        if (node.roomObject != null)
        {
            // Check connection between left and right child rooms if they both have roomObjects
            if (node.leftChild != null && node.leftChild.roomObject != null &&
                node.rightChild != null && node.rightChild.roomObject != null)
            {
                connectedRooms.Add(new KeyValuePair<BSPNode, BSPNode>(node.leftChild, node.rightChild));
            }
        }
    }
    private void PlaceTilesAlongLine(Vector3 start, Vector3 end, bool skipLastTile)
    {
        GameObject tileAndCornerParent = EnsureParentStructure();
        GameObject tileParent = tileAndCornerParent.transform.Find("Tile").gameObject;

        Vector3 direction = (end - start).normalized;
        float distance = Vector3.Distance(start, end);
        float tileStep = tileSize.x;
        int tileCount = Mathf.CeilToInt(distance / tileStep);


        if (skipLastTile)
            tileCount--;

        Quaternion rotation = Quaternion.LookRotation(direction);

        for (int i = 0; i < tileCount; i++)
        {
            Vector3 tilePosition = start + direction * tileStep * i;
            GameObject tile =Instantiate(tilePrefab, tilePosition, rotation);
            tile.transform.parent = tileParent.transform;
            if (i == tileCount - 1)
            {
                GameObject tile2 = Instantiate(tilePrefab, end, rotation);
                tile2.transform.parent = tileParent.transform;
            }
        }
    }


    private GameObject EnsureParentStructure()
    {
        // "TileAndCorner" 오브젝트를 찾거나 새로 생성
        GameObject tileAndCornerParent = transform.Find("TileAndCorner")?.gameObject ?? new GameObject("TileAndCorner");

        // "TileAndCorner" 오브젝트를 BSPGenerator의 하위로 설정
        if (tileAndCornerParent.transform.parent != transform)
        {
            tileAndCornerParent.transform.parent = transform;
        }

        // "Tile"과 "Corner" 자식 오브젝트를 찾거나 생성
        GameObject tileParent = tileAndCornerParent.transform.Find("Tile")?.gameObject ?? new GameObject("Tile");
        GameObject cornerParent = tileAndCornerParent.transform.Find("Corner")?.gameObject ?? new GameObject("Corner");

        // "Tile"과 "Corner"를 "TileAndCorner"의 하위로 설정
        if (tileParent.transform.parent != tileAndCornerParent.transform)
        {
            tileParent.transform.parent = tileAndCornerParent.transform;
        }
        if (cornerParent.transform.parent != tileAndCornerParent.transform)
        {
            cornerParent.transform.parent = tileAndCornerParent.transform;
        }

        // "TileAndCorner" 오브젝트 반환
        return tileAndCornerParent;
    }

}