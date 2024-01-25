using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testalpha : MonoBehaviour
{
    // Start is called before the first frame update
    private Material material;
    
    void Start()
    {
        // 머티리얼 초기화
        material = GetComponent<Renderer>().material;
    }

    private void Update()
    {
        if(Input.GetKeyUp(KeyCode.A))
        {
            OnHit();
        }
    }

    // 투명도 설정 함수
    public void SetTransparency(float alpha)
    {
        if (material != null)
        {
            Color color = material.GetColor("_BaseColor");
            color.a = alpha; // 알파 값 조절
            material.SetColor("_BaseColor", color);
        }
    }

    // 플레이어가 피격되었을 때 호출될 함수
    public void OnHit()
    {
        // 투명도를 0.5로 설정
        SetTransparency(0.5f);

        // 필요한 경우 투명도를 다시 원래대로 되돌릴 수 있습니다.
        StartCoroutine(ResetTransparency());
    }

    // 알파값을 원래대로 되돌리는 코루틴
    IEnumerator ResetTransparency()
    {
        // 1초 기다림
        yield return new WaitForSeconds(1);

        // 투명도를 원래 값인 1.0으로 설정
        SetTransparency(1.0f);
    }
}
