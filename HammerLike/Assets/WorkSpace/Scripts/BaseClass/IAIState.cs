public interface IAIState
{
    void OnEnter();
    void OnUpdate();
    void OnExit();
}

public enum AIStateType
{
    IDLE,
    CHASE,
    WANDER,
    HIT,
    ATTACK
}