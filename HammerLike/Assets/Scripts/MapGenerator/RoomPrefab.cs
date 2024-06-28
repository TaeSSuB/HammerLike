using UnityEngine;
using System;
using System.Collections.Generic;
using DG.Tweening;

public class RoomPrefab : MonoBehaviour
{
    public GameObject EntranceN;
    public GameObject EntranceE;
    public GameObject EntranceW;
    public GameObject EntranceS;
    public int monsterCount = -1;
    public event Action onMonsterCountChange;
    public BoxCollider Ground;
    public GameObject monsterParent;
    public GameObject nextMonsterGroup;

    public List<GameObject> Doors = new List<GameObject>(); // 다수의 문을 관리
    public bool doorsOpened = false;
    public bool doorsClosed = false;
    public GameObject meshBakingTileGroup;
    public GameObject meshBakingWallGroup;

    private void Awake()
    {
        onMonsterCountChange += UpdateRoomState;
    }

    private void OnDestroy()
    {
        onMonsterCountChange -= UpdateRoomState;
    }

    private void Start()
    {
        // RoomManager에서 몬스터 활성화 시 카운트가 갱신되므로 여기서 CountMonsters를 호출하지 않음
    }

    private void UpdateRoomState()
    {
        if (monsterCount <= 0 && !doorsOpened)
        {
            OpenDoors();
        }
    }

    public int CountMonsters()
    {
        int count = 0;
        if (monsterParent != null)
        {
            foreach (Transform monster in monsterParent.transform)
            {
                if (IsPositionInside(monster.position))
                {
                    count++;
                }
            }
        }
        return count;
    }

    public void UpdateMonsterCount(int changeAmount)
    {
        monsterCount += changeAmount;
        onMonsterCountChange?.Invoke();
    }

    public void OpenDoors()
    {
        if (!doorsOpened && Doors.Count > 0)
        {
            doorsOpened = true;
            float moveDistance = 7.0f;
            float duration = 3.0f;
            foreach (GameObject door in Doors)
            {
                door.transform.DOMoveY(door.transform.position.y - moveDistance, duration).SetEase(Ease.InOutQuad);
            }
        }
    }

    public void CloseDoors()
    {
        if (!doorsClosed && Doors.Count > 0)
        {
            doorsClosed = true;
            float moveDistance = 7.0f;
            float duration = 3.0f;
            foreach (GameObject door in Doors)
            {
                door.transform.DOMoveY(door.transform.position.y + moveDistance, duration).SetEase(Ease.InOutQuad);
            }
            SetMonstersIdleForced(false); // 문이 닫힌 후 IDLE 강제 해제
        }
    }

    public bool IsPositionInside(Vector3 position)
    {
        Vector3 localPos = Ground.transform.InverseTransformPoint(position);
        localPos.y = 0;
        Bounds localBounds = new Bounds(Ground.center, Ground.size);
        return localBounds.Contains(localPos);
    }

    public void SetMonstersIdleForced(bool forceIdle)
    {
        if (monsterParent != null)
        {
            foreach (Transform monster in monsterParent.transform)
            {
                  SetIdleForcedRecursive(monster, forceIdle);
            }
        }
    }

    private void SetIdleForcedRecursive(Transform obj, bool forceIdle)
    {
        // 근데 이렇게 될 경우 부모 오브젝트에서 AIStateManager를 찾을 때 까지 재귀적으로 돌아야하는데
        // 쓸데 없이 코스트가 큰거 같다 root 오브젝트를 두지 않고 
        AIStateManager aiStateManager = obj.GetComponent<AIStateManager>();
        if (aiStateManager != null)
        {
            aiStateManager.ForceIdle(forceIdle);
        }
        foreach (Transform child in obj)
        {
            SetIdleForcedRecursive(child, forceIdle);
        }
    }


}
