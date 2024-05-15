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
    private bool isCheck = false;

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
        CountMonsters();
        isCheck = true;
    }

    private void Update()
    {
        if (monsterCount == 0 && isCheck)
        {
            OpenDoors();
            if (!nextMonsterGroup.gameObject.activeSelf)
                nextMonsterGroup.SetActive(true);
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
        monsterCount = count;
        return count;
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
            Debug.Log($"FollowOption changed in {gameObject.name} due to no remaining monsters.");
        }
    }

    public void OpenDoors()
    {
        if (!doorsOpened && Doors.Count > 0 && monsterCount <= 0)
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
        if (Doors.Count > 0)
        {
            float moveDistance = 7.0f;
            float duration = 3.0f;
            foreach (GameObject door in Doors)
            {
                door.transform.DOMoveY(door.transform.position.y + moveDistance, duration).SetEase(Ease.InOutQuad);
            }

        }
    }

    public bool IsPositionInside(Vector3 position)
    {
        Vector3 localPos = Ground.transform.InverseTransformPoint(position);
        localPos.y = 0;
        Bounds localBounds = new Bounds(Ground.center, Ground.size);
        return localBounds.Contains(localPos);
    }
}
