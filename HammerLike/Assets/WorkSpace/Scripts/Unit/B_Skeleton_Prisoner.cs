using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class B_Skeleton_Prisoner : B_Enemy
{
    [SerializeField] GameObject weaponColliderObj;
    /// <summary>
    /// Dead : 유닛 사망 함수. 스켈레톤은 퍼펫 분리 추가
    /// </summary>
    protected override void Dead()
    {
        base.Dead();

        weaponColliderObj.SetActive(false);

        // 스켈레톤 유닛은 Dead 시 puppet 분리
        DisconnectMusclesRecursive(GameManager.Instance.Player.transform.position);
    }

    public override void StartAttack()
    {
        base.StartAttack();

        // 각 축에 대한 가중치 반영
        //weaponColliderObj.transform.localScale += GameManager.Instance.ApplyCoordScaleAfterNormalize(transform.forward);

        weaponColliderObj.SetActive(true);
    }

    public override void EndAttack()
    {
        base.EndAttack();
        weaponColliderObj.SetActive(false);
        //weaponColliderObj.transform.localScale = Vector3.one;
    }
}
