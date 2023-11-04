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
	}

	public override void UpdateState()
	{
		base.UpdateState();

		if (Funcs.IsAnimationAlmostFinish(player.animCtrl, animName))
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
