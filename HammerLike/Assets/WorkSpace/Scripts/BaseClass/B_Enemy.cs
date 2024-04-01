using RootMotion.Dynamics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// require ai state manager
[RequireComponent(typeof(AIStateManager))]
public class B_Enemy : B_UnitBase
{
    private AIStateManager aIStateManager;

    // Temp - Chasing
    [SerializeField] private float chasingStartDis = 20f;

    // get aIStateManager
    public AIStateManager AIStateManager => aIStateManager;

    // override Init() method
    public override void Init()
    {
        base.Init();
        aIStateManager = GetComponent<AIStateManager>();
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
        if (UnitStatus.currentHP <= 0)
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
                    aIStateManager.SetState(AIStateType.ATTACK);
                }
                else if(aIStateManager?.CurrentStateType != AIStateType.ATTACK)
                {
                    aIStateManager.SetState(AIStateType.CHASE);
                }
            }
        }
    }

    protected override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);

        if(other.CompareTag("WeaponCollider") && UnitStatus.currentHP > 0)
        {
            // Get player
            B_Player player = other.GetComponentInParent<B_Player>();

            // Get hit dir from player
            Vector3 hitDir = (transform.position - player.transform.position).normalized;
            Vector3 coordDir = GameManager.instance.ApplyCoordScale(hitDir);

            // Take Damage and Knockback dir from player
            TakeDamage(coordDir, player.UnitStatus.atkDamage);
            
            //ChangeState(new ChaseState(this));
            if (aIStateManager?.CurrentStateType != AIStateType.HIT && aIStateManager?.CurrentStateType != AIStateType.DEAD)
                aIStateManager?.SetState(AIStateType.HIT);
        }
    }
}
