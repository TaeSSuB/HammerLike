using HutongGames.PlayMaker.Actions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

// Player Unit

public class B_Player : B_UnitBase
{
    [Header("Player Settings")]
    
    [SerializeField] private GameObject chargeVFXObj;
    [SerializeField] private GameObject weaponObj;

    // a.HG - 240505 무기 콜라이더 오브젝트 배치 및 회전 값으로 일괄 처리
    //[SerializeField] private GameObject weaponTrackObj;
    [SerializeField] private BoxCollider weaponCollider;
    //private Vector3 initWeaponColliderScale;
    //private Vector3 initWeaponColliderCenter;

    [SerializeField] private WeaponOrbit weaponOrbit;

    [Header("Camera Settings")]
    [SerializeField] private Camera zoomCam;
    [SerializeField] private float zoomAmount;
    [SerializeField] private float startZoom;
    [SerializeField] private float rotDeadZone = 0.1f;

    public event Action<int> OnHPChanged;
    public event Action<float> OnChargeChanged;

    protected override void ApplyStatus()
    {
        unitStatus = Instantiate(GameManager.Instance.PlayerStatus);
    }

    void TrackWeaponDirXZ()
    {
        Vector3 dir = weaponOrbit.gameObject.transform.position - weaponCollider.gameObject.transform.position;
        float coordScale = GameManager.Instance.CalcCoordScale(dir);

        Quaternion newRot = Quaternion.LookRotation(dir);
        weaponCollider.gameObject.transform.rotation = Quaternion.Euler(0, newRot.eulerAngles.y, 0);

        weaponCollider.gameObject.transform.localScale = new Vector3(1, 1, coordScale);
        //weaponCollider.size = new Vector3(initWeaponColliderScale.x, initWeaponColliderScale.y * coordScale, initWeaponColliderScale.z);
        //weaponCollider.center = new Vector3(initWeaponColliderCenter.x, initWeaponColliderCenter.y * coordScale, initWeaponColliderCenter.z);
        //weaponCollider.size = initWeaponColliderScale * coordScale;
        //weaponCollider.center = initWeaponColliderCenter * coordScale;
    }

    //init override
    public override void Init()
    {
        base.Init();
        // Init logic
        GameManager.Instance.SetPlayer(this);
        chargeVFXObj.SetActive(false);
        
        // Get Weapon Collider Component
        //weaponCollider.enabled = false;
        //weaponCollider.isTrigger = true;
        //initWeaponColliderScale = weaponCollider.size;
        //initWeaponColliderCenter = weaponCollider.center;

        startZoom = zoomCam.orthographicSize;
    }

    protected override void Update()
    {
        base.Update();

        InputMovement();
        LookAtMouse();
        CheckCharge();
        TrackWeaponDirXZ();


        // Y Scale
        //weaponCollider.size = new Vector3(initWeaponColliderScale.x, initWeaponColliderScale.y * coordScale, initWeaponColliderScale.z);
        //weaponCollider.center = new Vector3(initWeaponColliderCenter.x, initWeaponColliderCenter.y * coordScale, initWeaponColliderCenter.z);
        //weaponCollider.size = initWeaponColliderScale * coordScale;
        //weaponCollider.center = initWeaponColliderCenter * coordScale;
    }

    private Vector3 InputMovement()
    {
        if(isLockMove)
            return Vector3.zero;

        // Input logic
        Vector3 moveDir = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        moveDir.Normalize();

        var move = Move(transform.position + moveDir);
        //Debug.Log("Move - " + move);
        //Debug.Log("ACSN - " + GameManager.instance.ApplyCoordScaleNormalize(moveDir));
        MoveAnim(moveDir);

        if(Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(1))
        {
            ResetAttack();
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
        if(inDir != Vector3.zero)
        {
            Anim.SetBool("IsMoving", true);
        }
        else
        {
            Anim.SetBool("IsMoving", false);
        }
    }

    #region Dash
    void Dash(Vector3 inDir)
    {
        if (inDir == Vector3.zero)
        {
            inDir = transform.forward;
        }

        StartCoroutine(DashCoroutine(inDir));
    }

    private IEnumerator DashCoroutine(Vector3 inDir)
    {
        float dashTime = (unitStatus as SO_PlayerStatus).dashDuration;
        float dashSpeed = (unitStatus as SO_PlayerStatus).dashSpeed;

        StartDash();

        Vector3 coordDir = GameManager.Instance.ApplyCoordScaleNormalize(inDir);
        //transform.LookAt(coordDir + transform.position);

        Debug.DrawLine(transform.position, coordDir + transform.position, Color.red, 3f);

        agent.isStopped = true;
        agent.enabled = false;
        rigid.velocity = Vector3.zero;

        while (dashTime > 0)
        {
            transform.LookAt(coordDir + transform.position);
            // move with rigid body
            rigid.velocity = coordDir * dashSpeed;
            dashTime -= Time.deltaTime;
            yield return null;
        }
        
        rigid.velocity = Vector3.zero;
        agent.enabled = true;
        agent.isStopped = false;

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

    #endregion
    
    #region Damage
    public override void Attack()
    {
        // Attack logic
        Anim.SetBool("bAttack", true);
        Anim.SetBool("IsInWardAttack", true);
        // Attack damage = Original attack damage

        Anim.speed = (unitStatus as SO_PlayerStatus).atkSpeed;
    }
    void CheckCharge()
    {
        // Charge attack logic
        if (Input.GetMouseButton(0))
        {
            (unitStatus as SO_PlayerStatus).chargeRate += Time.deltaTime * (unitStatus as SO_PlayerStatus).chargeRateIncrease;

            // clamp charge
            (unitStatus as SO_PlayerStatus).chargeRate = Mathf.Clamp((unitStatus as SO_PlayerStatus).chargeRate, 1f, (unitStatus as SO_PlayerStatus).maxChargeRate);

            // 0 ~ 1 사이의 값으로 정규화
            float normalizeChargeRate = ((unitStatus as SO_PlayerStatus).chargeRate - 1) / ((unitStatus as SO_PlayerStatus).maxChargeRate - 1);

            if ((unitStatus as SO_PlayerStatus).chargeRate > (unitStatus as SO_PlayerStatus).minChargeRate)
            {
                Anim.SetBool("IsCharge", true);

                // VFX
                chargeVFXObj.SetActive(true);
                weaponObj.GetComponent<Renderer>().material.SetFloat("_ChargeAmount", normalizeChargeRate * 4f);

                // Move Speed Reduce
                UnitStatus.moveSpeed = (unitStatus as SO_PlayerStatus).MoveSpeedOrigin - (unitStatus as SO_PlayerStatus).MoveSpeedOrigin * normalizeChargeRate;

                // Cam Zoom
                zoomCam.orthographicSize = startZoom - (zoomAmount * normalizeChargeRate);
            }

            // 확장성을 위해 폐기
            // Init 해두고 재사용하기엔 편할 듯
            //Anim.SetFloat("fAttackSpd", (unitStatus as SO_PlayerStatus).atkSpeed);

            OnChargeChanged?.Invoke(normalizeChargeRate);

            // zoom
        }
        else if (Input.GetMouseButtonUp(0))
        {
            Anim.SetBool("IsCharge", false);

            var chargeRate = (unitStatus as SO_PlayerStatus).chargeRate;
            var minChargeRate = (unitStatus as SO_PlayerStatus).minChargeRate;
            var maxChargeRate = (unitStatus as SO_PlayerStatus).maxChargeRate;

            if (chargeRate >= minChargeRate)
            {
                if (chargeRate >= maxChargeRate)
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

            OnChargeChanged?.Invoke(0f);
            (unitStatus as SO_PlayerStatus).chargeRate = 1f;

            UnitStatus.moveSpeed = (unitStatus as SO_PlayerStatus).MoveSpeedOrigin;
        }
    }
    void ChargeAttack()
    {
        // Charge attack logic
        Anim.SetBool("bAttack", true);
        Anim.SetBool("IsOutWardAttack", true);

        ApplyChargeDamage();

        Anim.speed = (unitStatus as SO_PlayerStatus).atkSpeed;
    }
    void MaximumChargeAttack()
    {
        // Maximum charge attack logic
        Anim.SetBool("bAttack", true);
        Anim.SetBool("IsOutWardAttack", true);

        ApplyChargeDamage();

        Anim.speed = (unitStatus as SO_PlayerStatus).atkSpeed;
    }
    void ApplyChargeDamage()
    {
        var resultAtk = (unitStatus as SO_PlayerStatus).atkDamage * (unitStatus as SO_PlayerStatus).chargeRate;

        (unitStatus as SO_PlayerStatus).atkDamage = (int)resultAtk;
    }
    void ResetDamage()
    {
        (unitStatus as SO_PlayerStatus).atkDamage = (unitStatus as SO_PlayerStatus).AtkDamageOrigin;
    }

    public override void TakeDamage(Vector3 damageDir, int damage = 0, bool knockBack = true)
    {
        base.TakeDamage(damageDir, damage, knockBack);

        OnHPChanged?.Invoke(unitStatus.currentHP);
    }

    #endregion

    #region Animation Event
    public override void StartAttack()
    {
        base.StartAttack();
    }
    public override void EndAttack()
    {
        base.EndAttack();
        // End attack logic
        Anim.SetBool("bAttack", false);
        Anim.SetBool("IsOutWardAttack", false);
        Anim.SetBool("IsInWardAttack", false);

        Anim.SetTrigger("tIdle");
        Anim.speed = 1f;//(unitStatus as SO_PlayerStatus).atkSpeed;

        chargeVFXObj.SetActive(false);
        weaponObj.GetComponent<Renderer>().material.SetFloat("_ChargeAmount", 0f);
        zoomCam.orthographicSize = startZoom;

        ResetDamage();
    }

    void EnableWeaponCollider()
    {
        weaponOrbit.trailRenderer.Clear();
        weaponOrbit.trailRenderer.emitting = true;
        // Enable weapon collider logic
        weaponCollider.enabled = true;
    }
    void DisableWeaponCollider()
    {
        // Disable weapon collider logic
        weaponOrbit.trailRenderer.emitting = false;
        weaponCollider.enabled = false;
    }

    /// <summary>
    /// 240505 a.HG : 임시 공격 리셋 메서드
    /// </summary>
    void ResetAttack()
    {
        // Reset attack logic
        isAttacking = false;
        Anim.SetBool("bAttack", false);
        Anim.SetBool("IsOutWardAttack", false);
        Anim.SetBool("IsInWardAttack", false);
        weaponOrbit.trailRenderer.Clear();
        DisableWeaponCollider();
        chargeVFXObj.SetActive(false);
        weaponObj.GetComponent<Renderer>().material.SetFloat("_ChargeAmount", 0f);
        zoomCam.orthographicSize = startZoom;
        ResetDamage();
    }

    #endregion

    // lookat mouse position
    protected void LookAtMouse()
    {
        if(isLockRotate)
            return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100, groundLayer))
        {
            Vector3 lookAt = hit.point;
            lookAt.y = transform.position.y;

            // DeadZone
            if (Vector3.Distance(transform.position, lookAt) > rotDeadZone)
                transform.LookAt(lookAt);
        }
    }

    protected override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);

        if (other.CompareTag("Item"))
        {
            var item = other.GetComponent<GroundItem>();

            var inventory = B_InventoryManager.Instance.playerInventory;

            if (inventory.AddItem(new B_Item(item.item), 1))
                Destroy(other.gameObject);
            //Item _item = new Item(item.item);
            //Debug.Log(_item.Id);
            //inventory.AddItem(_item, 1);
            //Destroy(other.gameObject);
        }
    }
}
