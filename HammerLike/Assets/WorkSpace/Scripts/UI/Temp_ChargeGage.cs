using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Temp_ChargeGage : MonoBehaviour
{
    public Image gaugeImage; // 게이지 이미지
    public GameObject particleObject; // 파티클 이미지 오브젝트

    private SO_PlayerStatus playerStatus; // 플레이어의 상태 정보
    private RectTransform gaugeRectTransform;

    private void Awake()
    {
        gaugeRectTransform = gaugeImage.GetComponent<RectTransform>();
        particleObject.SetActive(false); // 초기에는 파티클 이미지 숨김
    }

    private void Start()
    {
        //playerStatus = GameManager.instance.Player.UnitStatus as SO_PlayerStatus;
    }

    private void Update()
    {
        if(playerStatus != null)
            UpdateGauge();
        else
            playerStatus = GameManager.instance.Player.UnitStatus as SO_PlayerStatus;
    }

    private void UpdateGauge()
    {
        float chargeRate = (playerStatus.chargeRate / playerStatus.maxChargeRate);

        float chargeRatio = Mathf.Clamp(chargeRate, 0f, 1f); // 0 ~ 1 사이의 값
        gaugeRectTransform.anchorMax = new Vector2(gaugeRectTransform.anchorMax.x, chargeRatio);

        // 파티클 이미지 활성화/비활성화
        if (chargeRatio >= 1f)
        {
            particleObject.SetActive(true);
        }
        else
        {
            particleObject.SetActive(false);
        }
    }
}
