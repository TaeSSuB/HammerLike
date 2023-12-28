using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Johnson;

[Serializable]
public struct MonsterStat
{
    public float maxHp;
    public float curHp;

    [Space(7.5f)]
    public float walkSpd;
    public float runSpd;

    [Space(7.5f)]
    public EnvasionStat envasionStat;

    [Space(7.5f)]
    public float upperHomingSpd; //상체 회전 속도
    public float legHomingSpd; //하체 회전 속도

    public float detectionRange;  // 플레이어를 인식할 범위 설정. 원하는 값으로 조절 가능.

}

public class Monster : MonoBehaviour
{

    private Transform playerTransform;
    // Note: 기능 구현 할 때는 접근지정자 크게 신경 안쓰고 작업함.
    // 차후 기능 작업 끝나고 나면 추가적으로 정리 예정!!
    // 불편해도 양해바랍니다!!! 스마미센!!

    public MonsterStat stat;
    public Slider healthSlider;
    [Header("State Machine")]
    public MonsterFSM fsm;

    [Space(10f)]
    [Header("Default Comps")]
    public Transform meshTr;
    public Animator animCtrl;
    public Rigidbody rd;

    [Space(10f)]
    [Header("Action Table")]
    // Note: 해당 부분은 몬스터에 맞는 액션으로 수정 필요
    public MonsterMove move;
    public MonsterAtk atk;
    public MonsterAim monsterAim;

    [Space(10f)]
    [Header("Cam Controller")]
    public CamCtrl camCtrl; // Note: 몬스터가 카메라를 직접 제어할 필요가 있을지 확인 필요

    [Space(10f)]
    [Header("Anim Bones")]
    public Transform headBoneTr;
    public Transform spineBoneTr;
    public Transform hpBoneTr;
    private HashSet<int> processedAttacks = new HashSet<int>();

    private bool isKnockedBack = false;
    private bool canTakeKnockBackDamage = true;

    private void Awake()
    {
        if (!fsm)
        { fsm = GetComponent<MonsterFSM>(); }

        if (!fsm)
        {
            gameObject.AddComponent<MonsterFSM>();
        }

        if (!rd)
        {
            rd = GetComponent<Rigidbody>();
        }
        stat.curHp = stat.maxHp;
    }

    void Start()
    {
        if (healthSlider != null)
        {
            healthSlider.maxValue = stat.maxHp;
            healthSlider.value = stat.curHp;
        }
    }

    void Update()
    {
        //move.Move(stat.walkSpd);
        DetectPlayer();
        ChasePlayer();

        if (healthSlider != null)
        {
            Vector3 screenPosition = Camera.main.WorldToScreenPoint(transform.position + new Vector3(-2.7f, 1.2f, 0));
            healthSlider.transform.position = screenPosition;
        }
    }

    private void LateUpdate()
    {
        Vector3 lookDir = monsterAim.Aiming();
        //Funcs.LookAtSpecificBone(spineBoneTr, eGizmoDir.Forward, lookDir, Vector3.zero);
    }

    private void FixedUpdate()
    {

    }

    private void OnEnable()
    {

    }
    private void OnDisable()
    {

    }

    private void OnDestroy()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("WeaponCollider"))
        {
            WeaponCollider weaponCollider = other.GetComponent<WeaponCollider>();
            if (weaponCollider != null && !processedAttacks.Contains(weaponCollider.CurrentAttackId))
            {
                // 데미지 및 넉백 처리
                PlayerAtk playerAttack = other.GetComponentInParent<PlayerAtk>();
                if (playerAttack != null)
                {
                    TakeDamage(playerAttack.attackDamage);
                    ApplyKnockback(playerAttack.transform.forward);
                    processedAttacks.Add(weaponCollider.CurrentAttackId);
                }

            }
        }
        if (other.gameObject.CompareTag("KnockBackable") && isKnockedBack && canTakeKnockBackDamage)
        {
            TakeDamage(10f); // KnockBackDamage
            canTakeKnockBackDamage = false;
            StartCoroutine(KnockBackDamageCooldown());
        }
    }

    private IEnumerator KnockBackDamageCooldown()
    {
        yield return new WaitForSeconds(1f); // 넉백 데미지 쿨다운
        canTakeKnockBackDamage = true;
    }

    private void TakeDamage(float damage)
    {
        if (stat.curHp > 0)  // 몬스터가 살아있을 때만 피격 처리
        {
            stat.curHp -= damage;
            if (healthSlider != null)
            {
                healthSlider.value = stat.curHp;
                ShowHealthSlider();  // 체력 UI 슬라이더 표시
            }

            if (stat.curHp <= 0)
            {
                Die();
            }
        }
    }

    private void ApplyKnockback(Vector3 direction)
    {
        float knockbackIntensity = 30f; // 넉백 강도
        direction.y = 0; // Y축 변화 제거
        GetComponent<Rigidbody>().AddForce(direction.normalized * knockbackIntensity, ForceMode.Impulse);
        isKnockedBack = true;
        StartCoroutine(KnockBackDuration());
    }

    private IEnumerator KnockBackDuration()
    {
        yield return new WaitForSeconds(1f); // 넉백 지속 시간
        isKnockedBack = false;
    }

    private void Die()
    {
        // 몬스터 사망 처리
        // 예: gameObject.SetActive(false); 또는 Destroy(gameObject);
        Destroy(gameObject);
    }
    private void ShowHealthSlider()
    {
        if (healthSlider != null)
        {
            healthSlider.gameObject.SetActive(true);
            StopCoroutine("HideHealthSlider");  // 이미 진행 중인 코루틴이 있다면 중단
            StartCoroutine("HideHealthSlider");  // 새 코루틴 시작
        }
    }

    private IEnumerator HideHealthSlider()
    {
        yield return new WaitForSeconds(2f);
        if (healthSlider != null && stat.curHp > 0)  // 몬스터가 살아있을 때만 슬라이더 비활성화
        {
            healthSlider.gameObject.SetActive(false);
        }
    }

    private void DetectPlayer()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, stat.detectionRange);

        foreach (var hit in hits)
        {
            if (hit.CompareTag("Player"))  // Player 태그를 가진 오브젝트를 인식
            {
                playerTransform = hit.transform;
                monsterAim.SetTarget(playerTransform);
                break;
            }
        }
    }


    void ChasePlayer()
    {
        if (playerTransform != null)
        {
            // 플레이어를 향해 이동
            Vector3 moveDirection = (playerTransform.position - transform.position).normalized;
            transform.position += moveDirection * stat.walkSpd * Time.deltaTime;

            // 플레이어를 바라보도록 Y축 회전
            Vector3 lookDirection = new Vector3(playerTransform.position.x, transform.position.y, playerTransform.position.z);
            transform.LookAt(lookDirection);

            animCtrl.SetBool("IsChasing", true);
        }
        else
        {
            animCtrl.SetBool("IsChasing", false);
        }
    }



}
