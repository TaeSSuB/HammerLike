using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Temp. 스켈레톤 간수장 한정. 이후 범용 AI로 변경 필요.
/// </summary>
public class B_BossController : MonoBehaviour
{
    [SerializeField] private BossAIStateType currentStateType = BossAIStateType.IDLE;
    private IBossAIState currentState;

    private Dictionary<BossAIStateType, IBossAIState> states = new Dictionary<BossAIStateType, IBossAIState>();

    private B_Boss b_Boss;

    public BossAIStateType CurrentStateType { get { return currentStateType; } }
    
    private Dictionary<BossAIStateType, float> minimumStateTimes = new Dictionary<BossAIStateType, float>()
    {
        { BossAIStateType.IDLE, 2f },
        { BossAIStateType.CHASE, 10f },
        { BossAIStateType.SWING, 7f },
        { BossAIStateType.THROW, 4f },
        { BossAIStateType.PULL, 5f },
        { BossAIStateType.ROAR, 5f },
        { BossAIStateType.ATTACK, 4f } // 기본 공격도 최소 시간 설정 가능
    };
    private Dictionary<BossAIStateType, float> patternCooldowns = new Dictionary<BossAIStateType, float>()
    {
        { BossAIStateType.CHASE, 10f },
        { BossAIStateType.SWING, 20f },
        { BossAIStateType.THROW, 15f },
        // { BossAIStateType.PULL, 20f },
        { BossAIStateType.ROAR, 25f },
        { BossAIStateType.ATTACK, 5f}
    };    
    
    private Dictionary<BossAIStateType, float> lastPatternTimes = new Dictionary<BossAIStateType, float>();

    private float lastStateChangeTime = 0;
    
    void Start()
    {
        foreach (var pattern in patternCooldowns.Keys)
        {
            lastPatternTimes[pattern] = Time.time - patternCooldowns[pattern]; // 초기 쿨타임 만료 상태
        }

        b_Boss = GetComponent<B_Boss>();

        states[BossAIStateType.IDLE] = new IdleBossState(this.b_Boss);
        states[BossAIStateType.CHASE] = new ChasingBossState(this.b_Boss, 0);
        states[BossAIStateType.ATTACK] = new MeleeAttackBossState(this.b_Boss);
        states[BossAIStateType.SWING] = new SwingBossState(this.b_Boss, 1);
        states[BossAIStateType.THROW] = new ThrowBossState(this.b_Boss, 2);
        states[BossAIStateType.PULL] = new PullBossState(this.b_Boss, 3);
        states[BossAIStateType.ROAR] = new RoarBossState(this.b_Boss, 4);
        states[BossAIStateType.DEAD] = new DeadBossState(this.b_Boss);

        SetState(BossAIStateType.IDLE);
    }

    public void SetState(BossAIStateType newState)
    {
        if(newState == currentStateType)
        {
            return;
        }

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
        currentState?.OnUpdate();
    }

    public BossAIStateType CheckPattern()
    {
        if (states.ContainsKey(currentStateType) && currentState != null)
        {
            if (Time.time < lastStateChangeTime + minimumStateTimes[currentStateType])
            {
                return currentStateType; // 현재 상태의 최소 시간이 만료되지 않았다면 상태 유지
            }
        }

        List<BossAIStateType> availablePatterns = new List<BossAIStateType>();
        foreach (var pattern in patternCooldowns)
        {
            if (Time.time >= lastPatternTimes[pattern.Key] + pattern.Value)
            {
                availablePatterns.Add(pattern.Key);
            }
        }

        if (availablePatterns.Count == 0)
        {
            return BossAIStateType.ATTACK; // 모든 패턴이 쿨타임인 경우 기본 공격 실행
        }
        else
        {
            int idx = Random.Range(0, availablePatterns.Count);
            var selectedPattern = availablePatterns[idx];
            lastPatternTimes[selectedPattern] = Time.time; // 선택된 패턴의 마지막 실행 시간 업데이트
            lastStateChangeTime = Time.time; // 상태 변경 시간 업데이트
            return selectedPattern;
        }
    }
}

