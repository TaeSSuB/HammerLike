using RootMotion.Dynamics;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

// enum for types unit
public enum UnitType
{
    Melee,
    Range,
    Magic,
    Boss,
    Develop
}

public class B_UnitBase : B_ObjectBase
{
    protected SO_UnitStatus unitStatus;

    [Header("Unit Data")]
    [SerializeField] private UnitType unitType = UnitType.Melee;
    // Anim
    [SerializeField] private Animator anim;

    [Header("Knockback")]
    // animation curve for knockback
    [SerializeField] protected AnimationCurve knockbackCurve;
    [SerializeField] protected AnimationCurve partsBreakForceCurve;
    [SerializeField] protected float knockBackMultiplier = 10f;

    // Temp 240402 - Puppet 테스트 목적, a.HG
    public BehaviourPuppet puppet;
    public Vector3 remainKnockBackDir;
    public float remainKnockBackForce = 0f;

    // Ground check variables
    [Header("Ground Check")]
    public LayerMask groundLayer; // Define which layer is considered as ground
    public float groundCheckDistance = 0.1f; // Distance to check for ground
    [SerializeField] private bool isGrounded; // Variable to store if unit is grounded

    public SO_UnitStatus UnitStatus { get => unitStatus;}
    public Animator Anim { get => anim;}

    // lock move and rotate
    public bool isLockMove { get; private set; }
    public bool isLockRotate { get; private set; }


    protected virtual void ApplyStatus()
    {
        switch (unitType)
        {
            case UnitType.Melee:
                unitStatus = UnitManager.instance.baseUnitStatus.MakeCopyStatus();
                break;
            case UnitType.Range:
                unitStatus = UnitManager.instance.rangerUnitStatus.MakeCopyStatus();
                break;
            case UnitType.Magic:
                // add please
                break;
            case UnitType.Boss:
                // add please
                break;
            case UnitType.Develop:
                unitStatus = UnitManager.instance.devUnitStatus.MakeCopyStatus();
                break;
        }
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

        if (UnitStatus.currentAttackCooltime > 0)
        {
            UnitStatus.currentAttackCooltime -= Time.deltaTime;
            Anim.SetFloat("fRemainShot", UnitStatus.currentAttackCooltime);
        }
            
    }

    public virtual Vector3 Move(Vector3 inDir)
    {
        if (!isGrounded || isLockMove)
            return Vector3.zero;

        inDir.Normalize();
        var moveDir = manager.ApplyCoordScale(inDir);

        rigid.velocity = moveDir * unitStatus.moveSpeed;

        return moveDir * unitStatus.moveSpeed;
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

        UnitStatus.currentHP = UnitStatus.currentHP - damage;

        ClampHP();

        //CheckDead();

        //Knockback(damageDir, Mathf.Clamp(damage, 0f, 15f));
        remainKnockBackDir = damageDir;
        remainKnockBackForce = Mathf.Clamp(damage, 0f, 15f);


        if (!CheckDead())
        {
            // If the unit is not dead, apply smooth knockback
            Knockback(damageDir, Mathf.Clamp(damage, 0f, 15f));
        }
    }

    protected virtual void StartAttack()
    {

    }

    protected virtual void EndAttack()
    {

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
            return true;
        }
        else
        {
            return false;
        }
    }

    // knockback function with mass
    public void Knockback(Vector3 inDir, float force)
    {
        // Apply a smoothed knockback over time rather than as an impulse
        StartCoroutine(SmoothKnockback(inDir, force, rigid, knockbackCurve, 1f));
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

        Vector3 knockbackVelocity = inDir * force / unitStatus.mass * knockBackMultiplier;

        // 초기 속도를 저장합니다.
        Vector3 initialVelocity = inRigid.velocity;
        float knockbackDuration = inDuration; // Duration over which the force is applied
        float startTime = Time.time;

        DisableMovementAndRotation();

        while (Time.time < startTime + knockbackDuration)
        {
            float elapsed = (Time.time - startTime) / knockbackDuration;

            // Apply the force using the animation curve
            inRigid.velocity = Vector3.Lerp(initialVelocity, knockbackVelocity, inCurve.Evaluate(elapsed));

            yield return null;
        }

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

        Debug.Log("DisableMovementAndRotation()");
    }

    public virtual void EnableMovementAndRotation()
    {
        // Enable movement and rotation logic
        // For example, you can reset the flag to allow movement and rotation
        // or enable the disabled components responsible for movement and rotation
        isLockMove = false;
        isLockRotate = false;

        Debug.Log("EnableMovementAndRotation()");
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

                // Temp 240402 - 파츠 별 넉백.., a.HG
                // 1. StartCoroutine(SmoothKnockback)
                // 2. ImpulseKnockbackToPuppet
                // 3. AddForce Each (Loop)

                if (muscleRigid != null)
                {
                    Vector3 dir = (muscleRigid.transform.position - GameManager.instance.Player.transform.position).normalized;
                    dir = GameManager.instance.ApplyCoordScale(dir);
                    dir.y = 0f;

                    //muscleRigid.AddForce(dir * remainKnockBackForce * knockBackMultiplier, ForceMode.Impulse);
                    StartCoroutine(SmoothKnockback(dir, remainKnockBackForce, muscleRigid, partsBreakForceCurve, 0.2f));
                    //uscleRigid.velocity = (remainKnockBackDir * remainKnockBackForce * knockBackMultiplier);
                }

                puppet.puppetMaster.DisconnectMuscleRecursive(i, MuscleDisconnectMode.Explode);

                //puppet.puppetMaster.muscles[i].rigidbody.AddForce(remainKnockBackForce, ForceMode.Impulse);

                // root RigidBody 물리력 고정 및 콜라이더 비활성화
                rigid.isKinematic = true;
                col.enabled = false;
            }
        }
    }
}
