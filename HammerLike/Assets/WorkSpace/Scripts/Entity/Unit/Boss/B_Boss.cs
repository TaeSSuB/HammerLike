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
    [SerializeField] private bool enableHitEvent = false;
    public bool EnableHitEvent => enableHitEvent;

    public void SetEnableHitEvent(bool value)
    {
        enableHitEvent = value;
    }

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

    protected void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("WeaponCollider") && UnitStatus.currentHP > 0)
        {
            if(isInvincible) return;

            B_EnemyWeapon enemyWeapon = other.GetComponent<B_EnemyWeapon>();

            if(enemyWeapon != null) Debug.Log("EnemyWeapon : " + enemyWeapon.name);

            // Get player
            // 현재는 B_Player지만, B_UnitBase를 상속받는 모든 유닛에게 적용 가능. a.HG
            B_Player player = other.GetComponentInParent<B_Player>();

            if(player == null) return;

            // Get hit dir from player
            Vector3 hitDir = (transform.position - player.transform.position).normalized;

            var chargeAmount = 1; 
            //var chargeAmount = (player.UnitStatus as SO_PlayerStatus).chargeRate;
            //var chargeAmount = (float)(player.UnitStatus.atkDamage / 10f); // 10f : player.AtkDamageOrigin
            
            // Take Damage and Knockback dir from player
            TakeDamage(hitDir, player.UnitStatus.atkDamage, player.UnitStatus.knockbackPower * chargeAmount, true);

            if(EnableHitEvent)
                Anim.SetTrigger("tHit");

            var vfxPos = other.ClosestPointOnBounds(transform.position);
            B_VFXPoolManager.Instance.PlayVFX(VFXName.Hit, vfxPos);

            if (UnitStatus.currentHP > 0)
            {
                B_AudioManager.Instance.PlaySound(AudioCategory.SFX, AudioTag.Battle);
            }
            else
            {
                B_AudioManager.Instance.PlaySound(AudioCategory.SFX, AudioTag.Death);
            }
            
        }
    }

    protected void OnCollisionEnter(Collision collision)
    {
        // When hit Other Enemy
        if (collision.gameObject.CompareTag("Enemy"))
        {
            if (IsKnockback) return;

            var other = collision.gameObject.GetComponent<B_Enemy>();
            
            if (other == null) return;
            if (!other.IsKnockback) return;

            // Get hit dir from another enemy
            Vector3 hitDir = (transform.position - collision.transform.position).normalized;

            // Take Damage and Knockback dir from another enemy
            TakeDamage(hitDir, (int)(other.Rigid.mass / 2f), other.Rigid.mass, false);

            if(EnableHitEvent)
                Anim.SetTrigger("tHit");

            var vfxPos = collision.contacts[0].point;
            B_VFXPoolManager.Instance.PlayVFX(VFXName.Hit, vfxPos);

            if (UnitStatus.currentHP > 0)
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

    protected override void Dead(bool isSelf = false)
    {
        base.Dead(isSelf);
        bossController.SetState(BossAIStateType.DEAD);

        //DisconnectMusclesRecursive(GameManager.Instance.Player.transform.position);
        //Invoke(nameof(DisconnectMusclesRecursive), 0.1f);
    }

    public override void Attack()
    {
        base.Attack();

        var xzPlayerPos = new Vector3(GameManager.Instance.Player.transform.position.x, transform.position.y, GameManager.Instance.Player.transform.position.z);

        transform.LookAt(xzPlayerPos);
    }    

    #endregion

    #region Animation Event

    public override void OnStartAttack()
    {
        base.OnStartAttack();
    }

    public override void OnEndAttack()
    {
        base.OnEndAttack();
    }

    #endregion


}
