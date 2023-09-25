using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApartmentRoomSpawner : MonoBehaviour
{
    [System.Serializable]
    public class ApartmentRoom
    {
        public bool isEnter = false;
        public Dictionary<string, bool> dicDirection;
        public Vector2Int roomPosition;
        public GameObject roomPrefab;
        public List<Vector2Int> neighborPosition;
        public List<int> neighborRoomsIdx;
    }

    public List<ApartmentRoom> rooms;
    string[] directionStr = { "Up", "Left", "Down", "Right" };

    public Vector2Int dungeonSize;
    public int defaultRoomSize = 1;
    [Range(0, 16)]
    public int dirOffset = 0;
    public Vector2Int startPos;

    public Vector2 offset;

    public GameObject roomPrefab;

    public List<Vector2Int> openList = new List<Vector2Int>();
    public List<Vector2Int> closeList = new List<Vector2Int>();

    CRoomTemplates templates;

    private void Start()
    {
        //templates = GameObject.FindGameObjectWithTag("Rooms").GetComponent<CRoomTemplates>();
        SpawnGrid();
    }

    public void RandomConnectGrid()
    {
        for (int i = 0; i < rooms.Count; i++)
        {
            var pos = rooms[i].roomPosition;

            var rndNum = Random.Range(0, 16 - dirOffset);
            var binaryRndNum = System.Convert.ToString(rndNum, 2);
            if (rndNum < 8)
            {
                binaryRndNum = "0" + binaryRndNum;

                if (rndNum < 4)
                {
                    binaryRndNum = "0" + binaryRndNum;
                    if (rndNum < 2)
                    {
                        binaryRndNum = "0" + binaryRndNum;
                    }
                }
            }

            //List<bool> boolList = new List<bool>();
            Dictionary<string, bool> boolDic = new Dictionary<string, bool>();

            for (int j = 0; j < 4; j++)
            {
                var binaryBoolean = int.Parse(binaryRndNum[j].ToString());
                bool resultBoolean = (binaryBoolean == 0) ? false : true;
                //boolList.Add(resultBoolean);
                boolDic.Add(directionStr[j], resultBoolean);
            }

            rooms[i].dicDirection = boolDic;

            if (pos.x - defaultRoomSize < 0)
            {
                //Debug.Log("-x no");
                rooms[i].dicDirection["Left"] = false;
            }
            if (pos.x + defaultRoomSize >= dungeonSize.x * defaultRoomSize)
            {
                //Debug.Log("+x no");
                rooms[i].dicDirection["Right"] = false;
            }
            if (pos.y - defaultRoomSize < 0)
            {
                //Debug.Log("-y no");
                rooms[i].dicDirection["Down"] = false;
            }
            if (pos.y + defaultRoomSize >= dungeonSize.y * defaultRoomSize)
            {
                //Debug.Log("+y no");
                rooms[i].dicDirection["Up"] = false;
            }
        }


        openList.Clear();
        closeList.Clear();

        for (int i = 0; i < rooms.Count; i++)
        {
            int dirNum = 0;

            foreach (var dir in rooms[i].dicDirection)
            {
                if(dir.Value)
                {
                    dirNum++;

                    foreach (var neighbor in rooms)
                    {
                        if (dir.Key == "Right")
                        {
                            var pos = rooms[i].roomPosition + new Vector2Int(defaultRoomSize, 0);

                            if (neighbor.roomPosition == pos)
                            {
                                neighbor.dicDirection["Left"] = true;
                                if (dirNum > rooms[i].neighborPosition.Count)
                                {
                                    rooms[i].neighborPosition.Add(neighbor.roomPosition);
                                    rooms[i].neighborRoomsIdx.Add(i);
                                }
                            }
                        }
                        else if (dir.Key == "Left")
                        {
                            var pos = rooms[i].roomPosition - new Vector2Int(defaultRoomSize, 0);

                            if (neighbor.roomPosition == pos)
                            {
                                neighbor.dicDirection["Right"] = true;
                                if (dirNum > rooms[i].neighborPosition.Count)
                                {
                                    rooms[i].neighborPosition.Add(neighbor.roomPosition);
                                    rooms[i].neighborRoomsIdx.Add(i);
                                }
                            }
                        }
                        else if (dir.Key == "Up")
                        {
                            var pos = rooms[i].roomPosition + new Vector2Int(0, defaultRoomSize);

                            if (neighbor.roomPosition == pos)
                            {
                                neighbor.dicDirection["Down"] = true;
                                if (dirNum > rooms[i].neighborPosition.Count)
                                {
                                    rooms[i].neighborPosition.Add(neighbor.roomPosition);
                                    rooms[i].neighborRoomsIdx.Add(i);
                                }
                            }
                        }
                        else if (dir.Key == "Down")
                        {
                            var pos = rooms[i].roomPosition - new Vector2Int(0, defaultRoomSize);

                            if (neighbor.roomPosition == pos)
                            {
                                neighbor.dicDirection["Up"] = true;
                                if (dirNum > rooms[i].neighborPosition.Count)
                                {
                                    rooms[i].neighborPosition.Add(neighbor.roomPosition);
                                    rooms[i].neighborRoomsIdx.Add(i);
                                }
                            }
                        }
                    }
                }
            } 

            if(dirNum <= 0)
            {
                closeList.Add(rooms[i].roomPosition);
            }
            else
            {
                openList.Add(rooms[i].roomPosition);
            }
        }

    }

    public void GetLargestArea()
    {
        openList.Clear();
        closeList.Clear();

        List<int> newList = new List<int>();

        var startRoom = rooms[0];
        var currentRoom = startRoom;

        foreach (var newInt in newList)
        {
            for (int i = 0; i < currentRoom.neighborRoomsIdx.Count; i++)
            {
                if (newInt != currentRoom.neighborRoomsIdx[i])
                    newList.Add(currentRoom.neighborRoomsIdx[i]);
            }
            

        }

    }

    public void SpawnGrid()
    {
        RoomsInitialize();
        rooms.Clear();

        for (int x = 0; x < dungeonSize.x; x++)
        {
            for (int y = 0; y < dungeonSize.y; y++)
            {
                var room = new ApartmentRoom();
                room.roomPosition = startPos + new Vector2Int(x * defaultRoomSize, y * defaultRoomSize);
                rooms.Add(room);
            }
        }
    }

    public void MatchRoomToGrid()
    {
        RoomsInitialize();

        foreach (var room in rooms)
        {
            room.roomPrefab = Instantiate(roomPrefab, (Vector2)room.roomPosition, Quaternion.identity);
        }
    }

    void RoomsInitialize()
    {
        foreach(var room in rooms)
        {
            DestroyImmediate(room.roomPrefab);
        }
    }

    private void OnDrawGizmos()
    {
        for (int i = 0; i < rooms.Count; i++)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube((Vector2)rooms[i].roomPosition, new Vector2(defaultRoomSize, defaultRoomSize));

            foreach (var open in openList)
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawCube((Vector2)open, new Vector2(defaultRoomSize / 10, defaultRoomSize / 10));
            }
            foreach (var close in closeList)
            {
                Gizmos.color = Color.gray;
                Gizmos.DrawCube((Vector2)close, new Vector2(defaultRoomSize / 10, defaultRoomSize / 10));
            }

            foreach (var dir in rooms[i].dicDirection)
            {
                if(dir.Value)
                {
                    Gizmos.color = Color.green;
                    if (dir.Key == "Right")
                        Gizmos.DrawLine((Vector2)rooms[i].roomPosition, rooms[i].roomPosition + new Vector2(defaultRoomSize, 0));
                    else if (dir.Key == "Left")
                        Gizmos.DrawLine((Vector2)rooms[i].roomPosition, rooms[i].roomPosition - new Vector2(defaultRoomSize, 0));
                    else if (dir.Key == "Up")
                        Gizmos.DrawLine((Vector2)rooms[i].roomPosition, rooms[i].roomPosition + new Vector2(0, defaultRoomSize));
                    else if (dir.Key == "Down")
                        Gizmos.DrawLine((Vector2)rooms[i].roomPosition, rooms[i].roomPosition - new Vector2(0, defaultRoomSize));
                }
            }
        }
    }
}
