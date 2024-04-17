using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 본 스크립트는 4월 12일 발표용 빌드를 위해 임시로 작성됨.
/// 추후 삭제 및 UIManager 통합 예정. (4월 13일 이후) - a.HG
/// </summary>
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
        float chargeRate = (playerStatus.chargeRate - 1) / (playerStatus.maxChargeRate - 1);

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
