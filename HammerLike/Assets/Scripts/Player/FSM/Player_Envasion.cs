using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using Johnson;

public enum eEnvasionType
{ 
	Roll,
	Dash,
	End
}

public class Player_Envasion : cState
{

	public Player player;

	public eEnvasionType tempType; //무기 타입에 따라 달라져야 하니까 
								   //이거 대신 player에서 장착중인 무기 타입 받아오셔도 될듯

	public string animName;

    public float rollSpeed = 5f; // 매 초 구르기 속도
    public float rollDuration = 1f; // 구르기 지속 시간
    private float rollTimer; // 구르기 타이머

    public Player_Envasion(Player _player)
	{
		player = _player;
	}

	public override void EnterState()
	{
		base.EnterState();

		Debug.Log("Enter Evasion");

		player.animCtrl.SetLayerWeight(1, 0f);


		//var temp = player.aim.rayResultPoint;
		//temp.y = player.transform.position.y;
		player.transform.forward = player.move.lastMoveDir;
		//이거 안될텐데 여튼 마지막 이동 방향쪽으로 바라보게 해주심 될 듯


		player.animCtrl.SetTrigger("tEnvasion");
		player.animCtrl.SetInteger("iEnvasion", (int)tempType);//Enum 형태 등으로 회피 상태 정해두고
															   //넘기시면 될 듯!

		animName = Funcs.GetEnumName<eEnvasionType>((int)tempType);

        rollTimer = rollDuration;
    }

	public override void UpdateState()
	{
        base.UpdateState();

        if (rollTimer > 0)
        {
           
            Vector3 worldDirection = player.move.lastMoveDir.normalized;

            
            bool isMovingVertically = Mathf.Abs(worldDirection.z) > Mathf.Abs(worldDirection.x);

            
            bool isMovingDiagonally = Mathf.Abs(worldDirection.x) > 0 && Mathf.Abs(worldDirection.z) > 0;

            
            float adjustedSpeed = rollSpeed;
            if (isMovingVertically)
            {
                // 상하 이동에 대해 속도 1.4배로 조정
                adjustedSpeed *= 1.4f;
            }
            else if (isMovingDiagonally)
            {
                // 대각선 이동 대해 조정 (상하 이동과 동일하게 1.4배로 적용) /// 근데 아마 좌우는 놔두고 상하만 1.4 해야될듯 ?
                adjustedSpeed *= 1.4f;
            }

           
            player.transform.Translate(worldDirection * adjustedSpeed * Time.deltaTime, Space.World);

            
            rollTimer -= Time.deltaTime;
        }

        if (Funcs.IsAnimationAlmostFinish(player.animCtrl, animName) || rollTimer <= 0)
        {
            player.fsm.SetNextState("Player_Idle");
        }
    }


	public override void FixedUpdateState()
	{
		base.FixedUpdateState();
	}

	public override void LateUpdateState()
	{
		base.LateUpdateState();
	}

	public override void ExitState()
	{
		base.ExitState();
	}
}
