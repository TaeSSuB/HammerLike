using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponCollider : MonoBehaviour
{
    public int CurrentAttackId { get; private set; }
    public bool hasProcessedAttack = false;

    public void ResetAttackProcessing()
    {
        hasProcessedAttack = false;
    }
    public void SetAttackId(int id)
    {
        CurrentAttackId = id;
    }
}
