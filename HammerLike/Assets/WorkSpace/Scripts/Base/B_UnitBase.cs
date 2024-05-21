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
public class B_UnitBase : B_Entity
{
    protected SO_UnitStatus unitStatus;

    [Header("Unit Data")]
    [SerializeField] protected int unitIndex = -1;
    [SerializeField] private Animator anim;
    [SerializeField] protected NavMeshAgent agent;
    protected bool isAttacking = false;
    protected bool isAlive = true;

    [Header("Knockback")]
    protected float knockBackMultiplier = 1f;
    protected float partsKnockBackMultiplier = 1f;
    protected float knockbackDuration = 0.5f;
    protected float maxKnockBackForce = 100f;
    protected float maxPartsBreakForce = 100f;
    protected AnimationCurve knockbackCurve;
    protected AnimationCurve partsBreakForceCurve;
    protected ForceMode forceMode = ForceMode.Force;
    protected Vector3 remainKnockBackDir;
    protected float remainKnockBackForce = 0f;

    // Ground check variables
    [Header("Ground Check")]
    private LayerMask groundLayer; // Define which layer is considered as ground
    private float groundCheckDistance = 0.1f; // Distance to check for ground
    [SerializeField] protected bool isGrounded; // Variable to store if unit is grounded

    #region Getters & Setters

    public SO_UnitStatus UnitStatus => unitStatus;
    public Animator Anim => anim;
    public NavMeshAgent Agent => agent;

    public bool IsAlive => isAlive;
    public bool IsAttacking => isAttacking;

    public bool IsLockMove { get; private set; }
    public bool IsLockRotate { get; private set; }
    public bool IsKnockback { get; private set; }

    public bool SetAttacking
    {
        set
        {
            isAttacking = value;
        } 
    }

    public int SetUnitIndex
    {
        set
        {
            unitIndex = value;
            ApplyStatus();
        }
    }

    #endregion


    #region Unity Callbacks & Init
    public override void Init()
    {
        base.Init();

        agent = GetComponent<NavMeshAgent>();

        ApplyStatus();

        InitHP();
        ApplySystemSettings();

        // Get the Animator component if it's not already assigned
        if (Anim == null)
        {
            anim = GetComponent<Animator>();
        }
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        if (!isAlive)
            return;
    }

    protected override void FixedUpdate() 
    {
        base.FixedUpdate();
        
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

    protected override void OnTriggerEnter(Collider other)
    {

    }

    #endregion

    #region Control & Apply Status Data

    protected virtual void ApplyStatus()
    {
        if(unitStatus != null)
        {
            Debug.Log("Unit Status is already set");
            return;
        }

        if(unitIndex == -1)
        {
            Debug.LogError("Unit Index is not set");
            return;
        }
        unitStatus = Instantiate(UnitManager.Instance.GetUnitStatus(unitIndex));

        Rigid.mass = UnitStatus.mass;
    }

    protected virtual void ApplySystemSettings()
    {
        var systemSettings = GameManager.Instance.SystemSettings;

        knockbackDuration = systemSettings.KnockbackDuration;
        knockBackMultiplier = systemSettings.KnockBackScale;
        maxKnockBackForce = systemSettings.MaxKnockBackForce;
        knockbackCurve = systemSettings.KnockbackCurve;
        forceMode = systemSettings.KnockbackForceMode;

        partsKnockBackMultiplier = systemSettings.PartsKnockBackScale;
        maxPartsBreakForce = systemSettings.MaxPartsBreakForce;
        partsBreakForceCurve = systemSettings.PartsBreakForceCurve;

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

    public virtual void TakeDamage(Vector3 damageDir, int damage, float knockBackPower, bool knockBack = true)
    {
        if (isInvincible)
        {
            return;
        }

#if UNITY_EDITOR
        ApplySystemSettings();
#endif

        UnitStatus.currentHP = UnitStatus.currentHP - damage;

        ClampHP();

        Debug.Log(this.gameObject.name + " TakeDamage : " + damage);
        //ChangeCursor.Instance.SetCursorAttack();    // 명진. 0514 임시 Cursor 변경 싱글톤으로 받아감
        if (knockBack)
        {
            remainKnockBackDir = damageDir;
            remainKnockBackForce = knockBackPower;

            Knockback(damageDir, remainKnockBackForce);
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

    protected virtual bool CheckDead(bool isSelf = false)
    {
        // Dead if hp is 0
        if (UnitStatus.currentHP <= 0)
        {
            if(isAlive)
            {
                Dead(isSelf);
                isAlive = false;
            }
            return true;
        }
        else
        {
            isAlive = true;
            return false;
        }
    }

    protected virtual void UpdateAttackCoolTime()
    {
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

        agent.speed = unitStatus.moveSpeed * coordScale;

        return agent.nextPosition;
    }

    public virtual void Attack()
    {
        SetAttacking = true;
    }

    #region Knockback    
    // knockback function with mass
    public void Knockback(Vector3 inDir, float force)
    {
        // Apply Coord Scale inDir
        inDir = GameManager.Instance.ApplyCoordScaleAfterNormalize(inDir);

        var resultKnockPower = Mathf.Clamp(force * knockBackMultiplier, 0f, maxKnockBackForce);

        StartCoroutine(CoSmoothKnockback(inDir, resultKnockPower, Rigid, knockbackCurve, knockbackDuration, forceMode));
    }

    /// <summary>
    /// CoSmoothKnockback : 부드러운 넉백 적용 코루틴
    /// </summary>
    /// <param name="inDir"></param>
    /// <param name="force"></param>
    /// <param name="inRigid"></param>
    /// <param name="inCurve"></param>
    /// <param name="inDuration"></param>
    /// <returns></returns>
    protected IEnumerator CoSmoothKnockback(Vector3 inDir, float force, Rigidbody inRigid, AnimationCurve inCurve, float inDuration = 0.5f, ForceMode inForceMode = ForceMode.VelocityChange)
    {
        if(inRigid == null)
        {
            inRigid = Rigid;
        }
        
        Vector3 knockbackVelocity = inDir * force / unitStatus.mass;

        if(inForceMode != ForceMode.VelocityChange)
        {
            knockbackVelocity = inDir * force;
        }

        float knockbackDuration = inDuration; // Duration over which the force is applied
        float startTime = Time.time;

        DisableMovementAndRotation();

        IsKnockback = true;
        isInvincible = true;

        while (Time.time < startTime + knockbackDuration)
        {
            float elapsed = (Time.time - startTime) / knockbackDuration;

            inRigid.AddForce(knockbackVelocity * inCurve.Evaluate(elapsed), inForceMode);

            yield return null;
        }

        IsKnockback = false;
        isInvincible = false;

        EnableMovementAndRotation();

        CheckDead(true);
    }
    #endregion

    /// <summary>
    /// Dead : 유닛 사망 함수
    /// </summary>
    protected virtual void Dead(bool isSelf = false)
    {
        DisableMovementAndRotation();
    }

    #endregion

    #region Animation Event
    /// <summary>
    /// StartAttack : 공격 시작 애니메이션 이벤트 함수
    /// </summary>
    public virtual void StartAttack()
    {
        SetAttacking = true;
    }

    /// <summary>
    /// EndAttack : 공격 종료 애니메이션 이벤트 함수
    /// </summary>
    public virtual void EndAttack()
    {
        SetAttacking = false;
    }
    #endregion
}
