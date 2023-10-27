using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Johnson;


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
	public EnvasionStat envasionStat;

	//public float evasionDist;	//회피 거리
	//public float evasionCost;	//회피 스테미너 사용량

	[Space(7.5f)]
	public float upperHomingSpd; //상체 회전 속도
	public float legHomingSpd; //하체 회전 속도
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

	public PlayerStat stat;


    [Header("State Machine")]
    public PlayerFSM fsm;

	[Space(10f)]
	[Header("Default Comps")]
	public Transform meshTr;
    public Animator animCtrl;
	public Rigidbody rd;


	[Space(10f)]
	[Header("Action Table")]
	public PlayerMove move;
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

	public float resetTime;
	public Coroutine legResetCor = null;

	//[Tooltip("Temp Test")]
	//[Space(10f)]
	//public Inventory inven;

	//public Weapon curWeapon;

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

		
		

	}
	// Start is called before the first frame update
	void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
		move.Move(stat.walkSpd);
		aim.Aiming();


		var temp = aim.rayResultPoint;
		temp.y = transform.position.y;
		//transform.forward  = temp;
		transform.LookAt(temp);

	}

	private void LateUpdate()
	{

		//이거보다 회피, 이동기시 그대로

		//마우스 위치로 플레이어의 방향 자체가 달라지고
		//(대신 하체가 회전하는 잔발 애니메이션이 있어야겠지)
		//다리는 바라보는 방향이랑 움직이는 방향이랑 상대적인 방향을 체크해서
		//다리 애니메이션을 8방향이던 재생을 해야 할거같은디...?
		//플레이어의 회전값을 상체/하체중 하나를 기준이 할 수 있는게 있어야
		//조작감이 안 어색할거 같음.
		//상체든 하체든.



		float angle = Mathf.Acos(Vector3.Dot(move.moveDir, aim.lookDir)) * Mathf.Rad2Deg;

		if (move.isRest)
		{//멈춰있는 경우
			Debug.Log("멈춰있음");
			//일정 시간 지나면 하체가 상체 바라보도록

			//상체만 움직이다가 90도 이상 차이나면 하체가 회전.
			if (angle >= 90f)
			{

			}
		}
		else
		{//이동중인 경우
			Debug.Log("움직이는 중");
			//이동방항에 맞춰 하체 회전
			Funcs.LookAtSpecificBone(hipBoneTr, eGizmoDir.Foward, move.moveDir, Vector3.zero);
			Funcs.LookAtSpecificBone(spineBoneTr, eGizmoDir.Foward, aim.lookDir, Vector3.zero);
			if (legResetCor != null)
			{
				legResetCor = null;
			}

			//마우스 이동값이 없으면 상체가 하체방향으로 가기
			
		}
	
		//Funcs.LookAtSpecificBone(hipBoneTr, eGizmoDir.Foward, move.moveDir, Vector3.zero);
		//Funcs.LookAtSpecificBone(spineBoneTr, eGizmoDir.Foward, aim.lookDir, Vector3.zero);
		//다 떠나서 상체 방향이랑 하체 방향의 차이가 90도 이상 나는 경우
		//상체가 하체 방향에 맞게 회전하기
		if (angle >= 90f)
		{
			Debug.Log("각도 90도 이상 차이");
		}
	}

	public IEnumerator LegRotCorou()
	{
		yield return new WaitForSeconds(resetTime);

		move.moveDir = aim.lookDir;
		//Funcs.LookAtSpecificBone(hipBoneTr, eGizmoDir.Foward, move.moveDir, Vector3.zero);
		legResetCor = null;
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
}
