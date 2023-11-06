using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
    }

    void Start()
    {

    }

    void Update()
    {
        //move.Move(stat.walkSpd);
        DetectPlayer();
        ChasePlayer();
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
            Vector3 moveDirection = (playerTransform.position - transform.position).normalized;
            transform.position += moveDirection * stat.walkSpd * Time.deltaTime;

            animCtrl.SetBool("IsChasing", true);
        }
        else
        {
            animCtrl.SetBool("IsChasing", false);
        }
    }


}
