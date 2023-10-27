using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterFSM : StateCtrl
{
    Monster monster;

    public override void InitState()
    {
        curState = AddState(new Monster_Idle(monster));
        // 다른 몬스터 상태들도 추가 가능
        // 예: AddState(new Monster_Attack(monster));
        // AddState(new Monster_Death(monster));
    }

    public override void Release()
    {
        // 필요한 경우, 사용한 리소스나 할당된 메모리 해제 로직 추가
    }

    protected override void Awake()
    {
        base.Awake();
        monster = GetComponent<Monster>();
    }

    // 나머지 메서드들도 필요에 따라 오버라이드하여 사용
}