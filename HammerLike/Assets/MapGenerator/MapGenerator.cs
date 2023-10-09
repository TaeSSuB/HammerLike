using System.Collections.Generic;
using UnityEngine;

public class Room
{
    public Vector3 position;
    public enum RoomType { Start, End, MiddleBoss, Reward, Normal, Hidden }
    public RoomType roomType;
    public List<Room> connectedRooms = new List<Room>();
}

public class MapGenerator : MonoBehaviour
{
    public GameObject startRoomPrefab, endRoomPrefab, middleBossRoomPrefab, rewardRoomPrefab, normalRoomPrefab, hiddenRoomPrefab;
    public int numberOfNormalRooms = 5;
    public float hiddenRoomProbability = 0.2f;
    public Vector3 dungeonMaxSize = new Vector3(50, 0, 50);

    private LineRenderer lineRendererPrefab;
    private List<Room> rooms = new List<Room>();
    private List<LineRenderer> corridors = new List<LineRenderer>();

    private void Awake()
    {
        lineRendererPrefab = new GameObject("Corridor").AddComponent<LineRenderer>();
        lineRendererPrefab.gameObject.SetActive(false);
    }

    public void GenerateMap()
    {
        rooms.Clear();

        // Start Room 생성
        CreateRoom(startRoomPrefab, Room.RoomType.Start, Vector3.zero);

        // Normal Rooms 생성
        for (int i = 0; i < numberOfNormalRooms; i++)
        {
            Vector3 newPosition = GetRandomNonOverlappingPosition(normalRoomPrefab);
            CreateRoom(normalRoomPrefab, Room.RoomType.Normal, newPosition);
        }

        // MiddleBossRoom 생성
        Vector3 middleBossPosition = GetRandomNonOverlappingPosition(middleBossRoomPrefab, true);
        CreateRoom(middleBossRoomPrefab, Room.RoomType.MiddleBoss, middleBossPosition);

        // RewardRoom 생성
        Vector3 rewardPosition = GetRandomNonOverlappingPosition(rewardRoomPrefab, true);
        CreateRoom(rewardRoomPrefab, Room.RoomType.Reward, rewardPosition);

        // End Room 생성
        Vector3 endRoomPosition = new Vector3(dungeonMaxSize.x - endRoomPrefab.GetComponent<Renderer>().bounds.size.x,
                                              0,
                                              dungeonMaxSize.z - endRoomPrefab.GetComponent<Renderer>().bounds.size.z);
        CreateRoom(endRoomPrefab, Room.RoomType.End, endRoomPosition);

        CreateCorridors();
    }

    private void CreateRoom(GameObject roomPrefab, Room.RoomType roomType, Vector3 position)
    {
        Room room = new Room
        {
            position = position,
            roomType = roomType
        };

        Instantiate(roomPrefab, room.position, Quaternion.identity);
        rooms.Add(room);
    }

    private Vector3 GetRandomNonOverlappingPosition(GameObject roomPrefab, bool isSpecialRoom = false)
    {
        Vector3 position;
        bool isOverlapping;
        int attemptCount = 0;
        int maxAttempts = 1000;

        do
        {
            if (isSpecialRoom) // middleBossRoom과 rewardRoom에 대한 로직
            {
                position = new Vector3(Random.Range(dungeonMaxSize.x * 0.3f, dungeonMaxSize.x * 0.7f),
                                       0,
                                       Random.Range(dungeonMaxSize.z * 0.3f, dungeonMaxSize.z * 0.7f));
            }
            else
            {
                position = new Vector3(Random.Range(0, dungeonMaxSize.x), 0, Random.Range(0, dungeonMaxSize.z));
            }
            isOverlapping = false;

            foreach (var room in rooms)
            {
                float roomWidth = roomPrefab.GetComponent<Renderer>().bounds.size.x;
                float roomHeight = roomPrefab.GetComponent<Renderer>().bounds.size.z;

                float existingRoomWidth = roomPrefab.GetComponent<Renderer>().bounds.size.x;
                float existingRoomHeight = roomPrefab.GetComponent<Renderer>().bounds.size.z;

                bool overlapX = position.x < room.position.x + existingRoomWidth && position.x + roomWidth > room.position.x;
                bool overlapZ = position.z < room.position.z + existingRoomHeight && position.z + roomHeight > room.position.z;

                if (overlapX && overlapZ)
                {
                    isOverlapping = true;
                    break;
                }
            }

            attemptCount++;

        } while (isOverlapping && attemptCount < maxAttempts);

        if (attemptCount == maxAttempts)
        {
            Debug.LogError("Failed to find non-overlapping position. Consider increasing dungeon size or decreasing number of rooms.");
        }

        return position;
    }

    public void CreateCorridors()
    {
        Room currentRoom = rooms.Find(room => room.roomType == Room.RoomType.Start);
        Room endRoom = rooms.Find(room => room.roomType == Room.RoomType.End);

        List<Room> unvisitedRooms = new List<Room>(rooms);
        unvisitedRooms.Remove(currentRoom);

        while (currentRoom != endRoom)
        {
            Room closestRoom = GetClosestRoom(currentRoom, unvisitedRooms);
            if (closestRoom != null)
            {
                CreateCorridor(currentRoom, closestRoom);
                currentRoom.connectedRooms.Add(closestRoom);
                closestRoom.connectedRooms.Add(currentRoom);

                currentRoom = closestRoom;
                unvisitedRooms.Remove(closestRoom);
            }
            else
            {
                break;
            }
        }
    }

    private Room GetClosestRoom(Room currentRoom, List<Room> otherRooms)
    {
        Room closestRoom = null;
        float minDistance = float.MaxValue;

        foreach (var room in otherRooms)
        {
            float distance = Vector3.Distance(currentRoom.position, room.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                closestRoom = room;
            }
        }

        return closestRoom;
    }
    public void CreateCorridor(Room roomA, Room roomB)
    {
        LineRenderer corridor = Instantiate(lineRendererPrefab, transform);
        corridor.gameObject.SetActive(true);

        corridor.SetPosition(0, roomA.position);
        corridor.SetPosition(1, roomB.position);
        corridors.Add(corridor);
    }
    // TODO: 기타 메서드들 (통로 생성, Hidden Room 생성 등)을 추가하세요.
}
