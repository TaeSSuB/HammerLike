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

    [Header("Unit Status")]
    public Sprite unitSprite;

    public int index = 0;

    public string entityName = "";
    public string _koreanName = "";

    public float mass = 1f;

    public float armor = 1f; // temp data

    public float detectRange = 10f;

    [Header("Health Point")]
    public float moveSpeed = 5f;
    protected float moveSpeedOrigin = 5f;

    [Header("Health Point")]
    public int currentHP = 100;
    public int maxHP = 100;
    public int restoreHP = 0;
    public float restoreHPCooltime = 0f;
    public float currentRestoreHPCooltime = 0f;
    
    [Header("Attack")]
    public int atkDamage = 10;
    public float atkRange = 1f;
    public float atkSpeed = 1f;

    [Header("Attack Cooltime")]
    public float maxAttackCooltime = 1f;
    public float currentAttackCooltime = 0f;

    [Header("KnockBack")]
    public float knockbackPower = 1f;
    public float knockbackResistance = 0f;


    #endregion

    public float MoveSpeedOrigin { get => moveSpeedOrigin;}

    /// <summary>
    /// 스탯 Initialize
    /// ..일관화 필요
    /// </summary>
    public virtual void Init()
    {
        currentHP = maxHP;

        currentRestoreHPCooltime = restoreHPCooltime;
        currentAttackCooltime = maxAttackCooltime;
        
        moveSpeedOrigin = moveSpeed;
    }

    /// <summary>
    /// OnAfterDeserialize
    /// - 데이터 수정 이후 초기화 실행
    /// </summary>
    /// <exception cref="System.NotImplementedException"></exception>
    public virtual void OnAfterDeserialize()
    {
        Init();
        //throw new System.NotImplementedException();
    }

    /// <summary>
    /// OnBeforeSerialize
    /// - 데이터 수정 이전 지속 실행
    /// </summary>
    /// <exception cref="System.NotImplementedException"></exception>
    public virtual void OnBeforeSerialize()
    {

    }

    public void OnApplicationQuit()
    {
        moveSpeed = moveSpeedOrigin;
    }
}
