using RoomGen;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class RoomManager : MonoBehaviour
{
    public Transform playerTransform; // Player transform assigned through the Unity Inspector
    public GameObject monsterParent; // Direct reference to the Monster parent object
    public CamCtrl camCtrl;

    private List<RoomPrefab> rooms = new List<RoomPrefab>();
    private RoomPrefab currentRoom = null; // Track the current room the player is in
    public GameObject entranceA;
    public GameObject entranceB;

    [Header("Temp")]
    [SerializeField] public RectTransform bossPanel;
    [SerializeField] private Vector2 bossPanelPos;
    [SerializeField] private float duration;
    private Vector2 disableBossPanelPos;
    private bool isPanelOn = false;
    [SerializeField] private GameObject boss;
    private void Awake()
    {
        disableBossPanelPos = bossPanel.anchoredPosition;
    }

    void Start()
    {
        // Find all RoomPrefab objects in the scene
        RoomPrefab[] roomPrefabs = FindObjectsOfType<RoomPrefab>();
        rooms.AddRange(roomPrefabs);

        if (playerTransform == null)
        {
            Debug.LogError("Player Transform is not assigned in the RoomManager.");
        }
        if (monsterParent == null)
        {
            // Debug.LogError("Monster Parent is not assigned in the RoomManager.");
        }
    }

    void Update()
    {
        if (playerTransform != null)
        {
            CheckCurrentRoom();
            if (currentRoom != null)
            {
                UpdateRoomMonsterStatus(currentRoom);
            }
        }
    }

    private void CheckCurrentRoom()
    {
        RoomPrefab foundRoom = null;
        foreach (RoomPrefab room in rooms)
        {
            if (room.IsPositionInside(playerTransform.position))
            {
                foundRoom = room; // Found the room the player is currently in
                break; // Stop checking once the correct room is found
            }
        }

        if (foundRoom != currentRoom)
        {
            currentRoom = foundRoom; // Update the current room
            if(foundRoom != null&&foundRoom.monsterParent!=null) 
            monsterParent = foundRoom.monsterParent;
            if (currentRoom != null)
            {
                Debug.Log($"Player has entered a new room: {currentRoom.gameObject.name}");
                camCtrl.UpdateCameraBounds(currentRoom.Ground.bounds);
                if (currentRoom.Doors.Count > 0 && currentRoom.Doors[0].transform.position.y < 0)    // 일단 현재는 몬스터 없을때 있을때 구분 안함
                {
                    currentRoom.CloseDoors();
                }
            }
            else
            {
                Debug.Log("Player is not in any room.");
            }
        }
    }

    private void UpdateRoomMonsterStatus(RoomPrefab room)
    {
        int monsterCount = CountMonstersInRoom(room);
        room.monsterCount = monsterCount;
        //Debug.Log($"{room.gameObject.name} currently has {monsterCount} monsters.");
        if (room.gameObject.name == "Room_Boss")
        {
            EnablePanel();
            if (!boss.gameObject.activeSelf)
            {
                boss.gameObject.SetActive(true);
            }
        }

        if (monsterCount > 0)   // TODO) ?꾪닾以?
        {
            camCtrl.followOption = FollowOption.LimitedInRoom;
        }
        else
        {
            camCtrl.followOption = FollowOption.FollowToObject;
            /*if(!room.doorOpened)
            room.OpenDoor();*/
            if (entranceA != null && entranceB != null && entranceA.activeSelf && entranceB.activeSelf)
            {
                entranceA.SetActive(false);
                entranceB.SetActive(false);
            }
        }
    }

    // Count monsters within a specified room
    private int CountMonstersInRoom(RoomPrefab room)
    {
        int monsterCount = 0;

        if (monsterParent != null)
        {
            foreach (Transform monsterParentTransform in monsterParent.transform)
            {
                if (monsterParentTransform.gameObject.activeInHierarchy)
                {
                    foreach (Transform child in monsterParentTransform)
                    {
                        AIStateManager monsterComponent = child.GetComponent<AIStateManager>();
                        if (monsterComponent != null && monsterComponent.CurrentStateType != AIStateType.DEAD && room.IsPositionInside(child.position))
                        {
                            monsterCount++;
                            room.monsterCount = monsterCount;
                        }
                        B_BossController bBoss = child.GetComponent<B_BossController>();
                        if (bBoss != null && bBoss.CurrentStateType != BossAIStateType.DEAD && room.IsPositionInside(child.position))
                        {
                            monsterCount++;
                            room.monsterCount = monsterCount;
                        }
                    }
                }
            }
        }
        return monsterCount;
    }

    private void OnEnable()
    {
        Monster.OnMonsterDeath += HandleMonsterDeath;
    }

    private void OnDisable()
    {
        Monster.OnMonsterDeath -= HandleMonsterDeath;
    }

    private void HandleMonsterDeath(Vector3 monsterPosition)
    {
        RoomPrefab room = FindRoomContainingPosition(monsterPosition);
        if (room != null && room == currentRoom)
        {
            UpdateRoomMonsterStatus(room);
        }
    }

    private RoomPrefab FindRoomContainingPosition(Vector3 position)
    {
        foreach (RoomPrefab room in rooms)
        {
            if (room.IsPositionInside(position))
            {
                return room;
            }
        }
        return null;
    }

    private void EnablePanel()
    {
        isPanelOn = true;
        bossPanel.DOAnchorPos(bossPanelPos, duration);

    }
}
