using HutongGames.PlayMaker.Actions;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(B_BossController))]
public class B_Boss : B_UnitBase
{
    #region Variables

    [Header("Boss Variables")]
    private B_BossController bossController;

    public B_BossController BossController => bossController;

    #endregion

    #region Unity Callbacks & Init
    
    public override void Init()
    {
        base.Init();

        bossController = GetComponent<B_BossController>();
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

            if(player == null) return;

            // Get hit dir from player
            Vector3 hitDir = (transform.position - player.transform.position).normalized;

            var chargeAmount = player.UnitStatus.atkDamage / player.UnitStatus.AtkDamageOrigin;
            
            // Take Damage and Knockback dir from player
            TakeDamage(hitDir, player.UnitStatus.atkDamage, player.UnitStatus.knockbackPower * chargeAmount);

            Anim.SetTrigger("tHit");

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

            Anim.SetTrigger("tHit");

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
        bossController.SetState(BossAIStateType.DEAD);

        //DisconnectMusclesRecursive(GameManager.Instance.Player.transform.position);
        //Invoke(nameof(DisconnectMusclesRecursive), 0.1f);
    }

    public override void Attack()
    {
        base.Attack();

        transform.LookAt(GameManager.Instance.Player.transform);
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
