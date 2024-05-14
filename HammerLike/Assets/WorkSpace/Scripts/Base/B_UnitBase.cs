using RootMotion.Dynamics;
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
    public bool isAttacking = false;

    [Header("Knockback")]
    protected float knockBackMultiplier = 1f;
    protected float partsKnockBackMultiplier = 1f;
    public float knockbackDuration = 0.5f;
    protected float maxKnockBackForce = 100f;
    protected float maxPartsBreakForce = 100f;
    protected AnimationCurve knockbackCurve;
    protected AnimationCurve partsBreakForceCurve;

    // Temp 240402 - Puppet 테스트 목적, a.HG
    public BehaviourPuppet puppet;
    protected Vector3 remainKnockBackDir;
    protected float remainKnockBackForce = 0f;

    // Ground check variables
    [Header("Ground Check")]
    public LayerMask groundLayer; // Define which layer is considered as ground
    public float groundCheckDistance = 0.1f; // Distance to check for ground
    protected bool isGrounded; // Variable to store if unit is grounded

    public SO_UnitStatus UnitStatus { get => unitStatus;}
    public Animator Anim { get => anim;}
    public NavMeshAgent Agent { get => agent;}

    private bool isAlive = true;
    public bool IsAlive { get => isAlive; }
    // lock move and rotate
    public bool isLockMove { get; private set; }
    public bool isLockRotate { get; private set; }
    public bool isKnockback;

    public int SetUnitIndex
    {
        set
        {
            unitIndex = value;
            ApplyStatus();
        }
    }

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
        CheckGrounded();

        if (!isAlive)
            return;

        if (UnitStatus.currentAttackCooltime > 0)
        {
            UpdateAttackCoolTime();
            //UnitStatus.currentAttackCooltime -= Time.deltaTime;
            //Anim.SetFloat("fRemainShot", UnitStatus.currentAttackCooltime);

            // Init - On Attack State
            // current Attack -> Max Attack Cooltime
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
        knockBackMultiplier = GameManager.Instance.SystemSettings.KnockBackScale;
        maxKnockBackForce = GameManager.Instance.SystemSettings.MaxKnockBackForce;
        knockbackCurve = GameManager.Instance.SystemSettings.KnockbackCurve;

        partsKnockBackMultiplier = GameManager.Instance.SystemSettings.PartsKnockBackScale;
        maxPartsBreakForce = GameManager.Instance.SystemSettings.MaxPartsBreakForce;
        partsBreakForceCurve = GameManager.Instance.SystemSettings.PartsBreakForceCurve;
    }

    public void InitHP()
    {
        UnitStatus.currentHP = UnitStatus.maxHP;
    }

    public void RestoreHP(int hpRate = 0)
    {
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
        ChangeCursor.Instance.SetCursorAttack();    // 명진. 0514 임시 Cursor 변경 싱글톤으로 받아감
        if (knockBack)
        {
            remainKnockBackDir = damageDir;
            remainKnockBackForce = knockBackPower;


            if (!CheckDead())
            {
                // If the unit is not dead, apply smooth knockback
                Knockback(damageDir, remainKnockBackForce);
            }
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
    private void CheckGrounded()
    {

        // min dis 0.01f to prevent raycast from hitting itself
        var minDis = Vector3.up * 0.01f;

        RaycastHit hit;
        // Check if there's ground below the unit within the groundCheckDistance
        isGrounded = Physics.Raycast(transform.position + minDis, -Vector3.up, out hit, groundCheckDistance + minDis.magnitude, groundLayer);
        Debug.DrawRay(transform.position, -Vector3.up * (groundCheckDistance), Color.red);
    }

    protected virtual bool CheckDead()
    {
        // Dead if hp is 0
        if (UnitStatus.currentHP <= 0)
        {
            if(isAlive)
            {
                Dead();
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
        // Disable movement and rotation logic
        // For example, you can set a flag to prevent movement and rotation
        // or disable specific components responsible for movement and rotation
        isLockMove = true;
        isLockRotate = true;

        //Debug.Log("DisableMovementAndRotation()");
    }

    public virtual void EnableMovementAndRotation()
    {
        // Enable movement and rotation logic
        // For example, you can reset the flag to allow movement and rotation
        // or enable the disabled components responsible for movement and rotation
        isLockMove = false;
        isLockRotate = false;

        //Debug.Log("EnableMovementAndRotation()");
    }

    #endregion

    #region Action

        /// <summary>
    /// 유닛 이동 함수
    /// a.HG : 240501 - NavMesh 도입. 위치 기반으로 변경. inDir -> inPos
    /// </summary>
    /// <param name="inPos">타겟 포지션</param>
    /// <returns>Agent 목표 위치</returns>
    public virtual Vector3 Move(Vector3 inPos)
    {
        if (!isGrounded || isLockMove || !agent.enabled)
            return Vector3.zero;

        var targetDir = inPos - transform.position;

        agent.SetDestination(inPos);

        var coordScale = GameManager.Instance.CalcCoordScale(targetDir);

        agent.speed = unitStatus.moveSpeed * coordScale;

        return agent.nextPosition;
    }

    public virtual void Attack()
    {
        isAttacking = true;
    }

    #region Knockback    
    // knockback function with mass
    public void Knockback(Vector3 inDir, float force)
    {
        // Apply Coord Scale inDir
        inDir = GameManager.Instance.ApplyCoordScaleAfterNormalize(inDir);

        var resultKnockPower = Mathf.Clamp(force * knockBackMultiplier, 0f, maxKnockBackForce);
        //Debug.Log(this.gameObject.name + " Knockback : " + resultKnockPower);
        
        StartCoroutine(CoSmoothKnockback(inDir, resultKnockPower, Rigid, knockbackCurve, knockbackDuration));
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
    private IEnumerator CoSmoothKnockback(Vector3 inDir, float force, Rigidbody inRigid, AnimationCurve inCurve, float inDuration = 0.5f, ForceMode inForceMode = ForceMode.VelocityChange)
    {
        if(inRigid == null)
        {
            inRigid = Rigid;
        }

        Vector3 knockbackVelocity = inDir * force / unitStatus.mass;

        // 초기 속도를 저장합니다.
        Vector3 initialVelocity = inRigid.velocity;
        float knockbackDuration = inDuration; // Duration over which the force is applied
        float startTime = Time.time;

        DisableMovementAndRotation();
        isKnockback = true;

        while (Time.time < startTime + knockbackDuration)
        {
            float elapsed = (Time.time - startTime) / knockbackDuration;

            // Apply the force using the animation curve
            //inRigid.velocity = Vector3.Lerp(initialVelocity, knockbackVelocity, inCurve.Evaluate(elapsed));

            inRigid.AddForce(knockbackVelocity * inCurve.Evaluate(elapsed), inForceMode);

            yield return null;
        }

        isKnockback = false;
        EnableMovementAndRotation();
    }
    #endregion

    /// <summary>
    /// Dead : 유닛 사망 함수
    /// </summary>
    protected virtual void Dead()
    {
        //Destroy(gameObject);
        //gameObject.SetActive(false);
        Debug.Log("Dead");
        DisableMovementAndRotation();
    }

    #endregion

    #region Animation Event
    /// <summary>
    /// StartAttack : 공격 시작 애니메이션 이벤트 함수
    /// </summary>
    public virtual void StartAttack()
    {
        
    }

    /// <summary>
    /// EndAttack : 공격 종료 애니메이션 이벤트 함수
    /// </summary>
    public virtual void EndAttack()
    {
        isAttacking = false;
    }
    #endregion

    #region PuppetMaster
    /// <summary>
    /// DisconnectMusclesRecursive : PuppetMaster의 Muscle을 해제 및 넉백 적용
    /// </summary>
    /// <param name="inPos">넉백 기준점</param>
    protected void DisconnectMusclesRecursive(Vector3 inPos, bool isSelf = false)
    {
        if (puppet != null && puppet.puppetMaster != null)
        {
            for (int i = 0; i < puppet.puppetMaster.muscles.Length; i++)
            {
                Rigidbody muscleRigid = puppet.puppetMaster.muscles[i].rigidbody;

                float partsKnockBackTime = 0.2f;

                // Temp 240402 - 파츠 별 넉백.., a.HG
                // 1. StartCoroutine(SmoothKnockback)
                // 2. ImpulseKnockbackToPuppet
                // 3. AddForce Each (Loop)

                if (muscleRigid != null && !isSelf)
                {
                    Vector3 dir = (muscleRigid.transform.position - inPos).normalized;
                    dir = GameManager.Instance.ApplyCoordScaleAfterNormalize(dir);

                    remainKnockBackForce = Mathf.Clamp(remainKnockBackForce * partsKnockBackMultiplier, 0f, maxPartsBreakForce);

                    StartCoroutine(CoSmoothKnockback(dir, remainKnockBackForce, muscleRigid, partsBreakForceCurve, partsKnockBackTime, ForceMode.Impulse));
                }

                puppet.puppetMaster.DisconnectMuscleRecursive(i, MuscleDisconnectMode.Sever);

                // root RigidBody 물리력 고정 및 콜라이더 비활성화
                Rigid.isKinematic = true;
                col.enabled = false;
            }
        }
    }
    #endregion
}
