using UnityEngine;
using RootMotion.Dynamics;

[RequireComponent(typeof(B_PartsKnockBack))]
public class B_Skeleton_Archer : B_Enemy
{
    [Header("Skeleton_Archer")]
    private B_PartsKnockBack partsKnockBack;
    [SerializeField] private BehaviourPuppet puppet;
    [SerializeField] private Transform muzzleTR;
    [SerializeField] private GameObject projectilePrefab;

    public override void Init()
    {
        base.Init();

        partsKnockBack = GetComponent<B_PartsKnockBack>();
    }
    /// <summary>
    /// Dead : 유닛 사망 함수. 스켈레톤은 퍼펫 분리 추가
    /// </summary>
    protected override void Dead(bool isSelf = false)
    {
        base.Dead(isSelf);

        // 스켈레톤 유닛은 Dead 시 puppet 분리
        // Temp. 임시로 플레이어 위치 기반 분리
        partsKnockBack.DisconnectMusclesRecursive(this, puppet, GameManager.Instance.Player.transform.position);
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

}
