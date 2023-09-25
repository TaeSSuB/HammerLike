using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CRoomTemplates : MonoBehaviour
{
    public GameObject[] rightRoomsArray;
    public GameObject[] leftRoomsArray;
    public GameObject[] topRoomsArray;
    public GameObject[] bottomRoomsArray;

    public GameObject rightRoomEnd;
    public GameObject leftRoomEnd;
    public GameObject topRoomEnd;
    public GameObject bottomRoomEnd;

    public GameObject closedRoom;
    public GameObject bossRoom;
    public GameObject bossRoomDoor;

    public List<GameObject> rooms;

    public GameObject bossRoomInstance;

    bool isBossSpawn;

    public int roomPosMultiply = 1;
    public int roomMaxSpread = 3;

    private void Start()
    {
        Invoke("SpawnBossRoom", 2f);
    }

    void SpawnBossRoom()
    {
        for (int i = 0; i < rooms.Count; i++)
        {
            if(i == rooms.Count - 1)
            {
                rooms[i].SetActive(false);
                bossRoomInstance = Instantiate(bossRoom, rooms[i].transform.position, Quaternion.identity);
                var bossRoomSpawner = bossRoomInstance.GetComponent<RoomSpawnManager>();
                for (int j = 0; j < bossRoomSpawner.matches.Length; j++)
                {
                    bossRoomSpawner.matches[j].isActive = rooms[i].GetComponent<RoomSpawnManager>().matches[j].isActive;

                    if (bossRoomSpawner.matches[j].isActive)
                    {
                        bossRoomSpawner.openRoomCorridor(j);
                        var connectRoomMatches = rooms[i].GetComponent<RoomSpawnManager>().matches[j].roomInstance.GetComponent<RoomSpawnManager>().matches;
                        for (int k = 0; k < connectRoomMatches.Length; k++)
                        {
                            var reverseIdx = Mathf.Abs(k - j);

                            if (reverseIdx == 2)
                            {
                                connectRoomMatches[k].roomInstance = bossRoomInstance;
                                Instantiate(bossRoomDoor, connectRoomMatches[k].wall.transform.position, Quaternion.identity);
                                bossRoomSpawner.matches[j].roomInstance = connectRoomMatches[k].roomInstance;
                            }
                        }
                    }
                }
            }
        }
    }
}
