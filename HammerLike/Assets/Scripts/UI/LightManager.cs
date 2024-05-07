using UnityEngine;

public class LightManager : MonoBehaviour
{
    public Light targetLight; // 대상 Light 컴포넌트

    void Start()
    {
        if (targetLight == null)
        {
            Debug.LogError("Light component is not assigned.");
            return;
        }

        // 특정 레이어 설정 예시
        //SetLightRenderingLayer("LayerName");
    }

    // 레이어 이름으로 Rendering Layer 설정
    public void SetLightRenderingLayer(string layerName)
    {
        int layer = LayerMask.NameToLayer(layerName);
        if (layer == -1)
        {
            Debug.LogError("Layer not found: " + layerName);
            return;
        }

        targetLight.renderingLayerMask = (int)(1 << layer); // 명시적 캐스팅
    }

    // 모든 레이어에 대해 Light의 영향을 활성화
    public void IncludeAllLayers()
    {
        targetLight.renderingLayerMask = int.MaxValue; // 모든 레이어 포함
    }

    // 모든 레이어에 대해 Light의 영향을 비활성화
    public void ExcludeAllLayers()
    {
        targetLight.renderingLayerMask = 0; // 모든 레이어 제외
    }
}
