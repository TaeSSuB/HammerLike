using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class B_Skeleton_Archer : B_Enemy
{
    [Header("Skeleton_Archer")]
    [SerializeField] private Transform muzzleTR;
    [SerializeField] private GameObject projectilePrefab;

    /// <summary>
    /// Dead : 유닛 사망 함수. 스켈레톤은 퍼펫 분리 추가
    /// </summary>
    protected override void Dead()
    {
        base.Dead();

        // 스켈레톤 유닛은 Dead 시 puppet 분리
        DisconnectMusclesRecursive(GameManager.Instance.Player.transform.position);
    }

    protected void Shot()
    {
        // Shot logic
        GameObject projectile = Instantiate(projectilePrefab, muzzleTR.position, transform.localRotation);
        //projectile.GetComponent<B_Projectile>().Init(unitStatus.projectileSpeed, transform.forward);
    }

    public override void EndAttack()
    {
        base.EndAttack();
        Shot();
    }

    protected override void UpdateAttackCoolTime()
    {
        base.UpdateAttackCoolTime();
        Anim.SetFloat("fRemainShot", UnitStatus.currentAttackCooltime);
    }
}
