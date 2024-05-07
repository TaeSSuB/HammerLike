using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Rewired;

public class UI_InGame : MonoBehaviour
{
    [Header("HP UI")]
    [SerializeField] protected Image hpBarImg;
    [SerializeField] protected TMP_Text hpMaxText;
    [SerializeField] protected TMP_Text hpCurrentText;
    protected RectTransform hpBarRctTR;

    [Header("Charge UI")]
    [SerializeField] protected Image chargeBarImg;
    protected RectTransform chargeBarRctTR;
    [SerializeField] protected GameObject chargeParticle;
    [SerializeField] private float decreaseSpeed = 1f;
    [SerializeField] Image chargeScreen;
    private float maximumChargeAmount;
    

    [Header("Gold UI")]
    [SerializeField] protected TMP_Text goldText;

    [Header("Inventory UI")]
    [SerializeField] protected GameObject inventoryUI;

    [Header("QuickSlot UI")]
    [SerializeField] protected GameObject quickSlotUI;

    [Header("Map UI")]
    [SerializeField] protected GameObject mapUI;

    [SerializeField] private B_Player player;
    private SO_PlayerStatus playerStatus;

    private Coroutine chargeCoroutine;
    

    private void Start()
    {
        // Start 이후 실행 초기화 메서드. a.HG
        StartCoroutine(CoInitialize());
        
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

        hpBarRctTR = hpBarImg.gameObject.GetComponent<RectTransform>();

        chargeBarRctTR = chargeBarImg.gameObject.GetComponent<RectTransform>();
        chargeParticle.SetActive(false);
        maximumChargeAmount = chargeScreen.material.GetFloat("_MaxChargeAmount");
        UpdateHP(playerStatus.maxHP);
        UpdateGauge(playerStatus.chargeRate);

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
        float currentRate = chargeBarRctTR.anchorMax.y;
        float timeToReachTarget = Mathf.Abs(currentRate - targetRate) / decreaseSpeed;

        float elapsedTime = 0f;
        while (elapsedTime < timeToReachTarget)
        {
            elapsedTime += Time.deltaTime;
            float newRate = Mathf.Lerp(currentRate, targetRate, elapsedTime / timeToReachTarget);
            chargeBarRctTR.anchorMax = new Vector2(chargeBarRctTR.anchorMax.x, newRate);

            // Material에 ChargeAmount 값을 업데이트
            if (chargeScreen.material != null)
            {
                chargeScreen.material.SetFloat("_ChargeAmount", newRate*maximumChargeAmount);
            }

            yield return null;
        }

        chargeBarRctTR.anchorMax = new Vector2(chargeBarRctTR.anchorMax.x, targetRate);
        chargeParticle.SetActive(targetRate >= 1f);

        // 최종 값도 설정
        if (chargeScreen.material != null)
        {
            chargeScreen.material.SetFloat("_ChargeAmount", targetRate* maximumChargeAmount);
        }
    }


    private void UpdateHP(int hp)
    {
        hpMaxText.text = playerStatus.maxHP.ToString();
        hpCurrentText.text = hp.ToString();

        float hpRatio = (float)hp / playerStatus.maxHP;
        hpBarRctTR.anchorMax = new Vector2(hpBarRctTR.anchorMax.x, hpRatio);
    }

    private IEnumerator CoInitialize()
    {
        yield return new WaitUntil(() => GameManager.Instance.Player != null);

        Initialize();
    }

    
}