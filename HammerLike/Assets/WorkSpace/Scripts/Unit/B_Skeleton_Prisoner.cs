using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class B_Skeleton_Prisoner : B_Enemy
{
    /// <summary>
    /// Dead : 유닛 사망 함수. 스켈레톤은 퍼펫 분리 추가
    /// </summary>
    protected override void Dead()
    {
        base.Dead();

        // 스켈레톤 유닛은 Dead 시 puppet 분리
        DisconnectMusclesRecursive(GameManager.Instance.Player.transform.position);
    }
}
