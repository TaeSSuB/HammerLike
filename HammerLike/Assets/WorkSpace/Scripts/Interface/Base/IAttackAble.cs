using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAttackAble
{
    void Attack();
    void ChargeAttack();
    void MaximumChargeAttack();

    void CancelAttack();

    void OnStartAttack();
    void OnEndAttack();
}
