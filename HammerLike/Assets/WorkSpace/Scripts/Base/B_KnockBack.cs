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

    public float remainKnockBackForce = 0f;

    //public float RemainKnockBackForce => remainKnockBackForce;

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


    public void Knockback(B_UnitBase unitBase, Vector3 inDir, float force, bool slowMotion = false)
    {
        // Apply Coord Scale inDir
        inDir = GameManager.Instance.ApplyCoordScaleAfterNormalize(inDir);

        var resultKnockPower = Mathf.Clamp(force * knockBackMultiplier, 0f, maxKnockBackForce);

        remainKnockBackForce = resultKnockPower;

        if (slowMotion)
        {
            StartCoroutine(CoApplySlowMotionAndKnockback(unitBase, inDir, resultKnockPower, unitBase.Rigid, knockbackCurve, knockbackDuration, forceMode));
        }
        else
        {
            unitBase.CheckDead();

            StartCoroutine(CoSmoothKnockback(unitBase, inDir, resultKnockPower, unitBase.Rigid, knockbackCurve, knockbackDuration, forceMode));
        }
    }

    public IEnumerator CoApplySlowMotionAndKnockback(B_UnitBase unitBase, Vector3 direction, float force, Rigidbody rigidbody, AnimationCurve forceCurve, float knockbackDuration = 0.5f, ForceMode forceMode = ForceMode.VelocityChange)
    {
        // 피격 판정 후 슬로우모션 실행
        yield return StartCoroutine(GameManager.Instance.CoSlowMotion(0.1f, 0.75f));
        
        unitBase.CheckDead();

        // 슬로우모션 끝난 후 넉백 시작
        yield return StartCoroutine(CoSmoothKnockback(unitBase, direction, force, rigidbody, forceCurve, knockbackDuration, forceMode));
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

        var originRad = unitBase.Agent.radius;
        unitBase.Agent.radius = originRad * 0.5f;

        while (Time.time < startTime + knockbackDuration)
        {
            float elapsed = (Time.time - startTime) / knockbackDuration;

            rigidbody.AddForce(knockbackVelocity * inCurve.Evaluate(elapsed), inForceMode);

            yield return null;
        }

        unitBase.IsKnockback = false;
        unitBase.IsInvincible = false;

        unitBase.Agent.radius = originRad;

        unitBase.EnableMovementAndRotation();

        yield return null;
    }
}
