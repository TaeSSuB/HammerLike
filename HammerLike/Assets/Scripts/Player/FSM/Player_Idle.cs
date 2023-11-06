using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

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
		///
		/// 우클릭 시 Charge 공격 curCharging 일 때 
		///
		if(Input.GetMouseButtonDown(0))
		{
			player.animCtrl.SetTrigger("tAtk");
		}

		///
		/// TODO) 이동중일 때 Charge에 대한 걸 안만들어 놨음
		///
        if (Input.GetMouseButton(1))
        {
			player.animCtrl.SetBool("tCharge", true);

            player.atk.curCharging += Time.deltaTime;
            Debug.Log("우측키 누름");
        }
        if (Input.GetMouseButtonUp(1) && player.atk.curCharging>=1)
        {
			//player.animCtrl.SetTrigger("tAtk");
			player.animCtrl.SetBool("tCharge", false);
			player.animCtrl.SetFloat("fAtkVal", 0);
			player.atk.curCharging = 0;
            Debug.Log("우측키 땜");
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            player.fsm.SetNextState("Player_Envasion");
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
