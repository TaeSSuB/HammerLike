using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.UI;
using UnityEngine.XR;

public class B_Slime : B_Enemy
{
    [Header("Slime")]
    [SerializeField] private Collider weaponCollider;
    public float duration = 1f;
    public float multiplier = 5f;

    public float deadMultiplier = 2f;

    float firingAngle = 45.0f;

    public AnimationCurve inCurve;

    public override void Init()
    {
        base.Init();

        weaponCollider.enabled = false;
    }

    protected override void Dead()
    {
        base.Dead();
        
        weaponCollider.enabled = false;

        StartCoroutine(CoSlimeDead());
    }

    public override void Attack()
    {
        base.Attack();
        Debug.Log("Slime Attack");
        //base.Attack();
        //AIStateManager?.SetState(AIStateType.IDLE);

        StartCoroutine(CoSlimeAttack());

        //if (UnitStatus.currentAttackCooltime <= 0)
        //{
        //    StartCoroutine(CoSlimeAttack());

        //    // Attack
        //    UnitStatus.currentAttackCooltime = UnitStatus.maxAttackCooltime;
        //}   
    }

    public override void StartAttack()
    {
        base.StartAttack();
        
        col.enabled = false;
        weaponCollider.enabled = true;

        Rigid.isKinematic = true;
    }

    public override void EndAttack()
    {
        base.EndAttack();

        col.enabled = true;
        weaponCollider.enabled = false;

        Rigid.isKinematic = false;
    }

    IEnumerator CoSlimeAttack()
    {
        var lastTargetPos = GameManager.Instance.Player.transform.position;

        // Shortest distance to the target
        float target_Distance = Vector3.Distance(transform.position, lastTargetPos);


        // Calculate the velocity needed to throw the object to the target at specified angle.
        float projectile_Velocity = target_Distance / (Mathf.Sin(2 * firingAngle * Mathf.Deg2Rad) / 9.8f);

        // Extract the X  & Y componenent of the velocity
        float Vx = Mathf.Sqrt(projectile_Velocity) * Mathf.Cos(firingAngle * Mathf.Deg2Rad);
        float Vy = Mathf.Sqrt(projectile_Velocity) * Mathf.Sin(firingAngle * Mathf.Deg2Rad);

        // Rotate projectile to face the target.
        //var newRot = Quaternion.LookRotation(targetTr.position - transform.position);
        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);

        //transform.rotation = newRot;

        float flightDuration = target_Distance / Vx;

        // Move projectile to the target and adjust height based on the cross product
        float elapse_time = 0;
        float delayTime = 0.5f;

        agent.enabled = false;
        //rigid.mass = 100f;

        Vector3 dir = lastTargetPos - transform.position;
        Debug.DrawLine(transform.position, lastTargetPos, Color.red, 1f);

        //transform.LookAt(lastTargetPos);
        isAttacking = true;
        
        yield return new WaitForSeconds(delayTime);

        StartAttack();

        while (elapse_time < flightDuration)
        {
            transform.Translate(0, (Vy - (9.8f * elapse_time)) * Time.deltaTime * multiplier, 0f);

            Vector3 meshForwardXZ = transform.forward;

            transform.Translate(meshForwardXZ * Vx * Time.deltaTime * multiplier * (1 + inCurve.Evaluate(elapse_time)), Space.World);

            Debug.DrawLine(transform.position, meshForwardXZ, Color.green, 1f);
            Debug.DrawLine(transform.position, Vx * dir, Color.blue, 1f);
            Debug.DrawLine(transform.position, (1 + inCurve.Evaluate(elapse_time)) * dir, Color.yellow, 1f);

            elapse_time += Time.deltaTime;

            yield return null;
        }

        EndAttack();
        unitStatus.currentAttackCooltime = unitStatus.maxAttackCooltime;

        // Reset height
        transform.position = new Vector3(transform.position.x, 0f, transform.position.z);
        agent.enabled = true;
        Rigid.velocity = Vector3.zero;
        Rigid.isKinematic = false;
        //rigid.mass = unitStatus.mass;

        yield return new WaitForSeconds(delayTime);

    }

    IEnumerator CoSlimeDead()
    {
        float duration = 1f;

        while (duration > 0f)
        {
            transform.Translate(0, -deadMultiplier * Time.deltaTime, 0);

            duration -= Time.deltaTime;

            yield return new WaitForSeconds(Time.deltaTime);
        }

        transform.position = new Vector3(transform.position.x, -5f, transform.position.z);
    }
}
