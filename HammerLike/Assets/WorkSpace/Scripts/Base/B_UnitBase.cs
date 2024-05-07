using RootMotion.Dynamics;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

// enum for types unit
public enum UnitType
{
    Melee,
    Range,
    Magic,
    Boss,
    Develop,
    Slime  // Temp..
}

[RequireComponent(typeof(NavMeshAgent))]
public class B_UnitBase : B_Entity
{
    protected SO_UnitStatus unitStatus;

    [Header("Unit Data")]
    [SerializeField] private UnitType unitType = UnitType.Melee;
    // Anim
    [SerializeField] private Animator anim;
    [SerializeField] protected NavMeshAgent agent;
    public bool isAttacking = false;

    [Header("Knockback")]
    protected float knockBackMultiplier = 1f;
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



    protected virtual void ApplyStatus()
    {
        switch (unitType)
        {
            case UnitType.Melee:
                unitStatus = Instantiate(UnitManager.Instance.GetUnitStatus(2));
                break;
            case UnitType.Range:
                unitStatus = Instantiate(UnitManager.Instance.GetUnitStatus(3)); 
                break;
            case UnitType.Magic:
                // add please
                break;
            case UnitType.Slime:
                unitStatus = Instantiate(UnitManager.Instance.GetUnitStatus(5));
                break;
            case UnitType.Boss:
                // add please
                break;
            case UnitType.Develop:
                unitStatus = Instantiate(UnitManager.Instance.GetUnitStatus(99));
                break;
        }

        rigid.mass = UnitStatus.mass;
    }

    protected virtual void ApplySystemSettings()
    {
        knockBackMultiplier = GameManager.Instance.SystemSettings.KnockBackScale;
        maxKnockBackForce = GameManager.Instance.SystemSettings.MaxKnockBackForce;
        maxPartsBreakForce = GameManager.Instance.SystemSettings.MaxPartsBreakForce;
        knockbackCurve = GameManager.Instance.SystemSettings.KnockbackCurve;
        partsBreakForceCurve = GameManager.Instance.SystemSettings.PartsBreakForceCurve;
    }

    //init override
    public override void Init()
    {
        base.Init();

        agent = GetComponent<NavMeshAgent>();

        ApplyStatus();

        UnitManager.Instance.AddUnit(this);

        InitHP();
        ApplySystemSettings();

        // Get the Animator component if it's not already assigned
        if (Anim == null)
        {
            anim = GetComponent<Animator>();
        }
    }

    protected virtual void UpdateAttackCoolTime()
    {
        UnitStatus.currentAttackCooltime -= Time.deltaTime;
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


    #region HP

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

        //CheckDead();

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
    }

    // Clamp hp between 0 and maxHP
    protected float ClampHP()
    {
        return Mathf.Clamp(UnitStatus.currentHP, 0f, UnitStatus.maxHP);
    }
    #endregion

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
            Dead();
            isAlive = false;
            return true;
        }
        else
        {
            isAlive = true;
            return false;
        }
    }

    public virtual void Attack()
    {
        isAttacking = true;
    }

    public virtual void StartAttack()
    {
        
    }

    public virtual void EndAttack()
    {
        isAttacking = false;
    }

    // knockback function with mass
    public void Knockback(Vector3 inDir, float force)
    {
        // Apply Coord Scale inDir
        inDir = GameManager.Instance.ApplyCoordScaleNormalize(inDir);

        var resultKnockPower = Mathf.Clamp(force * knockBackMultiplier, 0f, maxKnockBackForce);
        Debug.Log(this.gameObject.name + " Knockback : " + resultKnockPower);
        
        StartCoroutine(SmoothKnockback(inDir, resultKnockPower, rigid, knockbackCurve));
    }

    // Temp 240402 - Puppet 테스트 목적, a.HG
    public void ImpulseKnockbackToPuppet(Vector3 inDir, float force, Rigidbody rigid)
    {
        Debug.Log("Add force!!");
        rigid.velocity = (inDir * force * knockBackMultiplier);
    }

    // knockback coroutine with animation curve
    private IEnumerator SmoothKnockback(Vector3 inDir, float force, Rigidbody inRigid, AnimationCurve inCurve, float inDuration = 0.5f)
    {
        if(inRigid == null)
        {
            inRigid = rigid;
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

            inRigid.AddForce(knockbackVelocity * inCurve.Evaluate(elapsed), ForceMode.Force);

            yield return null;
        }

        isKnockback = false;
        EnableMovementAndRotation();
    }

    protected virtual void Dead()
    {
        //Destroy(gameObject);
        //gameObject.SetActive(false);
        Debug.Log("Dead");
        DisableMovementAndRotation();
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

    protected override void OnTriggerEnter(Collider other)
    {

    }

    // Temp 240402 - Puppet 테스트 목적, a.HG
    // From Monster.DisconnectMusclesRecursive()
    protected void DisconnectMusclesRecursive()
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

                if (muscleRigid != null)
                {
                    Vector3 dir = (muscleRigid.transform.position - GameManager.Instance.Player.transform.position).normalized;
                    dir = GameManager.Instance.ApplyCoordScaleNormalize(dir);

                    remainKnockBackForce = Mathf.Clamp(remainKnockBackForce, 0f, maxPartsBreakForce);

                    StartCoroutine(SmoothKnockback(dir, remainKnockBackForce, muscleRigid, partsBreakForceCurve, partsKnockBackTime));
                }

                puppet.puppetMaster.DisconnectMuscleRecursive(i, MuscleDisconnectMode.Sever);

                // root RigidBody 물리력 고정 및 콜라이더 비활성화
                rigid.isKinematic = true;
                col.enabled = false;
            }
        }
    }
}
