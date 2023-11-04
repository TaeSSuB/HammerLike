﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Idle : cState
{

	public Player player;

	public Player_Idle(Player _player)
	{
		player = _player;
	}

	public override void EnterState()
	{
		base.EnterState();

		Debug.Log("Enter Player Idle");

		player.animCtrl.SetLayerWeight(1, 0f);

		player.animCtrl.SetTrigger("tIdle");
	}
	public override void UpdateState()
	{
		base.UpdateState();

		player.aim.Aiming();

		var temp = player.aim.rayResultPoint;
		temp.y = player.transform.position.y;
		player.transform.LookAt(temp);

		//움직임 없고 몸 회전할때 발 회전 애니메이션 재생하는거 따로 State로 뺴셔도 되고
		//아니면 여기서 그냥 LateUpdate에서 잔발 애니메이션만 재생하시면 될듯함니당

		if (player.move.Move(player.stat.walkSpd))
		{
			player.fsm.SetNextState("Player_Move");
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
