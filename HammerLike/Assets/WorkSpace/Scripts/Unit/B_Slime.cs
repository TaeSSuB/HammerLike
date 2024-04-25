using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.UI;
using UnityEngine.XR;

public class B_Slime : B_Enemy
{
    [Header("Slime")]
    public float duration = 1f;
    public float multiplier = 5f;

    public float deadMultiplier = 2f;

    float firingAngle = 45.0f;

    public AnimationCurve inCurve;

    protected override void Dead()
    {
        base.Dead();

        StartCoroutine(CoSlimeDead());
    }

    protected override void Attack()
    {
        Debug.Log("Slime Attack");
        //base.Attack();
        AIStateManager?.SetState(AIStateType.IDLE);

        if (UnitStatus.currentAttackCooltime <= 0)
        {
            StartCoroutine(CoSlimeAttack());

            // Attack
            UnitStatus.currentAttackCooltime = UnitStatus.maxAttackCooltime;
        }   
    }

    IEnumerator CoSlimeAttack()
    {
        var targetTr = GameManager.instance.Player.transform;

        // Shortest distance to the target
        float target_Distance = Vector3.Distance(transform.position, targetTr.position);


        // Calculate the velocity needed to throw the object to the target at specified angle.
        float projectile_Velocity = target_Distance / (Mathf.Sin(2 * firingAngle * Mathf.Deg2Rad) / 9.8f);

        // Extract the X  & Y componenent of the velocity
        float Vx = Mathf.Sqrt(projectile_Velocity) * Mathf.Cos(firingAngle * Mathf.Deg2Rad);
        float Vy = Mathf.Sqrt(projectile_Velocity) * Mathf.Sin(firingAngle * Mathf.Deg2Rad);

        // Rotate projectile to face the target.
        //var newRot = Quaternion.LookRotation(targetTr.position - transform.position);
        transform.LookAt(targetTr);
        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);

        //transform.rotation = newRot;

        float flightDuration = target_Distance / Vx;

        // Move projectile to the target and adjust height based on the cross product
        float elapse_time = 0;

        rigid.isKinematic = true;
        //rigid.mass = 100f;

        Vector3 dir = targetTr.position - transform.position;
        Debug.DrawLine(transform.position, targetTr.position, Color.red, 1f);

        while (elapse_time < flightDuration)
        {
            transform.Translate(0, (Vy - (9.8f * elapse_time)) * Time.deltaTime * multiplier, 0f);

            Vector3 meshForwardXZ = transform.forward;
            //dir.y = 0;
            //Vector3 meshForwardXZ = dir.normalized;


            Debug.Log("meshForwardXZ: " + meshForwardXZ);

            transform.Translate(meshForwardXZ * Vx * Time.deltaTime * multiplier * (1 + inCurve.Evaluate(elapse_time)), Space.World);

            Debug.DrawLine(transform.position, meshForwardXZ, Color.green, 1f);
            Debug.DrawLine(transform.position, Vx * dir, Color.blue, 1f);
            Debug.DrawLine(transform.position, (1 + inCurve.Evaluate(elapse_time)) * dir, Color.yellow, 1f);

            elapse_time += Time.deltaTime;

            yield return null;
        }

        // Reset height
        transform.position = new Vector3(transform.position.x, targetTr.position.y, transform.position.z);

        rigid.isKinematic = false;
        //rigid.mass = unitStatus.mass;

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
