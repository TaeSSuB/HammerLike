using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// require ai state manager
[RequireComponent(typeof(AIStateManager))]
public class B_Enemy : B_UnitBase
{
    private AIStateManager aIStateManager;

    // get aIStateManager
    public AIStateManager AIStateManager => aIStateManager;

    // override Init() method
    public override void Init()
    {
        base.Init();
        aIStateManager = GetComponent<AIStateManager>();
    }

    protected override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);

        if(other.CompareTag("WeaponCollider"))
        {
            // Get player
            B_Player player = other.GetComponentInParent<B_Player>();

            // Get hit dir from player
            Vector3 hitDir = (transform.position - player.transform.position).normalized;
            Vector3 coordDir = GameManager.instance.ApplyCoordScale(hitDir);

            // Take Damage and Knockback dir from player
            TakeDamage(coordDir, 10);
            
            //ChangeState(new ChaseState(this));
            if (aIStateManager?.CurrentStateType != AIStateType.HIT)
                aIStateManager?.SetState(AIStateType.HIT);
        }
    }
}
