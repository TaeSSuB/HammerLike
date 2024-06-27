using System;
using System.Collections;

using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// B_UnitBase : 유닛 베이스 클래스
/// - 동적 Entity인 '유닛'의 BaseClass
/// - unitIndex를 통해 UnitStatus를 할당
///     - UnitManager를 통한 생성시 DB를 통해 자동으로 할당
///     - 씬 배치 시 직접 할당 필요
/// - 유닛의 기본적인 동작을 정의
/// - NavMeshAgent를 이용한 이동 로직
/// </summary>
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(B_KnockBack))]
public class B_UnitBase : B_Entity, IAttackAble
{
    public SO_UnitStatus InitStatus { protected get; set; }
    public SO_UnitStatus RunTimeStatus { get; protected set; }
    protected B_KnockBack knockBack;

    [Header("Unit Data")]
    [SerializeField] protected int unitIndex = -1;
    [SerializeField] private Animator anim;
    protected NavMeshAgent agent;
    protected bool isAttacking = false;
    protected bool isAlive = true;

    // Ground check variables
    [Header("Ground Check")]
    private LayerMask groundLayer; // Define which layer is considered as ground
    private float groundCheckDistance = 0.1f; // Distance to check for ground
    [SerializeField] protected bool isGrounded; // Variable to store if unit is grounded

    #region Getters & Setters

    public SO_UnitStatus UnitStatus => RunTimeStatus;
    public Animator Anim => anim;
    public NavMeshAgent Agent => agent;

    public bool IsAlive => isAlive;
    public bool IsAttacking => isAttacking;

    public bool IsLockMove { get; private set; }
    public bool IsLockRotate { get; private set; }
    public bool IsKnockback { get; set; }

    public Action OnHitEvent;

    public bool SetAttacking
    {
        set
        {
            isAttacking = value;
            Anim.SetBool("IsAttacking", isAttacking);
        } 
    }

    public int SetUnitIndex
    {
        set
        {
            unitIndex = value;
            StatusInitialize();
        }
    }

    #endregion


    #region Unity Callbacks & Init
    public override void Init()
    {
        base.Init();

        agent = GetComponent<NavMeshAgent>();
        knockBack = GetComponent<B_KnockBack>();

        StatusInitialize();

        InitHP();
        ApplySystemSettings();

        // Get the Animator component if it's not already assigned
        if (Anim == null)
        {
            anim = GetComponent<Animator>();
        }

        // 초기 HP 값에 따른 Dead 판별
        isAlive = true;
        CheckDead(true);
        
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (!isAlive)
            return;
    }

    protected virtual void FixedUpdate() 
    {
        CheckGrounded();

        if (!isAlive)
            return;

        if (UnitStatus.currentAttackCooltime > 0)
        {
            UpdateAttackCoolTime();
        }

        if(UnitStatus.currentRestoreHPCooltime > 0)
        {
            UnitStatus.currentRestoreHPCooltime -= Time.deltaTime;
        }

        if (UnitStatus.currentRestoreHPCooltime <= 0)
        {
            RestoreHP((int)UnitStatus.restoreHP);
            UnitStatus.currentRestoreHPCooltime = UnitStatus.restoreHPCooltime;
        }
    }

    #endregion

    #region Control & Apply Status Data

    protected virtual void StatusInitialize()
    {
        if(InitStatus != null)
        {
            Debug.Log("Unit Status is already set");
            RunTimeStatus = Instantiate(InitStatus);
            return;
        }

        if(unitIndex == -1)
        {
            Debug.LogError("Unit Index is not set");
            return;
        }
        InitStatus = UnitManager.Instance.GetUnitStatus(unitIndex);
        RunTimeStatus = Instantiate(InitStatus);

        Rigid.mass = UnitStatus.mass;

        // Temp. 마찰력 전용 계산식 필요
        Rigid.drag =  UnitStatus.mass * 0.5f;
        Rigid.angularDrag = UnitStatus.mass * 0.5f;
    }

    protected virtual void ApplySystemSettings()
    {
        var systemSettings = GameManager.Instance.SystemSettings;

        groundLayer = systemSettings.GroundLayer;
        groundCheckDistance = systemSettings.GroundCheckDistance;
    }

    public void InitHP()
    {
        UnitStatus.currentHP = UnitStatus.maxHP;
    }

    public virtual void RestoreHP(int hpRate = 0)
    {
        if(IsKnockback || !isAlive)
        {
            return;
        }
        
        UnitStatus.currentHP = UnitStatus.currentHP + hpRate;
        ClampHP();
    }

    public virtual void TakeDamage(Vector3 damageDir, int damage, float knockBackPower, bool enableKnockBack = true, bool slowMotion = false, bool isForced = false)
    {
        if (isInvincible && !isForced)
        {
            return;
        }

#if UNITY_EDITOR
        ApplySystemSettings();
#endif

        UnitStatus.currentHP = UnitStatus.currentHP - damage;

        ClampHP();

        OnHitEvent?.Invoke();

        Debug.Log(this.gameObject.name + " TakeDamage : " + damage);
        //ChangeCursor.Instance.SetCursorAttack();    // 명진. 0514 임시 Cursor 변경 싱글톤으로 받아감
        if (enableKnockBack)
        {
            var remainKnockBackDir = damageDir;
            var remainKnockBackForce = knockBackPower;

            knockBack.Knockback(this, remainKnockBackDir, remainKnockBackForce, slowMotion);
        }
        else
        {
            CheckDead();
        }
    }

    // Clamp hp between 0 and maxHP
    protected int ClampHP()
    {
        UnitStatus.currentHP = Mathf.Clamp(UnitStatus.currentHP, 0, UnitStatus.maxHP);
        
        return UnitStatus.currentHP;
    }
    #endregion

    #region Check or Update State
    protected void CheckGrounded()
    {
        // min dis 0.01f to prevent raycast from hitting itself
        var minDis = Vector3.up * 0.01f;

        RaycastHit hit;

        isGrounded = Physics.Raycast(transform.position + minDis, -Vector3.up, out hit, groundCheckDistance + minDis.magnitude, groundLayer);
        Debug.DrawRay(transform.position, -Vector3.up * (groundCheckDistance), Color.red);
    }

    public virtual bool CheckDead(bool isSelf = false)
    {
        if(!isAlive) return true;

        // Dead if hp is 0
        if (UnitStatus.currentHP <= 0)
        {
            Dead(isSelf);
            isAlive = false;

            return true;
        }
        else
        {
            return false;
        }
    }

    protected virtual void UpdateAttackCoolTime()
    {
        if(!IsAttacking)
            UnitStatus.currentAttackCooltime -= Time.deltaTime;
    }

    public virtual void DisableMovementAndRotation()
    {
        IsLockMove = true;
        IsLockRotate = true;
    }

    public virtual void EnableMovementAndRotation()
    {
        IsLockMove = false;
        IsLockRotate = false;
    }

    #endregion

    #region Action

        /// <summary>
    /// 유닛 이동 함수
    /// a.HG : 240501 - NavMesh 도입. 위치 기반으로 변경. inDir -> inPos
    /// </summary>
    /// <param name="inPos">타겟 포지션</param>
    /// <param name="isForce">강제 이동 여부</param>
    /// <returns>Agent 목표 위치</returns>
    public virtual Vector3 Move(Vector3 inPos, bool isForce = false)
    {
        if (!isGrounded || (IsLockMove && !isForce) || !agent.enabled)
            return Vector3.zero;

        var targetDir = inPos - transform.position;

        agent.SetDestination(inPos);

        var coordScale = GameManager.Instance.CalcCoordScale(targetDir);

        agent.speed = UnitStatus.moveSpeed * coordScale;

        return agent.nextPosition;
    }

    public virtual void StopMove()
    {
        agent.isStopped = true;
        agent.ResetPath();
    }

    public virtual void Attack()
    {
        SetAttacking = true;
    }

    /// <summary>
    /// Dead : 유닛 사망 함수
    /// </summary>
    protected virtual void Dead(bool isSelf = false)
    {
        DisableMovementAndRotation();
        
        UnitManager.Instance.DropItem(unitIndex, transform.position, Quaternion.identity);
    }

    #endregion

    #region Animation Event
    /// <summary>
    /// StartAttack : 공격 시작 애니메이션 이벤트 함수
    /// </summary>
    public virtual void OnStartAttack()
    {
        SetAttacking = true;
    }

    /// <summary>
    /// EndAttack : 공격 종료 애니메이션 이벤트 함수
    /// </summary>
    public virtual void OnEndAttack()
    {
        SetAttacking = false;
    }

    public virtual void ChargeAttack(){}

    public virtual void MaximumChargeAttack(){}

    public virtual void CancelAttack(){}
    
    #endregion
}
