using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class B_UnitBase : B_ObjectBase
{
    private SO_UnitStatus unitStatus;

    // Anim
    [SerializeField] protected Animator anim;

    // Ground check variables
    [Header("Ground Check")]
    public LayerMask groundLayer; // Define which layer is considered as ground
    public float groundCheckDistance = 0.1f; // Distance to check for ground
    [SerializeField] private bool isGrounded; // Variable to store if unit is grounded

    public SO_UnitStatus UnitStatus { get => unitStatus;}

    //init override
    public override void Init()
    {
        base.Init();

        unitStatus = UnitManager.instance.baseUnitStatus.MakeCopyStatus();
        UnitManager.instance.AddUnit(this);

        InitHP();

        // Get the Animator component if it's not already assigned
        if(anim == null)
        {
            anim = GetComponent<Animator>();
        }
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        CheckGrounded();

        // input spacebar will Debug.Log about its name and unitStatus maxHP value
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Name: " + this.gameObject.name + ", Max HP: " + UnitStatus.maxHP);
        }

    }

    public virtual void Move(Vector3 inDir)
    {
        if (isGrounded)
        {
            // use Rigidbody to move
            rigid.velocity = inDir * UnitStatus.moveSpeed;
            //transform.Translate(inDir * unitStatus.moveSpeed * Time.deltaTime);
        }
    }
    void CheckGrounded()
    {
        RaycastHit hit;
        // Check if there's ground below the unit within the groundCheckDistance
        isGrounded = Physics.Raycast(transform.position, -Vector3.up, out hit, col.bounds.extents.y + groundCheckDistance, groundLayer);
        Debug.DrawRay(transform.position, -Vector3.up * (col.bounds.extents.y + groundCheckDistance), Color.red);
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

    public void TakeDamage(int damage = 0)
    {
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

    protected virtual void CheckDead()
    {
        // Dead if hp is 0
        if (UnitStatus.currentHP <= 0)
        {
            Dead();
        }
    }

    protected virtual void Dead()
    {
        //Destroy(gameObject);
        //gameObject.SetActive(false);
        Debug.Log("Dead");
    }

}
