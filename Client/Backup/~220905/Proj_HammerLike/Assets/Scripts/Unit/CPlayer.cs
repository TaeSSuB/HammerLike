using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using hger;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]
public class CPlayer : CUnitBase
{
    [Header("Player")]
    public float accelSpeed;
    public float dashSpeed = 10f;
    public bool isShield = false;
    public bool isInterAction = false;

    public Transform spriteTr;
    public Transform handTr;
    public GameObject shieldSprite;

    Animator anim;
    Rigidbody2D rigid;
    //public Image hpBar;
    public GameObject hpLayout;
    public GameObject tresureLayout;
    public GameObject heartObj;
    public Text weaponText;
    public GameObject dashVFX;
    public CWeaponSwitcher weaponSwitcher;
    public GameObject InventorySocket;
    public List<CTresure> tresureSocket;

    int xInput, yInput;
    float currentAccel = 1f;
    bool isDashing = false;

    //private void OnTriggerEnter2D(Collider2D collision)
    //{
    //    Debug.Log(collision);
    //}

    void Walk(float _speed, float _currentAccel, Vector2 _dir)
    {
        gameObject.transform.Translate(_dir * _speed * _currentAccel * Time.deltaTime);
    }

    void Movement()
    {
        Vector2 dir = new Vector2(xInput, yInput).normalized;

        if (!isDashing)
            Walk(moveSpeed, currentAccel, dir);
    }

    void Dash()
    {
        StartCoroutine(CoDash(dashSpeed));
    }

    //public void Damaged(int damage)
    //{
    //    StartCoroutine(CoHitted(1f, damage));
    //}

    public void DamagedHeart(int damage)
    {
        var damageQuo = damage / 5; // 5단위로 데미지 입음

        Image[] hearts = hpLayout.GetComponentsInChildren<Image>();

        var beforeHPQuo = Mathf.CeilToInt((currentHp + damage) / 10f) - 1;

        for (int i = 0; i < damageQuo; i++)
        {
            if (hearts.Length > beforeHPQuo && beforeHPQuo >= 0)
            {
                hearts[beforeHPQuo].fillAmount -= 0.5f;

                if (hearts[beforeHPQuo].fillAmount < 0.5f)
                {
                    beforeHPQuo--;
                }

            }
        }
    }

    public void HealedHeart(int heal)
    {
        var healQuo = heal / 5; // 5단위로 회복

        Image[] hearts = hpLayout.GetComponentsInChildren<Image>();

        var beforeHPQuo = Mathf.CeilToInt((currentHp - heal) / 10f) - 1;

        for (int i = 0; i < healQuo; i++)
        {
            if (hearts[beforeHPQuo].fillAmount > 0.5f)
            {
                beforeHPQuo++;
            }

            if (hearts.Length > beforeHPQuo && beforeHPQuo >= 0)
                hearts[beforeHPQuo].fillAmount += 0.5f;
        }
    }

    public void UnitHealed(int heal)
    {
        currentHp += heal;
        if (currentHp > maxHp)
            currentHp = maxHp;
        HealedHeart(heal);
    }

    public override void UnitDamaged(int damage)
    {
        //base.UnitDamaged(damage);
        StartCoroutine(CoHitted(1f, damage));
    }

    public void KnockBack(Vector2 force)
    {
        if (force.magnitude / rigid.mass > 30f)
        {
            force = force.normalized * 30f;
        }

        rigid.AddForce(force, ForceMode2D.Impulse);
    }

    void Aiming()
    {
        Vector3 mousePos = Utils.GetMouseWorldPosition();

        Vector3 mouseDir = mousePos - transform.position;

        if (mouseDir.x < 0)
        {
            //handTr.localScale = new Vector2(1, -1);
            spriteTr.localScale = new Vector2(-1, 1);
        }
        else
        {
            handTr.localScale = new Vector2(1, 1);
            spriteTr.localScale = new Vector2(1, 1);
        }

        // tan = y/x
        float rotZ = Mathf.Atan2(mouseDir.y, mouseDir.x) * Mathf.Rad2Deg;

        handTr.rotation = Quaternion.Euler(0, 0, rotZ);
    }

    void Dead()
    {
        Debug.Log("Player Die");
        state = State.Dead;
    }

    IEnumerator CoDash(float dashSpeed)
    {
        //Vector3 mousePos = Utils.GetMouseWorldPosition();
        //Vector3 mouseDir = mousePos - transform.position;

        //Vector2 dir = mouseDir.normalized;
        Vector2 dir = new Vector2(xInput, yInput).normalized;

        anim.SetTrigger("TDash");

        //rigid.AddForce(dir * dashSpeed * rigid.mass, ForceMode2D.Impulse);
        KnockBack(dir * dashSpeed * rigid.mass);

        isDashing = true;
        isDodge = true;
        //damageCoeffcient = 0.5f;

        Instantiate(dashVFX, gameObject.transform.position, dashVFX.transform.rotation);

        yield return new WaitForSeconds(0.2f);

        rigid.velocity = Vector2.zero;
        isDashing = false;

        yield return new WaitForSeconds(0.1f);

        isDodge = false;
        //damageCoeffcient = 1f;
    }

    IEnumerator CoHitted(float delay, int damage)
    {
        if (!isShield && state != State.Dead)
        {
            // 완전 회피
            if (!isDodge)
            {
                currentHp -= (int)(damage * damageCoeffcient);

                DamagedHeart(damage);

                //anim.SetTrigger("TDamaged");
                anim.SetBool("BDamaged", true);
                //GetComponent<Collider2D>().enabled = false;
                isDodge = true;
                yield return new WaitForSeconds(delay);

                anim.SetBool("BDamaged", false);
                //GetComponent<Collider2D>().enabled = true;
                isDodge = false;
            }
            else
            {
                yield return new WaitForSeconds(delay);
                isDodge = false;
            }
        }
        else if (isShield)
        {
            Debug.Log("Defend Attack");
            yield return new WaitForSeconds(delay);
            isShield = false;
            shieldSprite.SetActive(false);
        }

        if (currentHp <= 0)
        {
            Dead();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        hpLayout = GameObject.Find("HPLayout");

        for(int i = 0; i < maxHp; i += 10)
            Instantiate(heartObj, hpLayout.transform);

        //hpBar = GameObject.Find("PCurrentHP").GetComponent<Image>();
        weaponText = GameObject.Find("PWeaponText").GetComponent<Text>();
        weaponSwitcher = GetComponentInChildren<CWeaponSwitcher>();
        SetHP();
        shieldSprite.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (state != State.Dead)
        {
            //if(hpBar != null)
            //    hpBar.fillAmount = (float)currentHp / (float)maxHp;
            //if (hpLayout != null)
            //    Instantiate(heartObj, hpLayout.transform);

            if (weaponText != null)
                weaponText.text = (weaponSwitcher.currentIdx + 1).ToString() + "/" + (weaponSwitcher.myWeaponsList.Count).ToString();

            if (!isInterAction)
            {
                if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
                {
                    state = State.Move;

                    if (Input.GetKey(KeyCode.D))
                        xInput = 1;
                    else if (Input.GetKey(KeyCode.A))
                        xInput = -1;
                    else
                        xInput = 0;

                    if (Input.GetKey(KeyCode.W))
                        yInput = 1;
                    else if (Input.GetKey(KeyCode.S))
                        yInput = -1;
                    else
                        yInput = 0;

                    Movement();

                    //if (Input.GetMouseButtonDown(1) && !isDashing)
                    if (Input.GetMouseButtonDown(1) && !isDashing)
                    {
                        Dash();
                    }

                    //anim.SetBool("BMove", true);
                }
                else
                {
                    state = State.Idle;
                    //anim.SetBool("BMove", false);
                }

                if (state == State.Move)
                {
                    anim.SetBool("BMove", true);
                }
                else
                {
                    anim.SetBool("BMove", false);
                }

                //if(Input.GetMouseButtonDown(0))
                Aiming();
            }
        }
    }

}
