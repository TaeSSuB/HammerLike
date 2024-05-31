using System;
using UnityEngine;

/// <summary>
/// B_Enemy : 적 유닛 클래스
/// - 모든 적 유닛은 AI를 가진다고 가정, AIStateManager 필수
/// </summary>
[RequireComponent(typeof(AIStateManager))]
public class B_Enemy : B_UnitBase
{
    private AIStateManager aIStateManager;

    // Temp - HP UI
    [Header("HP UI")]
    [SerializeField] Transform hpPosTR;

    // get aIStateManager
    public AIStateManager AIStateManager => aIStateManager;

    #region Unity Callbacks & Init
    public override void Init()
    {
        base.Init();

        if(hpPosTR == null)
            hpPosTR = transform;

        FindObjectOfType<B_UIManager>().CreateHPWorldUI(hpPosTR, this);

        aIStateManager = GetComponent<AIStateManager>();
    }

    protected override void Update()
    {
        base.Update();

        // Temp - Set Chasing
        if (aIStateManager?.CurrentStateType != AIStateType.HIT && aIStateManager?.CurrentStateType != AIStateType.DEAD)
        {
            var moveDir = GameManager.Instance.Player.transform.position - transform.position;

            float applyCoordScale = GameManager.Instance.CalcCoordScale(moveDir);
            var targetDis = moveDir.magnitude / applyCoordScale;

            if (targetDis <= unitStatus.detectRange)
            {
                if (targetDis <= unitStatus.atkRange)
                {
                    aIStateManager?.SetState(AIStateType.ATTACK);
                    //Attack();
                }
                else
                {
                    if(!IsAttacking)
                        aIStateManager?.SetState(AIStateType.CHASE);
                }
            }
            else
            {
                aIStateManager?.SetState(AIStateType.IDLE);
            }

        }
    }

    protected override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);

        if(other.CompareTag("WeaponCollider") && UnitStatus.currentHP > 0)
        {
            if(isInvincible) return;

            // Get player
            // 현재는 B_Player지만, B_UnitBase를 상속받는 모든 유닛에게 적용 가능. a.HG
            B_Player player = other.GetComponentInParent<B_Player>();

            if(player == null) return;
            
            // Get hit dir from player
            Vector3 hitDir = (transform.position - player.transform.position).normalized;

            //var chargeAmount = (player.UnitStatus as SO_PlayerStatus).chargeRate;
            var chargeAmount = (float)(player.UnitStatus.atkDamage / player.AtkDamageOrigin);
            
            if((player.UnitStatus as SO_PlayerStatus).maxChargeRate <= chargeAmount)
            {
                TakeDamage(hitDir, player.UnitStatus.atkDamage, player.UnitStatus.knockbackPower * chargeAmount, true, true);
            }
            else
            {
                TakeDamage(hitDir, player.UnitStatus.atkDamage, player.UnitStatus.knockbackPower * chargeAmount, true, false);
            }
            
            //GameManager.Instance.SlowMotion(0.1f, 0.1f);

            // Take Damage and Knockback dir from player


            var vfxPos = other.ClosestPointOnBounds(transform.position);
            B_VFXPoolManager.Instance.PlayVFX(VFXName.Hit, vfxPos);

            if (unitStatus.currentHP > 0)
            {
                B_AudioManager.Instance.PlaySound(AudioCategory.SFX, AudioTag.Battle);
            }
            else
            {
                B_AudioManager.Instance.PlaySound(AudioCategory.SFX, AudioTag.Death);
            }

            //ChangeState(new ChaseState(this));
            if (aIStateManager?.CurrentStateType != AIStateType.HIT && aIStateManager?.CurrentStateType != AIStateType.DEAD)
                aIStateManager?.SetState(AIStateType.HIT);
        }
    }

    protected override void OnCollisionEnter(Collision collision)
    {
        base.OnCollisionEnter(collision);

        // When hit Other Enemy
        if (collision.gameObject.CompareTag("Enemy"))
        {
            CheckDead();
            
            if (IsKnockback) return;

            var other = collision.gameObject.GetComponent<B_Enemy>();
            
            if (other == null) return;
            if (!other.IsKnockback) return;

            // Get hit dir from another enemy
            Vector3 hitDir = (transform.position - collision.transform.position).normalized;

            TakeDamage(hitDir, (int)(other.Rigid.mass / 2f), other.Rigid.mass, true);

            var vfxPos = collision.contacts[0].point;
            B_VFXPoolManager.Instance.PlayVFX(VFXName.Hit, vfxPos);

            if (unitStatus.currentHP > 0)
            {
                B_AudioManager.Instance.PlaySound(AudioCategory.SFX, AudioTag.Battle);
            }
            else
            {
                B_AudioManager.Instance.PlaySound(AudioCategory.SFX, AudioTag.Death);
            }

            if (aIStateManager?.CurrentStateType != AIStateType.HIT && aIStateManager?.CurrentStateType != AIStateType.DEAD)
                aIStateManager?.SetState(AIStateType.HIT);
        }
    }
    #endregion
    
    #region Check or Update State
    protected override void Dead(bool isSelf = false)
    {
        base.Dead(isSelf);
        //aIStateManager.SetState(AIStateType.DEAD);

        //DisconnectMusclesRecursive(GameManager.Instance.Player.transform.position);
        //Invoke(nameof(DisconnectMusclesRecursive), 0.1f);
    }
    #endregion
    
    #region Action
    public override void Attack()
    {
        base.Attack();

        //transform.LookAt(GameManager.Instance.Player.transform);
    }    
    #endregion

    #region Animation Event
    public override void StartAttack()
    {
        base.StartAttack();
    }

    public override void EndAttack()
    {
        base.EndAttack();
    }
    #endregion

}
