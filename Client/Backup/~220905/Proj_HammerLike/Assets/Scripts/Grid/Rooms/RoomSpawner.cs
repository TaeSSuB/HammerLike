using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum OpenDir { W, A, S, D };

public class RoomSpawner : MonoBehaviour
{
    public OpenDir openDir;
    public bool isSpawn = false;

    public CRoomTemplates templates;
    public CRoom room;

    // Start is called before the first frame update
    void Awake()
    {
        templates = GameObject.FindGameObjectWithTag("Rooms").GetComponent<CRoomTemplates>();
        //room = GetComponentInParent<CRoom>();
        //Invoke("SortSpawnPos", 0.2f);
        SortSpawnPos();
        //Spawn();
        //float rnd = Random.Range(0f, 0.1f);
        Invoke("Spawn", 0.05f );
    }

    void SortSpawnPos()
    {
        switch (openDir)
        {
            case OpenDir.W:
                transform.localPosition = new Vector3Int(0, room.gridSize.y * templates.roomPosMultiply);
                break;
            case OpenDir.A:
                transform.localPosition = new Vector3Int(-(room.gridSize.x) * templates.roomPosMultiply, 0);
                break;
            case OpenDir.S:
                transform.localPosition = new Vector3Int(0, -(room.gridSize.y) * templates.roomPosMultiply);
                break;
            case OpenDir.D:
                transform.localPosition = new Vector3Int(room.gridSize.x * templates.roomPosMultiply, 0);
                break;
            default:
                Debug.Log("OpenDir is Null");
                break;
        }
    }

    void Spawn()
    {
        if (!isSpawn)
        {
            GameObject roomInstance;

            switch (openDir)
            {
                case OpenDir.W:
                    transform.localPosition = new Vector3Int(0, room.gridSize.y * templates.roomPosMultiply);
                    roomInstance = InstanciateRoom(templates.bottomRoomsArray, templates.bottomRoomEnd);
                    room.spawnManager.matches[0].roomInstance = roomInstance;
                    roomInstance.GetComponent<RoomSpawnManager>().matches[2].roomInstance = room.spawnManager.gameObject;
                    break;

                case OpenDir.A:
                    transform.localPosition = new Vector3Int(-(room.gridSize.x) * templates.roomPosMultiply, 0);
                    roomInstance = InstanciateRoom(templates.rightRoomsArray, templates.rightRoomEnd);
                    room.spawnManager.matches[1].roomInstance = roomInstance;
                    roomInstance.GetComponent<RoomSpawnManager>().matches[3].roomInstance = room.spawnManager.gameObject;
                    break;

                case OpenDir.S:
                    transform.localPosition = new Vector3Int(0, -(room.gridSize.y) * templates.roomPosMultiply);
                    roomInstance = InstanciateRoom(templates.topRoomsArray, templates.topRoomEnd);
                    room.spawnManager.matches[2].roomInstance = roomInstance;
                    roomInstance.GetComponent<RoomSpawnManager>().matches[0].roomInstance = room.spawnManager.gameObject;
                    break;

                case OpenDir.D:
                    transform.localPosition = new Vector3Int(room.gridSize.x * templates.roomPosMultiply, 0);
                    roomInstance = InstanciateRoom(templates.leftRoomsArray, templates.leftRoomEnd);
                    room.spawnManager.matches[3].roomInstance = roomInstance;
                    roomInstance.GetComponent<RoomSpawnManager>().matches[1].roomInstance = room.spawnManager.gameObject;
                    break;

                default:
                    Debug.Log("OpenDir is Null");
                    break;
            }
            isSpawn = true;
        }
    }

    GameObject InstanciateRoom(GameObject[] roomArray, GameObject endRoom)
    {
        GameObject roomInstance = null;

        if (roomArray.Length > 0)
        {
            int _randNum = Random.Range(0, roomArray.Length);

            //roomArray[_randNum].GetComponent<RoomSpawnManager>().roomSpreadNum++;
            var spreadNum = room.spawnManager.roomSpreadNum;

            if (spreadNum < templates.roomMaxSpread)
            {
                roomInstance = Instantiate(roomArray[_randNum], transform.position, roomArray[_randNum].transform.rotation);
                GetExist(roomInstance);
                roomInstance.GetComponent<RoomSpawnManager>().roomSpreadNum = room.spawnManager.roomSpreadNum + 1; // 인스턴스 + 1
            }
            else if (spreadNum >= templates.roomMaxSpread)
            {
                roomInstance = Instantiate(endRoom, transform.position, endRoom.transform.rotation);
                Debug.Log("End Line");
            }
        }

        return roomInstance;
    }

    void GetExist(GameObject obj)
    {
        var matches = obj.GetComponentInParent<RoomSpawnManager>().matches;

        // Up (W) = 0
        if (obj.transform.position.y > room.transform.position.y)
        {
            if (!matches[2].isActive) // Down is Active?
            {
                room.spawnManager.matches[0].isActive = false;
                room.spawnManager.closeRoomCorridor(0);
                Debug.Log("Error");
            }
        }
        // Down (S) = 2
        else if (obj.transform.position.y < room.transform.position.y)
        {
            if (!matches[0].isActive) // Up is Active?
            {
                room.spawnManager.matches[2].isActive = false;
                room.spawnManager.closeRoomCorridor(2);
                Debug.Log("Error");
            }
        }

        // Right (D) = 3
        if (obj.transform.position.x > room.transform.position.x)
        {
            if (!matches[1].isActive) // Left is Active?
            {
                room.spawnManager.matches[3].isActive = false;
                room.spawnManager.closeRoomCorridor(3);
                Debug.Log("Error");
            }
        }
        // Left (A) = 1
        else if (obj.transform.position.x < room.transform.position.x)
        {
            if (!matches[3].isActive) // Right is Active?
            {
                room.spawnManager.matches[1].isActive = false;
                room.spawnManager.closeRoomCorridor(1);
                Debug.Log("Error");
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D col2d)
    {
        if (col2d.CompareTag("SpawnPoint"))
        {
            if (!col2d.GetComponent<RoomSpawner>().isSpawn && !isSpawn && col2d.transform.position != Vector3.zero)
            {
                //GetExist(col2d.gameObject);
                var closeRoom = Instantiate(templates.closedRoom, transform.position, templates.closedRoom.transform.rotation);
                GetExist(closeRoom);
                Destroy(gameObject);
            }
            isSpawn = true;
        }

        if (col2d.CompareTag("Room"))
            GetExist(col2d.gameObject);
    }
}
