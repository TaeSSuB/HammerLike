using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine;
using UnityEngine.VFX;

public class VisualEffectController : MonoBehaviour
{
    // Visual Effect 컴포넌트를 참조하기 위한 변수
    public VisualEffect visualEffect;

    // Start는 스크립트가 처음 실행될 때 호출됩니다.
    void Start()
    {
        // Visual Effect 컴포넌트가 설정되었는지 확인
        if (visualEffect == null)
        {
            Debug.LogError("Visual Effect component is not assigned.");
            return;
        }

        // Visual Effect 실행
        PlayVisualEffect();
    }

    private void Update()
    {
        if(Input.GetMouseButtonUp(1))
        {
            PlayVisualEffect();
        }
    }

    // Visual Effect를 실행하는 메소드
    public void PlayVisualEffect()
    {
        if (visualEffect != null)
        {
            visualEffect.Play();
        }
    }

    // Visual Effect를 중지하는 메소드
    public void StopVisualEffect()
    {
        if (visualEffect != null)
        {
            visualEffect.Stop();
        }
    }
}
