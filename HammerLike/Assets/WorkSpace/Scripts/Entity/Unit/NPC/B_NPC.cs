using HutongGames.PlayMaker.Actions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using DG.Tweening;

/// <summary>
/// Town 과 던전안에 있는 NPC들을 정의하는 Scripts 
/// 각각의 NPC들은 아래에서 재정의 해서 사용하며 기본적으로 대화 할 때 플레이어 기준으로 회전하는 스크립트등
/// 일반적인 내용만 받아감
/// 상호작용은 크게 StartInteraction 과 End
/// </summary>
public class B_NPC : B_UnitBase
{
    // Player를 나타내는 Transform
    public Transform playerTransform;

    // NPC의 초기 회전 값을 저장하기 위한 변수
    private Quaternion initialRotation;
    private NavMeshObstacle navMeshObstacle;
    private UI_InGame ui_InGame;
    private float rotationDuration = 0.5f;

    private bool isPlayerNearby = false;

    public Transform headTransform; // NPC 머리 위치

    private GameObject interactiveObj;

    public override void Init()
    {
        base.Init();
        navMeshObstacle = GetComponent<NavMeshObstacle>();
        if (navMeshObstacle == null)
        {
            navMeshObstacle = gameObject.AddComponent<NavMeshObstacle>();
            //navMeshObstacle.carving = true;  // Enable carving
        }
        ui_InGame = B_UIManager.Instance.UI_InGame;
        playerTransform = GameManager.Instance.Player.transform;
    }

    protected void Start()
    {
        // 초기 회전 값 저장
        initialRotation = transform.rotation;
        Init();

        interactiveObj = B_UIManager.Instance.CreateInteractiveWorldUI(headTransform);
        interactiveObj.SetActive(false);
    }

    protected void StartInteraction()
    {
        // 플레이어를 바라보도록 회전
        Vector3 directionToPlayer = (playerTransform.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(directionToPlayer.x, 0, directionToPlayer.z));

        // DoTween을 사용하여 회전
        transform.DORotateQuaternion(lookRotation, rotationDuration);

        ui_InGame.TopPanelGameObject.SetActive(false);
        ui_InGame.BottomPanelGameObject.SetActive(false);
    }

    protected void EndInteraction()
    {
        // 초기 회전 값으로 복귀
        transform.DORotateQuaternion(initialRotation, rotationDuration);
        ui_InGame.TopPanelGameObject.SetActive(true);
        ui_InGame.BottomPanelGameObject.SetActive(true);
    }

    protected override void Update()
    {
        base.Update();

        if(isPlayerNearby&& Input.GetKeyDown(KeyCode.F))
        {
            interactiveObj.SetActive(false);
            StartInteraction();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Enter triggered by: " + other.name);
            if (!isPlayerNearby) // 중복 Enter 방지
            {
                isPlayerNearby = true;
                interactiveObj.SetActive(true);
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Exit triggered by: " + other.name);
            if (isPlayerNearby) // 중복 Exit 방지
            {
                isPlayerNearby = false;
                interactiveObj.SetActive(false);
                EndInteraction();
            }
        }
    }

}
