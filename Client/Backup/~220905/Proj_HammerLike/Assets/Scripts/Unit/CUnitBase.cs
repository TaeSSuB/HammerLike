using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//public enum State { Idle, Move, Chase, Attack, Dead };
public enum State { Idle, Move, Attack, Dead };

public class CUnitBase : MonoBehaviour
{
    [Header("Unit Base")]
    public State state = State.Idle;
    public int maxHp = 1;
    public int currentHp = 1;
    public int atkDmg = 1;
    public float moveSpeed, atkSpeed, atkRange, moveDelay;
    public float damageCoeffcient = 1f;
    public bool isDodge = false;
    //public Vector2 moveDir;

    public void SetHP()
    {
        currentHp = maxHp;
    }

    public virtual void UnitDamaged(int damage)
    {
        // 완전 회피
        if (!isDodge)
        {
            currentHp -= (int)(damage * damageCoeffcient);
        }

        //// 데미지 감소
        //currentHp -= (int)(damage * damageCoeffcient);
    }
}
