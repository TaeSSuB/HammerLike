using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class B_Projectile : MonoBehaviour
{
    [SerializeField] protected Rigidbody rigid;
    [SerializeField] protected Collider col;

    public float projectileSpeed;

    void Start()
    {
        Init();
        // 일정 시간 후에 자동 파괴
        Destroy(gameObject, 5f);
    }

    public void Init()
    {
        // 투사체 초기화
        Rigidbody rigid = GetComponent<Rigidbody>();
        var resultforward = GameManager.Instance.ApplyCoordScaleAfterNormalize(transform.forward);
        //rigid.velocity = resultforward * projectileSpeed;
        rigid.velocity = transform.forward * projectileSpeed * resultforward.magnitude;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            B_Player targetPlayer = GameManager.Instance.Player;

            if (targetPlayer != null)
            {
                //SoundManager soundManager = SoundManager.Instance;
                //soundManager.PlaySFX(soundManager.audioClip[8]);

                // 투사체 파괴
                DestroyProjectile();
            }

        }
    }
    private void DestroyProjectile()
    {
        Destroy(gameObject);
    }
}
