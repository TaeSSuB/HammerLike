using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.AI;

[System.Serializable]
public abstract class cState
{
	protected cState()
	{ }
	

	

	public virtual void EnterState()
	{

	}

	public virtual void UpdateState()
	{ }

	public virtual void FixedUpdateState()
	{ }

	public virtual void LateUpdateState()
	{ }

	public virtual void ExitState()
	{ }
}
