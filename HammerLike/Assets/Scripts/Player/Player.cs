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

        aim.Aiming();
        move.Move(stat.walkSpd);




    }

    private void LateUpdate()
    {
        //이거보다 회피, 이동기시 그대로

        //** 마우스 방향대로 플레이어 자체가 회전 / 상체방향 = 플레이어 방향**
        //이동이 없는 경우
        //플레이어 자체가 회전하면 됨
        //2. 90도 이상 차이나면 하체가 상체 방향으로 회전
        //3. 걸을때는 플레이어의 상체 방향에 따른 하체 각도에 따라서
        //옆으로 걷는 애니메이션 재생하거나 하체 회전하고나서 걷는 애니메이션 재생하기

        //Debug.Log("플레이어 상하체 차이 각도 : " + angle);

        if (move.isRest)
        {//이동이 없는 경우
         //몸 방향은 마우스 방향으로
         //상체는 마우스 방향으로 바라보기
         //하체는 마지막 이동 방향으로 고정.

            var temp = aim.rayResultPoint;
            temp.y = transform.position.y;
            transform.LookAt(temp);

            Funcs.LookAtSpecificBone(hipBoneTr, eGizmoDir.Foward, move.lastMoveDir, Vector3.zero);
            Funcs.LookAtSpecificBone(spineBoneTr, eGizmoDir.Foward, aim.lookDir, Vector3.zero);
            Funcs.LookAtSpecificBone(headBoneTr, eGizmoDir.Foward, aim.lookDir, Vector3.zero); //이거 애니메이션 때문에 약간 오차가 발생함.
                                                                                               //추후 따로 보정 회전값을 넣어 주던지 계산을 해주던지 해야함.

            float angle = Mathf.Acos(Vector3.Dot(move.lastMoveDir, aim.lookDir)) * Mathf.Rad2Deg;

            if (move.restTime >= 5f | angle >= 90f)
            { //정지하고 일정 시간이 지나거나 상하체 각도가 90도 이상 차이나는 경우
              //하체가 상체 방향으로 돌기

                //하체 도는 중간에 움직이면 취소하고 움직일때의 규칙으로 변경하기

            }
        }
        else
        { //움직이고 있는 경우
          //몸 방향은 마우스 방향
          //상체는 그대로 마우스 방향
          //하체는
          //1. 0~80도 정도 까지는 앞으로 걷는 애니메이션 + 움직이는 방향으로
          //2. 80~100도는 옆으로 걷는 애니메이션 + 상체랑 같은 방향
          //3. 100~170도는 뒤로걷는 애니메이션 + 상체랑 같은 방향

            var temp = aim.rayResultPoint;
            temp.y = transform.position.y;
            transform.LookAt(temp);

            float angle = Mathf.Acos(Vector3.Dot(move.lastMoveDir, transform.forward)) * Mathf.Rad2Deg;

            //if (angle < 80)
            //{
            //	Debug.Log("80 미만");
            //}
            //else if (angle < 100)
            //{
            //	Debug.Log("80~100");
            //}
            //else if (angle < 170)
            //{
            //	Debug.Log("100~170");
            //}
            ////내가 원하는 그 느낌 맞음


            //좌우 체크하기
            //1. 포워드와 direction 외적해서 법선 구하기
            Vector3 crossVec = Vector3.Cross(move.lastMoveDir, transform.forward);

            //2. 나온 법선이랑 Up벡터 내적하기
            float dot = Vector3.Dot(crossVec, Vector3.up);

            //3. 나온값은 Cos@ 값임. 이게 음수면 오른쪽, 양수면 왼쪽
            // cos0 = 1 / cos90 = 0 / cos180 = -1
            //왜 그렇냐?
            //Cross는 결과값이 벡터(방향과 크기)로 나오는데
            //이것이 비교값(현재는 월드UP)과 같은 방향이면 (cos 0 = 1
            //다른 방향이면 (cos 180 = -1
            //임.
            //근데 Cross(외적)은 내적과 다르게 교환법칙이 성립안하는,
            //즉 순서에 따라 결과값이 다르므로! 이렇게 구별이 가능하다
            //왼쪽이면 90도 미만이니까(같은 up방향) 양수,
            //오른쪽이면 90도 초과이니까 음수.

            //if (dot > 0.1f )
            //{//오른쪽
            //	Debug.Log("Right");
            //}
            //else if (dot < -0.1f)
            //{//왼쪽
            //	Debug.Log("Left");
            //}
            //else
            //{ //가운데
            //	Debug.Log("Middle");
            //}

            float angleOffset = 10f;

            if (angle < 90f - angleOffset)
            {
                if (dot > 0.1f)
                {
                    Debug.Log("1");
                }
                else if (dot < -0.1f)
                {
                    Debug.Log("2");
                }
            }
            else if (angle < 90f + angleOffset)
            {
                if (dot > 0.1f)
                {
                    Debug.Log("6");
                }
                else if (dot < -0.1f)
                {
                    Debug.Log("3");
                }
            }
            else if (angle <= 180f)
            {
                if (dot > 0.1f)
                {
                    Debug.Log("5");
                }
                else if (dot < -0.1f)
                {
                    Debug.Log("4");
                }
            }

            //히히 됐다 이제 애니메이션 맞추기 하면 될 듯


            //if (dot > 0.1f && angle <= 80f)
            //{//오른쪽
            //    Debug.Log("1사 분면");
            //}
            //else if (dot < -0.1f && angle <= 80f)
            //{//왼쪽
            //    Debug.Log("2사");
            //}
            //else if (dot > 0.1f && angle >= 90f)
            //{
            //    Debug.Log("4사");
            //}
            //else if (dot < -0.1f && angle >= 90f)
            //{
            //    Debug.Log("3사");
            //}



        }




        //if (move.isRest)
        //{//멈춰 있는 경우
        //	Debug.Log("플레이어 정지 상태");
        //	//1. 플레이어 방향 = 상체 방향
        //	//하체 = 마지막 이동방향으로 있기

        //	//Funcs.LookAtSpecificBone(hipBoneTr, eGizmoDir.Foward, move.lastMoveDir, Vector3.zero);
        //	//Funcs.LookAtSpecificBone(headBoneTr, eGizmoDir.Foward, aim.lookDir, Vector3.zero);
        //	//Funcs.LookAtSpecificBone(spineBoneTr, eGizmoDir.Foward, aim.lookDir, Vector3.zero);

        //	if (angle < 90f)
        //	{ //2. 상하체 각도가 90도 미만인 경우 마지막 이동방향으로 하체 고정하기

        //		//if(animCtrl.GetBool())
        //		animCtrl.SetBool("bRot", false);

        //		Funcs.LookAtSpecificBone(hipBoneTr, eGizmoDir.Foward, move.lastMoveDir, Vector3.zero);
        //		Funcs.LookAtSpecificBone(headBoneTr, eGizmoDir.Foward, aim.lookDir, Vector3.zero);
        //		Funcs.LookAtSpecificBone(spineBoneTr, eGizmoDir.Foward, aim.lookDir, Vector3.zero);
        //	}
        //	else
        //	{ //2. 상하체 각도가 90도 이상인 경우 하체가 상체쪽으로 돌아가기
        //	  //2-1. 오른쪽에 있는지 왼쪽에 있는지 파악하기
        //	  ////1. 포워드와 direction 외적해서 법선 구하기
        //		Vector3 crossVec = Vector3.Cross(move.lastMoveDir, aim.lookDir);

        //		//2. 나온 법선이랑 Up벡터 내적하기
        //		float dot = Vector3.Dot(crossVec, Vector3.up);

        //		//3. 나온값은 Cos@ 값임. 이게 음수면 오른쪽, 양수면 왼쪽
        //		// cos0 = 1 / cos90 = 0 / cos180 = -1
        //		//왜 그렇냐?
        //		//Cross는 결과값이 벡터(방향과 크기)로 나오는데
        //		//이것이 비교값(현재는 월드UP)과 같은 방향이면 (cos 0 = 1
        //		//다른 방향이면 (cos 180 = -1
        //		//임.
        //		//근데 Cross(외적)은 내적과 다르게 교환법칙이 성립안하는,
        //		//즉 순서에 따라 결과값이 다르므로! 이렇게 구별이 가능하다
        //		//왼쪽이면 90도 미만이니까(같은 up방향) 양수,
        //		//오른쪽이면 90도 초과이니까 음수.

        //		if (dot > 0.1f)
        //		{//오른쪽
        //			Debug.Log("Right");

        //			animCtrl.SetLayerWeight(2, 1f);
        //			animCtrl.SetBool("bRot", true);
        //			animCtrl.SetInteger("iRotDir", Mathf.RoundToInt(dot));
        //			//hipBoneTr.forward = Vector3.Lerp(hipBoneTr.forward,)
        //		}
        //		else if (dot < -0.1f)
        //		{//왼쪽
        //			animCtrl.SetLayerWeight(2, 1f);
        //			animCtrl.SetBool("bRot", true);
        //			animCtrl.SetInteger("iRotDir", Mathf.RoundToInt(dot));
        //			Debug.Log("Left");
        //		}
        //		else
        //		{ //가운데
        //			animCtrl.SetBool("bRot", true);
        //			//animCtrl.SetInteger("iRotDir", UnityEngine.Random.Range(-1,1));

        //			Debug.Log("Middle");
        //			//animCtrl.SetLayerWeight((int)eHumanoidAvatarMask.Leg, offsetAngle);
        //			//return 0;
        //		}
        //	}

        //}
        //else
        //{ //움직이는 경우


        //}




        //if (move.isRest)
        //{//멈춰있는 경우
        //	Debug.Log("멈춰있음");
        //	//일정 시간 지나면 하체가 상체 바라보도록

        //	//상체만 움직이다가 90도 이상 차이나면 하체가 회전.
        //	if (angle >= 90f)
        //	{

        //	}
        //}
        //else
        //{//이동중인 경우
        //	Debug.Log("움직이는 중");
        //	//이동방항에 맞춰 하체 회전
        //	Funcs.LookAtSpecificBone(hipBoneTr, eGizmoDir.Foward, move.moveDir, Vector3.zero);
        //	Funcs.LookAtSpecificBone(spineBoneTr, eGizmoDir.Foward, aim.lookDir, Vector3.zero);
        //	if (legResetCor != null)
        //	{
        //		legResetCor = null;
        //	}

        //	//마우스 이동값이 없으면 상체가 하체방향으로 가기

        //}

        ////Funcs.LookAtSpecificBone(hipBoneTr, eGizmoDir.Foward, move.moveDir, Vector3.zero);
        ////Funcs.LookAtSpecificBone(spineBoneTr, eGizmoDir.Foward, aim.lookDir, Vector3.zero);
        ////다 떠나서 상체 방향이랑 하체 방향의 차이가 90도 이상 나는 경우
        ////상체가 하체 방향에 맞게 회전하기
        //if (angle >= 90f)
        //{
        //	Debug.Log("각도 90도 이상 차이");
        //}


        //public IEnumerator LegRotCorou()
        //{
        //	yield return new WaitForSeconds(resetTime);

        //	move.moveDir = aim.lookDir;
        //	//Funcs.LookAtSpecificBone(hipBoneTr, eGizmoDir.Foward, move.moveDir, Vector3.zero);
        //	legResetCor = null;
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
