using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.AI;

[System.Serializable]
public abstract class cState
{
	//public cState(CObj objScript)
	//{
	//	if (!me)
	//	{
	//		me = objScript;
	//		fsm = me.fsm;
	//	}

	//}


	//[HideInInspector]
	//public CObj me;
	//[HideInInspector]
	//public cFSM fsm;


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
