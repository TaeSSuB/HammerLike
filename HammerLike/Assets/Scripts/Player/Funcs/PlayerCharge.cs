using UnityEngine;
using UnityEngine.UI;

public class PlayerCharge : MonoBehaviour
{
    public Slider chargeSlider;
    public Player player; // player 변수를 public으로 선언하여 Inspector에서 할당할 수 있도록 함

    private void Start()
    {
        // 시작 시 슬라이더를 숨깁니다.
        chargeSlider.gameObject.SetActive(false);
    }

    private void Update()
    {
        // player.atk.curCharging 값이 1 이상일 때만 슬라이더를 보여줍니다.
        if (player.atk.curCharging >= 1f)
        {
            // 슬라이더가 활성화되어 있지 않다면 활성화합니다.
            if (!chargeSlider.gameObject.activeSelf)
            {
                chargeSlider.gameObject.SetActive(true);
            }

            // 슬라이더의 값을 curCharging 값에 맞게 업데이트합니다.
            // 여기서는 10으로 나눠서 0~1 사이의 값을 갖도록 조정합니다.
            chargeSlider.value = (player.atk.curCharging - 1) / (10 - 1);
        }
        else if (chargeSlider.gameObject.activeSelf)
        {
            // 슬라이더를 비활성화합니다.
            chargeSlider.gameObject.SetActive(false);
        }
    }
}
