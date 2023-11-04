using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFSM : StateCtrl
{

	Player player;

	public override void InitState()
	{
		//throw new System.NotImplementedException();
		curState = AddState(new Player_Idle(player));
		AddState(new Player_Rotate(player));
		AddState(new Player_Move(player));
		AddState(new Player_Envasion(player));
		AddState(new Player_Death(player));
	}

	public override void Release()
	{
		//throw new System.NotImplementedException();
	}

	protected override void Awake()
	{
		base.Awake();

		player = GetComponent<Player>();

	}

	protected override void Start()
	{
		base.Start();
	}

	protected override void Update()
	{
		base.Update();
	}

	protected override void FixedUpdate()
	{
		base.FixedUpdate();
	}

	protected override void LateUpdate()
	{
		base.LateUpdate();
	}
	
}
