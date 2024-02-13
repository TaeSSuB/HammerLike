using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Johnson;
using UnityEngine.AI;

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
    public float attackRange; // °ø°? ???¤°?¸®
    [Space(7.5f)]
    public float walkSpd;
    public float runSpd;

    [Space(7.5f)]
    public EnvasionStat envasionStat;

    [Space(7.5f)]
    public float upperHomingSpd; //???¼ ?¸?? ¼???
    public float legHomingSpd; //???¼ ?¸?? ¼???

    public float detectionRange;  // ??·¹??¾?¸? ??½??? ¹??§ ¼³?¤. ¿ø??´? °ª?¸·? ?¶?? °¡´?.

}

[System.Serializable]
public class DropItem
{
    public int itemID; // ¾Æ???? ID
    public float dropChance; // ??¶ø ?®·?
}


public class Monster : MonoBehaviour
{

    private Transform playerTransform;
    // Note: ±?´? ±¸?? ?? ¶§´? ??±????¤?? ??°? ½?°æ ¾?¾²°? ??¾÷??.
    // ?÷?? ±?´? ??¾÷ ³¡³ª°? ³ª¸? ?ß°¡???¸·? ?¤¸® ¿¹?¤!!
    // º?Æ??Ø?? ¾??Ø¹?¶ø´?´?!!! ½º¸¶¹?¼¾!!

    public MonsterStat stat;
    public MonsterType monsterType;
    public Slider healthSlider;
    [Header("State Machine")]
    public MonsterFSM fsm;

    [Header("Ranged Attack Settings")]
    public GameObject ProjectilePrefab; // ¿ø°?¸® °ø°??? ?§?? ?????¼ ??¸®Æ?
    public float ProjectileSpeed; // ?????¼ ¼???
    public Transform ProjectileSpawnPoint; // ¹ß???¼ ??¼º ?§?¡
    private GameObject currentProjectile;

    [Space(10f)]
    [Header("Default Comps")]
    public Transform meshTr;
    public Animator animCtrl;
    public Rigidbody rd;

    [Space(10f)]
    [Header("Action Table")]
    // Note: ?Ø´? º?ºÐ?º ¸?½º??¿¡ ¸?´? ¾×¼??¸·? ¼??¤ ??¿?
    public MonsterMove move;
    public MonsterAtk atk;
    public MonsterAim monsterAim;

    [Space(10f)]
    [Header("Cam Controller")]
    public CamCtrl camCtrl; // Note: ¸?½º??°¡ ??¸Þ¶?¸? ?÷?? ??¾??? ??¿?°¡ ?????? ?®?? ??¿?

    [Header("Drop Items")]
    public List<DropItem> dropItems = new List<DropItem>(); // ??¶ø ¾Æ???? ¸?·?

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
    public LineRenderer lineRenderer; // LineRenderer ???¶

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
        //animCtrl.SetTrigger("tIdle");
        if (healthSlider != null)
        {
            healthSlider.maxValue = stat.maxHp;
            healthSlider.value = stat.curHp;
        }

        // LineRenderer ±?º? ¼³?¤
        if (lineRenderer != null)
        {
            lineRenderer.positionCount = 2; // ½????¡°? ³¡?¡
            lineRenderer.widthMultiplier = 0.05f; // ¼±?? ³?º?
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
        if (Input.GetKeyDown(KeyCode.K))
        {
            ApplyKnockback(frontKnockbackDirection);
        }

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
            lineRenderer.SetPosition(0, transform.position); // ¼±?? ½????¡: ¸?½º???? ?§?¡
            lineRenderer.SetPosition(1, transform.position + transform.forward * 5f); // ¼±?? ³¡?¡: ¸?½º??°¡ ¹?¶?º¸´? ¹æ??
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
            FaceTarget(); // ??·¹??¾?¸? ??¼????¸·? ¹?¶?º¸°? ??´? ¸Þ¼­??
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
            if (weaponCollider != null)
            {
                // Debug 로그로 현재 공격 ID를 출력합니다.
                Debug.Log($"[Monster] Attack ID: {weaponCollider.CurrentAttackId}");

                if (!processedAttacks.Contains(weaponCollider.CurrentAttackId))
                {
                    // 공격 처리 전 해시셋에 해당 공격 ID가 없다는 것을 로그로 기록합니다.
                    Debug.Log($"[Monster] Processing new attack ID: {weaponCollider.CurrentAttackId}");

                    PlayerAtk playerAttack = other.GetComponentInParent<PlayerAtk>();
                    if (playerAttack != null)
                    {
                        TakeDamage(playerAttack.attackDamage);
                        Vector3 hitPoint = other.ClosestPointOnBounds(transform.position);
                        Vector3 knockbackDirection = DetermineKnockbackDirection(hitPoint, other.transform);
                        ApplyKnockback(knockbackDirection);

                        // 공격 처리 후 해당 공격 ID를 해시셋에 추가합니다.
                        processedAttacks.Add(weaponCollider.CurrentAttackId);
                        Debug.Log($"[Monster] Added attack ID to processedAttacks: {weaponCollider.CurrentAttackId}");
                    }
                }
                else
                {
                    // 중복된 공격이 들어왔을 때 해당 공격 ID를 로그로 출력합니다.
                    Debug.Log($"[Monster] Duplicate attack ID encountered: {weaponCollider.CurrentAttackId}");
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
        if (isKnockedBack) return; // 이미 넉백 중인 경우 넉백을 적용하지 않음

        float knockbackIntensity = 300f; // 넉백 강도
        direction.y = 0; // Y축 방향을 0으로 설정하여 수평 넉백을 보장
        Vector3 force = direction.normalized * knockbackIntensity;

        // 넉백 적용 전 Velocity 로깅
        Debug.Log($"[Monster] Pre-Knockback Velocity: {rd.velocity}");

        // 넉백 방향과 힘 로깅
        Debug.Log($"[Monster] Applying Knockback. Direction: {direction}, Force: {force}");

        // 넉백 힘 적용
        rd.AddForce(force, ForceMode.Impulse);
        isKnockedBack = true;

        // 넉백 적용 후 예상 Velocity 로깅 (실제 적용 후의 Velocity는 다음 프레임에서 확인 가능)
        Debug.Log($"[Monster] Expected Post-Knockback Velocity: {rd.velocity + force}");

        StartCoroutine(KnockBackDuration());
    }


    private IEnumerator KnockBackDamageCooldown()
    {
        yield return new WaitForSeconds(1f); // ³?¹? ??¹??? ?ð´?¿?
        canTakeKnockBackDamage = true;
    }

    private void TakeDamage(float damage)
    {
        if (stat.curHp <= 0) return; // ??¹? ??¸??? °æ¿? ??¹???¸? ¹Þ?? ¾??½

        if (stat.curHp > 0)  // ¸?½º??°¡ ??¾Æ???? ¶§¸¸ ??°? ?³¸®
        {
            stat.curHp -= damage;
            if (healthSlider != null)
            {
                healthSlider.value = stat.curHp;
                ShowHealthSlider();  // ?¼·? UI ½½¶???´? ??½?
            }

            if (stat.curHp <= 0)
            {
                Die();
            }
        }
    }

    /*private void ApplyKnockback(Vector3 direction)
    {
        float knockbackIntensity = 300f; // ³?¹? °­??
        direction.y = 0; // Y?? º??­ ??°?
        GetComponent<Rigidbody>().AddForce(direction.normalized * knockbackIntensity, ForceMode.Impulse);
        isKnockedBack = true;
        StartCoroutine(KnockBackDuration());
    }*/

    private IEnumerator KnockBackDuration()
    {
        yield return new WaitForSeconds(1.5f); // ³?¹? ??¼? ½?°?
        isKnockedBack = false;
    }

    private void Die()
    {
        // ¸?½º?? ??¸? ?³¸®
        // ¿¹: gameObject.SetActive(false); ¶?´? Destroy(gameObject);
        animCtrl.SetBool("IsChasing", false);
        animCtrl.SetTrigger("tDead");
        DisableAttackCollider();
        DisableAttackMeshRenderer();
        // NavMeshAgent º??°¼º?­
        if (nmAgent != null && nmAgent.isActiveAndEnabled)
        {
            nmAgent.isStopped = true;
            nmAgent.enabled = false;
        }

        playerTransform = null;
        DropItems();
        //Destroy(gameObject);
    }
    private void ShowHealthSlider()
    {
        if (healthSlider != null)
        {
            healthSlider.gameObject.SetActive(true);
            StopCoroutine("HideHealthSlider");  // ??¹? ?ø?? ?ß?? ??·?Æ¾?? ??´?¸? ?ß´?
            StartCoroutine("HideHealthSlider");  // ?? ??·?Æ¾ ½???
        }
    }

    private IEnumerator HideHealthSlider()
    {
        yield return new WaitForSeconds(2f);
        if (healthSlider != null && stat.curHp > 0)  // ¸?½º??°¡ ??¾Æ???? ¶§¸¸ ½½¶???´? º??°¼º?­
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
        if (stat.curHp <= 0) return; // ?¼·??? 0 ????¸? °¨?? ?ß??
        if (Vector3.Distance(transform.position, target.position) <= stat.detectionRange)
        {
            playerTransform = target; // ±??¸ ·??÷?? ????
            player = target.GetComponent<Player>(); // target¿¡¼­ Player ??Æ÷³?Æ®¸? °¡?®¿?

            if (player != null)
            {
                monsterAim.SetTarget(target); // MonsterAim ½º??¸³Æ®¿¡?? ?¸°? ¼³?¤
            }
        }
        else
        {
            playerTransform = null;
            player = null; // Player ???¶?? ?Ø??
            monsterAim.SetTarget(null); // MonsterAim ½º??¸³Æ®?? ?¸°??? ?Ø??
        }
    }


    void ChasePlayer()
    {
        if (stat.curHp <= 0 || animCtrl.GetBool("IsAttacking") || animCtrl.GetBool("IsAiming")) return; // ?¼·??? 0 ????°?³ª °ø°? ?ß??¸? ?ß°? ?ß??
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

    // ??·¹??¾?¸? ¹?¶?º¸°? ??´? ¸Þ¼­??
    private void FaceTarget()
    {
        Vector3 direction = (playerTransform.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * stat.legHomingSpd);
    }



    void Attack()
    {
        float distanceToTarget = Vector3.Distance(transform.position, playerTransform.position);
        if (stat.curHp <= 0 || distanceToTarget > stat.attackRange) return; // ?¼·??? 0 ????°?³ª ???¤°?¸® ¹???¸? °ø°? ?ß??
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
            // ?ß??¿¡ Æ?¼??? ????
        }

    }

    void DropItems()
    {
        ItemManager itemManager = FindObjectOfType<ItemManager>();
        foreach (DropItem dropItem in dropItems)
        {
            if (UnityEngine.Random.Range(0f, 100f) < dropItem.dropChance)
            {
                // ¾Æ???? ??¼º ¹× ??¶ø
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

        // ?ß???? ¸Ø?ß±? ?§?Ø NavMeshAgent¸? º??°¼º?­??´?´?.
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
        if (stat.curHp > 0)  // ?¼·??? 0 ?????? ¶§¸¸ tIdle Æ®¸®°?¸? ¼³?¤
        {


            // ?ß???? ??°³??±? ?§?Ø NavMeshAgent¸? ?°¼º?­??´?´?.
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
            FireProjectile(); // ¿ø°?¸® ?????¼ ¹ß?? ¸Þ¼­??
        }
    }

    private void FireProjectile()
    {
        if (currentProjectile != null || ProjectilePrefab == null) return;

        Vector3 spawnPosition = ProjectileSpawnPoint != null ? ProjectileSpawnPoint.position : transform.position;
        Vector3 targetDirection = (playerTransform.position - spawnPosition).normalized;
        Quaternion spawnRotation = Quaternion.LookRotation(targetDirection);

        // ?????¼ ??½º??½º ??¼º
        currentProjectile = Instantiate(ProjectilePrefab, spawnPosition, spawnRotation);

        // ?????¼¿¡ Rigidbody ??Æ÷³?Æ®¸? °¡?®¿?°?³ª ?ß°¡
        Rigidbody rb = currentProjectile.GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = currentProjectile.AddComponent<Rigidbody>();
        }

        // ?ß·? ¿????? ¹Þ?? ¾???·? ¼³?¤
        rb.useGravity = false;

        // ?????¼¿¡ ¼??? ??¿?
        rb.velocity = targetDirection * ProjectileSpeed;

        Projectile projectileComponent = currentProjectile.GetComponent<Projectile>();
        if (projectileComponent != null)
        {
            projectileComponent.SetShooter(this);
        }
        // ?????¼ Æ?±? ·??÷?º ?Ø´? ?????¼ ½º??¸³Æ®¿¡ ±¸??
    }


    // ?????¼°¡ Æ?±???¾??? ¶§ ??????´? ¸Þ¼­??
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

    // °ø°?¿? Collider º??°¼º?­
    public void DisableAttackCollider()
    {
        attackCollider.enabled = false;
    }




}