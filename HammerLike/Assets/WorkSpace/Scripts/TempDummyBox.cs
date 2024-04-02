using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TempDummyBox : MonoBehaviour
{
    public Rigidbody rigid;

    public float knockbackMultiplier = 1.0f;
    public float knockbackIncrease = 1.5f; // Multiplier increase per hit
    public float knockbackResetTime = 1.0f;
    private float lastKnockbackTime = -Mathf.Infinity; // Initialize to a time in the past

    protected void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("WeaponCollider"))
        {
            // Get player
            B_Player player = other.GetComponentInParent<B_Player>();

            // Get hit dir from player
            Vector3 hitDir = (transform.position - player.transform.position).normalized;
            Vector3 coordDir = GameManager.instance.ApplyCoordScale(hitDir);
            //Vector3 coordDir = hitDir * GameManager.instance.CalcCoordScale(hitDir);

            // Take Damage and Knockback dir from player
            Knockback(coordDir, 5);


        }
    }
    // knockback function with mass
    public void Knockback(Vector3 inDir, float force)
    {
        float timeSinceLastKnockback = Time.time - lastKnockbackTime;
        if (timeSinceLastKnockback < knockbackResetTime)
        {
            knockbackMultiplier *= knockbackIncrease; // Increase multiplier for consecutive hits
        }
        else
        {
            knockbackMultiplier = 1.0f; // Reset multiplier after knockbackResetTime
        }

        // Apply a smoothed knockback over time rather than as an impulse
        StartCoroutine(SmoothKnockback(inDir, force));
        //rigid.AddForce(inDir * force, ForceMode.Impulse);

        //Debug.Log(rigid.velocity);
        lastKnockbackTime = Time.time;
    }

    private IEnumerator DecayKnockbackForce(Vector3 initialForce)
    {
        Vector3 currentForce = initialForce;
        while (currentForce.magnitude > 0.1f) // Continue until the force is negligible
        {
            // Decay the force based on time and friction
            currentForce -= currentForce * Time.deltaTime; //* unitStatus.friction;
            // Optional: Check for balance recovery mechanics here

            // Apply the decayed force
            rigid.AddForce(currentForce * Time.deltaTime, ForceMode.VelocityChange);
            yield return null;
        }
    }
    private IEnumerator SmoothKnockback(Vector3 inDir, float force)
    {
        Vector3 knockbackVelocity = inDir * force;
        // 초기 속도를 저장합니다.
        Vector3 initialVelocity = rigid.velocity;
        float knockbackDuration = 0.3f; // Duration over which the force is applied
        float startTime = Time.time;

        //DisableMovementAndRotation();

        while (Time.time < startTime + knockbackDuration)
        {
            float elapsed = (Time.time - startTime) / knockbackDuration;

            rigid.velocity = Vector3.Lerp(initialVelocity, knockbackVelocity, elapsed);

            yield return null;
        }

        //EnableMovementAndRotation();
    }
}
