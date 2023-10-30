using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster_Idle : cState
{
    public Monster monster;

    public Monster_Idle(Monster _monster)
    {
        monster = _monster;
    }

    public override void EnterState()
    {
        base.EnterState();
        // 여기에 몬스터가 대기 상태로 들어갔을 때의 로직을 추가
    }

    public override void UpdateState()
    {
        base.UpdateState();
        // 몬스터의 대기 상태에서 수행될 로직을 여기에 추가
    }

    // 나머지 메서드들도 필요하다면 오버라이드하여 사용
}