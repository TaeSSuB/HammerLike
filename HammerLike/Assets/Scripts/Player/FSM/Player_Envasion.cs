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

        // 이동 중이면 마지막 이동 방향을 사용, 아니면 마우스 커서 방향을 사용
        Vector3 targetDirection;
        if (player.move.Move(player.stat.walkSpd)) // 현재 이동하고 있는지 아닌지
        {
            targetDirection = player.move.lastMoveDir.normalized;
        }
        else
        {
            Vector3 directionToMouse = player.lastMousePosition - player.transform.position;
            targetDirection = directionToMouse.normalized;
        }

        if (targetDirection != Vector3.zero)
        {
            player.transform.forward = targetDirection;
        }

        player.animCtrl.SetTrigger("tEnvasion");
        player.animCtrl.SetInteger("iEnvasion", (int)tempType);

        animName = Funcs.GetEnumName<eEnvasionType>((int)tempType);

        rollTimer = rollDuration;
    }




    public override void UpdateState()
    {
        base.UpdateState();
        if (rollTimer > 0)
        {
            // 플레이어가 마우스 방향으로 구르도록 방향 설정
            Vector3 rollDirection = player.transform.forward.normalized;

            Debug.Log($"Rolling in direction: {rollDirection}");

            bool isMovingVertically = Mathf.Abs(rollDirection.z) > Mathf.Abs(rollDirection.x);
            bool isMovingDiagonally = Mathf.Abs(rollDirection.x) > 0 && Mathf.Abs(rollDirection.z) > 0;

            float adjustedSpeed = rollSpeed;
            if (isMovingVertically)
            {
                adjustedSpeed *= 1.4f;
            }
            else if (isMovingDiagonally)
            {
                adjustedSpeed *= 1.4f;
            }

            player.transform.Translate(rollDirection * adjustedSpeed * Time.deltaTime, Space.World);

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
