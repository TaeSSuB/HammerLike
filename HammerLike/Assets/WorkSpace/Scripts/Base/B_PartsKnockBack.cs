using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion.Dynamics;

[RequireComponent(typeof(B_KnockBack))]
public class B_PartsKnockBack : MonoBehaviour
{
    private B_KnockBack knockBack;

    protected float partsKnockBackMultiplier = 1f;
    protected float maxPartsBreakForce = 100f;
    protected AnimationCurve partsBreakForceCurve;

    protected float remainKnockBackForce = 0f;

    protected void Start()
    {
        knockBack = GetComponent<B_KnockBack>();


        ApplySystemSettings();
    }

    protected virtual void ApplySystemSettings()
    {
        var systemSettings = GameManager.Instance.SystemSettings;

        partsKnockBackMultiplier = systemSettings.PartsKnockBackScale;
        maxPartsBreakForce = systemSettings.MaxPartsBreakForce;
        partsBreakForceCurve = systemSettings.PartsBreakForceCurve;
    }

    /// <summary>
    /// DisconnectMusclesRecursive : PuppetMaster의 Muscle을 해제 및 넉백 적용
    /// </summary>
    /// <param name="inPos">넉백 기준점</param>
    public void DisconnectMusclesRecursive(B_UnitBase unitBase, BehaviourPuppet behaviourPuppet, Vector3 inPos, bool isSelf = false)
    {
        if (behaviourPuppet != null && behaviourPuppet.puppetMaster != null)
        {
            for (int i = 0; i < behaviourPuppet.puppetMaster.muscles.Length; i++)
            {
                Rigidbody muscleRigid = behaviourPuppet.puppetMaster.muscles[i].rigidbody;

                float partsKnockBackTime = 0.2f;

                // Temp 240402 - 파츠 별 넉백.., a.HG
                // 1. StartCoroutine(SmoothKnockback)
                // 2. ImpulseKnockbackToPuppet
                // 3. AddForce Each (Loop)

                if (muscleRigid != null && !isSelf)
                {
                    Vector3 dir = (muscleRigid.transform.position - inPos).normalized;
                    dir = GameManager.Instance.ApplyCoordScaleAfterNormalize(dir);

                    remainKnockBackForce = knockBack.remainKnockBackForce;
                    
                    remainKnockBackForce = Mathf.Clamp(remainKnockBackForce * partsKnockBackMultiplier, 0f, maxPartsBreakForce);

                    StartCoroutine(knockBack.CoSmoothKnockback(unitBase, dir, remainKnockBackForce, muscleRigid, partsBreakForceCurve, partsKnockBackTime, ForceMode.Impulse));
                }

                behaviourPuppet.puppetMaster.DisconnectMuscleRecursive(i, MuscleDisconnectMode.Sever);

                // root RigidBody 물리력 고정 및 콜라이더 비활성화
                unitBase.Rigid.isKinematic = true;
                unitBase.Col.enabled = false;
            }
        }
    }
}
