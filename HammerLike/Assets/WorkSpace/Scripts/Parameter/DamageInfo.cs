using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 데미지 정보 클래스
/// </summary>
public class DamageInfo
{
    public int Damage { get; set; }
    public Vector3 DamageDir { get; set; }
    public float KnockBackPower { get; set; }
    
    public bool EnableKnockBack { get; set; } = true;
    public bool EnableSlowMotion { get; set; } = false;
    public bool IsForced { get; set; } = false;

    public DamageInfo(int damage, Vector3 damageDir, float knockBackPower)
    {
        Damage = damage;
        DamageDir = damageDir;
        KnockBackPower = knockBackPower;
    }
}
