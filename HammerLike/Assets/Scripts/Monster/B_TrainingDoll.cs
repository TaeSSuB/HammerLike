using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class TrainingDoll : B_Enemy
{

    protected override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);

        if (other.CompareTag("WeaponCollider") && UnitStatus.currentHP > 0)
        {
            
            // Get player
            B_Player player = other.GetComponentInParent<B_Player>();

            if (player == null) return;

            // Get hit dir from player
            Vector3 hitDir = (transform.position - player.transform.position).normalized;


            Sequence shakeSequence = DOTween.Sequence();
            shakeSequence.Append(transform.DOShakeRotation(0.3f, new Vector3(0, 0, 30), 10, 90, false))
                          .Append(transform.DORotateQuaternion(Quaternion.identity, 0.2f));

            //var chargeAmount = (player.UnitStatus as SO_PlayerStatus).chargeRate;
            var chargeAmount = (float)(player.UnitStatus.atkDamage / player.AtkDamageOrigin);

            // Take Damage and Knockback dir from player
            TakeDamage(hitDir, player.UnitStatus.atkDamage, player.UnitStatus.knockbackPower * chargeAmount, false);

            var vfxPos = other.ClosestPointOnBounds(transform.position);
            B_VFXPoolManager.Instance.PlayVFX(VFXName.Hit, vfxPos);

            // Audio handling
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

    protected virtual void Knockback(Vector3 inDir, float force)
    {

    }


}
