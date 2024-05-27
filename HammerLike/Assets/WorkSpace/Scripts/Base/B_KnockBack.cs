using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class B_KnockBack : MonoBehaviour
{
    [Header("Knockback")]
    protected float knockBackMultiplier = 1f;
    protected float knockbackDuration = 0.5f;
    protected float maxKnockBackForce = 100f;

    protected AnimationCurve knockbackCurve;

    protected ForceMode forceMode = ForceMode.Force;
    protected Vector3 remainKnockBackDir;
    protected float remainKnockBackForce = 0f;

    protected void Start()
    {
        ApplySystemSettings();
    }

    protected virtual void ApplySystemSettings()
    {
        var systemSettings = GameManager.Instance.SystemSettings;

        knockbackDuration = systemSettings.KnockbackDuration;
        knockBackMultiplier = systemSettings.KnockBackScale;
        maxKnockBackForce = systemSettings.MaxKnockBackForce;
        knockbackCurve = systemSettings.KnockbackCurve;
        forceMode = systemSettings.KnockbackForceMode;
    }


    public void Knockback(B_UnitBase unitBase, Vector3 inDir, float force)
    {
        // Apply Coord Scale inDir
        inDir = GameManager.Instance.ApplyCoordScaleAfterNormalize(inDir);

        var resultKnockPower = Mathf.Clamp(force * knockBackMultiplier, 0f, maxKnockBackForce);

        StartCoroutine(CoSmoothKnockback(unitBase, inDir, resultKnockPower, unitBase.Rigid, knockbackCurve, knockbackDuration, forceMode));
    }

    
    /// <summary>
    /// CoSmoothKnockback : 부드러운 넉백 적용 코루틴
    /// </summary>
    /// <param name="inDir"></param>
    /// <param name="force"></param>
    /// <param name="inRigid"></param>
    /// <param name="inCurve"></param>
    /// <param name="inDuration"></param>
    /// <returns></returns>
    public IEnumerator CoSmoothKnockback(B_UnitBase unitBase, Vector3 inDir, float force, Rigidbody rigidbody,AnimationCurve inCurve, float inDuration = 0.5f, ForceMode inForceMode = ForceMode.VelocityChange)
    {
        Vector3 knockbackVelocity = inDir * force / unitBase.UnitStatus.mass;

        if(inForceMode != ForceMode.VelocityChange)
        {
            knockbackVelocity = inDir * force;
        }

        float knockbackDuration = inDuration; // Duration over which the force is applied
        float startTime = Time.time;

        unitBase.DisableMovementAndRotation();

        unitBase.IsKnockback = true;
        unitBase.IsInvincible = true;

        while (Time.time < startTime + knockbackDuration)
        {
            float elapsed = (Time.time - startTime) / knockbackDuration;

            rigidbody.AddForce(knockbackVelocity * inCurve.Evaluate(elapsed), inForceMode);

            yield return null;
        }

        unitBase.IsKnockback = false;
        unitBase.IsInvincible = false;

        unitBase.EnableMovementAndRotation();

        unitBase.CheckDead(true);
    }
}
