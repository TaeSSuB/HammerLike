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
    public Transform playerTransform;
    private Quaternion initialRotation;
    private NavMeshObstacle navMeshObstacle;
    private UI_InGame ui_InGame;
    private float rotationDuration = 0.5f;
    private bool isPlayerNearby = false;
    public Transform headTransform;
    private GameObject interactiveObj;
    private bool isInteracting = false;

    public override void Init()
    {
        base.Init();
        navMeshObstacle = GetComponent<NavMeshObstacle>();
        if (navMeshObstacle == null)
        {
            navMeshObstacle = gameObject.AddComponent<NavMeshObstacle>();
        }
        ui_InGame = B_UIManager.Instance.UI_InGame;
        playerTransform = GameManager.Instance.Player.transform;
    }

    protected void Start()
    {
        initialRotation = transform.rotation;
        Init();

        interactiveObj = B_UIManager.Instance.CreateInteractiveWorldUI(headTransform);
        interactiveObj.SetActive(false);
    }

    public void StartInteraction()
    {
        interactiveObj.SetActive(false);
        if (isInteracting) return;
        isInteracting = true;

        Vector3 directionToPlayer = (playerTransform.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(directionToPlayer.x, 0, directionToPlayer.z));
        transform.DORotateQuaternion(lookRotation, rotationDuration);

        ui_InGame.TopPanelGameObject.SetActive(false);
        ui_InGame.BottomPanelGameObject.SetActive(false);

        // 여기에 대화 UI를 표시하는 로직을 추가하세요.
        ShowDialogueUI();
    }

    public void EndInteraction()
    {
        if (!isInteracting) return;
        isInteracting = false;

        transform.DORotateQuaternion(initialRotation, rotationDuration);
        ui_InGame.TopPanelGameObject.SetActive(true);
        ui_InGame.BottomPanelGameObject.SetActive(true);

        // 대화 UI를 숨기는 로직을 추가하세요.
        HideDialogueUI();
    }

    private void ShowDialogueUI()
    {
        // 대화 UI 표시 로직
        Debug.Log("Dialogue Started with NPC: " + name);
        // 예시: ui_InGame.ShowDialogueWindow(this);
    }

    private void HideDialogueUI()
    {
        // 대화 UI 숨기기 로직
        Debug.Log("Dialogue Ended with NPC: " + name);
        // 예시: ui_InGame.HideDialogueWindow();
    }

    protected override void Update()
    {
        base.Update();

        /*if (isPlayerNearby && Input.GetKeyDown(KeyCode.F))
        {
            interactiveObj.SetActive(false);
            StartInteraction();
        }*/
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (!isPlayerNearby)
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
            if (isPlayerNearby)
            {
                isPlayerNearby = false;
                interactiveObj.SetActive(false);
                EndInteraction();
            }
        }
    }
}
