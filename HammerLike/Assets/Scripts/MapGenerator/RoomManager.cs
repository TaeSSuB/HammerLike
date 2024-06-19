using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class RoomManager : MonoBehaviour
{
    public Transform playerTransform;
    public GameObject monsterParent;
    public CamCtrl camCtrl;

    private List<RoomPrefab> rooms = new List<RoomPrefab>();
    private RoomPrefab currentRoom = null;
    public GameObject entranceA;
    public GameObject entranceB;
    public float activationDistance = 10.0f; // 몬스터 그룹 활성화 거리

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
        RoomPrefab[] roomPrefabs = FindObjectsOfType<RoomPrefab>();
        rooms.AddRange(roomPrefabs);

        if (playerTransform == null)
        {
            Debug.LogError("Player Transform is not assigned in the RoomManager.");
        }
        if (monsterParent == null)
        {
            Debug.LogError("Monster Parent is not assigned in the RoomManager.");
        }
    }

    void Update()
    {
        if (playerTransform != null)
        {
            CheckCurrentRoom();
            ActivateNearbyMonsterGroups();
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
                foundRoom = room;
                break;
            }
        }

        if (foundRoom != currentRoom)
        {
            currentRoom = foundRoom;
            if (foundRoom != null && foundRoom.monsterParent != null)
                monsterParent = foundRoom.monsterParent;
            if (currentRoom != null)
            {
                Debug.Log($"Player has entered a new room: {currentRoom.gameObject.name}");
                camCtrl.UpdateCameraBounds(currentRoom.Ground.bounds);
                int monsterCount = CountMonstersInRoom(currentRoom);
                if (monsterCount > 0)
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
        int monsterCount = room.monsterCount; // CountMonstersInRoom(room);
        if (room.gameObject.name == "Room_Boss")
        {
            EnablePanel();
            if (!boss.gameObject.activeSelf)
            {
                boss.gameObject.SetActive(true);
            }
        }

        if (monsterCount > 0)
        {
            camCtrl.followOption = FollowOption.LimitedInRoom;
        }
        else
        {
            camCtrl.followOption = FollowOption.FollowToObject;
            if (room.Doors.Count > 0 && !room.doorsOpened)
            {
                room.OpenDoors();
            }
            if (entranceA != null && entranceB != null && entranceA.activeSelf && entranceB.activeSelf)
            {
                entranceA.SetActive(false);
                entranceB.SetActive(false);
            }
        }
    }

    private void ActivateNearbyMonsterGroups()
    {
        foreach (RoomPrefab room in rooms)
        {
            if (room.monsterParent != null && !room.monsterParent.activeSelf)
            {
                float distanceToPlayer = Vector3.Distance(room.monsterParent.transform.position, playerTransform.position);
                if (distanceToPlayer <= activationDistance)
                {
                    room.monsterParent.SetActive(true);
                }
            }
        }
    }

    private int CountMonstersInRoom(RoomPrefab room)
    {
        int monsterCount = 0;

        if (room.monsterParent != null)
        {
            foreach (Transform monsterParentTransform in room.monsterParent.transform)
            {
                if (monsterParentTransform.gameObject.activeInHierarchy)
                {
                    foreach (Transform child in monsterParentTransform)
                    {
                        AIStateManager monsterComponent = child.GetComponent<AIStateManager>();
                        if (monsterComponent != null && monsterComponent.CurrentStateType != AIStateType.DEAD && room.IsPositionInside(child.position))
                        {
                            monsterCount++;
                        }
                        B_BossController bBoss = child.GetComponent<B_BossController>();
                        if (bBoss != null && bBoss.CurrentStateType != BossAIStateType.DEAD && room.IsPositionInside(child.position))
                        {
                            monsterCount++;
                        }
                    }
                }
            }
        }
        room.monsterCount = monsterCount;
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
        if (room != null)
        {
            room.UpdateMonsterCount(-1);  // 몬스터 수 감소
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
