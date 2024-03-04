using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Player_Death :  cState
{

	public Player player;

	public Player_Death(Player _player) 
	{
		player = _player;
	}

	public override void EnterState()
	{
	base.EnterState();
        player.animCtrl.speed = 1f;
        player.isAttacking = false;
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

