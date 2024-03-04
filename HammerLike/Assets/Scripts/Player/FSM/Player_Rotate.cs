using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Rotate : cState
{

	public Player player;

	public Player_Rotate(Player _player)
	{
		player = _player;
	}

	public override void EnterState()
	{
		base.EnterState();

        player.animCtrl.speed = 1f;
    }
	public override void UpdateState()
	{
		base.UpdateState();
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

