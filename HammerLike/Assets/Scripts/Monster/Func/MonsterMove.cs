using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterMove : MonoBehaviour
{
    Monster monster;
    Vector3 destination;
    private MonsterAim monsterAim;
    private void Awake()
    {
        monster = GetComponent<Monster>();
        monsterAim = GetComponent<MonsterAim>();
    }

    public void Move(float moveSpeed)
    {
        if (monsterAim.CurrentTarget)  // MonsterAim 클래스에서 현재 타겟을 가져오도록 하는 기능 추가 필요
        {
            Vector3 direction = (monsterAim.CurrentTarget.position - transform.position).normalized;
            monster.rd.velocity = direction * moveSpeed;
        }
        else
        {
            monster.rd.velocity = Vector3.zero;
        }
    }

    // AI sets the destination
    public void SetDestination(Vector3 newDestination)
    {
        destination = newDestination;
    }
}
