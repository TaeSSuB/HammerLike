using System.Collections;
using UnityEngine;
using UnityEngine.UI; // UI 컴포넌트 사용을 위해 추가

public class UpdateCameraShake : MonoBehaviour
{
    public RectTransform panel; // 흔들릴 패널의 RectTransform
    public float shakeIntensity = 0.5f; // 흔들림의 강도
    public float shakeTime = 0.5f; // 흔들림 지속 시간

    private Vector3 originalPosition; // 패널의 원래 위치
    private float timer;

    void Start()
    {
        if (panel != null)
            originalPosition = panel.localPosition; // 시작 시 패널의 원래 위치 저장
    }

    public void ShakePanel()
    {
        if (timer <= 0)
        {
            StartCoroutine(DoShake());
        }
    }

    IEnumerator DoShake()
    {
        timer = shakeTime;
        while (timer > 0)
        {
            panel.localPosition = originalPosition + (Vector3)Random.insideUnitCircle * shakeIntensity;
            timer -= Time.deltaTime;
            yield return null;
        }
        panel.localPosition = originalPosition; // 흔들림이 끝나면 패널을 원래 위치로
    }

    // UI 슬라이더에서 호출할 수 있는 메서드
    public void SetShakeIntensity(float intensity)
    {
        shakeIntensity = intensity;
    }
}
