using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Rewired;
using DG.Tweening;
using Broccoli.Utils;
using Unity.VisualScripting;

public class UI_InGame : MonoBehaviour
{
    [Header("HP UI")]
    [SerializeField] protected Slider playerHPSlider;
    [SerializeField] protected Slider bossHPSlider;
    [SerializeField] protected GameObject bossHPUI;

    [Header("Charge UI")]
    [SerializeField] protected Slider chargeSlider;
    [SerializeField] protected GameObject chargeParticle;
    [SerializeField] Image chargeScreen;
    private float maximumChargeAmount;

    [Header("Gold UI")]
    [SerializeField] protected TMP_Text goldText;

    [Header("Inventory UI")]
    [SerializeField] protected GameObject inventoryUI;

    [Header("Equipment UI")]
    [SerializeField] protected Image currentWeaponImage;


    [Header("QuickSlot UI")]
    [SerializeField] protected GameObject quickSlotUI;

    [Header("Map UI")]
    [SerializeField] protected GameObject mapUI;


    [Header("Move Panel")]
    [SerializeField] protected RectTransform topPanel;
    [SerializeField] private Vector2 topPanelPos;
    [SerializeField] protected RectTransform bottomPanel;
    [SerializeField] private Vector2 bottomPanelPos;
    [SerializeField] private float duration;
    private Vector2 disableTopPanelPos;
    private Vector2 disableBottomPanelPos;
    private bool isPanelOn = false;

    [SerializeField] private B_Player player;
    private SO_PlayerStatus playerStatus;

    private Coroutine chargeCoroutine;

    public GameObject BossHPUI { get => bossHPUI; }

    private void Awake()
    {
        disableTopPanelPos = topPanel.anchoredPosition;
        disableBottomPanelPos = bottomPanel.anchoredPosition;
    }


    private void Start()
    {
        // Start 이후 실행 초기화 메서드. a.HG
        chargeSlider.value = 0;
        StartCoroutine(CoInitialize());
        EnablePanel();
    }

    private void Update()
    {
        /*if(Input.GetKeyDown(KeyCode.N))
        {
            
            OnOffPanel();
        }*/
    }

    private void OnApplicationQuit()
    {
        // 메모리 해제. a.HG
        //B_Player player = GameManager.Instance.Player; // 메모리 해제 이슈로 주석 처리. a.HG
        Debug.Log("UI_InGame OnApplicationQuit");

        if (player == null)
        {
            Debug.Log("UI_InGame OnDisable - player is null");
            player = FindObjectOfType<B_Player>();
        }

        player.OnHPChanged -= UpdateHP;
        player.OnChargeChanged -= UpdateGauge;
    }

    public void Initialize()
    {
        //Debug.Log("UI_InGame Initialize");
        player = GameManager.Instance.Player;
        playerStatus = player.UnitStatus as SO_PlayerStatus;

        chargeParticle.SetActive(false);
        maximumChargeAmount = chargeScreen.material.GetFloat("_MaxChargeAmount");
        // UpdateHP(playerStatus.maxHP);
        // UpdateGauge(playerStatus.chargeRate);

        // 이벤트 등록. 메모리 해제 주의. a.HG
        player.OnHPChanged += UpdateHP;
        player.OnChargeChanged += UpdateGauge;
    }

    private void UpdateGauge(float chargeRate)
    {
        if (chargeCoroutine != null)
            StopCoroutine(chargeCoroutine);

        chargeCoroutine = StartCoroutine(SmoothChange(chargeRate));
    }

    private IEnumerator SmoothChange(float targetRate)
    {
        float currentRate = chargeSlider.value;
        float timeToReachTarget = Mathf.Abs(currentRate - targetRate);

        float elapsedTime = 0f;
        while (elapsedTime < timeToReachTarget)
        {
            elapsedTime += Time.deltaTime;
            float newRate = Mathf.Lerp(currentRate, targetRate, elapsedTime / timeToReachTarget);
            chargeSlider.value = newRate;

            // Material에 ChargeAmount 값을 업데이트
            if (chargeScreen.material != null)
            {
                chargeScreen.material.SetFloat("_ChargeAmount", newRate*maximumChargeAmount);
            }

            yield return null;
        }

        //chargeSlider.value = newRate;
        chargeParticle.SetActive(targetRate >= 1f);

        // 최종 값도 설정
        if (chargeScreen.material != null)
        {
            chargeScreen.material.SetFloat("_ChargeAmount", targetRate* maximumChargeAmount);
        }
    }


    private void UpdateHP(int hp)
    {
        float hpRatio = (float)hp / playerStatus.maxHP;
        playerHPSlider.value = hpRatio;
    }

    private IEnumerator CoInitialize()
    {
        yield return new WaitUntil(() => GameManager.Instance.Player != null);

        Initialize();
    }

    private void EnablePanel()
    {
        isPanelOn = true;
        topPanel.DOAnchorPos(topPanelPos, duration);
        bottomPanel.DOAnchorPos(bottomPanelPos, duration);
    }

    private void DisablePanel()
    {
        isPanelOn = false;
        topPanel.DOAnchorPos(disableTopPanelPos, duration);
        bottomPanel.DOAnchorPos(disableBottomPanelPos, duration);
    }

    private void OnOffPanel()
    {
        if (isPanelOn)
            DisablePanel();
        else
            EnablePanel();
    }

    public void SetActiveBossHPUI(bool isActive)
    {
        bossHPUI.SetActive(isActive);
    }

    public void UpdateBossHP(B_Boss b_boss)
    {
        bossHPSlider.value = (float)b_boss.UnitStatus.currentHP / b_boss.UnitStatus.maxHP;
    }

    public void UpdateWeaponImage(Sprite sprite)
    {
        currentWeaponImage.sprite = sprite;
    }

}