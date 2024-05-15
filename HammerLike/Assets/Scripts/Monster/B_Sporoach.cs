using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class B_Sporoach : B_Enemy
{
    [SerializeField] private Transform muzzleTR;
    [SerializeField] private GameObject projectilePrefab;

    // Start is called before the first frame update
    

    protected void Shot()
    {
        GameObject projectile = Instantiate(projectilePrefab, muzzleTR.position, Quaternion.identity);

    }

    protected override void Dead()
    {

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
