using RootMotion.Dynamics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class B_UnitBase : B_ObjectBase
{
    protected SO_UnitStatus unitStatus;
    public float knockbackMultiplier = 1.0f;
    public float knockbackIncrease = 1.5f; // Multiplier increase per hit
    public float knockbackResetTime = 1.0f;
    private float lastKnockbackTime = -Mathf.Infinity; // Initialize to a time in the past

    // lock move and rotate
    public bool isLockMove { get; private set; }
    public bool isLockRotate { get; private set; }

    // Anim
    [SerializeField] private Animator anim;

    // Ground check variables
    [Header("Ground Check")]
    public LayerMask groundLayer; // Define which layer is considered as ground
    public float groundCheckDistance = 0.1f; // Distance to check for ground
    [SerializeField] private bool isGrounded; // Variable to store if unit is grounded

    public SO_UnitStatus UnitStatus { get => unitStatus;}
    public Animator Anim { get => anim;}

    protected virtual void ApplyStatus()
    {
        // Apply status to the unit
        unitStatus = UnitManager.instance.baseUnitStatus.MakeCopyStatus();
    }

    //init override
    public override void Init()
    {
        base.Init();

        ApplyStatus();
        UnitManager.instance.AddUnit(this);

        InitHP();

        // Get the Animator component if it's not already assigned
        if(Anim == null)
        {
            anim = GetComponent<Animator>();
        }
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        CheckGrounded();
    }

    public virtual void Move(Vector3 inDir)
    {
        if (!isGrounded || isLockMove)
            return;

        var moveDir = manager.ApplyCoordScale(inDir);

        rigid.velocity = moveDir * unitStatus.moveSpeed;
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

    public void TakeDamage(Vector3 damageDir, int damage = 0)
    {
        if (isInvincible)
        {
            return;
        }

        Knockback(damageDir, damage);

        UnitStatus.currentHP = UnitStatus.currentHP - damage;
        ClampHP();
        CheckDead();
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

    protected virtual void CheckDead()
    {
        // Dead if hp is 0
        if (UnitStatus.currentHP <= 0)
        {
            Dead();
        }
    }

    // knockback function with mass
    public void Knockback(Vector3 inDir, float force)
    {
        float timeSinceLastKnockback = Time.time - lastKnockbackTime;
        if (timeSinceLastKnockback < knockbackResetTime)
        {
            knockbackMultiplier *= knockbackIncrease; // Increase multiplier for consecutive hits
        }
        else
        {
            knockbackMultiplier = 1.0f; // Reset multiplier after knockbackResetTime
        }

        // Apply a smoothed knockback over time rather than as an impulse
        StartCoroutine(SmoothKnockback(inDir, force));
        //rigid.AddForce(inDir * force, ForceMode.Impulse);

        //Debug.Log(rigid.velocity);
        lastKnockbackTime = Time.time;
    }

    private IEnumerator SmoothKnockback(Vector3 inDir, float force)
    {
        Vector3 knockbackVelocity = inDir.normalized * force / unitStatus.mass;
        // 초기 속도를 저장합니다.
        Vector3 initialVelocity = rigid.velocity;
        float knockbackDuration = 1f; // Duration over which the force is applied
        float startTime = Time.time;

        DisableMovementAndRotation();

        while (Time.time < startTime + knockbackDuration)
        {
            float elapsed = (Time.time - startTime) / knockbackDuration;

            rigid.velocity = Vector3.Lerp(initialVelocity, knockbackVelocity, elapsed);

            yield return null;
        }

        EnableMovementAndRotation();
    }
    protected virtual void Dead()
    {
        //Destroy(gameObject);
        //gameObject.SetActive(false);
        Debug.Log("Dead");
    }

    protected virtual void DisableMovementAndRotation()
    {
        // Disable movement and rotation logic
        // For example, you can set a flag to prevent movement and rotation
        // or disable specific components responsible for movement and rotation
        isLockMove = true;
        isLockRotate = true;

        Debug.Log("DisableMovementAndRotation()");
    }

    protected virtual void EnableMovementAndRotation()
    {
        // Enable movement and rotation logic
        // For example, you can reset the flag to allow movement and rotation
        // or enable the disabled components responsible for movement and rotation
        isLockMove = false;
        isLockRotate = false;

        Debug.Log("EnableMovementAndRotation()");
    }


}
