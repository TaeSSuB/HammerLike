using UnityEngine;
using System.Collections.Generic;

public class AIStateManager : MonoBehaviour
{
    private IAIState currentState;
    private Dictionary<AIStateType, IAIState> states = new Dictionary<AIStateType, IAIState>();
    private B_UnitBase unitBase;

    void Awake()
    {
        unitBase = GetComponent<B_UnitBase>();

        // �� ���� �ʱ�ȭ �� ������ ����
        //states[AIStateType.IDLE] = new IdleState(unitBase);
        //states[AIStateType.CHASE] = new ChaseState(unitBase);
        states[AIStateType.WANDER] = new WanderState(unitBase);
        //states[AIStateType.HIT] = new HitState(unitBase);
        //states[AIStateType.ATTACK] = new AttackState(unitBase);

        SetState(AIStateType.IDLE); // �⺻ ���� ����
    }

    public void SetState(AIStateType newState)
    {
        if (currentState != null)
        {
            currentState.OnExit();
        }

        currentState = states[newState];
        currentState.OnEnter();
    }

    void Update()
    {
        currentState?.OnUpdate();
    }
}
