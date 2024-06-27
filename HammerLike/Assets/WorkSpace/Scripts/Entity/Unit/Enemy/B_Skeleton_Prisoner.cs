using RootMotion.Dynamics;
using UnityEngine;

[RequireComponent(typeof(B_PartsKnockBack))]
public class B_Skeleton_Prisoner : B_Enemy
{
    [Header("Skeleton_Prisoner")]
    [SerializeField] private BehaviourPuppet puppet;
    private B_PartsKnockBack partsKnockBack;
    [SerializeField] GameObject weaponColliderObj;

    public GameObject WeaponColliderObj => weaponColliderObj;

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

        B_AudioManager.Instance.PlaySound("Skeleton_Dead", AudioCategory.SFX);

        if(weaponColliderObj != null)
            weaponColliderObj.SetActive(false);

        // 스켈레톤 유닛은 Dead 시 puppet 분리
        // Temp. 임시로 플레이어 위치 기반 분리
        partsKnockBack.DisconnectMusclesRecursive(this, puppet, GameManager.Instance.Player.transform.position);
    }

    public override void OnStartAttack()
    {
        base.OnStartAttack();

        // 각 축에 대한 가중치 반영
        //weaponColliderObj.transform.localScale += GameManager.Instance.ApplyCoordScaleAfterNormalize(transform.forward);

        weaponColliderObj.SetActive(true);

        B_AudioManager.Instance.PlaySound("Attack_Knife", AudioCategory.SFX);
    }

    public override void OnEndAttack()
    {
        base.OnEndAttack();
        weaponColliderObj.SetActive(false);
        //weaponColliderObj.transform.localScale = Vector3.one;
    }
}
