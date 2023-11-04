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

    public PlayerStat stat;


    [Header("State Machine")]
    public PlayerFSM fsm; //정말 큰 애니메이션 기준 
    //public PlayerMoveFSM moveFsm; 

    [Space(10f)]
    [Header("Default Comps")]
    public Transform meshTr;
    public Animator animCtrl;
    public Rigidbody rd;


    [Space(10f)]
    [Header("Action Table")]
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

    //public eGizmoDir preMoveDir;
    
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

        //aim.Aiming();
        //move.Move(stat.walkSpd);




    }


 //   public void CalcDirectionUpperAndLeg()
 //   {
	//	float angle = Mathf.Acos(Vector3.Dot(hipBoneTr.forward, transform.forward)) * Mathf.Rad2Deg;

	//	//좌우 체크하기
	//	//1. 포워드와 direction 외적해서 법선 구하기
	//	Vector3 crossVec = Vector3.Cross(hipBoneTr.forward, transform.forward);

	//	//2. 나온 법선이랑 Up벡터 내적하기
	//	float dot = Vector3.Dot(crossVec, Vector3.up);

 //       if (dot > 0.1f)
 //       {
 //           Debug.Log("오른쪽");
 //           //오른쪽 하체 회전 애니메이션 재생 하기
 //       }
 //       else if (dot < -0.1f)
 //       {
 //           Debug.Log("왼쪽");
 //           //왼쪽 하체 회전 애니메이션 재생 하기
 //       }
 //       else
 //       {
 //           Debug.Log("거의 직선?");
 //       }
	//}

    private void LateUpdate()
    {
        //Debug.Log("플레이어 상하체 차이 각도 : " + angle);

        //Funcs.LookAtSpecificBone(hipBoneTr, eGizmoDir.Forward, move.lastMoveDir, Vector3.zero);
        //Funcs.LookAtSpecificBone(spineBoneTr, eGizmoDir.Forward, aim.lookDir, Vector3.zero);
        //Funcs.LookAtSpecificBone(headBoneTr, eGizmoDir.Forward, aim.lookDir, Vector3.zero); //이거 애니메이션 때문에 약간 오차가 발생함.
        //                                                                                   //추후 따로 보정 회전값을 넣어 주던지 계산을 해주던지 해야함.


        //231103 2158
        //기본적으로 그냥 움직임이 있든 없든 몸 자체가 조준 방향으로 돌아가는 방식이 맞는거 같음.
        //와이?!
        //움직임이 없을때, 회전 할 경우 하체는 안 움직이고 상체만 움직이다가 일정각도 이상이 되었을 때 돌아가기
        //이거랑, 현재 몸 각도에서 상대적 움직이는 방향을 체크해서 발 애니메이션을 재생하는거랑 모순이 발생함.
        //몸 각도에서 상대적 방향 체크 해서 발 애니메이션 재생하는거 자체가 몸이랑 하체 각도가 같을 경우를 상정하는거고

        //움직임이 없을때, 회전 할 경우 하체는 안 움직이고 상체만 움직이다가 일정각도 이상이 되었을 때 돌아가기
        //가 되려면 마지막 발 방향이 있어야 하는데
        //이제 이동 방향으로 하체 방향이 가는게 아니라 상대적 방향에 맞는 하체 애니메이션을 재생하는 방식임.




  //      if (move.isRest)
  //      {
  //          //이전 회전값과 현재 회전값 비교해서 

  //          if (transform.forward == aim.lookDir)
		//	{
  //              return;
		//	}
  //          Vector3 preForward = transform.forward;

  //          float angle = Mathf.Acos(Vector3.Dot(transform.forward, aim.lookDir)) * Mathf.Rad2Deg;
  //          Vector3 crossVec = Vector3.Cross(transform.forward, aim.lookDir);
  //          float dot = Vector3.Dot(crossVec, Vector3.up);


  //          string animName = string.Empty;
  //          if (dot > 0.1f)
  //          {
  //              animCtrl.SetLayerWeight(1, 1f);
  //              animName = "Rot_Right";
  //          }
  //          if (dot < -0.1f)
  //          {
  //              animCtrl.SetLayerWeight(1, 1f);
  //              animName = "Rot_Left";
  //          }
  //          else
  //          {
  //          }


  //          if (!animCtrl.GetCurrentAnimatorStateInfo(1).IsName(animName))
  //          {
  //              animCtrl.SetTrigger("tRotate");
  //              animCtrl.SetInteger("iRot", Mathf.RoundToInt(dot));
  //          }


  //          var temp = aim.rayResultPoint;
  //          temp.y = transform.position.y;
  //          transform.LookAt(temp);

  //      }
  //      else
  //      { //움직이고 있는 경우
  //        //몸 방향은 마우스 방향
  //        //상체는 그대로 마우스 방향
  //        //하체는
  //        //1. 0~80도 정도 까지는 앞으로 걷는 애니메이션 + 움직이는 방향으로
  //        //2. 80~100도는 옆으로 걷는 애니메이션 + 상체랑 같은 방향
  //        //3. 100~170도는 뒤로걷는 애니메이션 + 상체랑 같은 방향

  //          if (!animCtrl.GetCurrentAnimatorStateInfo(1).IsName("Walk"))
  //          {
  //              animCtrl.SetTrigger("tWalk");
  //          }

  //          var temp = aim.rayResultPoint;
  //          temp.y = transform.position.y;
  //          transform.LookAt(temp);

  //          //231101 이럴필요 없이 걍 블랜드 트리 쓰면 되노;
  //          Vector3 relativeDir = Quaternion.Euler(-transform.rotation.eulerAngles) * move.lastMoveDir;
  //          animCtrl.SetFloat("MoveX", relativeDir.x);
  //          animCtrl.SetFloat("MoveZ", relativeDir.z);
		//}


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
}
