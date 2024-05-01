using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class B_Skeleton_Archer : B_Enemy
{
    [SerializeField] private Transform muzzleTR;
    [SerializeField] private GameObject projectilePrefab;
    
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
}
