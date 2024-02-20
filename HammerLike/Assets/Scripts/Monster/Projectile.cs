using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float lifetime = 5f; // 투사체의 생명주기(초)
    private Monster shooter; // 투사체를 발사한 몬스터

    void Start()
    {
        // 일정 시간 후에 자동 파괴
        Destroy(gameObject, lifetime);
    }

    public void SetShooter(Monster monster)
    {
        // 투사체를 발사한 몬스터 설정
        shooter = monster;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // 부모 오브젝트에 있는 Player 컴포넌트에 접근
            Player playerComponent = other.GetComponentInParent<Player>();

            if (playerComponent != null&&!playerComponent.isEvading)
            {
                // 플레이어에게 데미지를 주는 로직
                playerComponent.TakeDamage(shooter.stat.attackPoint);
                 
                    SoundManager soundManager = SoundManager.Instance;
                    soundManager.PlaySFX(soundManager.audioClip[8]);
                
                // 투사체 파괴
                DestroyProjectile();
            }

        }
    }


    private void DestroyProjectile()
    {
        // 투사체를 파괴하기 전에 몬스터에게 알림
        if (shooter != null)
        {
            shooter.ProjectileDestroyed();
        }

        // 투사체 파괴
        Destroy(gameObject);
    }
}
