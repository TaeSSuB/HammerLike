using HutongGames.PlayMaker.Actions;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class B_Player : B_UnitBase
{
    [SerializeField] private Collider weaponCollider;
    
    protected override void ApplyStatus()
    {
        unitStatus = GameManager.instance.PlayerStatus.MakeCopyStatus();
    }

    //init override
    public override void Init()
    {
        base.Init();
        // Init logic
    }

    protected override void Update()
    {
        base.Update();

        InputMovement();
        LookAtMouse();
        CheckCharge();
    }
    
    private Vector3 InputMovement()
    {
        if(isLockMove)
            return Vector3.zero;

        // Input logic
        Vector3 moveDir = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        moveDir.Normalize();

        Move(moveDir);
        MoveAnim(moveDir);

        if(Input.GetKeyDown(KeyCode.Space))
        {
            Dash(moveDir);
        }

        return moveDir;
    }

    private void MoveAnim(Vector3 inDir)
    {
        // Move animation logic
        // Set the move direction to the animator
        // Local Direction of the player
        Vector3 localDir = transform.InverseTransformDirection(inDir);
        Anim.SetFloat("MoveX", localDir.x);
        Anim.SetFloat("MoveZ", localDir.z);
    }

    void Dash(Vector3 inDir)
    {
        if (inDir == Vector3.zero)
            return;

        StartCoroutine(DashCoroutine(inDir));
    }

    private IEnumerator DashCoroutine(Vector3 inDir)
    {
        float dashTime = (unitStatus as SO_PlayerStatus).dashDuration;
        float dashSpeed = (unitStatus as SO_PlayerStatus).dashSpeed;

        Vector3 lookDir = inDir + transform.position;//
        transform.LookAt(lookDir);

        StartDash();

        Vector3 coordDir = GameManager.instance.ApplyCoordScale(inDir);

        while (dashTime > 0)
        {
            // move with rigid body
            rigid.velocity = coordDir * dashSpeed;
            dashTime -= Time.deltaTime;
            yield return null;
        }

        EndDash();
    }

    protected void StartDash()
    {
        // Start dash logic
        Anim.SetTrigger("tEnvasion");
        Anim.speed = 1.2f / (unitStatus as SO_PlayerStatus).dashDuration;//(unitStatus as SO_PlayerStatus).dashSpeed;

        SetInvincible(true);
        DisableMovementAndRotation();
    }
    
    protected void EndDash()
    {
        // End dash logic & Initialize
        Anim.SetTrigger("tIdle");
        Anim.speed = 1f;//(unitStatus as SO_PlayerStatus);
        SetInvincible(false);
        EnableMovementAndRotation();
    }

    void CheckCharge()
    {
        // Charge attack logic
        if(Input.GetMouseButton(0))
        {
            (unitStatus as SO_PlayerStatus).chargeRate += Time.deltaTime;

            if((unitStatus as SO_PlayerStatus).chargeRate > (unitStatus as SO_PlayerStatus).minChargeRate)
                Anim.SetBool("IsCharge", true);

            // 확장성을 위해 폐기
            // Init 해두고 재사용하기엔 편할 듯
            //Anim.SetFloat("fAttackSpd", (unitStatus as SO_PlayerStatus).atkSpeed);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            Anim.SetBool("IsCharge", false);

            var chargeRate = (unitStatus as SO_PlayerStatus).chargeRate;
            var minChargeRate = (unitStatus as SO_PlayerStatus).minChargeRate;
            var maxChargeRate = (unitStatus as SO_PlayerStatus).maxChargeRate;

            if (chargeRate >= minChargeRate)
            {
                if(chargeRate >= maxChargeRate)
                {
                    // Max charge attack logic
                    MaximumChargeAttack();
                }
                else
                {
                    ChargeAttack();
                }
            }
            else
            {
                Attack();
            }

            (unitStatus as SO_PlayerStatus).chargeRate = 1f;
        }
    }

    void Attack()
    {
        // Attack logic
        Anim.SetBool("bAttack", true);
        Anim.speed = (unitStatus as SO_PlayerStatus).atkSpeed;
    }

    void ChargeAttack()
    {
        // Charge attack logic
        Anim.SetBool("bAttack", true);
        Anim.speed = (unitStatus as SO_PlayerStatus).atkSpeed;
    }

    void MaximumChargeAttack()
    {
        // Maximum charge attack logic
        Anim.SetBool("bAttack", true);
        Anim.speed = (unitStatus as SO_PlayerStatus).atkSpeed;
    }

    void StartAttack()
    {

    }

    void EndAttack()
    {
        // End attack logic
        Anim.SetBool("bAttack", false);
        Anim.SetTrigger("tIdle");
        Anim.speed = 1f;//(unitStatus as SO_PlayerStatus).atkSpeed;
    }

    void EnableWeaponCollider()
    {
        // Enable weapon collider logic
        weaponCollider.enabled = true;
    }

    void DisableWeaponCollider()
    {
        // Disable weapon collider logic
        weaponCollider.enabled = false;
    }

    // lookat mouse position
    protected void LookAtMouse()
    {
        if(isLockRotate)
            return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100))
        {
            Vector3 lookAt = hit.point;
            lookAt.y = transform.position.y;
            transform.LookAt(lookAt);
        }
    }

}
