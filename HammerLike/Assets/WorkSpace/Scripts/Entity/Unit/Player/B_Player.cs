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
    public LayerMask mouseLayer;
    private Vector3 currentMovePos = Vector3.zero;
    private Vector3 currentDashPos = Vector3.zero;

    [SerializeField] private GameObject chargeVFXObj;
    [SerializeField] private float minChargeMoveRate = 0.1f;

    private int atkDamageOrigin;

    private float knockbackPowerOrigin;
    
    private float hitDuration;

    private bool isLockAttack;

    [Header("Weapon Settings")]
    [SerializeField] private GameObject weaponObj;
    [SerializeField] private Transform weaponTR;
    [SerializeField] private SO_Weapon weaponData;
    private Renderer weaponRenderer;

    [Header("Weapon Orbit Settings")]
    [SerializeField] private BoxCollider weaponCollider;
    [SerializeField] private WeaponOrbitPlayer weaponOrbit;
    [SerializeField] private float minAttackRotAmount = 20f;
    private Vector3 attackStartDir;
    private float attackSign;

    [Header("Camera Settings")]
    [SerializeField] private RotateType rotateType = RotateType.LookAtMouseSmooth;
    [SerializeField] private float rotSpeed = 10f;
    [SerializeField] private float atkRotSpeed = 5f;
    [SerializeField] private float rotDeadZone = 0.1f;

    public SO_Weapon WeaponData => weaponData;

    public int AtkDamageOrigin => atkDamageOrigin;

    #region Events
    public event Action<int> OnHPChanged;
    public event Action<float> OnChargeChanged;
    public event Action<B_Weapon> OnWeaponEquipped;
    public event Action OnPlayerDeath;
    #endregion

    [Header("Temp")]
    [SerializeField] private SceneLoader sceneLoader;

    #region Unity Callbacks & Init

    public float interactionRange = 2f;  // 상호작용 거리
    public KeyCode interactionKey = KeyCode.F;  // 상호작용 키
    private LineRenderer lineRenderer;
    private Vector3 mousePosition;

    //init override
    public override void Init()
    {
        base.Init();
        // Init logic
        GameManager.Instance.SetPlayer(this);

        // Temp - 피격 시간. 일단 넉백 시간으로 임시 할당
        hitDuration = GameManager.Instance.SystemSettings.KnockbackDuration;

        chargeVFXObj.SetActive(false);

        var currentWeaponObj = GameManager.Instance.Player.WeaponData;
        
        currentWeaponObj.Use();


        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.positionCount = 0;
        lineRenderer.startWidth = 0.05f;
        lineRenderer.endWidth = 0.05f;
        lineRenderer.loop = true;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = Color.cyan;
        lineRenderer.endColor = Color.cyan;

        lineRenderer.sortingLayerName = "Default";
        lineRenderer.sortingOrder = 5;

        //findItem.ItemObject.Use();
        //EquipWeapon(findItem.ItemObject as SO_Weapon);

        if (sceneLoader != null)
            OnPlayerDeath += sceneLoader.PlayerDead;
    }

    protected override void Update()
    {
        if (!isAlive)
            return;

        InputCharge();

        currentMovePos = InputMovement();
        currentDashPos = InputDash(currentMovePos);
        TrackWeaponDirXZ();
        HandleInteraction();
        
    }

    protected override void FixedUpdate() 
    {
        base.FixedUpdate();

        LookAtMouse();

        UpdateDashCoolTime();

        ApplyMovement(currentMovePos);
        ApplyDash(currentDashPos);
    }

    
    protected override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);

        if (other.CompareTag("Item"))
        {
            var item = other.GetComponent<GroundItem>();

            var inventory = B_InventoryManager.Instance.playerInventory;

            if(item == null) return;

            if (!item.canPurcahse|| item.item.itemType == ItemType.Gold)
            {

                if (item.isPickUpItem)
                {
                    inventory.AddItem(new B_Item(item.item), 1);
                }
                else
                {
                    Debug.Log("Directly Use Item : " + item.item.name);
                    item.item.Use();
                }
                if(item.item.itemType!=ItemType.Gold)
                inventory.goldAmount -= item.item.itemData.itemPrice;
                Destroy(other.gameObject);
                B_UIManager.Instance.UpdateGoldUI(B_InventoryManager.Instance.playerInventory.goldAmount);
            }
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
            TakeDamage(hitDir, unit.UnitStatus.atkDamage, unit.UnitStatus.knockbackPower, true);
            StartCoroutine(CoHitEvent(1f, hitDuration));

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
        if(IsLockMove)
            return Vector3.zero;

        // Input logic
        Vector3 moveDir = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        moveDir.Normalize();

        Vector3 coordDir = GameManager.Instance.ApplyCoordScaleAfterNormalize(moveDir);

        return coordDir;
    }

    private void ApplyMovement(Vector3 inMoveDir)
    {
        var move = Move(transform.position + inMoveDir);
        //Debug.Log("Move - " + move);
        //Debug.Log("ACSN - " + GameManager.instance.ApplyCoordScaleNormalize(moveDir));

        MoveAnim(inMoveDir);
    }

    private Vector3 InputDash(Vector3 inDashDir)
    {
        if(Input.GetKey(KeyCode.Space) || Input.GetMouseButton(1))
        {
            if(inDashDir == Vector3.zero)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, 100, mouseLayer))
                {
                    Vector3 lookAt = hit.point;
                    inDashDir = lookAt - transform.position;
                    inDashDir = GameManager.Instance.ApplyCoordScaleAfterNormalize(inDashDir);
                }
            }

            return inDashDir;
        }

        return Vector3.zero;
    }

    private void ApplyDash(Vector3 inDashDir)
    {
        if(inDashDir == Vector3.zero)
            return;
        if(IsLockMove)
            return;
        if((UnitStatus as SO_PlayerStatus).dashCooldown > 0)
            return;

        transform.LookAt(inDashDir + transform.position);
        Dash(inDashDir);
    }

    void InputCharge()
    {
        if(isLockAttack || isAttacking)
            return;
            
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
        if(IsLockRotate)
            return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100, mouseLayer))
        {
            // mouseLayer 상의 마우스 위치
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

                        // 캐릭터 - 마우스 방향 벡터 & 각도 계산
                        var lookAtDir = lookAt - transform.position;
                        Quaternion targetRotation = Quaternion.LookRotation(lookAtDir);

                        // 공격 여부에 따라 회전 속도 변경
                        float currentRotSpeed = IsAttacking ? atkRotSpeed : rotSpeed;

                        // 공격 상태, 마우스 방향으로 회전 판별
                        if (IsAttacking)
                        {
                            
                            // 공격 시작 방향과 마우스 방향의 각도 계산
                            float signedAngle = Vector3.SignedAngle(attackStartDir, lookAtDir, Vector3.up);

                            // attackSign == 0, 즉 공격 시작 방향이 없으면 공격 시작 방향 설정
                            if(attackSign == 0)
                            {
                                // SignedAngle 부호로 좌우 방향 판별
                                attackSign = Mathf.Sign(signedAngle);

                                // 일정 각도 이상 차이 없으면 고정 (수직 공격)
                                if(Mathf.Abs(signedAngle) < minAttackRotAmount)
                                {
                                    attackSign = 0;
                                }
                                
                                // 애니메이션 적용
                                Anim.SetFloat("fAttackX", attackSign);

                            }

                          

                            // 공격 시작 방향과 마우스 방향이 일정 각도 이상 차이가 나면 회전
                            if (Quaternion.Angle(targetRotation, transform.rotation) > minAttackRotAmount)
                            {
                                // Lerp 등의 함수는 방향에 상관없이 항상 최소 이동 거리만을 반환함.
                                // 회전 방향을 고정하기 위해 대신 Rotate와 DeltaTime 사용
                                transform.Rotate(Vector3.up, attackSign * Time.deltaTime * currentRotSpeed);
                            }
                        }
                        else
                        {
                            // 회전 속도 적용, 부드러운 회전
                            Quaternion newRot = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * currentRotSpeed);

                            transform.rotation = newRot;
                        }
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

    // Temp 20240411 - Hit Event (Shader) a.HG
    protected void HitEvent(float amount = 1f)
    {
        amount = Mathf.Clamp01(amount);

        var renderers  = MeshObj?.GetComponentsInChildren<Renderer>();

        if (renderers.Length > 0)
        {
            foreach (var renderer in renderers)
            {
                foreach (var material in renderer.materials)
                {
                    material.SetFloat("_HitAmount", amount);
                }
            }
        }
    }

    IEnumerator CoHitEvent(float amount = 1f, float duration = 0.5f)
    {
        amount = Mathf.Clamp01(amount);

        var renderers  = MeshObj?.GetComponentsInChildren<Renderer>();

        if (renderers.Length > 0)
        {
            foreach (var renderer in renderers)
            {
                foreach (var material in renderer.materials)
                {
                    material.SetFloat("_HitAmount", amount);
                }
            }
        }

        yield return new WaitForSeconds(duration);

        if (renderers.Length > 0)
        {
            foreach (var renderer in renderers)
            {
                foreach (var material in renderer.materials)
                {
                    material.SetFloat("_HitAmount", 0f);
                }
            }
        }
    }

    void Charging()
    {
        (unitStatus as SO_PlayerStatus).chargeRate += Time.deltaTime * (unitStatus as SO_PlayerStatus).chargeRateIncrease;

        // clamp charge
        (unitStatus as SO_PlayerStatus).chargeRate = Mathf.Clamp((unitStatus as SO_PlayerStatus).chargeRate, 1f, (unitStatus as SO_PlayerStatus).maxChargeRate);

        // 0 ~ 1 사이의 값으로 정규화
        float normalizeChargeRate = ((unitStatus as SO_PlayerStatus).chargeRate - 1) / ((unitStatus as SO_PlayerStatus).maxChargeRate - 1);

        var chargeRate = (unitStatus as SO_PlayerStatus).chargeRate;
        var maxChargeRate = (unitStatus as SO_PlayerStatus).maxChargeRate;

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
               if(chargeRate >= maxChargeRate)
            {
                DrawSkillRange();
            }
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

        attackStartDir = transform.forward;

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
        if(weaponData.itemSkill==null)
        {
            Anim.SetBool("bAttack", true);
            Anim.SetBool("IsOutWardAttack", true);
            if (weaponData.itemSkill.defaultAttack==true)
                ApplyChargeDamage();
        }
        else
        {
            Anim.SetTrigger("tActiveWeaponSkill");
            ClearSkillRange();
        }

        Anim.speed = (unitStatus as SO_PlayerStatus).atkSpeed;
    }

    /// <summary>
    /// 240505 a.HG : 임시 공격 리셋 메서드
    /// </summary>
    void ResetAttack()
    {
        // Reset attack logic
        SetAttacking = false;
        Anim.SetBool("bAttack", false);
        Anim.SetBool("IsOutWardAttack", false);
        Anim.SetBool("IsInWardAttack", false);
        
        if(!weaponOrbit.instanceMode)
            weaponOrbit.trailRenderer.Clear();
        
        DisableWeaponCollider();
        chargeVFXObj.SetActive(false);

        foreach (var weaponMat in weaponRenderer.materials)
        {
            weaponMat.SetFloat("_ChargeAmount", 0f);
        }

        //zoomCam.orthographicSize = startZoom;
        ResetDamage();
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

        StartDash();

        Debug.DrawLine(transform.position, coordDir + transform.position, Color.red, 3f);

        while (dashTime > 0)
        {
            // move with rigid body
            Move(transform.position + coordDir, true);

            dashTime -= Time.deltaTime;
            yield return null;
        }

        EndDash();
    }

    /// <summary>
    /// StartDash : 대쉬 시작
    /// moveSpeed를 대쉬 속도로 변경하고, 대쉬 상태를 고정
    /// </summary>
    protected void StartDash()
    {
        ResetAttack();

        // Start dash logic
        Anim.SetTrigger("tEvasion");
        Anim.SetInteger("iEvasion", (int)(unitStatus as SO_PlayerStatus).evasionType);
        Anim.speed = 1.2f / (unitStatus as SO_PlayerStatus).dashDuration;//(unitStatus as SO_PlayerStatus).dashSpeed;
        (unitStatus as SO_PlayerStatus).moveSpeed = (unitStatus as SO_PlayerStatus).dashSpeed;

        isLockAttack = true;
        SetInvincible(true);
        DisableMovementAndRotation();
    }
    
    /// <summary>
    /// EndDash : 대쉬 종료
    /// moveSpeed를 원래대로 돌리고, 대쉬 상태를 초기화
    /// </summary>
    protected void EndDash()
    {
        // Temp. 리셋 두번은 필요 없을 듯? a.HG
        ResetAttack();

        // End dash logic & Initialize
        Anim.SetTrigger("tIdle");
        Anim.speed = 1f;//(unitStatus as SO_PlayerStatus);
        (unitStatus as SO_PlayerStatus).moveSpeed = (unitStatus as SO_PlayerStatus).MoveSpeedOrigin;

        isLockAttack = false;
        SetInvincible(false);
        EnableMovementAndRotation();

        (unitStatus as SO_PlayerStatus).dashCooldown = (unitStatus as SO_PlayerStatus).DashCooldownOrigin;
    }

    #endregion

    #endregion

    protected override void Dead(bool isSelf = false)
    {
        base.Dead(isSelf);
        Anim.SetTrigger("tDeath");

        isLockAttack = true;

        DisableWeaponCollider();

        B_AudioManager.Instance.PlaySound("Player_Death", AudioCategory.SFX);

        OnPlayerDeath?.Invoke();
    }

    #endregion

    #region Control & Apply Status Data

    protected override void InitStatus()
    {
        unitStatus = Instantiate(GameManager.Instance.PlayerStatus);
        //unitStatus = GameManager.Instance.PlayerStatus;
    }

    void ApplyWeaponStatus(SO_Weapon inWeapon)
    {
        if(inWeapon != null)
        {
            // Status Apply
            (unitStatus as SO_PlayerStatus).atkDamage = inWeapon.attackPower;
            atkDamageOrigin = (unitStatus as SO_PlayerStatus).atkDamage;

            (unitStatus as SO_PlayerStatus).knockbackPower = inWeapon.knockbackPower;
            knockbackPowerOrigin = (unitStatus as SO_PlayerStatus).knockbackPower;
            
            (unitStatus as SO_PlayerStatus).atkRange = inWeapon.weaponRange;
            (unitStatus as SO_PlayerStatus).atkSpeed = inWeapon.attackSpeed;

            (unitStatus as SO_PlayerStatus).chargeRate = 1;

            (unitStatus as SO_PlayerStatus).evasionType = inWeapon.evasionType;
        }
        else
        {
            // Status Apply
            (unitStatus as SO_PlayerStatus).atkDamage = 1;
            atkDamageOrigin = 1;

            (unitStatus as SO_PlayerStatus).knockbackPower = 1;
            knockbackPowerOrigin = 1;
            
            (unitStatus as SO_PlayerStatus).atkRange = 1f;
            (unitStatus as SO_PlayerStatus).atkSpeed = 1f;

            (unitStatus as SO_PlayerStatus).chargeRate = 1;

            (unitStatus as SO_PlayerStatus).evasionType = 0;
        }
    }

    void ApplyWeaponResources(SO_Weapon inWeapon)
    {
        if(inWeapon != null)
        {
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

            weaponRenderer = b_Weapon.MeshRenderer;

            //weaponOrbit.ApplyWeapon(b_Weapon);

            chargeVFXObj = b_Weapon.VFXObj;

            OnWeaponEquipped?.Invoke(b_Weapon);
        }
        else
        {
            Destroy(weaponObj);
            weaponObj = null;
            weaponRenderer = null;

            weaponData = null;

            OnWeaponEquipped?.Invoke(null);
        }
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
    public override void TakeDamage(Vector3 damageDir, int damage = 0, float knockBackPower = 0, bool knockBack = true, bool slowMotion = false, bool isForced = false)
    {
        base.TakeDamage(damageDir, damage, knockBackPower, knockBack, slowMotion);

        OnHPChanged?.Invoke(unitStatus.currentHP);
    }

    public override void RestoreHP(int hpRate = 0)
    {
        base.RestoreHP(hpRate);

        OnHPChanged?.Invoke(unitStatus.currentHP);
    }

    protected void UpdateDashCoolTime()
    {
        (UnitStatus as SO_PlayerStatus).dashCooldown -= Time.deltaTime;
    }

    public void EquipWeapon(SO_Weapon inWeapon)
    {
        UnEquipWeapon();

        ApplyWeaponResources(inWeapon);

        ApplyWeaponStatus(inWeapon);
    }

    public void UnEquipWeapon()
    {
        ApplyWeaponResources(null);

        ApplyWeaponStatus(null);
    }

    #endregion

    #region Animation Event

    public void StartAttackDownWard()
    {
        StartAttack();
    }

    public void EndAttackDownWard()
    {
        EndAttack();
    }

    public void StartAttackOutWard()
    {
        StartAttack();
    }

    public void EndAttackOutWard()
    {
        EndAttack();
    }

    public void StartAttackInWard()
    {
        StartAttack();
    }

    public void EndAttackInWard()
    {
        EndAttack();
    }

    public void EndActiveSkill()
    {
        //if(attackSign != 1f) return;

        Debug.Log("EndActiveSkill");

        EndAttack();
    }

    public override void StartAttack()
    {
        base.StartAttack();

        agent.updateRotation=false;

        transform.LookAt(transform.position + attackStartDir);

        UnitStatus.moveSpeed = UnitStatus.MoveSpeedOrigin * 0.2f;

        B_AudioManager.Instance.PlaySound("Attack_Hammer", AudioCategory.SFX);
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
        
        foreach (var weaponMat in weaponRenderer.materials)
        {
            weaponMat.SetFloat("_ChargeAmount", 0f);
        }

        //zoomCam.orthographicSize = startZoom;

        ResetDamage();

        agent.updateRotation = true;

        DisableWeaponCollider();
        attackSign = 0f;

        UnitStatus.moveSpeed = UnitStatus.MoveSpeedOrigin;
    }

    void EnableWeaponCollider()
    {
        if(!weaponOrbit.instanceMode)
        {
            weaponOrbit.trailRenderer.Clear();        
            weaponOrbit.trailRenderer.emitting = true;
        }
        else
        {
            weaponOrbit.AddTrackInstanceTrail();
        }
            
        // Enable weapon collider logic
        weaponCollider.enabled = true;
    }
    void DisableWeaponCollider()
    {
        // Disable weapon collider logic
        if(!weaponOrbit.instanceMode)
        {
            weaponOrbit.trailRenderer.emitting = false;
        }
        else
        {
            weaponOrbit.ClearTrackInstanceTrail();
        }
        
        weaponCollider.enabled = false;
    }

    void TempAttackVFXEvent()
    {
        var dir = weaponOrbit.gameObject.transform.position - gameObject.transform.position;
        var dirNormalized = dir.normalized;

        B_VFXPoolManager.Instance.PlayVFX(VFXName.Hit, weaponOrbit.gameObject.transform.position - dirNormalized * 0.5f);
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

    void ActiveWeaponSkill()
    {
        //Vector3 targetPosition = GetMouseWorldPosition();

        if (weaponData.itemSkill != null)
        {
            weaponData.ActivateWeaponSkill(mousePosition, transform);
        }
    }

    

    #endregion
    void OnDestroy()
    {
        if(sceneLoader != null)
            OnPlayerDeath -= sceneLoader.PlayerDead;
    }

    private Vector3 GetMouseWorldPosition()
    {
        Plane playerPlane = new Plane(Vector3.up, transform.position);
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        float hitdist;

        if (playerPlane.Raycast(ray, out hitdist))
        {
            return ray.GetPoint(hitdist);
        }

        return transform.position;
    }



    #region Gizmos
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.forward * 2f);

        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position, currentMovePos * 2f);

        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, currentDashPos * 2f);

        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position, attackStartDir * 10f);

        
    }


    private void DrawSkillRange()
    {
        mousePosition = GetMouseWorldPosition();
        DrawCircle(mousePosition, 3f);
    }

    private void DrawCircle(Vector3 center, float radius)
    {
        int segments = 50;
        lineRenderer.positionCount = segments + 1;
        float angle = 0f;

        for (int i = 0; i < segments + 1; i++)
        {
            float x = Mathf.Sin(Mathf.Deg2Rad * angle) * radius;
            float z = Mathf.Cos(Mathf.Deg2Rad * angle) * radius;
            lineRenderer.SetPosition(i, center + new Vector3(x, 0, z));
            angle += 360f / segments;
        }
    }

    private void ClearSkillRange()
    {
        lineRenderer.positionCount = 0;
    }
    #endregion


    #region Interaction
    private void HandleInteraction()
    {
        if (Input.GetKeyDown(interactionKey))
        {
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, interactionRange);
            Collider closestCollider = null;
            float closestDistance = float.MaxValue;

            foreach (var hitCollider in hitColliders)
            {
                if (hitCollider.CompareTag("NPC") || hitCollider.CompareTag("Item"))
                {
                    float distance = Vector3.Distance(transform.position, hitCollider.transform.position);
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestCollider = hitCollider;
                    }
                }
            }

            if (closestCollider != null)
            {
                if (closestCollider.CompareTag("NPC"))
                {
                    var npc = closestCollider.GetComponent<B_NPC>();
                    if (npc != null)
                    {
                        npc.StartInteraction();
                    }
                }
                else if (closestCollider.CompareTag("Item"))
                {
                    var item = closestCollider.GetComponent<GroundItem>();
                    if (item != null)
                    {
                        PickUpItem(item);
                    }
                }
            }
        }
    }






    private void PickUpItem(GroundItem item)
    {
        if (item != null)
        {
            var inventory = B_InventoryManager.Instance.playerInventory;
            if (inventory.goldAmount >= item.item.itemData.itemPrice || item.item.itemType == ItemType.Gold)
            {
                if (item.isPickUpItem)
                {
                    inventory.AddItem(new B_Item(item.item), 1);
                }
                else
                {
                    Debug.Log("Directly Use Item: " + item.item.name);
                    item.item.Use();
                }
                if (item.item.itemType != ItemType.Gold)
                    inventory.goldAmount -= item.item.itemData.itemPrice;
                Destroy(item.gameObject);
                B_UIManager.Instance.UpdateGoldUI(inventory.goldAmount);
            }
        }
    }


    #endregion


}
