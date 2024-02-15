using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Johnson;
using UnityEngine.AI;
using RootMotion.Dynamics; // RootMotion 라이브러리 참조 추가
public enum MonsterType
{
    Melee,
    Ranged,
    Special
}

[Serializable]
public struct MonsterStat
{
    public float maxHp;
    public float curHp;
    public float attackPoint;
    public float attackRange; // °ø°Ý »çÁ¤°Å¸®
    [Space(7.5f)]
    public float walkSpd;
    public float runSpd;

    [Space(7.5f)]
    public EnvasionStat envasionStat;

    [Space(7.5f)]
    public float upperHomingSpd; //»óÃ¼ È¸Àü ¼Óµµ
    public float legHomingSpd; //ÇÏÃ¼ È¸Àü ¼Óµµ

    public float detectionRange;  // ÇÃ·¹ÀÌ¾î¸¦ ÀÎ½ÄÇÒ ¹üÀ§ ¼³Á¤. ¿øÇÏ´Â °ªÀ¸·Î Á¶Àý °¡´É.

}

[System.Serializable]
public class DropItem
{
    public int itemID; // ¾ÆÀÌÅÛ ID
    public float dropChance; // µå¶ø È®·ü
}


public class Monster : MonoBehaviour
{

    private Transform playerTransform;
    // Note: ±â´É ±¸Çö ÇÒ ¶§´Â Á¢±ÙÁöÁ¤ÀÚ Å©°Ô ½Å°æ ¾È¾²°í ÀÛ¾÷ÇÔ.
    // Â÷ÈÄ ±â´É ÀÛ¾÷ ³¡³ª°í ³ª¸é Ãß°¡ÀûÀ¸·Î Á¤¸® ¿¹Á¤!!
    // ºÒÆíÇØµµ ¾çÇØ¹Ù¶ø´Ï´Ù!!! ½º¸¶¹Ì¼¾!!

    public MonsterStat stat;
    public MonsterType monsterType;
    public Slider healthSlider;
    [Header("State Machine")]
    public MonsterFSM fsm;

    [Header("Ranged Attack Settings")]
    public GameObject ProjectilePrefab; // ¿ø°Å¸® °ø°ÝÀ» À§ÇÑ Åõ»çÃ¼ ÇÁ¸®ÆÕ
    public float ProjectileSpeed; // Åõ»çÃ¼ ¼Óµµ
    public Transform ProjectileSpawnPoint; // ¹ß»çÃ¼ »ý¼º À§Ä¡
    private GameObject currentProjectile;

    [Space(10f)]
    [Header("Default Comps")]
    public Transform meshTr;
    public Animator animCtrl;
    public Rigidbody rd;

    [Space(10f)]
    [Header("Action Table")]
    // Note: ÇØ´ç ºÎºÐÀº ¸ó½ºÅÍ¿¡ ¸Â´Â ¾×¼ÇÀ¸·Î ¼öÁ¤ ÇÊ¿ä
    public MonsterMove move;
    public MonsterAtk atk;
    public MonsterAim monsterAim;

    [Space(10f)]
    [Header("Cam Controller")]
    public CamCtrl camCtrl; // Note: ¸ó½ºÅÍ°¡ Ä«¸Þ¶ó¸¦ Á÷Á¢ Á¦¾îÇÒ ÇÊ¿ä°¡ ÀÖÀ»Áö È®ÀÎ ÇÊ¿ä

    [Header("Drop Items")]
    public List<DropItem> dropItems = new List<DropItem>(); // µå¶ø ¾ÆÀÌÅÛ ¸ñ·Ï

    [Space(10f)]
    [Header("Anim Bones")]
    public Transform headBoneTr;
    public Transform spineBoneTr;
    public Transform hpBoneTr;
    private HashSet<int> processedAttacks = new HashSet<int>();

    private bool isKnockedBack = false;
    private bool canTakeKnockBackDamage = true;

    public Transform target;
    NavMeshAgent nmAgent;
    public LineRenderer lineRenderer; // LineRenderer ÂüÁ¶

    public Collider attackCollider;
    public MeshRenderer attackMeshRenderer;
    private Player player;

    private LineRenderer leftLineRenderer;
    private LineRenderer frontLineRenderer;
    private LineRenderer rightLineRenderer;
    private Vector3 frontKnockbackDirection;
    private Vector3 leftKnockbackDirection;
    private Vector3 rightKnockbackDirection;
    public int debugData = 0;
    private int knockbackData = 0;
    public BehaviourPuppet puppet;
    private void Awake()
    {
        if (!fsm)
        { fsm = GetComponent<MonsterFSM>(); }

        if (!fsm)
        {
            gameObject.AddComponent<MonsterFSM>();
        }

        if (!rd)
        {
            rd = GetComponent<Rigidbody>();
        }
        stat.curHp = stat.maxHp;
    }

    void Start()
    {
        nmAgent = GetComponent<NavMeshAgent>();
        animCtrl.SetBool("IsChasing", true);
        if (healthSlider != null)
        {
            healthSlider.maxValue = stat.maxHp;
            healthSlider.value = stat.curHp;
        }

        // LineRenderer ±âº» ¼³Á¤
        if (lineRenderer != null)
        {
            lineRenderer.positionCount = 2; // ½ÃÀÛÁ¡°ú ³¡Á¡
            lineRenderer.widthMultiplier = 0.05f; // ¼±ÀÇ ³Êºñ
        }

        leftLineRenderer = CreateLineRenderer(Color.red);
        frontLineRenderer = CreateLineRenderer(Color.green);
        rightLineRenderer = CreateLineRenderer(Color.blue);
    }

    void Update()
    {
        if (stat.curHp > 0)
        {
            DetectPlayer();

            if (playerTransform != null)
            {
                ChasePlayer();

            }
            else
            {
                animCtrl.SetBool("IsChasing", false);
                animCtrl.SetTrigger("tIdle");
            }
        }
        else
        {
            animCtrl.SetBool("IsChasing", false);
        }

        if (Input.GetKeyDown(KeyCode.H))
        {
            stat.curHp = 0;
            Die();
        }
        // 플레이어 방향 기반 라인 렌더링 업데이트
        UpdateDirectionLines();
        /*if (Input.GetKeyDown(KeyCode.K))
        {
            ApplyKnockback(frontKnockbackDirection);
        }*/

    }
    void UpdateDirectionLines()
    {
        if (player != null)
        {
            Vector3 playerForward = player.transform.forward;
            Vector3 playerPosition = player.transform.position + Vector3.up * 0.5f;

            // 정면 방향
            Vector3 frontDirection = playerForward;
            // 좌측 대각선 방향
            Vector3 leftDirection = Quaternion.Euler(0, -45, 0) * playerForward;
            // 우측 대각선 방향
            Vector3 rightDirection = Quaternion.Euler(0, 45, 0) * playerForward;

            // 방향 저장
            frontKnockbackDirection = frontDirection;
            leftKnockbackDirection = leftDirection;
            rightKnockbackDirection = rightDirection;

            // 각 방향에 대한 라인 렌더러 설정
            SetLineRenderer(leftLineRenderer, playerPosition, playerPosition + leftDirection * 5); // 5는 라인의 길이
            SetLineRenderer(frontLineRenderer, playerPosition, playerPosition + frontDirection * 5);
            SetLineRenderer(rightLineRenderer, playerPosition, playerPosition + rightDirection * 5);
        }
    }

    private void SetLineRenderer(LineRenderer lineRenderer, Vector3 start, Vector3 end)
    {
        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, end);
    }


    Vector3 CalculateKnockbackDirectionBasedOnContext()
    {
        // 여기서는 예시를 위해 단순화된 로직을 사용합니다.
        // 실제 구현에서는 몬스터의 상태, 위치, 플레이어와의 관계 등을 고려하여 넉백 방향을 계산해야 합니다.
        // 예를 들어, 몬스터가 플레이어를 향하고 있다면, 플레이어와 반대 방향으로 넉백 방향을 설정할 수 있습니다.
        return transform.forward; // 현재는 몬스터가 바라보는 방향으로 설정
    }

    void DrawDirectionLine()
    {
        if (lineRenderer != null)
        {
            lineRenderer.SetPosition(0, transform.position); // ¼±ÀÇ ½ÃÀÛÁ¡: ¸ó½ºÅÍÀÇ À§Ä¡
            lineRenderer.SetPosition(1, transform.position + transform.forward * 5f); // ¼±ÀÇ ³¡Á¡: ¸ó½ºÅÍ°¡ ¹Ù¶óº¸´Â ¹æÇâ
        }
    }

    private void LateUpdate()
    {
        Vector3 lookDir = monsterAim.Aiming();
        //Funcs.LookAtSpecificBone(spineBoneTr, eGizmoDir.Forward, lookDir, Vector3.zero);
    }

    private void FixedUpdate()
    {
        if (playerTransform != null && stat.curHp > 0)
        {
            FaceTarget(); // ÇÃ·¹ÀÌ¾î¸¦ Áö¼ÓÀûÀ¸·Î ¹Ù¶óº¸°Ô ÇÏ´Â ¸Þ¼­µå
        }

    }

    private LineRenderer CreateLineRenderer(Color lineColor)
    {
        GameObject lineRendererObject = new GameObject("LineRenderer");
        lineRendererObject.transform.SetParent(transform);
        LineRenderer lineRenderer = lineRendererObject.AddComponent<LineRenderer>();
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = lineColor;
        lineRenderer.endColor = lineColor;
        lineRenderer.startWidth = 0.05f;
        lineRenderer.endWidth = 0.05f;
        lineRenderer.positionCount = 2;

        return lineRenderer;
    }



    private void OnEnable()
    {

    }
    private void OnDisable()
    {

    }

    private void OnDestroy()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("WeaponCollider"))
        {
            WeaponCollider weaponCollider = other.GetComponent<WeaponCollider>();
            if (weaponCollider != null && !processedAttacks.Contains(weaponCollider.CurrentAttackId))
            {
                // µ¥¹ÌÁö ¹× ³Ë¹é Ã³¸®
                PlayerAtk playerAttack = other.GetComponentInParent<PlayerAtk>();
                if (playerAttack != null)
                {
                    TakeDamage(playerAttack.attackDamage);
                    Vector3 hitPoint = other.ClosestPointOnBounds(transform.position); // 충돌 지점
                    Vector3 knockbackDirection = DetermineKnockbackDirection(hitPoint, other.transform);
                    ApplyKnockback(knockbackDirection);
                    processedAttacks.Add(weaponCollider.CurrentAttackId);
                }

            }
        }
        if (other.gameObject.CompareTag("KnockBackable") && isKnockedBack && canTakeKnockBackDamage)
        {
            TakeDamage(10f); // KnockBackDamage
            canTakeKnockBackDamage = false;
            StartCoroutine(KnockBackDamageCooldown());
        }
    }

    private Vector3 DetermineKnockbackDirection(Vector3 hitPoint, Transform trailMeshTransform)
    {
        // 트레일 메쉬의 길이 계산 (예시 코드, 실제 구현 필요)
        float trailMeshLength = Vector3.Distance(trailMeshTransform.position, trailMeshTransform.position + trailMeshTransform.forward * 10); // 메쉬 길이 예시
        float hitPositionRelative = Vector3.Distance(trailMeshTransform.position, hitPoint); // 피격 지점까지의 거리

        // 피격 위치가 트레일 메쉬의 어느 1/3 구간에 있는지 결정
        // 대각선 넉밴
        float sectionLength = trailMeshLength / 3;
        if (hitPositionRelative <= sectionLength)
        {
            // 우측 대각선 넉백
            return rightKnockbackDirection;
        }
        else if (hitPositionRelative > sectionLength && hitPositionRelative <= sectionLength * 2)
        {
            // 정면 넉백
            return frontKnockbackDirection;
        }
        else
        {
            // 좌측 대각선 넉백
            return leftKnockbackDirection;
        }
    }




    private void ApplyKnockback(Vector3 direction)
    {
        

        float knockbackIntensity = 300f; // 넉백 강도
        direction.y = 0; // Y축 방향을 0으로 설정하여 수평 넉백을 보장
        GetComponent<Rigidbody>().AddForce(direction.normalized * knockbackIntensity, ForceMode.Impulse);
        isKnockedBack = true;
        StartCoroutine(KnockBackDuration());
        knockbackData++;
        Debug.Log(knockbackData);
    }

    private IEnumerator KnockBackDamageCooldown()
    {
        yield return new WaitForSeconds(1f); // ³Ë¹é µ¥¹ÌÁö Äð´Ù¿î
        canTakeKnockBackDamage = true;
    }

    private void TakeDamage(float damage)
    {
        if (stat.curHp <= 0) return; // ÀÌ¹Ì »ç¸ÁÇÑ °æ¿ì µ¥¹ÌÁö¸¦ ¹ÞÁö ¾ÊÀ½

        if (stat.curHp > 0)  // ¸ó½ºÅÍ°¡ »ì¾ÆÀÖÀ» ¶§¸¸ ÇÇ°Ý Ã³¸®
        {
            stat.curHp -= damage;
            if (healthSlider != null)
            {
                healthSlider.value = stat.curHp;
                ShowHealthSlider();  // Ã¼·Â UI ½½¶óÀÌ´õ Ç¥½Ã
            }

            if (stat.curHp <= 0)
            {
                Die();
            }
        }
    }

    /*private void ApplyKnockback(Vector3 direction)
    {
        float knockbackIntensity = 300f; // ³Ë¹é °­µµ
        direction.y = 0; // YÃà º¯È­ Á¦°Å
        GetComponent<Rigidbody>().AddForce(direction.normalized * knockbackIntensity, ForceMode.Impulse);
        isKnockedBack = true;
        StartCoroutine(KnockBackDuration());
    }*/

    private IEnumerator KnockBackDuration()
    {
        yield return new WaitForSeconds(1f); // ³Ë¹é Áö¼Ó ½Ã°£
        isKnockedBack = false;
    }

    private void Die()
    {
        // ¸ó½ºÅÍ »ç¸Á Ã³¸®
        // ¿¹: gameObject.SetActive(false); ¶Ç´Â Destroy(gameObject);
        animCtrl.SetBool("IsChasing", false);
        animCtrl.SetTrigger("tDead");
        DisableAttackCollider();
        DisableAttackMeshRenderer();
        // NavMeshAgent ºñÈ°¼ºÈ­
        if (nmAgent != null && nmAgent.isActiveAndEnabled)
        {
            nmAgent.isStopped = true;
            nmAgent.enabled = false;
            
        }

        playerTransform = null;
        DropItems();
        DisconnectMusclesRecursive();
        //Destroy(gameObject);
    }
    private void DisconnectMusclesRecursive()
    {
        // BehaviourPuppet 컴포넌트가 있는지 확인하고 해당 작업 수행

        if (puppet != null && puppet.puppetMaster != null)
        {
            // 모든 근육을 순회하며 재귀적으로 연결 해제
            for (int i = 0; i < puppet.puppetMaster.muscles.Length; i++)
            {
                puppet.puppetMaster.DisconnectMuscleRecursive(i, MuscleDisconnectMode.Explode);
            }
        }
    }

    private void ShowHealthSlider()
    {
        if (healthSlider != null)
        {
            healthSlider.gameObject.SetActive(true);
            StopCoroutine("HideHealthSlider");  // ÀÌ¹Ì ÁøÇà ÁßÀÎ ÄÚ·çÆ¾ÀÌ ÀÖ´Ù¸é Áß´Ü
            StartCoroutine("HideHealthSlider");  // »õ ÄÚ·çÆ¾ ½ÃÀÛ
        }
    }

    private IEnumerator HideHealthSlider()
    {
        yield return new WaitForSeconds(2f);
        if (healthSlider != null && stat.curHp > 0)  // ¸ó½ºÅÍ°¡ »ì¾ÆÀÖÀ» ¶§¸¸ ½½¶óÀÌ´õ ºñÈ°¼ºÈ­
        {
            healthSlider.gameObject.SetActive(false);
        }
    }
    public Player Player
    {
        get { return player; }
    }
    private void DetectPlayer()
    {
        if (stat.curHp <= 0) return; // Ã¼·ÂÀÌ 0 ÀÌÇÏ¸é °¨Áö ÁßÁö
        if (Vector3.Distance(transform.position, target.position) <= stat.detectionRange)
        {
            playerTransform = target; // ±âÁ¸ ·ÎÁ÷À» À¯Áö
            player = target.GetComponent<Player>(); // target¿¡¼­ Player ÄÄÆ÷³ÍÆ®¸¦ °¡Á®¿È

            if (player != null)
            {
                monsterAim.SetTarget(target); // MonsterAim ½ºÅ©¸³Æ®¿¡µµ Å¸°Ù ¼³Á¤
            }
        }
        else
        {
            playerTransform = null;
            player = null; // Player ÂüÁ¶µµ ÇØÁ¦
            monsterAim.SetTarget(null); // MonsterAim ½ºÅ©¸³Æ®ÀÇ Å¸°Ùµµ ÇØÁ¦
        }
    }


    void ChasePlayer()
    {
        if (stat.curHp <= 0 || animCtrl.GetBool("IsAttacking") || animCtrl.GetBool("IsAiming")) return; // Ã¼·ÂÀÌ 0 ÀÌÇÏ°Å³ª °ø°Ý ÁßÀÌ¸é Ãß°Ý ÁßÁö
        float distanceToTarget = Vector3.Distance(transform.position, playerTransform.position);

        if (distanceToTarget <= stat.detectionRange)
        {
            if (distanceToTarget > stat.attackRange)
            {
                nmAgent.SetDestination(playerTransform.position);
                DisableAttackCollider();
                animCtrl.SetBool("IsChasing", true);
                animCtrl.SetBool("IsAttacking", false);
            }
            else
            {
                animCtrl.SetBool("IsChasing", false);
                Attack();
            }
        }
        else
        {
            animCtrl.SetBool("IsChasing", false);
            animCtrl.SetTrigger("tIdle");
            DisableAttackCollider();
        }
    }

    // ÇÃ·¹ÀÌ¾î¸¦ ¹Ù¶óº¸°Ô ÇÏ´Â ¸Þ¼­µå
    private void FaceTarget()
    {
        Vector3 direction = (playerTransform.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * stat.legHomingSpd);
    }



    void Attack()
    {
        float distanceToTarget = Vector3.Distance(transform.position, playerTransform.position);
        if (stat.curHp <= 0 || distanceToTarget > stat.attackRange) return; // Ã¼·ÂÀÌ 0 ÀÌÇÏ°Å³ª »çÁ¤°Å¸® ¹ÛÀÌ¸é °ø°Ý ÁßÁö
        if (monsterType == MonsterType.Melee)
        {
            FaceTarget();
            StartAttack();
            animCtrl.SetTrigger("tAttack");
        }
        else if (monsterType == MonsterType.Ranged)
        {
            FaceTarget();
            animCtrl.SetBool("IsAiming", true);
            HandleRangedAttack();
        }
        else
        {
            // ÃßÈÄ¿¡ Æ¯¼öÇü Á¦ÀÛ
        }

    }

    void DropItems()
    {
        ItemManager itemManager = FindObjectOfType<ItemManager>();
        foreach (DropItem dropItem in dropItems)
        {
            if (UnityEngine.Random.Range(0f, 100f) < dropItem.dropChance)
            {
                // ¾ÆÀÌÅÛ »ý¼º ¹× µå¶ø
                itemManager.DropItem(dropItem.itemID, transform.position);
            }
        }
    }

    void StartAttack()
    {
        if (monsterType == MonsterType.Melee)
        {
            animCtrl.SetBool("IsAttacking", true);
        }
        else if (monsterType == MonsterType.Ranged)
        {
            animCtrl.SetBool("IsAiming", true);
        }

        // ÃßÀûÀ» ¸ØÃß±â À§ÇØ NavMeshAgent¸¦ ºñÈ°¼ºÈ­ÇÕ´Ï´Ù.
        if (nmAgent != null && nmAgent.enabled)
        {
            nmAgent.isStopped = true;
        }
    }

    public void EndAttack()
    {
        if (monsterType == MonsterType.Melee)
        {
            animCtrl.SetBool("IsAttacking", false);
        }
        else if (monsterType == MonsterType.Ranged)
        {
            animCtrl.SetBool("IsAiming", false);
        }
        float distanceToTarget = Vector3.Distance(transform.position, playerTransform.position);
        if (stat.curHp > 0)  // Ã¼·ÂÀÌ 0 ÀÌ»óÀÏ ¶§¸¸ tIdle Æ®¸®°Å¸¦ ¼³Á¤
        {


            // ÃßÀûÀ» Àç°³ÇÏ±â À§ÇØ NavMeshAgent¸¦ È°¼ºÈ­ÇÕ´Ï´Ù.
            if (nmAgent != null && nmAgent.enabled && distanceToTarget <= stat.detectionRange)
            {
                nmAgent.isStopped = false;
                if (playerTransform != null && distanceToTarget > nmAgent.stoppingDistance)
                {
                    nmAgent.SetDestination(playerTransform.position);
                    animCtrl.SetBool("IsChasing", true);
                }
                else if (playerTransform != null && distanceToTarget <= nmAgent.stoppingDistance)
                {
                    Attack();
                }
            }
            else
            {
                animCtrl.SetTrigger("tIdle");
            }


        }

    }



    private void HandleRangedAttack()
    {
        if (playerTransform != null && Vector3.Distance(transform.position, playerTransform.position) <= stat.attackRange)
        {
            FaceTarget();
            animCtrl.SetTrigger("tShot");
            FireProjectile(); // ¿ø°Å¸® Åõ»çÃ¼ ¹ß»ç ¸Þ¼­µå
        }
    }

    private void FireProjectile()
    {
        if (currentProjectile != null || ProjectilePrefab == null) return;

        Vector3 spawnPosition = ProjectileSpawnPoint != null ? ProjectileSpawnPoint.position : transform.position;
        Vector3 targetDirection = (playerTransform.position - spawnPosition).normalized;
        Quaternion spawnRotation = Quaternion.LookRotation(targetDirection);

        // Åõ»çÃ¼ ÀÎ½ºÅÏ½º »ý¼º
        currentProjectile = Instantiate(ProjectilePrefab, spawnPosition, spawnRotation);

        // Åõ»çÃ¼¿¡ Rigidbody ÄÄÆ÷³ÍÆ®¸¦ °¡Á®¿À°Å³ª Ãß°¡
        Rigidbody rb = currentProjectile.GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = currentProjectile.AddComponent<Rigidbody>();
        }

        // Áß·Â ¿µÇâÀ» ¹ÞÁö ¾Êµµ·Ï ¼³Á¤
        rb.useGravity = false;

        // Åõ»çÃ¼¿¡ ¼Óµµ Àû¿ë
        rb.velocity = targetDirection * ProjectileSpeed;

        Projectile projectileComponent = currentProjectile.GetComponent<Projectile>();
        if (projectileComponent != null)
        {
            projectileComponent.SetShooter(this);
        }
        // Åõ»çÃ¼ ÆÄ±« ·ÎÁ÷Àº ÇØ´ç Åõ»çÃ¼ ½ºÅ©¸³Æ®¿¡ ±¸Çö
    }


    // Åõ»çÃ¼°¡ ÆÄ±«µÇ¾úÀ» ¶§ È£ÃâÇÏ´Â ¸Þ¼­µå
    public void ProjectileDestroyed()
    {
        currentProjectile = null;
    }
    public void EnableAttackMeshRenderer()
    {
        if (stat.curHp > 0)
            attackMeshRenderer.enabled = true;
    }
    public void DisableAttackMeshRenderer()
    {
        attackMeshRenderer.enabled = false;
    }


    public void EnableAttackCollider()
    {
        if (stat.curHp > 0)
            attackCollider.enabled = true;
    }

    // °ø°Ý¿ë Collider ºñÈ°¼ºÈ­
    public void DisableAttackCollider()
    {
        attackCollider.enabled = false;
    }




}