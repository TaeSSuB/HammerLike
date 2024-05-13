using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public interface IBossAIState
{
    void OnEnter();
    void OnUpdate();
    void OnExit();
}

public enum BossAIStateType
{
    IDLE,
    CHASE,
    HIT,
    ATTACK,
    SWING,
    THROW,
    PULL,
    ROAR,
    DEAD
}
