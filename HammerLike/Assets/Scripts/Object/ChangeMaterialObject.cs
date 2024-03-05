using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeMaterialObject : MonoBehaviour
{
    public Material[] hitMaterials; // 피격 시 적용할 머티리얼 배열
    private Material[] originalMaterials; // 원래 머티리얼을 저장할 배열
    public MeshRenderer meshRenderer; // MeshRenderer 컴포넌트

    private void Awake()
    {
        // MeshRenderer 컴포넌트를 찾아 할당
        meshRenderer = GetComponentInChildren<MeshRenderer>();
        // 원래의 머티리얼을 복사하여 저장
        originalMaterials = meshRenderer.materials;
    }

    // 피격 시 호출될 메서드
    public void OnHit()
    {
        // 피격 머티리얼로 변경하는 코루틴 호출
        StartCoroutine(ChangeMaterialsTemporarily());
    }

    // 머티리얼을 잠시 변경하고 원래대로 복원하는 코루틴
    IEnumerator ChangeMaterialsTemporarily()
    {
        meshRenderer.materials = hitMaterials;
        // 0.3초 대기
        yield return new WaitForSeconds(0.3f);
        // 원래 머티리얼로 복원
        meshRenderer.materials = originalMaterials;
    }
}
