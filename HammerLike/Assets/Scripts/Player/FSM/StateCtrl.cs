using System.Collections;
using System.Collections.Generic;

using System.Linq;

using UnityEngine;


public abstract class StateCtrl : MonoBehaviour
{
    //추후에 몬스터나 다른 오브젝트에도 쓰실꺼면
    //Player말고 

    //private cState[] states;
    protected List<KeyValuePair<string, cState>> states;

    protected cState preState = null;
    public cState GetPreState
    {
        get{ return preState; }
    }

    protected cState curState;
    public cState GetCurState
    {
        get { return curState; }
    }

    protected cState nextState= null;
    public cState GetNextState
	{
        get { return nextState; }
    }


    public cState GetState(int index)
    {
        if (index >= states.Count || states[index].Value == null)
        {
            Debug.Log($"{gameObject.name} does not have {index}th state.");
            return null;
        }

        return states[index].Value;
    }

    public cState GetState(string stateName)
    {
        var result = states.Find(x => x.Key == stateName);

        if (result.Equals(default(KeyValuePair<string, cState>)))
        {
            Debug.Log($"{gameObject.name} does not have \"{stateName}\" state.");
            return null;
        }

        return result.Value;
    }

    public bool CompareCurState(cState state)
    {
        return curState == state;
    }

    public bool CompareCurState(string stateName)
	{
        var result = GetState(stateName);

        if (result != null && curState == result)
        {
            return true;
        }

        return false;
        //var result = states.Find(x => x.Key == stateName);

        //if (result.Equals(default(KeyValuePair<string, cState>)))
        //{
        //    Debug.Log($"{gameObject.name} does not have \"{stateName}\" state.");
        //    return false;
        //}
        //else if(result.Value == curState)
        //{
        //    return true; 
        //}

        //return false;
    }

    public void SetNextState(cState state)
    {
        if (state == null)
        {
            return;
        }

        var result = states.Find(x => x.Value == state);
        if (result.Equals(default(KeyValuePair<string, cState>)))
        {
            Debug.Log($"{gameObject.name} does not have \"{state.GetType().Name}\" state.");
            return;
        }

        nextState = state;
    }

    public void SetNextState(int index)
    {
        nextState = GetState(index);
    }

    public void SetNextState(string stateName)
    {
        nextState = GetState(stateName);
    }


    public abstract void InitState();

    public abstract void Release();

	protected virtual void Awake()
	{
        states = new List<KeyValuePair<string, cState>>();

	}

    protected virtual void Start()
	{
        InitState();
	}

	protected virtual void Update()
	{
        if (nextState != null)
        {
            preState = curState;
            preState.ExitState();

            curState = nextState;
            nextState = null;
            curState.EnterState();
        }

        if (curState != null)
        { curState.UpdateState(); }
	}

	protected virtual void FixedUpdate()
	{
        if (curState != null)
        { curState.FixedUpdateState(); }
    }

	protected virtual void LateUpdate()
	{
        if (curState != null)
        { curState.LateUpdateState(); }
    }


	private  void OnEnable()
	{
		
	}

	private void OnDisable()
	{
		
	}

	private void OnDestroy()
	{
        preState = null;
        curState = null;
        nextState = null;


        //for (int i = 0; i < states.Count; ++i)
        //{
        //    states = new List<KeyValuePair<string, cState>>();
        //}

        //states = null;
	}



}
