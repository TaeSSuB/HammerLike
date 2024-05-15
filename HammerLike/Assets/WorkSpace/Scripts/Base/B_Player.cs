using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// B_Player : Player Base Class
/// </summary>
public class B_Player : B_UnitBase
{
    public enum RotateType
    {
        None,
        LookAtMouse,
        LookAtMouseSmooth
    }

    [Header("Player Settings")]
    [SerializeField] private GameObject chargeVFXObj;
    [SerializeField] private float minChargeMoveRate = 0.1f;

    [SerializeField] private GameObject weaponObj;
    [SerializeField] private Renderer weaponRenderer;
    [SerializeField] private Transform weaponTR;
    [SerializeField] private SO_Weapon weaponData;
    [SerializeField] private BoxCollider weaponCollider;

    [SerializeField] private WeaponOrbit weaponOrbit;

    private int atkDamageOrigin;
    private float knockbackPowerOrigin;


    [Header("Camera Settings")]
    [SerializeField] private RotateType rotateType = RotateType.LookAtMouseSmooth;
    [SerializeField] private float rotSpeed = 10f;
    [SerializeField] private float rotDeadZone = 0.1f;
    [SerializeField] private bool AllowRotate_L = true;
    [SerializeField] private bool AllowRotate_R = true;

    public SO_Weapon WeaponData => weaponData;

    public event Action<int> OnHPChanged;
    public event Action<float> OnChargeChanged;

    // OnWeaponEquipped
    public event Action<B_Weapon> OnWeaponEquipped;


    #region Unity Callbacks & Init

    //init override
    public override void Init()
    {
        base.Init();
        // Init logic
        GameManager.Instance.SetPlayer(this);
        chargeVFXObj.SetActive(false);

        var currentWeaponObj = GameManager.Instance.Player.WeaponData;
        var findItem = B_InventoryManager.Instance.playerWeaponContainer.FindItemOnInventory(currentWeaponObj.itemData);

        if(findItem == null) {
            B_InventoryManager.Instance.playerWeaponContainer.AddItem(currentWeaponObj.itemData, 1);
        }
        findItem = B_InventoryManager.Instance.playerWeaponContainer.FindItemOnInventory(currentWeaponObj.itemData);

        EquipWeapon(findItem.ItemObject as SO_Weapon);
    }

    protected override void Update()
    {
        if (!isAlive)
            return;

        InputCharge();

        TrackWeaponDirXZ();
    }

    protected override void FixedUpdate() 
    {
        base.FixedUpdate();

        LookAtMouse();

        InputMovement();
    }

    
    protected override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);

        if (other.CompareTag("Item"))
        {
            var item = other.GetComponent<GroundItem>();

            var inventory = B_InventoryManager.Instance.playerInventory;

            if(item == null) return;
            
            if(item.canPickUp)
            {
                inventory.AddItem(new B_Item(item.item), 1);
            }
            else
            {
                Debug.Log("Directly Use Item : " + item.item.name);
                item.item.Use();
            }

            Destroy(other.gameObject);
        }

        if(other.CompareTag("WeaponCollider") && UnitStatus.currentHP > 0)
        {
            if(isInvincible) return;

            // Get other B_Enemy or B_Boss Class
            B_UnitBase unit = other.GetComponentInParent<B_UnitBase>();

            if (!(unit as B_Enemy || unit as B_Boss)) return;

            // Get hit dir from player
            Vector3 hitDir = (transform.position - unit.transform.position).normalized;

            // Take Damage and Knockback dir from player
            TakeDamage(hitDir, unit.UnitStatus.atkDamage, unit.UnitStatus.knockbackPower);

            Anim.SetTrigger("tHit");

            var vfxPos = other.ClosestPointOnBounds(transform.position);
            B_VFXPoolManager.Instance.PlayVFX(VFXName.Hit, vfxPos);

            if (unitStatus.currentHP > 0)
            {
                B_AudioManager.Instance.PlaySound(AudioCategory.SFX, AudioTag.Battle);
            }
            else
            {
                B_AudioManager.Instance.PlaySound(AudioCategory.SFX, AudioTag.Death);
            }
        }
    }

    #endregion

    #region Input

    private Vector3 InputMovement()
    {
        if(isLockMove)
            return Vector3.zero;

        // Input logic
        Vector3 moveDir = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        moveDir.Normalize();

        Vector3 coordDir = Vector3.zero;

        var move = Move(transform.position + moveDir);
        //Debug.Log("Move - " + move);
        //Debug.Log("ACSN - " + GameManager.instance.ApplyCoordScaleNormalize(moveDir));

        MoveAnim(moveDir);

        if(Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(1))
        {
            ResetAttack();

            if(moveDir == Vector3.zero)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, 100, groundLayer))
                {
                    Vector3 lookAt = hit.point;
                    moveDir = lookAt - transform.position;
                    moveDir.y = 0;
                }
            }
            
            coordDir = GameManager.Instance.ApplyCoordScaleAfterNormalize(moveDir);

            transform.LookAt(coordDir + transform.position);

            Dash(coordDir);

        }

        return moveDir;
    }

    void InputCharge()
    {
        // Charge attack logic
        if (Input.GetMouseButton(0))
        {
            Charging();
        }
        else if (Input.GetMouseButtonUp(0))
        {
            AttackSwitch();
        }
    }

    /// <summary>
    /// LookAtMouse : 마우스 방향으로 캐릭터 회전
    /// - 마우스의 위치를 기준으로 캐릭터를 회전
    /// - LookAtMouseSmooth : 부드러운 회전
    /// </summary>
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
            {
                switch (rotateType)
                {
                    case RotateType.LookAtMouse:
                        transform.LookAt(lookAt);
                        break;
                    case RotateType.LookAtMouseSmooth:
                        Quaternion targetRotation = Quaternion.LookRotation(lookAt - transform.position);
                        Quaternion newRot = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotSpeed);
                        transform.rotation = newRot;
                        break;
                    default:
                        break;
                }
                //transform.LookAt(lookAt);
            }
        }
    }

    #endregion

    #region Action

    void Charging()
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

            foreach (var weaponMat in weaponRenderer.materials)
            {
                weaponMat.SetFloat("_ChargeAmount", normalizeChargeRate * 4f);
            }

            // Move Speed Reduce
            UnitStatus.moveSpeed = (unitStatus as SO_PlayerStatus).MoveSpeedOrigin - (unitStatus as SO_PlayerStatus).MoveSpeedOrigin * Mathf.Clamp(normalizeChargeRate, 0f, 1f - minChargeMoveRate);

            // Cam Zoom
            //zoomCam.orthographicSize = startZoom - (zoomAmount * normalizeChargeRate);
        }

        // 확장성을 위해 폐기
        // Init 해두고 재사용하기엔 편할 듯
        //Anim.SetFloat("fAttackSpd", (unitStatus as SO_PlayerStatus).atkSpeed);
        OnChargeChanged?.Invoke(normalizeChargeRate);
    }

    void AttackSwitch()
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

    public override void Attack()
    {
        // Attack logic
        Anim.SetBool("bAttack", true);
        Anim.SetBool("IsInWardAttack", true);
        // Attack damage = Original attack damage

        Anim.speed = (unitStatus as SO_PlayerStatus).atkSpeed;

        // Temp 240508
        AllowRotate_R = false;
    }

    void ChargeAttack()
    {
        // Charge attack logic
        Anim.SetBool("bAttack", true);
        Anim.SetBool("IsOutWardAttack", true);

        ApplyChargeDamage();

        Anim.speed = (unitStatus as SO_PlayerStatus).atkSpeed;

        // Temp 240508
        AllowRotate_L = false;
    }

    void MaximumChargeAttack()
    {
        // Maximum charge attack logic
        Anim.SetBool("bAttack", true);
        Anim.SetBool("IsOutWardAttack", true);

        ApplyChargeDamage();

        Anim.speed = (unitStatus as SO_PlayerStatus).atkSpeed;

        // Temp 240508
        AllowRotate_L = false;
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

        foreach (var weaponMat in weaponRenderer.materials)
        {
            weaponMat.SetFloat("_ChargeAmount", 0f);
        }

        //zoomCam.orthographicSize = startZoom;
        ResetDamage();

        AllowRotate_L = true;
        AllowRotate_R = true;
    }

    #region .Movement
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

    #region ..Dash
    void Dash(Vector3 inDir)
    {
        StartCoroutine(DashCoroutine(inDir));
    }

    private IEnumerator DashCoroutine(Vector3 coordDir)
    {
        float dashTime = (unitStatus as SO_PlayerStatus).dashDuration;
        float dashSpeed = (unitStatus as SO_PlayerStatus).dashSpeed;

        StartDash();

        //transform.LookAt(coordDir + transform.position);
        var lookAt = coordDir + transform.position;

        Debug.DrawLine(transform.position, coordDir + transform.position, Color.red, 3f);

        agent.isStopped = true;
        agent.enabled = false;
        Rigid.velocity = Vector3.zero;

        while (dashTime > 0)
        {
            // move with rigid body
            Rigid.velocity = coordDir * dashSpeed;
            dashTime -= Time.deltaTime;
            yield return null;
        }
        
        Rigid.velocity = Vector3.zero;
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

    #endregion

    protected override void Dead()
    {
        base.Dead();
        Anim.SetTrigger("tDeath");
    }

    #endregion

    #region Control & Apply Status Data

    protected override void ApplyStatus()
    {
        unitStatus = Instantiate(GameManager.Instance.PlayerStatus);
        //unitStatus = GameManager.Instance.PlayerStatus;
    }

    void ApplyChargeDamage()
    {
        var resultAtk = (unitStatus as SO_PlayerStatus).atkDamage * (unitStatus as SO_PlayerStatus).chargeRate;

        (unitStatus as SO_PlayerStatus).atkDamage = (int)resultAtk;
    }
    void ResetDamage()
    {
        (unitStatus as SO_PlayerStatus).atkDamage = atkDamageOrigin;
    }

    public override void TakeDamage(Vector3 damageDir, int damage = 0, float knockBackPower = 0, bool knockBack = true)
    {
        base.TakeDamage(damageDir, damage, knockBackPower, knockBack);

        OnHPChanged?.Invoke(unitStatus.currentHP);
    }

    public override void RestoreHP(int hpRate = 0)
    {
        base.RestoreHP(hpRate);

        OnHPChanged?.Invoke(unitStatus.currentHP);
    }

    public void EquipWeapon(SO_Weapon inWeapon)
    {
        UnEquipWeapon();

        weaponData = Instantiate(inWeapon);
        
        weaponObj = Instantiate(weaponData.prefab, weaponTR);
        
        B_Weapon b_Weapon = weaponObj.GetComponent<B_Weapon>();
        
        if(b_Weapon == null)
            b_Weapon = weaponObj.GetComponentInChildren<B_Weapon>();
        
        if(b_Weapon == null)
        {
            Debug.LogError("WeaponObj is not B_Weapon or is not in child.");
            return;
        }

        weaponRenderer = b_Weapon.MeshObj.GetComponent<Renderer>();

        if(weaponRenderer == null)
            weaponRenderer = b_Weapon.MeshObj.GetComponentInChildren<Renderer>();

        if(weaponRenderer == null)
        {
            Debug.LogError("weaponRenderer is not in B_Weapon.MeshObj or is not in child.");
            return;
        }

        weaponOrbit.ApplyWeapon(b_Weapon);

        chargeVFXObj = b_Weapon.VFXObj;

        // Status Apply
        (unitStatus as SO_PlayerStatus).atkDamage = weaponData.attackPower;
        atkDamageOrigin = (unitStatus as SO_PlayerStatus).atkDamage;

        (unitStatus as SO_PlayerStatus).knockbackPower = weaponData.knockbackPower;
        knockbackPowerOrigin = (unitStatus as SO_PlayerStatus).knockbackPower;
        
        (unitStatus as SO_PlayerStatus).atkRange = weaponData.weaponRange;
        (unitStatus as SO_PlayerStatus).atkSpeed = weaponData.attackSpeed;

        (unitStatus as SO_PlayerStatus).chargeRate = 1;

        OnWeaponEquipped?.Invoke(b_Weapon);
    }

    public void UnEquipWeapon()
    {
        Destroy(weaponObj);
        weaponObj = null;
        weaponRenderer = null;

        weaponData = null;

        // Status Apply
        (unitStatus as SO_PlayerStatus).atkDamage = 1;
        atkDamageOrigin = 1;

        (unitStatus as SO_PlayerStatus).knockbackPower = 1;
        knockbackPowerOrigin = 1;
        
        (unitStatus as SO_PlayerStatus).atkRange = 1f;
        (unitStatus as SO_PlayerStatus).atkSpeed = 1f;

        (unitStatus as SO_PlayerStatus).chargeRate = 1;

        OnWeaponEquipped?.Invoke(null);
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
        weaponRenderer.material.SetFloat("_ChargeAmount", 0f);
        //zoomCam.orthographicSize = startZoom;

        ResetDamage();

        AllowRotate_L = true;
        AllowRotate_R = true;
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

    #endregion

    #region Weapon

    /// <summary>
    /// TrackWeaponDirXZ : 무기의 방향을 XZ축으로 추적
    /// - weaponOrbit의 위치를 기준으로 무기의 방향을 Tracking
    /// </summary>
    void TrackWeaponDirXZ()
    {
        Vector3 dir = weaponOrbit.gameObject.transform.position - weaponCollider.gameObject.transform.position;
        float coordScale = GameManager.Instance.CalcCoordScale(dir);

        Quaternion newRot = Quaternion.LookRotation(dir);
        weaponCollider.gameObject.transform.rotation = Quaternion.Euler(0, newRot.eulerAngles.y, 0);

        weaponCollider.gameObject.transform.localScale = new Vector3(1, 1, coordScale);
    }

    #endregion

}
