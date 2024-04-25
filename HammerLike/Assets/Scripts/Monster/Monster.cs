using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Johnson;
using UnityEngine.AI;
using RootMotion.Dynamics; // RootMotion ?쇱씠釉뚮윭由?李몄“ 異붽?
using RootMotion.Demos;

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
    public float attackRange; // 째첩째횦 쨩챌횁짚째횇쨍짰
    [Space(7.5f)]
    public float walkSpd;
    public float runSpd;

    [Space(7.5f)]
    public EnvasionStat envasionStat;

    [Space(7.5f)]
    public float upperHomingSpd; //쨩처횄쩌 횊쨍?체 쩌횙쨉쨉
    public float legHomingSpd; //횉횕횄쩌 횊쨍?체 쩌횙쨉쨉

    public float detectionRange;  // 횉횄쨌쨔?횑쩐챤쨍짝 ?횓쩍횆횉횘 쨔체?짠 쩌쨀횁짚. 쩔첩횉횕쨈횂 째짧?쨍쨌횓 횁쨋?첵 째징쨈횋.

}

[System.Serializable]
public class DropItem
{
    public int itemID; // 쩐횈?횑횇횤 ID
    public float dropChance; // 쨉책쨋첩 횊짰쨌체
}


public class Monster : MonoBehaviour
{

    private Transform playerTransform;
    // Note: 짹창쨈횋 짹쨍횉철 횉횘 쨋짠쨈횂 횁짖짹횢횁철횁짚?횣 횇짤째횚 쩍횇째챈 쩐횊쩐짼째챠 ?횤쩐첨횉횚.
    // 횂첨횊횆 짹창쨈횋 ?횤쩐첨 쨀징쨀짧째챠 쨀짧쨍챕 횄횩째징?청?쨍쨌횓 횁짚쨍짰 쩔쨔횁짚!!
    // 쨘횘횈챠횉횠쨉쨉 쩐챌횉횠쨔횢쨋첩쨈횕쨈횢!!! 쩍쨘쨍쨋쨔횑쩌쩐!!

    public static event Action<Vector3> OnMonsterDeath;

    public MonsterStat stat;
    public MonsterType monsterType;
    public Slider healthSlider;
    [Header("State Machine")]
    public MonsterFSM fsm;

    [Header("Ranged Attack Settings")]
    public GameObject ProjectilePrefab; // 쩔첩째횇쨍짰 째첩째횦?쨩 ?짠횉횗 횇천쨩챌횄쩌 횉횁쨍짰횈횛
    public float ProjectileSpeed; // 횇천쨩챌횄쩌 쩌횙쨉쨉
    public Transform ProjectileSpawnPoint; // 쨔횩쨩챌횄쩌 쨩첵쩌쨘 ?짠횆징
    private GameObject currentProjectile;

    [Space(10f)]
    [Header("Default Comps")]
    public Transform meshTr;
    public Animator animCtrl;
    public Rigidbody rd;

    [Space(10f)]
    [Header("Action Table")]
    // Note: 횉횠쨈챌 쨘횓쨘횖?쨘 쨍처쩍쨘횇횒쩔징 쨍횂쨈횂 쩐횞쩌횉?쨍쨌횓 쩌철횁짚 횉횎쩔채
    public MonsterMove move;
    public MonsterAtk atk;
    public MonsterAim monsterAim;

    [Space(10f)]
    [Header("Cam Controller")]
    public CamCtrl camCtrl; // Note: 쨍처쩍쨘횇횒째징 횆짬쨍횧쨋처쨍짝 횁첨횁짖 횁짝쩐챤횉횘 횉횎쩔채째징 ?횜?쨩횁철 횊짰?횓 횉횎쩔채

    [Header("Drop Items")]
    public List<DropItem> dropItems = new List<DropItem>(); // 쨉책쨋첩 쩐횈?횑횇횤 쨍챰쨌횕

    [Space(10f)]
    [Header("Anim Bones")]
    public Transform headBoneTr;
    public Transform spineBoneTr;
    public Transform hpBoneTr;
    private HashSet<int> processedAttacks = new HashSet<int>();

    public bool isKnockedBack = false;
    public float knockbackDamage = 10f;
    private bool canTakeKnockBackDamage = true;

    public Transform target;
    NavMeshAgent nmAgent;
    public LineRenderer lineRenderer; // LineRenderer 횂체횁쨋

    public Collider attackCollider;
    public MeshRenderer attackMeshRenderer;
    private Player player;

    public int debugData = 0;
    private int knockbackData = 0;
    public BehaviourPuppet puppet;
    private int lastProcessedAttackId = -1;
    private NavMeshPuppet navMeshPuppet;
    public RaycastShooter raycastShooter;
    public GameObject[] targetBones;
    public ChangeMaterial[] changeMaterials;
    private float customForce;
    private bool canShot = true;
    private bool canDamage = true;
    private bool isRising = false;
    private Vector3 pos;

    public GameObject monsterPrefab; // Inspector?먯꽌 ?좊떦??紐ъ뒪???꾨━??
    private GameObject clonedMonster;
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
        pos = transform.position;
        nmAgent = GetComponent<NavMeshAgent>();
        animCtrl.SetBool("IsChasing", true);
        if (healthSlider != null)
        {
            healthSlider.maxValue = stat.maxHp;
            healthSlider.value = stat.curHp;
        }

        // LineRenderer 짹창쨘쨩 쩌쨀횁짚
        if (lineRenderer != null)
        {
            lineRenderer.positionCount = 2; // 쩍횄?횤횁징째첬 쨀징횁징
            lineRenderer.widthMultiplier = 0.05f; // 쩌짹?횉 쨀횎쨘챰
        }

        if (target == null)
        {
            GameObject player = GameObject.Find("Player"); // "Player"?쇰뒗 ?대쫫??媛吏?GameObject瑜?李얠뒿?덈떎.
            if (player != null) // GameObject媛 議댁옱?섎뒗吏 ?뺤씤?⑸땲??
            {
                target = player.transform; // 李얠? GameObject??Transform 而댄룷?뚰듃瑜?target???좊떦?⑸땲??
            }
            else
            {
                Debug.LogError("Player ?ㅻ툕?앺듃瑜?李얠쓣 ???놁뒿?덈떎. 'Player'?쇰뒗 ?대쫫???ㅻ툕?앺듃媛 ?ъ뿉 議댁옱?섎뒗吏 ?뺤씤?댁＜?몄슂.");
            }
        }
        if (raycastShooter == null)
        {
            GameObject playerWeapon = GameObject.Find("mixamorig:RightWeapon");
            if (playerWeapon != null)
            {
                raycastShooter = playerWeapon.GetComponent<RaycastShooter>();

            }
        }
        navMeshPuppet = GetComponent<NavMeshPuppet>();

        clonedMonster = Instantiate(monsterPrefab, transform.position, Quaternion.identity);
        clonedMonster.SetActive(false);

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
        // ?뚮젅?댁뼱 諛⑺뼢 湲곕컲 ?쇱씤 ?뚮뜑留??낅뜲?댄듃
        UpdateDirectionLines();
        if (Input.GetKeyDown(KeyCode.K))
        {
            raycastShooter.IndexRay();
        }


    }
    void UpdateDirectionLines()
    {
        if (player != null)
        {
            Vector3 playerForward = player.transform.forward;
            Vector3 playerPosition = player.transform.position + Vector3.up * 0.5f;

            // ?뺣㈃ 諛⑺뼢
            Vector3 frontDirection = playerForward;
            // 醫뚯륫 ?媛곸꽑 諛⑺뼢
            Vector3 leftDirection = Quaternion.Euler(0, -45, 0) * playerForward;
            // ?곗륫 ?媛곸꽑 諛⑺뼢
            Vector3 rightDirection = Quaternion.Euler(0, 45, 0) * playerForward;

            // 諛⑺뼢 ???

        }
    }

    private void LateUpdate()
    {
        Vector3 lookDir = monsterAim.Aiming();
        if (stat.curHp > 0 && isRising)
            SyncMeshWithPuppetMaster();

    }

    private void FixedUpdate()
    {
        if (playerTransform != null && stat.curHp > 0)
        {
            FaceTarget(); // 횉횄쨌쨔?횑쩐챤쨍짝 횁철쩌횙?청?쨍쨌횓 쨔횢쨋처쨘쨍째횚 횉횕쨈횂 쨍횧쩌짯쨉책
        }

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
            if (weaponCollider != null && lastProcessedAttackId != weaponCollider.CurrentAttackId)
            {
                // 쨉짜쨔횑횁철 쨔횞 쨀횏쨔챕 횄쨀쨍짰
                PlayerAtk playerAttack = other.GetComponentInParent<PlayerAtk>();
                if (playerAttack != null && canDamage)
                {

                    float customDamage = 0f;
                    float intensity = 50f;
                    float customIntensity = 0f;
                    if (player.atk.curCharging >= 0 && player.atk.curCharging < 2)
                    {
                        //customForce = raycastShooter.force; // ?먮낯 ?ш린
                        customDamage = playerAttack.attackDamage;
                        customIntensity = intensity;
                    }
                    else if (player.atk.curCharging >= 2 && player.atk.curCharging < 4)
                    {
                        //customForce = raycastShooter.force * 1.5f; // 1.5諛??ш린
                        customDamage = playerAttack.attackDamage * 1.5f;
                        customIntensity = intensity * 1.5f;
                    }
                    else // player.curCharging >= 4
                    {
                        //customForce = raycastShooter.force * 2f; // 2諛??ш린
                        customDamage = playerAttack.attackDamage * 2f;
                        customIntensity = intensity * 3f;
                    }

                    puppet.puppetMaster.pinWeight = 0f;
                    TakeDamage(customDamage);
                    if(stat.curHp>0)
                    { 
                    ApplyKnockback(player.transform.forward, customIntensity);
                    isRising = true;
                    }

                    lastProcessedAttackId = weaponCollider.CurrentAttackId;
                    weaponCollider.hasProcessedAttack = true; // 怨듦꺽 泥섎━ ?쒖떆

                }

            }
        }


        if (other.gameObject.CompareTag("KnockBackable") && isKnockedBack && canTakeKnockBackDamage)
        {
            TakeDamage(knockbackDamage); // KnockBackDamage
            canTakeKnockBackDamage = false;
            StartCoroutine(KnockBackDamageCooldown());
        }
    }

    private void ApplyKnockback(Vector3 direction, float intensity)
    {

        StartCoroutine(KnockbackCoroutine(direction, intensity));
        isKnockedBack = true;
        StartCoroutine(KnockBackDuration());
        knockbackData++;
        Debug.Log(knockbackData);
    }

    private IEnumerator KnockbackCoroutine(Vector3 direction, float intensity)
    {
        float duration = 1.0f; // ?됰갚 吏???쒓컙 (1珥?
        direction.y = 0; // Y異?諛⑺뼢??0?쇰줈 ?ㅼ젙?섏뿬 ?섑룊 ?됰갚??蹂댁옣
        Vector3 start = transform.position;
        Vector3 end = transform.position + direction.normalized * intensity; // 理쒖쥌 紐⑺몴 ?꾩튂

        float elapsedTime = 0;
        while (elapsedTime < duration)
        {
            transform.position = Vector3.Lerp(start, end, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // 留덉?留됱쑝濡?理쒖쥌 ?꾩튂瑜??뺤떎???ㅼ젙
        transform.position = end;
    }

    private IEnumerator KnockBackDamageCooldown()
    {
        yield return new WaitForSeconds(1f); // 쨀횏쨔챕 쨉짜쨔횑횁철 횆챨쨈횢쩔챤
        canTakeKnockBackDamage = true;
    }


    private IEnumerator CanDamage()
    {
        yield return new WaitForSeconds(2f); // 쨀횏쨔챕 쨉짜쨔횑횁철 횆챨쨈횢쩔챤
        canDamage = true;
    }

    private void TakeDamage(float damage)
    {
        if (stat.curHp <= 0) return; // ?횑쨔횑 쨩챌쨍횁횉횗 째챈쩔챙 쨉짜쨔횑횁철쨍짝 쨔횧횁철 쩐횎?쩍
        SoundManager soundManager = SoundManager.Instance;
        soundManager.PlaySFX(soundManager.audioClip[3]);


        if (monsterType == MonsterType.Melee)
        {
            for (int i = 0; i < 2; i++)
            {
                changeMaterials[i].OnHit();
            }
        }
        else if (monsterType == MonsterType.Ranged)
        {
            for (int i = 0; i < 4; i++)
            {
                changeMaterials[i].OnHit();
            }
        }

        if (stat.curHp > 0)  // 쨍처쩍쨘횇횒째징 쨩챙쩐횈?횜?쨩 쨋짠쨍쨍 횉횉째횦 횄쨀쨍짰
        {
            stat.curHp -= damage;
            if (attackCollider != null && attackCollider.enabled == true)
            {
                attackCollider.enabled = false;
            }

            if (attackMeshRenderer != null && attackMeshRenderer.enabled == true)
            {
                attackMeshRenderer.enabled = false;
            }

            if (healthSlider != null)
            {
                healthSlider.value = stat.curHp;
                //ShowHealthSlider();  // 횄쩌쨌횂 UI 쩍쩍쨋처?횑쨈천 횉짜쩍횄
            }

            if (stat.curHp <= 0)
            {
                Die();
            }
        }

        if (monsterType == MonsterType.Melee && attackCollider.enabled)
        {
            attackCollider.enabled = false;
        }

        if (monsterType == MonsterType.Melee && attackMeshRenderer.enabled)
        {
            attackMeshRenderer.enabled = false;

        }
    }

    private IEnumerator KnockBackDuration()
    {
        yield return new WaitForSeconds(0.2f); // 쨀횏쨔챕 횁철쩌횙 쩍횄째짙
        isKnockedBack = false;
        isRising = false;
        puppet.puppetMaster.pinWeight = 1;
    }

    private void Die()
    {
        // 쨍처쩍쨘횇횒 쨩챌쨍횁 횄쨀쨍짰
        // 쩔쨔: gameObject.SetActive(false); 쨋횉쨈횂 Destroy(gameObject);
        animCtrl.SetBool("IsChasing", false);
        animCtrl.SetTrigger("tDead");
        if (attackCollider != null)
            DisableAttackCollider();
        if (attackMeshRenderer != null)
            DisableAttackMeshRenderer();

        // NavMeshAgent 쨘챰횊째쩌쨘횊짯
        /*if (nmAgent != null && nmAgent.isActiveAndEnabled)
        {
            nmAgent.isStopped = true;
            nmAgent.enabled = false;
           

        }*/
        navMeshPuppet.enabled = false;
        SoundManager soundManager = SoundManager.Instance;
        soundManager.PlaySFX(soundManager.audioClip[2]);
        playerTransform = null;
        OnMonsterDeath?.Invoke(transform.position);
        DropItems();
        DisconnectMusclesRecursive();
        //destroyObject();
        //StartCoroutine(destroyObject());
        //StartCoroutine(respawObject());
        //Destroy(gameObject);
    }

    private IEnumerator destroyObject()
    {
        yield return new WaitForSeconds(4f);
        gameObject.SetActive(false);
        clonedMonster.SetActive(true);
        clonedMonster.transform.position = pos;
    }

    private IEnumerator respawObject()
    {
        yield return new WaitForSeconds(6f);


    }


    private void DisconnectMusclesRecursive()
    {
        if (puppet != null && puppet.puppetMaster != null)
        {
            /*Vector3 playerDirection = transform.position - player.transform.position;
            playerDirection.y = 0;
            Vector3 knockbackDirection = playerDirection.normalized;*/
            // 紐⑤뱺 洹쇱쑁???쒗쉶?섎ŉ ?ш??곸쑝濡??곌껐 ?댁젣
            for (int i = 0; i < puppet.puppetMaster.muscles.Length; i++)
            {

                puppet.puppetMaster.DisconnectMuscleRecursive(i, MuscleDisconnectMode.Explode);

                rd.isKinematic = true;

            }
        }
    }

    public Player Player
    {
        get { return player; }
    }
    private void DetectPlayer()
    {
        if (stat.curHp <= 0) return; // 泥대젰??0 ?댄븯硫?以묒?
        if (Vector3.Distance(transform.position, target.position) <= stat.detectionRange)
        {
            playerTransform = target; // ?뚮젅?댁뼱瑜?吏?뺥븯?붽쾶 ?꾨땶 ?ъ떎???뚮젅?댁뼱???꾩튂瑜?異붿쟻
            player = target.GetComponent<Player>(); // ?寃잛? ?ъ떎???뚮젅?댁뼱???뚮젅?댁뼱???뚮젅?댁뼱 而댄룷?뚰듃 諛쏆븘??

            if (player != null)
            {
                monsterAim.SetTarget(target); //
                animCtrl.SetBool("IsChasing", true);
                SoundManager soundManager = SoundManager.Instance;
                //soundManager.PlaySFX(soundManager.audioClip[4]);  // 諛쒖옄援??뚮━ 醫 ?좊ℓ???덈Т ??
            }
        }
        else
        {
            playerTransform = null;
            player = null; // Player 횂체횁쨋쨉쨉 횉횠횁짝
            monsterAim.SetTarget(null); // MonsterAim 쩍쨘횇짤쨍쨀횈짰?횉 횇쨍째횢쨉쨉 횉횠횁짝
        }
    }


    void ChasePlayer()
    {
        if (stat.curHp <= 0 || animCtrl.GetBool("IsAttacking")) return; // 泥대젰??0蹂대떎 ?묎굅??怨듦꺽以묒씠硫??쎌삩 ?섍린 ?뚮Ц???쒗븳??
        float distanceToTarget = Vector3.Distance(transform.position, playerTransform.position);

        if (distanceToTarget <= stat.detectionRange)
        {
            if (distanceToTarget > stat.attackRange)
            {
                //nmAgent.SetDestination(playerTransform.position);
                if (attackCollider != null)
                    DisableAttackCollider();
                animCtrl.SetBool("IsChasing", true);
                animCtrl.SetBool("IsAttacking", false);
                if (attackCollider != null)
                {
                    if (attackCollider.enabled)
                    {
                        attackCollider.enabled = false;
                    }

                }
                if (attackMeshRenderer != null)
                {

                    if (attackMeshRenderer.enabled)
                    {
                        attackMeshRenderer.enabled = false;

                    }
                }
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
            if (attackCollider != null)
                DisableAttackCollider();
        }
    }


    private void FaceTarget()
    {
        Vector3 direction = (playerTransform.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * stat.legHomingSpd);
    }

    private void SyncMeshWithPuppetMaster()
    {
        if (puppet != null && puppet.isActiveAndEnabled)
        {
            // 媛?堉덈??????Mesh???꾩튂? ?뚯쟾??PuppetMaster???곹깭??留욎땅?덈떎.
            foreach (var muscle in puppet.puppetMaster.muscles)
            {
                var boneTransform = muscle.transform;
                var targetTransform = muscle.target;

                if (boneTransform != null && targetTransform != null)
                {
                    boneTransform.position = targetTransform.position;
                    boneTransform.rotation = targetTransform.rotation;
                }
            }
        }
    }

    void Attack()
    {
        float distanceToTarget = Vector3.Distance(transform.position, playerTransform.position);
        if (stat.curHp <= 0 || distanceToTarget > stat.attackRange) return;
        if (monsterType == MonsterType.Melee)
        {
            FaceTarget();
            StartAttack();
            animCtrl.SetTrigger("tAttack");
            SoundManager soundManager = SoundManager.Instance;
            soundManager.PlaySFX(soundManager.audioClip[6]);
            DisableAttackComponentsAfterDelay();
        }
        else if (monsterType == MonsterType.Ranged)
        {
            FaceTarget();
            //nmAgent.isStopped=true;
            if (currentProjectile != null)
            {
                animCtrl.SetBool("IsAimIdle", true);
            }
            else
            {
                animCtrl.SetBool("IsAiming", true);
            }
            HandleRangedAttack();
        }
        else
        {
            // ?밸퀎??(?덉떆 Special) ?좊뱾 李멸퀬
        }

    }

    void DropItems()
    {
        ItemManager itemManager = FindObjectOfType<ItemManager>();
        foreach (DropItem dropItem in dropItems)
        {
            if (UnityEngine.Random.Range(0f, 100f) < dropItem.dropChance)
            {
                if (dropItem.itemID == 0)
                {
                    SoundManager soundManager = SoundManager.Instance;
                    soundManager.PlaySFX(soundManager.audioClip[0]);
                }
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
            //nmAgent.isStopped = true;
        }

        // 횄횩?청?쨩 쨍횠횄횩짹창 ?짠횉횠 NavMeshAgent쨍짝 쨘챰횊째쩌쨘횊짯횉횛쨈횕쨈횢.
        if (nmAgent != null && nmAgent.enabled)
        {
            //nmAgent.isStopped = true;
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
            animCtrl.SetBool("IsAimIdle", false);
        }
        float distanceToTarget = Vector3.Distance(transform.position, playerTransform.position);
        if (stat.curHp > 0)  // 횄쩌쨌횂?횑 0 ?횑쨩처?횕 쨋짠쨍쨍 tIdle 횈짰쨍짰째횇쨍짝 쩌쨀횁짚
        {


            // 횄횩?청?쨩 ?챌째쨀횉횕짹창 ?짠횉횠 NavMeshAgent쨍짝 횊째쩌쨘횊짯횉횛쨈횕쨈횢.
            if (nmAgent != null && nmAgent.enabled && distanceToTarget <= stat.detectionRange)
            {
                //nmAgent.isStopped = false;
                if (playerTransform != null && distanceToTarget > nmAgent.stoppingDistance)
                {
                    //nmAgent.SetDestination(playerTransform.position);
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
            if (currentProjectile != null || !canShot) return;
            animCtrl.SetTrigger("tShot");
            SoundManager soundManager = SoundManager.Instance;
            soundManager.PlaySFX(soundManager.audioClip[5]);
            FireProjectile(); // 쩔첩째횇쨍짰 횇천쨩챌횄쩌 쨔횩쨩챌 쨍횧쩌짯쨉책
            StartCoroutine(ShotTime());
        }
    }

    private IEnumerator ShotTime()
    {
        canShot = false;
        yield return new WaitForSeconds(6f); // 荑⑦???6珥?
        canShot = true;
    }

    private void FireProjectile()
    {
        if (currentProjectile != null || ProjectilePrefab == null) return;

        Vector3 spawnPosition = ProjectileSpawnPoint != null ? ProjectileSpawnPoint.position : transform.position;
        Vector3 targetDirection = (playerTransform.position - spawnPosition).normalized;
        Quaternion spawnRotation = Quaternion.LookRotation(targetDirection);

        // ?덈줈 ?앹꽦?섎뒗 ?????먯꽌 ?ъ궗泥대? 媛?몄샂
        currentProjectile = ProjectilePool.Instance.GetProjectile();
        currentProjectile.transform.position = spawnPosition;
        currentProjectile.transform.rotation = spawnRotation;
        currentProjectile.SetActive(true);

        // ?ъ궗泥댁쓽 ?띾룄 ?ㅼ젙
        Rigidbody rb = currentProjectile.GetComponent<Rigidbody>();
        rb.velocity = targetDirection * ProjectileSpeed;

        // ?ъ궗泥닿? 諛쒖궗??紐ъ뒪???ㅼ젙
        Projectile projectileComponent = currentProjectile.GetComponent<Projectile>();
        if (projectileComponent != null)
        {
            projectileComponent.SetShooter(this);
        }
    }



    // ?좊땲硫붿씠??愿?⑤맂 ?대깽???⑥닔 
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
        if (attackMeshRenderer != null)
            attackMeshRenderer.enabled = false;
    }


    public void EnableAttackCollider()
    {
        if (stat.curHp > 0)
            attackCollider.enabled = true;
    }


    public void DisableAttackCollider()
    {
        if (attackCollider != null)
            attackCollider.enabled = false;
    }

    public void startKnockback()
    {
        isKnockedBack = true;
        DisableAttackCollider(); // 怨듦꺽 愿??而댄룷?뚰듃 鍮꾪솢?깊솕
        DisableAttackMeshRenderer();
    }

    private IEnumerator DisableAttackComponentsAfterDelay()
    {
        yield return new WaitForSeconds(1f); // 1珥??湲?
        DisableAttackCollider(); // 怨듦꺽 愿??而댄룷?뚰듃 鍮꾪솢?깊솕
        DisableAttackMeshRenderer();
    }
    public void endKnockback()
    {
        isKnockedBack = false;
        DisableAttackCollider(); // 怨듦꺽 愿??而댄룷?뚰듃 鍮꾪솢?깊솕
        DisableAttackMeshRenderer();
    }




}