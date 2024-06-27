using UnityEngine;

public class WanderState : IAIState
{
    private B_UnitBase unit;
    private float changeDirectionEverySeconds = 1f;
    private float lastDirectionChangeTime;
    private Vector3 currentDirection;

    public WanderState(B_UnitBase unit)
    {
        this.unit = unit;
    }

    public void OnEnter()
    {
        PickRandomDirection();
    }

    public void OnUpdate()
    {
        if (Time.time - lastDirectionChangeTime > changeDirectionEverySeconds)
        {
            PickRandomDirection();
        }
        unit.Move(currentDirection);
    }

    public void OnExit()
    {
        unit.Move(unit.transform.position);
    }

    private void PickRandomDirection()
    {
        currentDirection = new Vector3(Random.Range(-1f, 1f), 0f,Random.Range(-1f, 1f)).normalized;
        lastDirectionChangeTime = Time.time;
    }
}