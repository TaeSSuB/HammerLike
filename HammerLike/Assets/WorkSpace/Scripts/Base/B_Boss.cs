using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class B_BossPattern
{
    public string name;
    public int priority; // 패턴 우선순위
    public float cooltime; // 패턴 쿨타임
    public float minWaitTime; // 패턴 최소 대기 시간
    public float lastActivatedTime; // 마지막 패턴 활성화 시간
    public Func<B_BossPattern, IEnumerator> patternAction; // 패턴 동작을 위한 코루틴 함수
    public bool isCurrentlyActive = false;

    private MonoBehaviour owner; // Coroutine 실행을 위한 MonoBehaviour 참조

    public B_BossPattern(string name, int priority, float cooltime, float minWaitTime, Func<B_BossPattern, IEnumerator> patternAction, MonoBehaviour owner)
    {
        this.name = name ?? nameof(patternAction); // 이름이 비어있는 경우 메소드 이름 사용
        this.priority = priority;
        this.cooltime = cooltime;
        this.minWaitTime = minWaitTime;
        this.patternAction = patternAction;
        this.lastActivatedTime = -Mathf.Infinity; // 활성화 가능하게 초기 설정
        this.isCurrentlyActive = false;

        this.owner = owner; // MonoBehaviour 인스턴스 저장
    }


    public bool IsReadyToActivatePattern(float lastAnyPatternActivatedTime)
    {
        // 쿨타임과 최소 대기 시간을 확인하여 패턴 실행 가능 여부 반환
        return Time.time >= lastActivatedTime + cooltime &&
               Time.time >= lastAnyPatternActivatedTime + minWaitTime && !isCurrentlyActive;
    }

    public IEnumerator ActivatePattern()
    {
        isCurrentlyActive = true;

        owner.StartCoroutine(patternAction(this));

        yield return new WaitForSeconds(cooltime);

        StopPattern();
    }

    public void StopPattern()
    {
        isCurrentlyActive = false;
        lastActivatedTime = Time.time;    
    }
}

public class B_Boss : B_UnitBase
{
    #region Variables

    [Header("Boss Variables")]
    protected List<B_BossPattern> bossPatterns = new List<B_BossPattern>();
    protected float lastPatternActivatedTime = -Mathf.Infinity;
    

    #endregion

    #region Unity Callbacks & Init
    
    public override void Init()
    {
        base.Init();

        //FindObjectOfType<B_UIManager>().CreateHPWorldUI(hpPosTR, this);
        // Find UI Manager and Active Boss HP UI
    }

    protected override void Update()
    {
        base.Update();

    }

    protected override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);

        if(other.CompareTag("WeaponCollider") && UnitStatus.currentHP > 0)
        {
            if(isInvincible) return;

            // Get player
            // 현재는 B_Player지만, B_UnitBase를 상속받는 모든 유닛에게 적용 가능. a.HG
            B_Player player = other.GetComponentInParent<B_Player>();

            // Get hit dir from player
            Vector3 hitDir = (transform.position - player.transform.position).normalized;

            var chargeAmount = player.UnitStatus.atkDamage / player.UnitStatus.AtkDamageOrigin;
            
            // Take Damage and Knockback dir from player
            TakeDamage(hitDir, player.UnitStatus.atkDamage, player.UnitStatus.knockbackPower * chargeAmount);

            var vfxPos = other.ClosestPointOnBounds(transform.position);
            B_VFXPoolManager.Instance.PlayVFX(VFXName.Hit, vfxPos);

            if (unitStatus.currentHP > 0)
            {
                B_AudioManager.Instance.PlaySound(AudioCategory.SFX, AudioTag.Battle);
            }
            else
            {
                B_AudioManager.Instance.PlaySound(AudioCategory.SFX, AudioTag.Death);
            }
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

            TakeDamage(hitDir, (int)(other.Rigid.mass / 2f), other.Rigid.mass);

            var vfxPos = collision.contacts[0].point;
            B_VFXPoolManager.Instance.PlayVFX(VFXName.Hit, vfxPos);

            if (unitStatus.currentHP > 0)
            {
                B_AudioManager.Instance.PlaySound(AudioCategory.SFX, AudioTag.Battle);
            }
            else
            {
                B_AudioManager.Instance.PlaySound(AudioCategory.SFX, AudioTag.Death);
            }
        }


    }
    #endregion

    #region Check or Update State

    protected override void Dead()
    {
        base.Dead();
        //aIStateManager.SetState(AIStateType.DEAD);

        //DisconnectMusclesRecursive(GameManager.Instance.Player.transform.position);
        //Invoke(nameof(DisconnectMusclesRecursive), 0.1f);
    }

    #endregion

    #region Animation Event

    public override void StartAttack()
    {
        base.StartAttack();
    }

    public override void EndAttack()
    {
        base.EndAttack();
    }

    #endregion


}
