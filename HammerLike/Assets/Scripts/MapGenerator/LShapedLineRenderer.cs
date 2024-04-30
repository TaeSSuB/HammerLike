using UnityEngine;


public class LShapedLineRenderer : MonoBehaviour
{
    public GameObject pointA;
    public GameObject pointB;
    public GameObject tilePrefab;  // ?????袁ⓥ봺??
    public GameObject cornerPrefab; // ?꾨뗀瑗??袁ⓥ봺??
    public GameObject container;
    private Vector3 tileSize;  // ????깆벥 ??쇱젫 ??由?

    void Start()
    {
        
        tileSize = GetPrefabSize(tilePrefab);  // ?袁ⓥ봺?諭????由곁몴?揶쎛?紐꾩긾

        container = new GameObject("TilesAndCornersContainer");

        if (pointA != null && pointB != null)
        {
            SetupLinePositions();
        }
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

    private void SetupLinePositions()
    {
        Vector3 startPosition = pointA.transform.position;
        Vector3 endPosition = pointB.transform.position;

        float deltaX = endPosition.x - startPosition.x;
        float deltaZ = endPosition.z - startPosition.z;



        if (deltaX == 0 || deltaZ == 0)
        {
            
            PlaceTilesAlongLine(startPosition, endPosition, false);
        }
        else
        {
            Vector3 midPoint = new Vector3((startPosition.x + endPosition.x) / 2, startPosition.y, (startPosition.z + endPosition.z) / 2);
            Vector3 intermediate1, intermediate2;

            if (Mathf.Abs(startPosition.x - endPosition.x) > Mathf.Abs(startPosition.z - endPosition.z))
            {
                intermediate1 = new Vector3(midPoint.x, startPosition.y, startPosition.z);
                intermediate2 = new Vector3(midPoint.x, endPosition.y, endPosition.z);
            }
            else
            {
                intermediate1 = new Vector3(startPosition.x, startPosition.y, midPoint.z);
                intermediate2 = new Vector3(endPosition.x, endPosition.y, midPoint.z);
            }

            

            DetermineAndPlaceCornerObjects(deltaX, deltaZ, intermediate1, intermediate2);
            Vector3 cornerKMJ = new Vector3(2, 0, 6);
            Vector3 cornerSize = GetPrefabSize(cornerPrefab); // 
            Vector3 direction1 = (intermediate2 - intermediate1).normalized;
            Vector3 offsetStart1 = intermediate1 + direction1 * cornerSize.x * 4; // 
            Vector3 direction0 = (intermediate1 - startPosition).normalized;
            Vector3 direction2 = (endPosition - intermediate2).normalized;
            Vector3 offsetStart2 = intermediate2 + direction2 * cornerSize.x * 4f; // 
            Vector3 offsetEnd1 = intermediate1 - direction0 * cornerSize.x * 2;
            Vector3 offsetEnd11 = offsetEnd1 - direction0 * cornerSize.z * 2;
            Vector3 offsetEnd2 = intermediate2 - direction1 * cornerSize.x * 2;
            Vector3 offsetEnd22 = offsetEnd2 - direction1 * cornerSize.z * 2;
            PlaceTilesAlongLine(startPosition, offsetEnd11, false);
            PlaceTilesAlongLine(offsetStart1, offsetEnd22, false);
            PlaceTilesAlongLine(offsetStart2, endPosition, false);
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
            GameObject tile = Instantiate(tilePrefab, tilePosition, rotation, container.transform);
            if (i == tileCount - 1)
            {
                Instantiate(tilePrefab, end, rotation, container.transform);
            }
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
            }
            else // c <= d
            {
                PlaceCornerObject(intermediate1 - new Vector3(2.0f, 0f, -2.0f), Quaternion.Euler(0, 90, 0));
                PlaceCornerObject(intermediate2 + new Vector3(2.0f, 0f, -2.0f), Quaternion.Euler(0, -90, 0));
            }
        }
        else if (c > 0 && d < 0)
        {
            if (Mathf.Abs(c) > Mathf.Abs(d))
            {
                PlaceCornerObject(intermediate1 - new Vector3(-2.0f, 0f, -2.0f), Quaternion.Euler(0, 180, 0));
                PlaceCornerObject(intermediate2 + new Vector3(-2.0f, 0f, -2.0f), Quaternion.Euler(0, 0, 0));
            }
            else // Abs(d) >= Abs(c)
            {
                PlaceCornerObject(intermediate1 - new Vector3(2.0f, 0f, 2.0f), Quaternion.Euler(0, 0, 0));
                PlaceCornerObject(intermediate2 + new Vector3(2.0f, 0f, 2.0f), Quaternion.Euler(0, 180, 0));
            }
        }
        else if (c < 0 && d > 0)
        {
            if (Mathf.Abs(c) > d)
            {
                PlaceCornerObject(intermediate1 - new Vector3(2.0f, 0f, 2.0f), Quaternion.identity);
                PlaceCornerObject(intermediate2 + new Vector3(2.0f, 0f, 2.0f), Quaternion.Euler(0, 180, 0));
            }
            else // d >= Abs(c)
            {
                PlaceCornerObject(intermediate1 - new Vector3(-2.0f, 0f, -2.0f), Quaternion.Euler(0, 180, 0));
                PlaceCornerObject(intermediate2 + new Vector3(-2.0f, 0f, -2.0f), Quaternion.identity);
            }
        }
        else if (c < 0 && d < 0)
        {
            if (Mathf.Abs(c) > Mathf.Abs(d))
            {
                PlaceCornerObject(intermediate1 - new Vector3(2.0f, 0f, -2.0f), Quaternion.Euler(0, 90, 0));
                PlaceCornerObject(intermediate2 + new Vector3(2.0f, 0f, -2.0f), Quaternion.Euler(0, -90, 0));
            }
            else // Abs(d) >= Abs(c)
            {
                PlaceCornerObject(intermediate1 - new Vector3(-2.0f, 0f, 2.0f), Quaternion.Euler(0, 270, 0));
                PlaceCornerObject(intermediate2 + new Vector3(-2.0f, 0f, 2.0f), Quaternion.Euler(0, 90, 0));
            }
        }
    }

    private void PlaceCornerObject(Vector3 position, Quaternion rotation)
    {
        Instantiate(cornerPrefab, position, rotation, container.transform);
    }
}