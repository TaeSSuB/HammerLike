using UnityEngine;
using System;
using DG.Tweening;
public class RoomPrefab : MonoBehaviour
{
    public GameObject EntranceN;
    public GameObject EntranceE;
    public GameObject EntranceW;
    public GameObject EntranceS;
    public int monsterCount = -1;

    // This could be an event that triggers state updates within the room
    public event Action onMonsterCountChange;
    public BoxCollider Ground; // Collider that defines the room's boundaries
    public GameObject monsterParent; // Direct reference to the Monster parent object
    public GameObject Door;
    public bool doorOpened = false;
    private bool isCheck = false;
    

    // This method checks if a given position is within the room's ground collider
    public bool IsPositionInside(Vector3 position)
    {
        Vector3 localPos = Ground.transform.InverseTransformPoint(position); // 
        localPos.y = 0; //

        //
        Bounds localBounds = new Bounds(Ground.center, Ground.size);
        return localBounds.Contains(localPos);
    }

    private void Update()
    {
        if (monsterCount == 0&&isCheck)
            OpenDoor();

       
    }

    public void Start()
    {
        CountMonsters();
        isCheck = true;
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

    public void OpenDoor()
    {
        if (!doorOpened&&Door!=null&&monsterCount<=0) // 문이 아직 열리지 않았다면
        {
            doorOpened = true; // 문이 열렸다고 표시
            float moveDistance = 7.0f; // 이동 거리
            float duration = 3.0f; // 지속 시간

            // DoTween을 사용하여 문을 엽니다.
            Door.transform.DOMoveY(Door.transform.position.y - moveDistance, duration).SetEase(Ease.InOutQuad);
        }
    }

    public void CloseDoor()
    {
        float moveDistance = 7.0f; //
        float duration = 3.0f; //
        Debug.Log("close Door");
        // DoTween을 사용하여 문을 엽니다.
        Door.transform.DOMoveY(Door.transform.position.y + moveDistance, duration).SetEase(Ease.InOutQuad);
    }
}
