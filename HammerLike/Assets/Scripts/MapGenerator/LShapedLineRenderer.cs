using UnityEngine;


public class LShapedLineRenderer : MonoBehaviour
{
    public GameObject pointA;
    public GameObject pointB;
    public GameObject verticalTilePrefab;  // 
    public GameObject horizontalTilePrefab;  // 
    public GameObject topLeftCornerPrefab; // 
    public GameObject topRightCornerPrefab; // 
    public GameObject bottomLeftCornerPrefab; // 
    public GameObject bottomRightCornerPrefab; // 
    public GameObject cornerPrefab; // 
    public GameObject cornerPrefab2; // 
    public GameObject container;
    private Vector3 tileSize;  //
    private float distance=0;

    void Start()
    {

        Generate();
    }

    public void Generate()
    {
        tileSize = GetPrefabSize(verticalTilePrefab);  // 타일 프리팹 사이즈 측정 Collider 혹은 MeshRenderer를 사용하고 없을시 1로 반환

        container = new GameObject("TilesAndCornersContainer");

        if (pointA != null && pointB != null)
        {
            SetupLinePositions();
        }

        GetDistance();
    }

    private Vector3 GetPrefabSize(GameObject prefab)
    {
        Renderer renderer = prefab.GetComponent<Renderer>();
        if (renderer != null)
        {
            return renderer.bounds.size;
        }
        return new Vector3(1, 0, 1);  // 疫꿸퀡????由곁몴?獄쏆꼹??(???쐭??? ??용뮉 野껋럩??
    }

    public void SetupLinePositions()
    {
        Vector3 startPosition = pointA.transform.position;
        Vector3 endPosition = pointB.transform.position;

        float deltaX = endPosition.x - startPosition.x;
        float deltaZ = endPosition.z - startPosition.z;



        if (deltaX == 0 || deltaZ == 0)
        {
            if(deltaX==0)
                PlaceTilesAlongLineVertical(startPosition, endPosition, false);
            if(deltaZ==0)
                PlaceTilesAlongLineHorizontal(startPosition, endPosition, false);
        }
        else
        {
            Vector3 midPoint1, midPoint2;

            if (pointA.name.Contains("N") && pointB.name.Contains("S") || pointA.name.Contains("S") && pointB.name.Contains("N"))
            {
                
                midPoint1 = new Vector3(startPosition.x, startPosition.y, (startPosition.z + endPosition.z) / 2);
                midPoint2 = new Vector3(endPosition.x, endPosition.y, (startPosition.z + endPosition.z) / 2);
                DetermineAndPlaceCornerObjects(pointA, pointB, midPoint1, midPoint2);
                
            }
            else
            {
                midPoint1 = new Vector3((startPosition.x + endPosition.x) / 2, startPosition.y, startPosition.z);
                midPoint2 = new Vector3((startPosition.x + endPosition.x) / 2, endPosition.y, endPosition.z);
                DetermineAndPlaceCornerObjects(pointA, pointB, midPoint1, midPoint2);

                
            }


            
        }
    }



    private void PlaceTilesAlongLineVertical (Vector3 start, Vector3 end, bool skipLastTile)
    {
        Vector3 direction = (end - start).normalized;
        float distance = Vector3.Distance(start, end);
        float tileStep = tileSize.x;
        int tileCount = Mathf.FloorToInt(distance / tileStep);

        if (skipLastTile)
            tileCount--;

        //Quaternion rotation = Quaternion.LookRotation(direction);
        Quaternion rotation = Quaternion.Euler(0,0,0);

        for (int i = 0; i <= tileCount; i+=4)
        {
            Vector3 tilePosition = start + direction * tileStep * i;
            GameObject tile = Instantiate(verticalTilePrefab, tilePosition, rotation, container.transform);
            if (i == tileCount - 1)
            {
                Instantiate(verticalTilePrefab, end, rotation, container.transform);
            }
        }
    }

    private void PlaceTilesAlongLineHorizontal(Vector3 start, Vector3 end, bool skipLastTile)
    {
        Vector3 direction = (end - start).normalized;
        float distance = Vector3.Distance(start, end);
        float tileStep = tileSize.x;
        bool isDecimal = false;
        if (distance % tileStep != 0)
        {
            isDecimal = true;
        }
        int tileCount = Mathf.FloorToInt(distance / tileStep);

        if (skipLastTile)
            tileCount--;

        //Quaternion rotation = Quaternion.LookRotation(direction);
        Quaternion rotation = Quaternion.identity;

        for (int i = 0; i <= tileCount; i+=2)
        {
            Vector3 tilePosition = start + direction * tileStep * i;
            GameObject tile = Instantiate(horizontalTilePrefab, tilePosition, rotation, container.transform);
            if (isDecimal || i==tileCount-1)
            {
                Instantiate(horizontalTilePrefab, end, rotation, container.transform);
            }
        }
    }

    private void PlaceTilesAlongLine(Vector3 start, Vector3 end, bool skipLastTile)
    {
        Vector3 direction = (end - start).normalized;
        float distance = Vector3.Distance(start, end);
        float tileStep = tileSize.x;
        int tileCount = Mathf.FloorToInt(distance / tileStep);

        if (skipLastTile)
            tileCount--;

        Quaternion rotation = Quaternion.LookRotation(direction);

        for (int i = 0; i < tileCount; i++)
        {
            Vector3 tilePosition = start + direction * tileStep * i;
            GameObject tile = Instantiate(verticalTilePrefab, tilePosition, rotation, container.transform);
            if (i == tileCount - 1)
            {
                Instantiate(verticalTilePrefab, end, rotation, container.transform);
            }
        }
    }

    private void PlaceTilesAlongLine2(Vector3 start, Vector3 end, bool skipLastTile)
    {
        Vector3 direction = (end - start).normalized;
        float distance = Vector3.Distance(start, end);
        float tileStep = tileSize.x;
        int tileCount = Mathf.FloorToInt(distance / tileStep);

        if (skipLastTile)
            tileCount--;

        Quaternion rotation = Quaternion.LookRotation(direction);

        for (int i = 0; i < tileCount; i++)
        {
            Vector3 tilePosition = start + direction * tileStep * i;
            GameObject tile = Instantiate(horizontalTilePrefab, tilePosition, rotation, container.transform);
            if (i == tileCount - 1)
            {
                Instantiate(horizontalTilePrefab, end, rotation, container.transform);
            }
        }
    }


   
    private void PlaceCornerObject(Vector3 position, Quaternion rotation)
    {
        Instantiate(cornerPrefab, position, rotation, container.transform);
    }

    private void PlaceTopLeftCornerObject(Vector3 position, Quaternion rotation)
    {
        Instantiate(topLeftCornerPrefab, position, rotation, container.transform);
    }

    private void PlaceTopRightCornerObject(Vector3 position, Quaternion rotation)
    {
        Instantiate(topRightCornerPrefab, position, rotation, container.transform);
    }

    private void PlaceBottomLeftCornerObject(Vector3 position, Quaternion rotation)
    {
        Instantiate(bottomLeftCornerPrefab, position, rotation, container.transform);
    }

    private void PlaceBottomRightCornerObject(Vector3 position, Quaternion rotation)
    {
        Instantiate(bottomRightCornerPrefab, position, rotation, container.transform);
    }

    private void PlaceCornerObject2(Vector3 position, Quaternion rotation)
    {
        Instantiate(cornerPrefab2, position, rotation, container.transform);
    }

    private void GetDistance()
    {
        Debug.Log("PointA : "+pointA.transform.position+" PointB : "+ pointB.transform.position);
        Debug.Log(pointA.transform.position.x-pointB.transform.position.x);
        Debug.Log(pointA.transform.position.z-pointB.transform.position.z);

        distance = Vector3.Distance(pointA.transform.position, pointB.transform.position);

        Debug.Log(distance);

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
                Debug.Log("1");
            }
            else
            {
                PlaceTopLeftCornerObject(intermediate1, Quaternion.identity);
                PlaceBottomRightCornerObject(intermediate2, Quaternion.identity);
                Debug.Log("2");
            }

            Vector3 startPosition = pointA.transform.position;
            Vector3 endPosition = pointB.transform.position;
            Vector3 midPoint1 = new Vector3(startPosition.x, startPosition.y, (startPosition.z + endPosition.z) / 2);
            Vector3 midPoint2 = new Vector3(endPosition.x, endPosition.y, (startPosition.z + endPosition.z) / 2);
            Vector3 cornerSize = GetPrefabSize(cornerPrefab); // 
            Vector3 direction1 = (midPoint2 - midPoint1).normalized;
            Vector3 offsetStart1 = midPoint1 + direction1 * cornerSize.x * 4; // 
            Vector3 direction0 = (midPoint1 - startPosition).normalized;
            Vector3 direction2 = (endPosition - midPoint2).normalized;
            Vector3 offsetStart2 = midPoint2 + direction2 * cornerSize.x * 8f; // 
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
                PlaceTopLeftCornerObject(intermediate2 , Quaternion.identity);
                Debug.Log("3");
            }
            else
            {
                PlaceBottomLeftCornerObject(intermediate1, Quaternion.identity);
                PlaceTopRightCornerObject(intermediate2, Quaternion.identity);
                Debug.Log("4");
            }

            Vector3 startPosition = pointA.transform.position;
            Vector3 endPosition = pointB.transform.position;
            Vector3 midPoint1 = new Vector3(startPosition.x, startPosition.y, (startPosition.z + endPosition.z) / 2);
            Vector3 midPoint2 = new Vector3(endPosition.x, endPosition.y, (startPosition.z + endPosition.z) / 2);
            Vector3 cornerSize = GetPrefabSize(cornerPrefab); // 
            Vector3 direction1 = (midPoint2 - midPoint1).normalized;
            Vector3 offsetStart1 = midPoint1 + direction1 * cornerSize.x * 4; // 
            Vector3 direction0 = (midPoint1 - startPosition).normalized;
            Vector3 direction2 = (endPosition - midPoint2).normalized;
            Vector3 offsetStart2 = midPoint2 + direction2 * cornerSize.x*16; // 
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
                Vector3 startPosition = pointA.transform.position;
                Vector3 endPosition = pointB.transform.position;
                Vector3 midPoint1 = new Vector3((startPosition.x + endPosition.x) / 2, startPosition.y, startPosition.z);
                Vector3 midPoint2 = new Vector3((startPosition.x + endPosition.x) / 2, endPosition.y, endPosition.z);
                Vector3 cornerSize = GetPrefabSize(cornerPrefab); // 
                Vector3 direction1 = (midPoint2 - midPoint1).normalized;
                Vector3 offsetStart1 = midPoint1 + direction1 * cornerSize.x * 16; // 
                Vector3 direction0 = (midPoint1 - startPosition).normalized;
                Vector3 direction2 = (endPosition - midPoint2).normalized;
                Vector3 offsetStart2 = midPoint2 + direction2 * cornerSize.x * 4f; // 
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
                Vector3 startPosition = pointA.transform.position;
                Vector3 endPosition = pointB.transform.position;
                Vector3 midPoint1 = new Vector3((startPosition.x + endPosition.x) / 2, startPosition.y, startPosition.z);
                Vector3 midPoint2 = new Vector3((startPosition.x + endPosition.x) / 2, endPosition.y, endPosition.z);
                Vector3 cornerSize = GetPrefabSize(cornerPrefab); // 
                Vector3 direction1 = (midPoint2 - midPoint1).normalized;
                Vector3 offsetStart1 = midPoint1 + direction1 * cornerSize.x * 12; // 
                Vector3 direction0 = (midPoint1 - startPosition).normalized;
                Vector3 direction2 = (endPosition - midPoint2).normalized;
                Vector3 offsetStart2 = midPoint2 + direction2 * cornerSize.x * 4f; // 
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
                Vector3 startPosition = pointA.transform.position;
                Vector3 endPosition = pointB.transform.position;
                Vector3 midPoint1 = new Vector3((startPosition.x + endPosition.x) / 2, startPosition.y, startPosition.z);
                Vector3 midPoint2 = new Vector3((startPosition.x + endPosition.x) / 2, endPosition.y, endPosition.z);
                Vector3 cornerSize = GetPrefabSize(cornerPrefab); // 
                Vector3 direction1 = (midPoint2 - midPoint1).normalized;
                Vector3 offsetStart1 = midPoint1 + direction1 * cornerSize.x * 16; // 
                Vector3 direction0 = (midPoint1 - startPosition).normalized;
                Vector3 direction2 = (endPosition - midPoint2).normalized;
                Vector3 offsetStart2 = midPoint2 + direction2 * cornerSize.x * 4f; // 
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
                Vector3 startPosition = pointA.transform.position;
                Vector3 endPosition = pointB.transform.position;
                Vector3 midPoint1 = new Vector3((startPosition.x + endPosition.x) / 2, startPosition.y, startPosition.z);
                Vector3 midPoint2 = new Vector3((startPosition.x + endPosition.x) / 2, endPosition.y, endPosition.z);
                Vector3 cornerSize = GetPrefabSize(cornerPrefab); // 
                Vector3 direction1 = (midPoint2 - midPoint1).normalized;
                Vector3 offsetStart1 = midPoint1 + direction1 * cornerSize.x * 12; // 
                Vector3 direction0 = (midPoint1 - startPosition).normalized;
                Vector3 direction2 = (endPosition - midPoint2).normalized;
                Vector3 offsetStart2 = midPoint2 + direction2 * cornerSize.x * 4f; // 
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
}