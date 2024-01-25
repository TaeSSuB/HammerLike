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

        var targetPosition = player.aim.rayResultPoint;
        targetPosition.y = player.transform.position.y;

        if (player.isAttacking)
        {
            // 공격 중일 때만 민감도 적용
            float rotationSpeed = (player.stat.sensitivity > 0) ? (1f / player.stat.sensitivity) * Time.deltaTime : float.MaxValue;
            Quaternion targetRotation = Quaternion.LookRotation(targetPosition - player.transform.position);
            player.transform.rotation = Quaternion.Slerp(player.transform.rotation, targetRotation, rotationSpeed);
        }
        else
        {
            // 공격 중이 아닐 때는 즉시 회전
            player.transform.LookAt(targetPosition);
        }

        if (player.move.Move(player.stat.walkSpd, player.rewiredPlayer))
        {
            player.fsm.SetNextState("Player_Move");
        }

        if (Input.GetMouseButtonDown(0))
        {
            player.animCtrl.SetTrigger("tAtk");
            player.atk.Attack();
        }

        if (Input.GetMouseButton(1))
        {
            player.animCtrl.SetBool("tCharge", true);
            player.atk.curCharging += Time.deltaTime;
            Debug.Log("우측키 누름");
        }

        if (Input.GetMouseButtonUp(1) && player.atk.curCharging >= 1)
        {
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
