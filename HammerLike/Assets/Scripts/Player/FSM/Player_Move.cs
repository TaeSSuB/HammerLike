using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Move : cState
{

	public Player player;

	public Player_Move(Player _player)
	{
		player = _player;
	}

	public override void EnterState()
	{
		base.EnterState();

		Debug.Log("Enter Player Move");

		player.animCtrl.SetLayerWeight(1, 1f);
		player.animCtrl.SetTrigger("tWalk");
	}
	public override void UpdateState()
	{
		base.UpdateState();

		player.aim.Aiming();

		if (!player.move.Move(player.stat.walkSpd))
		{
			player.fsm.SetNextState("Player_Idle");
		}

		if(Input.GetKeyDown(KeyCode.Space))
		{
			player.fsm.SetNextState("Player_Envasion");
		}

        if (Input.GetMouseButtonDown(0))
        {
            player.animCtrl.SetTrigger("tAtk");
			player.animCtrl.SetBool("bAttack", true);
            player.atk.Attack();
        }

        var temp = player.aim.rayResultPoint;
		temp.y = player.transform.position.y;
		player.transform.LookAt(temp);

		Vector3 relativeDir = Quaternion.Euler(-player.transform.rotation.eulerAngles) * player.move.lastMoveDir;
		player.animCtrl.SetFloat("MoveX", relativeDir.x);
		player.animCtrl.SetFloat("MoveZ", relativeDir.z);
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

