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
        // Convert the position to local space
        Vector3 localPos = transform.InverseTransformPoint(position);
        // Ignore the y-coordinate by setting it to zero
        localPos.y = 0;
        // Check if this position is inside the collider bounds
        return Ground.bounds.Contains(localPos);
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
