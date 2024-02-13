using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using Johnson;
using Rewired;
using RewiredPlayer = Rewired.Player;
using MoreMountains.Tools;
using PixelCrushers.DialogueSystem.Demo;
using TMPro;
using HutongGames.PlayMaker.Actions;
using Opsive.Shared.Editor.UIElements;

//FSM은 StateController로 관리
//Idle <-> Envasion <-> Death 정도로 관리 할 예정.
//State들은 MonoBehaviour 상속 안받고 그냥 컨트롤러에서 생성/초기화 해주기

//그 외 각 기능들은 Movement / Attack / Aim 등에서 구현
//그리고 각 State내의 Update에서 함수들 호출해서 가져오기
//(이렇게 안나누고 상하체 애니메이션 때문에 모두 FSM 만들면 답 없을거 같음)

//상하체 애니메이션 관련은 BodySep(aration) 에서 만들고
//어차피 특정 본 관리는 LateUpdate에서 해야하니까
//Idle State에서 관리해주기.
//(그외 State에서는 상하체 분리 기능이 들어갈 필요가 없음)


//카메라는 CamCtrl에서 관리하는데
//Zoom 카메라는 ㄹㅇ Zoom만 설정.
//Main Cam은 플레이어 따라갈 친구.
//시네머신쓰면 쉐이크 못넣으니까 땜핑 잘넣어보기


//결국 최종 사이클 관리는 Player에서해주고
//실질적인 함수 실행은 각 State에서
//함수 구현은 각 스크립트에서


//인벤토리는 여기서 관리하면 될거같고
//대신 무기는 어캐할지 좀 고민해봐야할듯
//Attack쪽에서 관리할지 어떻게 할지,,,,

[Serializable]
public struct PlayerStat
{
    public float maxHp;
    public float curHp;



    [Space(7.5f)]
    public float maxStamina;
    public float curStamina;
    public float staminaRecoverSpd;

    [Space(7.5f)]
    public float walkSpd;
    public float runSpd;

    [Space(7.5f)]
    public float sensitivity; // 민감도 추가

    [Space(7.5f)]
    public EnvasionStat envasionStat;

    //public float evasionDist;	//회피 거리
    //public float evasionCost;	//회피 스테미너 사용량

    [Space(7.5f)]
    public float upperHomingSpd; //상체 회전 속도
    public float legHomingSpd; //하체 회전 속도
    public float legHomingTime; //하체 회전 시간


}

public struct EnvasionStat
{
    public float evasionDist;   //회피 거리
    public float evasionCost;   //회피 스테미너 사용량
}

public class Player : MonoBehaviour
{
    //기능 구현 할 때는 접근지정자 크게 신경 안쓰고 작업함.
    //차후 기능 작업 끝나고 나면 추가적으로 정리 예정!!
    //불편해도 양해바람니다!!! 스마미센!!
    [Header("Health UI")]
    public RectTransform healthBar; // HP 바의 RectTransform
    public TextMeshProUGUI currentHealthText;
    public TextMeshProUGUI maxHealthText;
    private float originalAnchorMaxX; //HP 바의 원래 anchorMax x 값
    private float displayedHp;
    public float recoverySpeed = 5f;

    [Header("Direction Visualization")]
    public LineRenderer lookDirectionRenderer;
    public LineRenderer hammerDirectionRenderer;
    private Vector3 hammerDirection;
    private float hammerFollowSpeed = 5f; // 망치가 따라가는 속도
    private Vector3 previousLookDirection;
    private float additionalAngle = 10f; // HammerDirection이 LookDirection보다 추가적으로 이동하는 각도
    private float currentHammerAngle; // 현재 HammerDirection의 각도
    private float previousYRotation; // 클래스 멤버 변수로 추가

    public PlayerStat stat;
    public Vector3 lastMousePosition;
    public bool isRotationEnabled = true;
    [Header("Inventory")]
    public Inventory inventory; // 인벤토리 참조 추가

    [Header("State Machine")]
    public PlayerFSM fsm; //정말 큰 애니메이션 기준 

    //State 들은 Player_AAAA Player_BBBB 이런식으로 클래스 이름이 되어 있음니당.  

    [Space(10f)]
    [Header("Default Comps")]
    public Transform meshTr;
    public Animator animCtrl;
    public Rigidbody rd;


    [Space(10f)]
    [Header("Action Table")]
    //얘들은 그냥 작동하는 함수 Player에 다 놔두면 정신 없어서
    //따로 빼 놓은 친구들
    public PlayerMoveFunc move;
    public PlayerAtk atk;
    public PlayerAim aim;

    [Space(10f)]
    [Header("Cam Controller")]
    public CamCtrl camCtrl;


    [Space(10f)]
    [Header("Anim Bones")]
    public Transform headBoneTr;
    public Transform spineBoneTr;
    public Transform hipBoneTr;
    public RewiredPlayer rewiredPlayer;
    public bool isAttacking = false; // 공격 상태 플래그
                                     //public eGizmoDir preMoveDir;

    //[Tooltip("Temp Test")]
    //[Space(10f)]
    //public Inventory inven;

    public float chargeStartTime; // Charge 시작 시간
    private SaveDataManager saveDataManager;
    private void Awake()
    {
        if (!fsm)
        { fsm = GetComponent<PlayerFSM>(); }


        if (!fsm)
        {
            gameObject.AddComponent<PlayerFSM>();
        }

        if (!rd)
        {
            rd = GetComponent<Rigidbody>();
        }

        rewiredPlayer = ReInput.players.GetPlayer(0); // '0'은 첫 번째 플레이어의 ID
        saveDataManager = FindObjectOfType<SaveDataManager>();

    }
    // Start is called before the first frame update
    void Start()
    {
        if (ES3.KeyExists("playerStat"))
        {
            //Debug.Log("playerStat is Exist");
            //stat = ES3.Load<PlayerStat>("playerStat");
        }
        originalAnchorMaxX = healthBar.anchorMax.x;
        displayedHp = stat.curHp; // 초기화
        UpdateHealthBar();
        lookDirectionRenderer.startColor = Color.red;
        lookDirectionRenderer.endColor = Color.red;
        lookDirectionRenderer.startWidth = 0.05f;
        lookDirectionRenderer.endWidth = 0.05f;

        hammerDirectionRenderer.startColor = Color.green;
        hammerDirectionRenderer.endColor = Color.green;
        hammerDirectionRenderer.startWidth = 0.05f;
        hammerDirectionRenderer.endWidth = 0.05f;

        hammerDirection = transform.forward;

        previousLookDirection = transform.forward;
        currentHammerAngle = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            isRotationEnabled = !isRotationEnabled;
        }
        UpdateHealthBar();

        Vector3 currentLookDirection = transform.forward;
        float currentYRotation = transform.eulerAngles.y;
        float yRotationDifference = Mathf.DeltaAngle(previousYRotation, currentYRotation);

        // 회전 방향을 판단합니다.
        if (yRotationDifference > 0) // 시계 방향 회전
        {
            Debug.Log("시계방향");
        }
        else if (yRotationDifference < 0) // 반시계 방향 회전
        {
            Debug.Log("반시계방향");
        }

        // HammerDirection과 LookDirection 사이의 현재 각도 차이를 계산합니다.
        float angleBetweenDirections = Vector3.Angle(hammerDirection, currentLookDirection);

        // HammerDirection이 LookDirection과의 각도 차이가 일정 범위 내에 있을 때만 추가 각도를 적용합니다.
        float targetAngle;
        if (angleBetweenDirections <= Mathf.Abs(additionalAngle))
        {
            targetAngle = currentYRotation + (yRotationDifference > 0 ? additionalAngle : -additionalAngle);
        }
        else
        {
            targetAngle = currentYRotation;
        }

        Vector3 targetDirection = Quaternion.Euler(0, targetAngle, 0) * Vector3.forward;
        hammerDirection = Vector3.Lerp(hammerDirection, targetDirection, hammerFollowSpeed * Time.deltaTime);



        // LookDirection 라인 업데이트
        lookDirectionRenderer.SetPosition(0, transform.position);
        lookDirectionRenderer.SetPosition(1, transform.position + currentLookDirection * 5f);

        // HammerDirection 라인 업데이트
        hammerDirectionRenderer.SetPosition(0, transform.position);
        hammerDirectionRenderer.SetPosition(1, transform.position + hammerDirection * 5f);

        // 이전 Y축 회전값을 현재 Y축 회전값으로 업데이트
        previousYRotation = currentYRotation;
    }


    private void LateUpdate()
    {


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

    public void SaveData(int fileIndex)
    {
        if (saveDataManager != null)
        {

            saveDataManager.SavePlayerData(stat, fileIndex);
        }
    }

    public void OverwriteSaveData(int fileIndex)
    {
        // 먼저 기존 파일을 삭제합니다.
        if (saveDataManager != null)
        {
            saveDataManager.DeleteSaveFile(fileIndex);
        }

        // 이후 새로운 데이터로 세이브 파일을 생성합니다.
        SaveData(fileIndex);
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        //Ray rayF = new Ray(transform.position, transform.forward * 10f);
        //Gizmos.DrawRay(rayF);
        Gizmos.DrawRay(transform.position, transform.forward * 50f);

        Gizmos.color = Color.red;
        //Ray rayR = new Ray(transform.position, transform.right * 10f);
        Gizmos.DrawRay(transform.position, transform.right * 50f);



    }
    public Rewired.Player GetRewiredPlayer()
    {
        return rewiredPlayer;
    }
    public void RecoverHp(float amount)
    {
        stat.curHp = Mathf.Min(stat.curHp + amount, stat.maxHp);
        UpdateHealthBar();
    }

    public void TakeDamage(float damage)
    {
        Debug.Log("take Damage");
        if (stat.curHp > 0)
        {
            stat.curHp -= damage;
            UpdateHealthBar();
        }

        if (stat.curHp <= 0)
        {

        }

    }

    public void StartCharge()
    {
        chargeStartTime = Time.time;
        animCtrl.SetBool("IsCharge", true); // Charge 애니메이션 활성화
    }

    public void UpdateCharge()
    {
        atk.curCharging = Time.time - chargeStartTime;
    }

    public void PerformAttack()
    {
        animCtrl.SetBool("IsCharge", false); // Charge 애니메이션 비활성화
        animCtrl.SetTrigger("tAtk"); // Attack 애니메이션 활성화
        //atk.Attack(); // 공격 실행
        //atk.curCharging = 0f; // Charge 시간 초기화
    }


    public void StartAttack() // 공격 시작 시 호출
    {
        isAttacking = true;
    }

    public void EndAttack() // 공격 종료 시 호출
    {
        isAttacking = false;
    }

    void UpdateHealthUI()
    {
        currentHealthText.text = stat.curHp.ToString(); // 현재 체력을 문자열로 변환하여 UI에 표시
        maxHealthText.text = stat.maxHp.ToString();     // 최대 체력을 문자열로 변환하여 UI에 표시
    }

    void UpdateHealthBar()
    {
        if (healthBar != null)
        {
            // 서서히 displayedHp를 curHp에 가까워지게 함
            displayedHp = Mathf.Lerp(displayedHp, stat.curHp, Time.deltaTime * recoverySpeed);

            float healthRatio = displayedHp / stat.maxHp; // 화면에 표시되는 체력 비율 계산
            healthBar.anchorMax = new Vector2(originalAnchorMaxX * healthRatio, healthBar.anchorMax.y); // 너비 업데이트
            UpdateHealthUI();
        }
    }
}