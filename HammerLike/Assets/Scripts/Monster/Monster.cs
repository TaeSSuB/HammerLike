using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Johnson;
using UnityEngine.AI;
using RootMotion.Dynamics; // RootMotion ??깆뵠?됰슢??뵳?筌〓챷???곕떽?
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
    public float attackRange; // 吏몄꺽吏명슗 夷⑹콐?곸쭦吏명쉯夷띿㎞
    [Space(7.5f)]
    public float walkSpd;
    public float runSpd;

    [Space(7.5f)]
    public EnvasionStat envasionStat;

    [Space(7.5f)]
    public float upperHomingSpd; //夷⑹쿂?꾩찈 ?딆쮰?泥?姨뚰슇夷됱쮬
    public float legHomingSpd; //?됲슃?꾩찈 ?딆쮰?泥?姨뚰슇夷됱쮬

    public float detectionRange;  // ?됲쉪夷뚯쮷??묒찎梨ㅼ쮰吏???볦찉?놃쉲??夷붿껜?吏?姨뚯??곸쭦. 姨붿꺽?됲슃夷덊쉨 吏몄㎣?夷띿쮯???곸쮮?泥?吏몄쭠夷덊쉵.

}

[System.Serializable]
public class DropItem
{
    public int itemID; // 姨먰쉱??묓쉯??ID
    public float dropChance; // 夷됱콉夷뗭꺽 ?딆㎞夷뚯껜
}


public class Monster : MonoBehaviour
{

    private Transform playerTransform;
    // Note: 吏뱀갹夷덊쉵 吏뱀쮰?됱쿋 ?됲슆 夷뗭쭬夷덊쉨 ?곸쭡吏뱁슓?곸쿋?곸쭦????뉗ℓ吏명슊 姨랁쉯吏몄콌 姨먰쉳姨먯㏈吏몄콬 ??ㅼ찎泥⑦쉲??
    // ?귥꺼?딇쉮 吏뱀갹夷덊쉵 ??ㅼ찎泥?夷吏뺤?吏㏃㎏梨?夷吏㏃쮰梨??꾪슜吏몄쭠?泥?夷띿쮯???곸쭦夷띿㎞ 姨붿쮷?곸쭦!!
    // 夷섑슆?덉콬?됲슑夷됱쮬 姨먯콐?됲슑夷뷀슓夷뗭꺽夷덊슃夷덊슓!!! 姨띿쮼夷띿쮮夷뷀쉻姨뚯찎!!

    public static event Action<Vector3> OnMonsterDeath;

    public MonsterStat stat;
    public MonsterType monsterType;
    public Slider healthSlider;
    [Header("State Machine")]
    public MonsterFSM fsm;

    [Header("Ranged Attack Settings")]
    public GameObject ProjectilePrefab; // 姨붿꺽吏명쉯夷띿㎞ 吏몄꺽吏명슗?夷??吏좏쉲???뉗쿇夷⑹콐?꾩찈 ?됲쉧夷띿㎞?덊슋
    public float ProjectileSpeed; // ?뉗쿇夷⑹콐?꾩찈 姨뚰슇夷됱쮬
    public Transform ProjectileSpawnPoint; // 夷뷀슜夷⑹콐?꾩찈 夷⑹껨姨뚯쮼 ?吏좏쉮吏?
    private GameObject currentProjectile;

    [Space(10f)]
    [Header("Default Comps")]
    public Transform meshTr;
    public Animator animCtrl;
    public Rigidbody rd;

    [Space(10f)]
    [Header("Action Table")]
    // Note: ?됲슑夷덉콐 夷섑슀夷섑슄?夷?夷띿쿂姨띿쮼?뉙쉾姨붿쭠 夷랁쉨夷덊쉨 姨먰슎姨뚰쉲?夷띿쮯??姨뚯쿋?곸쭦 ?됲쉸姨붿콈
    public MonsterMove move;
    public MonsterAtk atk;
    public MonsterAim monsterAim;

    [Space(10f)]
    [Header("Cam Controller")]
    public CamCtrl camCtrl; // Note: 夷띿쿂姨띿쮼?뉙쉾吏몄쭠 ?놁㎚夷랁슙夷뗭쿂夷띿쭩 ?곸꺼?곸쭡 ?곸쭩姨먯광?됲슆 ?됲쉸姨붿콈吏몄쭠 ???夷⑺쉧泥??딆㎞????됲쉸姨붿콈

    [Header("Drop Items")]
    public List<DropItem> dropItems = new List<DropItem>(); // 夷됱콉夷뗭꺽 姨먰쉱??묓쉯??夷띿굅夷뚰슃

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
    public LineRenderer lineRenderer; // LineRenderer ?귥껜?곸쮮

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

    public GameObject monsterPrefab; // Inspector?癒?퐣 ?醫딅뼣??筌뤣딅뮞???袁ⓥ봺??
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

        // LineRenderer 吏뱀갹夷섏Ł 姨뚯??곸쭦
        if (lineRenderer != null)
        {
            lineRenderer.positionCount = 2; // 姨랁쉪??ㅽ쉧吏뺤㎏泥?夷吏뺥쉧吏?
            lineRenderer.widthMultiplier = 0.05f; // 姨뚯㏏???夷?롮쮼梨?
        }

        if (target == null)
        {
            GameObject player = GameObject.Find("Player"); // "Player"??곕뮉 ??已??揶쎛筌?GameObject??筌≪뼚???덈뼄.
            if (player != null) // GameObject揶쎛 鈺곕똻???롫뮉筌왖 ?類ㅼ뵥??몃빍??
            {
                target = player.transform; // 筌≪뼚? GameObject??Transform ?뚮똾猷??곕뱜??target???醫딅뼣??몃빍??
            }
            else
            {
                Debug.LogError("Player ??삵닏??븍뱜??筌≪뼚??????곷뮸??덈뼄. 'Player'??곕뮉 ??已????삵닏??븍뱜揶쎛 ??肉?鈺곕똻???롫뮉筌왖 ?類ㅼ뵥??곻폒?紐꾩뒄.");
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
        // ???쟿??곷선 獄쎻뫚堉?疫꿸퀡而???깆뵥 ???쐭筌???낅쑓??꾨뱜
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

            // ?類ｃ늺 獄쎻뫚堉?
            Vector3 frontDirection = playerForward;
            // ?ル슣瑜???揶쏄낯苑?獄쎻뫚堉?
            Vector3 leftDirection = Quaternion.Euler(0, -45, 0) * playerForward;
            // ?怨쀫? ??揶쏄낯苑?獄쎻뫚堉?
            Vector3 rightDirection = Quaternion.Euler(0, 45, 0) * playerForward;

            // 獄쎻뫚堉?????

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
            FaceTarget(); // ?됲쉪夷뚯쮷??묒찎梨ㅼ쮰吏??곸쿋姨뚰슇?泥?夷띿쮯??夷뷀슓夷뗭쿂夷섏쮰吏명슊 ?됲슃夷덊쉨 夷랁슙姨뚯㎝夷됱콉
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
                // 夷됱쭨夷뷀쉻?곸쿋 夷뷀슎 夷?륁쮷梨??꾩?夷띿㎞
                PlayerAtk playerAttack = other.GetComponentInParent<PlayerAtk>();
                if (playerAttack != null && canDamage)
                {

                    float customDamage = 0f;
                    float intensity = 50f;
                    float customIntensity = 0f;
                    if (player.atk.curCharging >= 0 && player.atk.curCharging < 2)
                    {
                        //customForce = raycastShooter.force; // ?癒?궚 ??由?
                        customDamage = playerAttack.attackDamage;
                        customIntensity = intensity;
                    }
                    else if (player.atk.curCharging >= 2 && player.atk.curCharging < 4)
                    {
                        //customForce = raycastShooter.force * 1.5f; // 1.5獄???由?
                        customDamage = playerAttack.attackDamage * 1.5f;
                        customIntensity = intensity * 1.5f;
                    }
                    else // player.curCharging >= 4
                    {
                        //customForce = raycastShooter.force * 2f; // 2獄???由?
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
                    weaponCollider.hasProcessedAttack = true; // ?⑤벀爰?筌ｌ꼶????뽯뻻

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
        float duration = 1.0f; // ??곌컶 筌왖????볦퍢 (1??
        direction.y = 0; // Y??獄쎻뫚堉??0??곗쨮 ??쇱젟??뤿연 ??묐즸 ??곌컶??癰귣똻??
        Vector3 start = transform.position;
        Vector3 end = transform.position + direction.normalized * intensity; // 筌ㅼ뮇伊?筌뤴뫚紐??袁⑺뒄

        float elapsedTime = 0;
        while (elapsedTime < duration)
        {
            transform.position = Vector3.Lerp(start, end, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // 筌띾뜆?筌띾맩?앮에?筌ㅼ뮇伊??袁⑺뒄???類ㅻ뼄????쇱젟
        transform.position = end;
    }

    private IEnumerator KnockBackDamageCooldown()
    {
        yield return new WaitForSeconds(1f); // 夷?륁쮷梨?夷됱쭨夷뷀쉻?곸쿋 ?놁괩夷덊슓姨붿광
        canTakeKnockBackDamage = true;
    }


    private IEnumerator CanDamage()
    {
        yield return new WaitForSeconds(2f); // 夷?륁쮷梨?夷됱쭨夷뷀쉻?곸쿋 ?놁괩夷덊슓姨붿광
        canDamage = true;
    }

    private void TakeDamage(float damage)
    {
        if (stat.curHp <= 0) return; // ??묒쮷??夷⑹콐夷랁쉧?됲슅 吏몄콌姨붿콡 夷됱쭨夷뷀쉻?곸쿋夷띿쭩 夷뷀슙?곸쿋 姨먰쉸?姨?
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

        if (stat.curHp > 0)  // 夷띿쿂姨띿쮼?뉙쉾吏몄쭠 夷⑹콡姨먰쉱???夷?夷뗭쭬夷띿쮰 ?됲쉲吏명슗 ?꾩?夷띿㎞
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
                //ShowHealthSlider();  // ?꾩찈夷뚰쉨 UI 姨띿찉夷뗭쿂??묒쮫泥??됱쭨姨랁쉪
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
        yield return new WaitForSeconds(0.2f); // 夷?륁쮷梨??곸쿋姨뚰슇 姨랁쉪吏몄쭥
        isKnockedBack = false;
        isRising = false;
        puppet.puppetMaster.pinWeight = 1;
    }

    private void Die()
    {
        // 夷띿쿂姨띿쮼?뉙쉾 夷⑹콐夷랁쉧 ?꾩?夷띿㎞
        // 姨붿쮷: gameObject.SetActive(false); 夷뗮쉲夷덊쉨 Destroy(gameObject);
        animCtrl.SetBool("IsChasing", false);
        animCtrl.SetTrigger("tDead");
        if (attackCollider != null)
            DisableAttackCollider();
        if (attackMeshRenderer != null)
            DisableAttackMeshRenderer();

        // NavMeshAgent 夷섏굅?딆㎏姨뚯쮼?딆㎝
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
            // 筌뤴뫀諭?域뱀눘?????쀬돳??렽?????怨몄몵嚥??怨뚭퍙 ??곸젫
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
        if (stat.curHp <= 0) return; // 筌ｋ????0 ??꾨릭筌?餓λ쵐?
        if (Vector3.Distance(transform.position, target.position) <= stat.detectionRange)
        {
            playerTransform = target; // ???쟿??곷선??筌왖?類λ릭?遺쎌쓺 ?袁⑤빒 ????????쟿??곷선???袁⑺뒄???곕뗄??
            player = target.GetComponent<Player>(); // ??野껋옕? ????????쟿??곷선?????쟿??곷선?????쟿??곷선 ?뚮똾猷??곕뱜 獄쏆룇釉??

            if (player != null)
            {
                monsterAim.SetTarget(target); //
                animCtrl.SetBool("IsChasing", true);
                SoundManager soundManager = SoundManager.Instance;
                //soundManager.PlaySFX(soundManager.audioClip[4]);  // 獄쏆뮇?꾣뤃????봺 ?ヂ ?醫듼꼻????댭???
            }
        }
        else
        {
            playerTransform = null;
            player = null; // Player ?귥껜?곸쮮夷됱쮬 ?됲슑?곸쭩
            monsterAim.SetTarget(null); // MonsterAim 姨띿쮼?뉗ℓ夷띿??덉㎞????뉗쮰吏명슓夷됱쮬 ?됲슑?곸쭩
        }
    }


    void ChasePlayer()
    {
        if (stat.curHp <= 0 || animCtrl.GetBool("IsAttacking")) return; // 筌ｋ????0癰귣????臾롪탢???⑤벀爰썰빳臾믪뵠筌???뚯궔 ??띾┛ ???????쀫립??
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
            // 揶??됰뜄???????Mesh???袁⑺뒄?? ???읈??PuppetMaster???怨밴묶??筌띿쉸???덈뼄.
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
            // ?諛명??(??됰뻻 Special) ?醫딅굶 筌〓㈇??
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

        // ?꾪슜?泥?夷?夷랁슑?꾪슜吏뱀갹 ?吏좏쉲??NavMeshAgent夷띿쭩 夷섏굅?딆㎏姨뚯쮼?딆㎝?됲슋夷덊슃夷덊슓.
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
        if (stat.curHp > 0)  // ?꾩찈夷뚰쉨???0 ??묒Ł泥???夷뗭쭬夷띿쮰 tIdle ?덉㎞夷띿㎞吏명쉯夷띿쭩 姨뚯??곸쭦
        {


            // ?꾪슜?泥?夷??梨뚯㎏夷?됲슃吏뱀갹 ?吏좏쉲??NavMeshAgent夷띿쭩 ?딆㎏姨뚯쮼?딆㎝?됲슋夷덊슃夷덊슓.
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
            FireProjectile(); // 姨붿꺽吏명쉯夷띿㎞ ?뉗쿇夷⑹콐?꾩찈 夷뷀슜夷⑹콐 夷랁슙姨뚯㎝夷됱콉
            StartCoroutine(ShotTime());
        }
    }

    private IEnumerator ShotTime()
    {
        canShot = false;
        yield return new WaitForSeconds(6f); // ?묅뫂???6??
        canShot = true;
    }

    private void FireProjectile()
    {
        if (currentProjectile != null || ProjectilePrefab == null) return;

        Vector3 spawnPosition = ProjectileSpawnPoint != null ? ProjectileSpawnPoint.position : transform.position;
        Vector3 targetDirection = (playerTransform.position - spawnPosition).normalized;
        Quaternion spawnRotation = Quaternion.LookRotation(targetDirection);

        // ??덉쨮 ??밴쉐??롫뮉 ???????癒?퐣 ??沅쀯㎗?? 揶쎛?紐꾩긾
        currentProjectile = ProjectilePool.Instance.GetProjectile();
        currentProjectile.transform.position = spawnPosition;
        currentProjectile.transform.rotation = spawnRotation;
        currentProjectile.SetActive(true);

        // ??沅쀯㎗?곸벥 ??얜즲 ??쇱젟
        Rigidbody rb = currentProjectile.GetComponent<Rigidbody>();
        rb.velocity = targetDirection * ProjectileSpeed;

        // ??沅쀯㎗?? 獄쏆뮇沅??筌뤣딅뮞????쇱젟
        Projectile projectileComponent = currentProjectile.GetComponent<Projectile>();
        if (projectileComponent != null)
        {
            projectileComponent.SetShooter(this);
        }
    }



    // ?醫딅빍筌롫뗄????온??ㅻ쭆 ??源????λ땾 
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
        DisableAttackCollider(); // ?⑤벀爰??온???뚮똾猷??곕뱜 ??쑵??源딆넅
        DisableAttackMeshRenderer();
    }

    private IEnumerator DisableAttackComponentsAfterDelay()
    {
        yield return new WaitForSeconds(1f); // 1????疫?
        DisableAttackCollider(); // ?⑤벀爰??온???뚮똾猷??곕뱜 ??쑵??源딆넅
        DisableAttackMeshRenderer();
    }
    public void endKnockback()
    {
        isKnockedBack = false;
        DisableAttackCollider(); // ?⑤벀爰??온???뚮똾猷??곕뱜 ??쑵??源딆넅
        DisableAttackMeshRenderer();
    }




}