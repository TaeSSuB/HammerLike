using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailMaterialChanger : MonoBehaviour
{
    public TrailRenderer trailRenderer;
    public Material[] materials; // 사용할 머티리얼 배열
    public float[] materialRatios; // 각 머티리얼의 비율

    private float totalLength; // 트레일의 총 길이

    void Start()
    {
        if (materials.Length != materialRatios.Length)
        {
            Debug.LogError("Materials and ratios length must be the same");
            return;
        }

        totalLength = trailRenderer.time;
        UpdateMaterial(0); // 시작할 때 첫 번째 머티리얼로 설정
    }

    void Update()
    {
        float currentLength = CalculateCurrentLength();
        UpdateMaterial(currentLength / totalLength);
    }

    // 현재 트레일의 길이를 계산하는 함수
    float CalculateCurrentLength()
    {
        // 여기서는 간단히 시간을 이용하지만, 실제 구현에서는 다른 방법을 사용할 수 있음
        return Time.timeSinceLevelLoad % totalLength;
    }

    // 트레일의 현재 비율에 따라 머티리얼을 업데이트하는 함수
    void UpdateMaterial(float currentRatio)
    {
        float cumulativeRatio = 0;

        for (int i = 0; i < materialRatios.Length; i++)
        {
            cumulativeRatio += materialRatios[i];
            if (currentRatio <= cumulativeRatio)
            {
                trailRenderer.material = materials[i];
                break;
            }
        }
    }
}
