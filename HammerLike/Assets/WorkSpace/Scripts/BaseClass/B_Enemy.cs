using RootMotion.Dynamics;
using System.Collections;
using System.Collections.Generic;
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

    protected override void StartAttack()
    {

    }

    protected override void EndAttack()
    {
        aIStateManager.SetState(AIStateType.IDLE);
    }

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
            var targetDis = Vector3.Distance(transform.position, GameManager.instance.Player.transform.position);

            if (targetDis <= chasingStartDis)
            {
                if (targetDis <= unitStatus.atkRange)
                {
                    aIStateManager?.SetState(AIStateType.ATTACK);
                }
                else if(aIStateManager?.CurrentStateType != AIStateType.ATTACK)
                {
                    aIStateManager?.SetState(AIStateType.CHASE);
                }
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
            B_Player player = other.GetComponentInParent<B_Player>();

            // Get hit dir from player
            Vector3 hitDir = (transform.position - player.transform.position).normalized;
            Vector3 coordDir = GameManager.instance.ApplyCoordScaleNormalize(hitDir);

            // Take Damage and Knockback dir from player
            TakeDamage(coordDir, player.UnitStatus.atkDamage);

            // Temp 240406 - VFX & SFX
            var vfx = B_VFXPoolManager.Instance.GetVFX();
            vfx.transform.position = other.ClosestPointOnBounds(transform.position);
            vfx.PlayVFX();

            B_AudioManager.Instance.PlaySound(AudioCategory.SFX, AudioTag.Battle);
            //

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
            Vector3 coordDir = GameManager.instance.ApplyCoordScaleNormalize(hitDir);

            TakeDamage(coordDir, (int)(other.rigid.velocity.magnitude * other.rigid.mass / 4f), true);

            // Temp 240406 - VFX & SFX
            var vfx = B_VFXPoolManager.Instance.GetVFX();
            vfx.transform.position = collision.contacts[0].point;
            vfx.PlayVFX();

            B_AudioManager.Instance.PlaySound(AudioCategory.SFX, AudioTag.Battle);
            //

            if (aIStateManager?.CurrentStateType != AIStateType.HIT && aIStateManager?.CurrentStateType != AIStateType.DEAD)
                aIStateManager?.SetState(AIStateType.HIT);
        }


    }


}
