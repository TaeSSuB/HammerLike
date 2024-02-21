using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponTrail : MonoBehaviour
{
    private TrailRenderer trailRenderer;
    private int currentAttackId=0;
    private int attackId=0;

    void Start()
    {
        trailRenderer = GetComponent<TrailRenderer>();
    }

    public void EnableTrail()
    {
        currentAttackId = attackId; // 현재 공격 ID 저장
        trailRenderer.enabled = true;
        trailRenderer.emitting = true;
        StartCoroutine(DisableTrailAfterDelay(attackId)); // 코루틴으로 지연 후 비활성화
    }

    private IEnumerator DisableTrailAfterDelay(int attackId)
    {
        yield return new WaitForSeconds(0.5f); // 0.5초 대기

        // 현재 공격 ID와 저장된 공격 ID가 같고, trailRenderer.emitting이 true인 경우에만 비활성화
        if (this.currentAttackId == attackId && trailRenderer.emitting)
        {
            trailRenderer.emitting = false;
        }
    }

    public void DisableTrail()
    {
        trailRenderer.emitting = false;
        attackId++;
    }
}
