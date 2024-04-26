using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class B_Prop : B_Entity
{
    public int hp;

    protected override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);

        if (other.CompareTag("WeaponCollider"))
        {
            TakeDamage(1, other.ClosestPoint(transform.position));
        }
    }

    public void TakeDamage(int damage, Vector3 pos)
    {
        hp -= damage;
        if (hp <= 0)
        {
            Dead();
        }
        else
        {
            B_VFXPoolManager.Instance.PlayVFX(VFXName.Hit, pos);
            B_AudioManager.Instance.PlaySound(AudioCategory.SFX, AudioTag.Battle);
        }
    }

    public override void Init()
    {
        base.Init();
    }

    protected virtual void Dead()
    {
        
    }
}
