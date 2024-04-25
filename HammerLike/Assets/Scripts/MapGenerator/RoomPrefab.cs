using UnityEngine;
using System;
public class RoomPrefab : MonoBehaviour
{
    public GameObject EntranceN;
    public GameObject EntranceE;
    public GameObject EntranceW;
    public GameObject EntranceS;
    public int monsterCount = 0;

    // This could be an event that triggers state updates within the room
    public event Action onMonsterCountChange;
    public BoxCollider Ground; // Collider that defines the room's boundaries
    public GameObject monsterParent; // Direct reference to the Monster parent object

    // This method checks if a given position is within the room's ground collider
    public bool IsPositionInside(Vector3 position)
    {
        Vector3 localPos = Ground.transform.InverseTransformPoint(position); // 방의 BoxCollider를 기준으로 로컬 위치 계산
        localPos.y = 0; // Y 축 무시

        // 로컬 경계 확인을 위해 BoxCollider의 center와 size를 사용
        Bounds localBounds = new Bounds(Ground.center, Ground.size);
        return localBounds.Contains(localPos);
    }


    // Count monsters within this room
    public int CountMonsters()
    {
        int monsterCount = 0;
        if (monsterParent != null)
        {
            foreach (Transform monster in monsterParent.transform)
            {
                if (IsPositionInside(monster.position))
                {
                    monsterCount++;
                }
            }
        }
        return monsterCount;
    }

    private void Awake()
    {
        onMonsterCountChange += UpdateRoomState;
    }

    private void OnDestroy()
    {
        onMonsterCountChange -= UpdateRoomState;
    }

    public void UpdateMonsterCount(int changeAmount)
    {
        monsterCount += changeAmount;
        onMonsterCountChange?.Invoke();
    }

    private void UpdateRoomState()
    {
        if (monsterCount <= 0)
        {
            // Update FollowOption or similar settings here
            Debug.Log($"FollowOption changed in {gameObject.name} due to no remaining monsters.");
        }
    }
}
