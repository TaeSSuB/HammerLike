using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class HPObject : MonoBehaviour
{
    // 오브젝트 고유의 최대 HP값
    protected float defaultMaxHp;

    // 아이템, 버프 등에 의해서 변동되는 최대 HP값
    protected abstract float extraHp
    {
        get;
    }

    // 아이템과 버프효과를 포함한 최대 HP값
    protected float maxHp
    {
        get { return defaultMaxHp + extraHp; }
    }

    // 현재 hp
    protected float hp;

    /// <summary>
    /// HPObejct가 공격받음
    /// </summary>
    /// <param name="direction"> 공격자 기준의 공격 방향([자신의 위치] - [공격자 위치]) </param>
    /// <param name="damage"> 데미지 </param>
    /// <param name="force"> 넉백 세기 </param>
    /// <param name="damageType"> 데미지 타입 </param>
    /// <param name="specialDamage"> 특수한 공격의 데미지(상태이상의 경우 지속시간에 영향을 줄 수 있음) </param>
    public abstract void OnDamaged(Vector3 direction, float damage, float force, DamageType damageType, float specialDamage);
}
