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

    //public Weapon curWeapon;
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
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            isRotationEnabled = !isRotationEnabled;
        }
        UpdateHealthBar();

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

            saveDataManager.SavePlayerData(stat,fileIndex);
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

    public void TakeDamage(float damage)
    {
        Debug.Log("take Damage");
        if(stat.curHp > 0)
        {
            stat.curHp -= damage;
            UpdateHealthBar();
        }

        if (stat.curHp <= 0)
        {
            
        }

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
