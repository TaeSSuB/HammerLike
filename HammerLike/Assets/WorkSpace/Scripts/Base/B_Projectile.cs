using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.ProBuilder;

public enum ProjectileType
{
    Default,    // 기본 직선형
    Parabola,   // 포물선 
    Missile
}

public class B_Projectile : MonoBehaviour
{
    [SerializeField] protected Rigidbody rigid;
    [SerializeField] protected Collider col;

    public float projectileSpeed;
    [SerializeField] protected ProjectileType projecType;

    [Header("Missile")]
    [SerializeField] LayerMask layerMask=0;
    float currentSpeed=0f;
    Transform target;
    void Start()
    {
        Init();
        // 일정 시간 후에 자동 파괴
        Destroy(gameObject, 5f);
    }

    
    public void Init()
    {
        if(projecType==ProjectileType.Default)
        {
        // 투사체 초기화
        Rigidbody rigid = GetComponent<Rigidbody>();
        var resultforward = GameManager.Instance.ApplyCoordScaleAfterNormalize(transform.forward);
        //rigid.velocity = resultforward * projectileSpeed;
        rigid.velocity = transform.forward * projectileSpeed * resultforward.magnitude;
        }
        else if(projecType == ProjectileType.Parabola)
        {
            ///
            /// 포물선을 이용하는 방법은 DOTween을 사용하는 방법과 
            ///
            Vector3 startPos = transform.position; // 발사구의 위치를 시작점으로 사용
            Vector3 goalPos = GameManager.Instance.Player.transform.position; // 플레이어의 위치를 목표점으로 사용
            Vector3 middlePos = new Vector3((startPos.x + goalPos.x) / 2, Mathf.Max(startPos.y, goalPos.y) + 3f, (startPos.z + goalPos.z) / 2); // 최고점 계산

            // 포물선 경로 설정
            transform.DOPath(new[] { startPos, middlePos, goalPos }, 1f, PathType.CatmullRom)
                .SetEase(Ease.OutQuad); // 이징 함수 조정
        }
        else if(projecType == ProjectileType.Missile)
        {
            rigid = GetComponent<Rigidbody>();
            rigid.velocity = Vector3.up * 5f;
            StartCoroutine(LaunchDelay());
        }
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

    private void Update()
    {
        if(target != null && projecType == ProjectileType.Missile)
        {
            if (currentSpeed <= projectileSpeed)
                currentSpeed += projectileSpeed * Time.deltaTime;

            transform.position += transform.up * currentSpeed * Time.deltaTime;

            Vector3 dir = (target.position - transform.position).normalized;
            transform.up = Vector3.Lerp(transform.up, dir, 0.25f);
        }
    }

    void SearchEnemy()
    {
        Collider[] cols = Physics.OverlapSphere(transform.position, 100f, layerMask);

        if (cols.Length > 0)
        {
            target = cols[Random.Range(0, cols.Length)].transform;
        }
    }

    IEnumerator LaunchDelay()
    {
        yield return new WaitUntil(() => rigid.velocity.y < 0f);
        yield return new WaitForSeconds(0.1f);

        SearchEnemy();
    }

}
