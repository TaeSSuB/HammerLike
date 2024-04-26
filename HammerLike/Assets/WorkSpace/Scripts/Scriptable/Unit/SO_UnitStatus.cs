using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UnitStatus", menuName = "B_ScriptableObjects/Unit/UnitStatus", order = 1)]
public class SO_UnitStatus : ScriptableObject
{
    public Sprite unitSprite;

    public int currentHP = 100;
    public int maxHP = 100;

    public int restoreHP = 0;
    public float restoreHPCooltime = 0f;
    public float currentRestoreHPCooltime = 0f;

    public float moveSpeed = 5f;

    public int atkDamage = 10;
    public int atkDamageOrigin = 10;
    public float atkRange = 1f;
    public float atkSpeed = 1f;
    public float maxAttackCooltime = 1f;
    public float currentAttackCooltime = 0f;

    public float mass = 1f;

    public AnimationCurve knockbackCurve;
    public AnimationCurve partsBreakForceCurve;
}
