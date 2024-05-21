using RootMotion.Dynamics;

using UnityEngine;

public class B_Skeleton_Archer : B_Enemy
{
    [Header("Skeleton_Archer")]
    [SerializeField] private BehaviourPuppet puppet;
    [SerializeField] private Transform muzzleTR;
    [SerializeField] private GameObject projectilePrefab;

    /// <summary>
    /// Dead : 유닛 사망 함수. 스켈레톤은 퍼펫 분리 추가
    /// </summary>
    protected override void Dead(bool isSelf = false)
    {
        base.Dead(isSelf);

        // 스켈레톤 유닛은 Dead 시 puppet 분리
        DisconnectMusclesRecursive(GameManager.Instance.Player.transform.position);
    }

    protected void Shot()
    {
        // Shot logic
        GameObject projectile = Instantiate(projectilePrefab, muzzleTR.position, transform.localRotation);
        projectile.GetComponent<B_Projectile>().projectileDamage = UnitStatus.atkDamage;
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

    
    #region PuppetMaster
    /// <summary>
    /// DisconnectMusclesRecursive : PuppetMaster의 Muscle을 해제 및 넉백 적용
    /// </summary>
    /// <param name="inPos">넉백 기준점</param>
    public void DisconnectMusclesRecursive(Vector3 inPos, bool isSelf = false)
    {
        if (puppet != null && puppet.puppetMaster != null)
        {
            for (int i = 0; i < puppet.puppetMaster.muscles.Length; i++)
            {
                Rigidbody muscleRigid = puppet.puppetMaster.muscles[i].rigidbody;

                float partsKnockBackTime = 0.2f;

                // Temp 240402 - 파츠 별 넉백.., a.HG
                // 1. StartCoroutine(SmoothKnockback)
                // 2. ImpulseKnockbackToPuppet
                // 3. AddForce Each (Loop)

                if (muscleRigid != null && !isSelf)
                {
                    Vector3 dir = (muscleRigid.transform.position - inPos).normalized;
                    dir = GameManager.Instance.ApplyCoordScaleAfterNormalize(dir);

                    remainKnockBackForce = Mathf.Clamp(remainKnockBackForce * partsKnockBackMultiplier, 0f, maxPartsBreakForce);

                    StartCoroutine(CoSmoothKnockback(dir, remainKnockBackForce, muscleRigid, partsBreakForceCurve, partsKnockBackTime, ForceMode.Impulse));
                }

                puppet.puppetMaster.DisconnectMuscleRecursive(i, MuscleDisconnectMode.Sever);

                // root RigidBody 물리력 고정 및 콜라이더 비활성화
                Rigid.isKinematic = true;
                col.enabled = false;
            }
        }
    }
    #endregion
}
