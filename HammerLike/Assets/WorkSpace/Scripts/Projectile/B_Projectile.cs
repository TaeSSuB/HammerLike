using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.ProBuilder;

public enum ProjectileType
{
    Default,    // 기본 직선형
    Parabola,   // 포물선 
    Launch,
    Missile
}

public class B_Projectile : MonoBehaviour
{
    [SerializeField] protected Rigidbody rigid;
    [SerializeField] protected Collider col;

    public int projectileDamage;
    public float projectileSpeed;
    [SerializeField] protected ProjectileType projecType;

    [Header("Missile")]
    [SerializeField] LayerMask layerMask = 0;
    float currentSpeed = 0f;
    Transform target;

    [Header("Launch")]
    [SerializeField] private float yOffset;
    [SerializeField] private float upTime;
    [SerializeField] private float fallTime;
    [SerializeField] private float delay;
    [SerializeField] private GameObject sprite;

    void Start()
    {
        Init();
        // 일정 시간 후에 자동 파괴
        if (projecType != ProjectileType.Launch)
            Destroy(gameObject, 5f);
    }


    public void Init()
    {
        if (projecType == ProjectileType.Default)
        {
            // 투사체 초기화
            Rigidbody rigid = GetComponent<Rigidbody>();
            var resultforward = GameManager.Instance.ApplyCoordScaleAfterNormalize(transform.forward);
            //rigid.velocity = resultforward * projectileSpeed;
            rigid.velocity = transform.forward * projectileSpeed * resultforward.magnitude;
        }
        else if (projecType == ProjectileType.Parabola)
        {
            ///
            /// 포물선을 이용하는 방법은 DOTween을 사용하는 방법과 Rigidbody 있는데 고민중
            ///
            Vector3 startPos = transform.position; // 발사구의 위치를 시작점으로 사용
            Vector3 goalPos = GameManager.Instance.Player.transform.position; // 플레이어의 위치를 목표점으로 사용
            Vector3 middlePos = new Vector3((startPos.x + goalPos.x) / 2, Mathf.Max(startPos.y, goalPos.y) + 3f, (startPos.z + goalPos.z) / 2); // 최고점 계산

            // 포물선 경로 설정
            transform.DOPath(new[] { startPos, middlePos, goalPos }, 1f, PathType.CatmullRom)
                .SetEase(Ease.OutQuad); // 이징 함수 조정
        }
        else if (projecType == ProjectileType.Missile)
        {
            rigid = GetComponent<Rigidbody>();
            rigid.velocity = Vector3.up * 10f;
            StartCoroutine(LaunchDelay());
        }
        else if (projecType == ProjectileType.Launch)
        {
            rigid = GetComponent<Rigidbody>();
            rigid.isKinematic = true; // 물리 엔진의 영향을 받지 않도록 설정
            StartCoroutine(LaunchMovement());
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")&& projecType!=ProjectileType.Launch)
        {
            B_Player targetPlayer = GameManager.Instance.Player;

            if (targetPlayer != null)
            {
                // 플레이어에게 데미지를 입힘
                targetPlayer.TakeDamage(transform.forward, projectileDamage, projectileDamage, true);

                var vfxPos = other.ClosestPointOnBounds(other.transform.position);
                B_VFXPoolManager.Instance.PlayVFX(VFXName.Hit, vfxPos);

                // 투사체 파괴
                DestroyProjectile();
            }

        }
        // Temp.. 나중에 수정
        else if(other.gameObject.layer == LayerMask.NameToLayer("Object"))
        {
            DestroyProjectile();
        }
       
    }
    private void DestroyProjectile()
    {
        Destroy(gameObject);
    }

    private void Update()
    {
        if (target != null && projecType == ProjectileType.Missile)
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


        Vector3 newPosition = new Vector3(target.position.x, transform.position.y, target.position.z);
        GameObject range = Instantiate(sprite, new Vector3(target.position.x, 0, target.position.z), Quaternion.identity);
        Destroy(range, 3f);
        transform.position = newPosition;
        rigid.useGravity = true;
        rigid.velocity = Vector3.down * yOffset / fallTime;

    }

    IEnumerator LaunchDelay(float delay = 0.1f)
    {
        yield return new WaitUntil(() => rigid.velocity.y < 0f);
        rigid.useGravity = false;
        yield return new WaitForSeconds(delay);
        Destroy(gameObject, delay);

        Collider[] cols = Physics.OverlapSphere(transform.position, 10000f, layerMask);

        if (cols.Length > 0)
        {
            target = cols[Random.Range(0, cols.Length)].transform;
        }
        if (projecType == ProjectileType.Launch)
            SearchEnemy();
    }

    IEnumerator LaunchMovement()
    {
        Vector3 startPosition = transform.position;
        Vector3 endPosition = new Vector3(startPosition.x, startPosition.y + yOffset, startPosition.z);
        float elapsedTime = 0;

        while (elapsedTime < upTime)
        {
            float ratio = elapsedTime / upTime;
            transform.position = Vector3.Lerp(startPosition, endPosition, ratio);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        Vector3 targetPosition = new Vector3(GameManager.Instance.Player.transform.position.x, endPosition.y, GameManager.Instance.Player.transform.position.z);
        transform.position = endPosition; // 최종 위치 보정
        yield return new WaitForSeconds(delay); // 지정된 지연 시간을 기다림
        GameObject range = Instantiate(sprite, new Vector3(targetPosition.x, 0, targetPosition.z), Quaternion.identity);
        //Destroy(range, fallTime);
        transform.position = targetPosition;
        // 하강 시작
        rigid.isKinematic = false; // 물리 엔진 재활성화
        float initialDownSpeed = yOffset / fallTime;
        rigid.velocity = Vector3.down * initialDownSpeed; // 하강 속도 설정

        yield return new WaitForSeconds(fallTime+0.5f); // 하강 시간 동안 기다림
        Destroy(gameObject); // 투사체 제거
    }

}