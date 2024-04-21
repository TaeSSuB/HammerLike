using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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

    [Header("Gold UI")]
    [SerializeField] protected TMP_Text goldText;

    [Header("Inventory UI")]
    [SerializeField] protected GameObject inventoryUI;

    [Header("QuickSlot UI")]
    [SerializeField] protected GameObject quickSlotUI;

    [Header("Map UI")]
    [SerializeField] protected GameObject mapUI;

    private SO_PlayerStatus playerStatus;

    private void Start()
    {
        // Start 이후 실행 초기화 메서드. a.HG
        StartCoroutine(CoInitialize());
    }

    private void OnDisable()
    {
        // 메모리 해제. a.HG
        B_Player player = GameManager.instance.Player;
        player.OnHPChanged -= UpdateHP;
        player.OnChargeChanged -= UpdateGauge;
    }

    public void Initialize()
    {
        Debug.Log("UI_InGame Initialize");
        B_Player player = GameManager.instance.Player;
        playerStatus = player.UnitStatus as SO_PlayerStatus;

        hpBarRctTR = hpBarImg.gameObject.GetComponent<RectTransform>();

        chargeBarRctTR = chargeBarImg.gameObject.GetComponent<RectTransform>();
        chargeParticle.SetActive(false);

        UpdateHP(playerStatus.maxHP);

        // 이벤트 등록. 메모리 해제 주의. a.HG
        player.OnHPChanged += UpdateHP;
        player.OnChargeChanged += UpdateGauge;
    }

    private void UpdateGauge(float chargeRate)
    {
        float chargeRatio = Mathf.Clamp(chargeRate, 0f, 1f); // 0 ~ 1 사이의 값
        chargeBarRctTR.anchorMax = new Vector2(chargeBarRctTR.anchorMax.x, chargeRatio);

        // 파티클 이미지 활성화/비활성화
        if (chargeRatio >= 1f)
        {
            chargeParticle.SetActive(true);
        }
        else
        {
            chargeParticle.SetActive(false);
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
        yield return new WaitUntil(() => GameManager.instance.Player != null);

        Initialize();
    }
}
