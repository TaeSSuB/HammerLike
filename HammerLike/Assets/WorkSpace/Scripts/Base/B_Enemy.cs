using Language.Lua;
using RootMotion.Dynamics;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

// require ai state manager
[RequireComponent(typeof(AIStateManager))]
public class B_Enemy : B_UnitBase
{
    private AIStateManager aIStateManager;

    // Temp - HP UI
    [Header("HP UI")]
    [SerializeField] Transform hpPosTR;

    // Temp - Chasing
    [SerializeField] private float chasingStartDis = 20f;
    // Temp - Dev
    [SerializeField] private bool isTester = false;

    // get aIStateManager
    public AIStateManager AIStateManager => aIStateManager;

    // override Init() method
    public override void Init()
    {
        base.Init();

        if(!isTester)
            aIStateManager = GetComponent<AIStateManager>();

        if(hpPosTR == null)
            hpPosTR = transform;

        FindObjectOfType<B_UIManager>().CreateHPWorldUI(hpPosTR, this);
    }

    // override Dead() method
    protected override void Dead()
    {
        base.Dead();
        //aIStateManager.SetState(AIStateType.DEAD);

        DisconnectMusclesRecursive();
        //Invoke(nameof(DisconnectMusclesRecursive), 0.1f);
    }
    
    #region Attack

    public override void Attack()
    {
        base.Attack();

        transform.LookAt(GameManager.Instance.Player.transform);
    }

    public override void StartAttack()
    {
        base.StartAttack();
    }

    public override void EndAttack()
    {
        base.EndAttack();
    }

    #endregion

    // Update
    protected override void Update()
    {
        base.Update();

        // if Dead return
        if (UnitStatus.currentHP <= 0 || isTester)
        {
            //DisableMovementAndRotation();
            return;
        }

        // Temp - Set Chasing
        if (aIStateManager?.CurrentStateType != AIStateType.HIT && aIStateManager?.CurrentStateType != AIStateType.DEAD)
        {
            var moveDir = GameManager.Instance.Player.transform.position - transform.position;

            float applyCoordScale = GameManager.Instance.CalcCoordScale(moveDir);
            var targetDis = moveDir.magnitude / applyCoordScale;

            if (targetDis <= chasingStartDis)
            {
                if (targetDis <= unitStatus.atkRange)
                {
                    aIStateManager?.SetState(AIStateType.ATTACK);
                    //Attack();
                }
                else
                {
                    if(!isAttacking)
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

            // Get hit dir from player
            Vector3 hitDir = (transform.position - player.transform.position).normalized;
            Vector3 coordDir = GameManager.Instance.ApplyCoordScaleNormalize(hitDir);

            // Take Damage and Knockback dir from player
            TakeDamage(coordDir, player.UnitStatus.atkDamage);

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

            if (isTester)
                return;

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
            if (isKnockback) return;

            var other = collision.gameObject.GetComponent<B_Enemy>();
            
            if (other == null) return;
            if (!other.isKnockback) return;

            // Get hit dir from another enemy
            Vector3 hitDir = (transform.position - collision.transform.position).normalized;
            Vector3 coordDir = GameManager.Instance.ApplyCoordScaleNormalize(hitDir);

            TakeDamage(coordDir, (int)(other.rigid.velocity.magnitude * other.rigid.mass / 4f), true);

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


}
