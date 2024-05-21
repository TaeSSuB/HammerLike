using RootMotion.Dynamics;
using UnityEngine;

public class B_Skeleton_Prisoner : B_Enemy
{
    [Header("Skeleton_Prisoner")]
    [SerializeField] private BehaviourPuppet puppet;
    [SerializeField] GameObject weaponColliderObj;

    public GameObject WeaponColliderObj => weaponColliderObj;
    
    /// <summary>
    /// Dead : 유닛 사망 함수. 스켈레톤은 퍼펫 분리 추가
    /// </summary>
    protected override void Dead(bool isSelf = false)
    {
        base.Dead(isSelf);

        if(weaponColliderObj != null)
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
