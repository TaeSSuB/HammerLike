using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 유닛 스테이터스 오브젝트
/// - 유닛의 스테이터스 데이터를 담은 ScriptableObject
/// </summary>
[CreateAssetMenu(fileName = "UnitStatus", menuName = "B_ScriptableObjects/Unit/UnitStatus", order = 1)]
public class SO_UnitStatus : ScriptableObject, ISerializationCallbackReceiver
{
    #region Variables
    public Sprite unitSprite;

    public int index = 0;

    public string entityName = "";
    public string _koreanName = "";

    public float mass = 1f;

    public float armor = 1f; // temp data

    public float detectRange = 10f;

    public float moveSpeed = 5f;
    protected float moveSpeedOrigin = 5f;

    public int currentHP = 100;
    public int maxHP = 100;

    public int restoreHP = 0;
    public float restoreHPCooltime = 0f;
    public float currentRestoreHPCooltime = 0f;

    public int atkDamage = 10;
    protected int atkDamageOrigin = 10;
    public float atkRange = 1f;
    public float atkSpeed = 1f;

    public float knockbackPower = 1f;
    public float knockbackResistance = 0f;

    public float maxAttackCooltime = 1f;
    public float currentAttackCooltime = 0f;

    #endregion

    public float MoveSpeedOrigin { get => moveSpeedOrigin;}
    public int AtkDamageOrigin { get => atkDamageOrigin;}

    /// <summary>
    /// 스탯 Initialize
    /// ..일관화 필요
    /// </summary>
    public void Init()
    {
        currentHP = maxHP;

        currentRestoreHPCooltime = restoreHPCooltime;
        currentAttackCooltime = maxAttackCooltime;
        
        moveSpeedOrigin = moveSpeed;
        atkDamageOrigin = atkDamage;
    }

    /// <summary>
    /// OnAfterDeserialize
    /// - 데이터 수정 이후 초기화 실행
    /// </summary>
    /// <exception cref="System.NotImplementedException"></exception>
    public void OnAfterDeserialize()
    {
        Init();
        //throw new System.NotImplementedException();
    }

    /// <summary>
    /// OnBeforeSerialize
    /// - 데이터 수정 이전 지속 실행
    /// </summary>
    /// <exception cref="System.NotImplementedException"></exception>
    public void OnBeforeSerialize()
    {

    }
}
