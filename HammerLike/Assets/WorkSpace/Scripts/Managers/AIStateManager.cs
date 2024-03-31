using UnityEngine;
using System.Collections.Generic;
using System.ComponentModel;

public class AIStateManager : MonoBehaviour
{
    // read only AIStateType currentStateType
    // only read on Inspector
    [SerializeField] private AIStateType currentStateType = AIStateType.IDLE;
    private IAIState currentState;
    private Dictionary<AIStateType, IAIState> states = new Dictionary<AIStateType, IAIState>();
    private B_UnitBase unitBase;

    public AIStateType CurrentStateType { get { return currentStateType; } }

    void Awake()
    {
        unitBase = GetComponent<B_UnitBase>();

        // 각 상태 초기화 및 디렉토리 저장
        states[AIStateType.IDLE] = new IdleState(unitBase);
        states[AIStateType.CHASE] = new ChaseState(unitBase);
        states[AIStateType.WANDER] = new WanderState(unitBase);
        states[AIStateType.HIT] = new HitState(unitBase);
        states[AIStateType.ATTACK] = new AttackState(unitBase);

        SetState(AIStateType.IDLE); // 기본 상태 설정
    }

    public void SetState(AIStateType newState)
    {
        if (currentState != null)
        {
            currentState.OnExit();
        }

        currentStateType = newState;
        currentState = states[currentStateType];

        // 상태 전환시 초기화
        currentState.OnEnter();
    }

    void Update()
    {
        // 상태 업데이트
        currentState?.OnUpdate();
    }
}
