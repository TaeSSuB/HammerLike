using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarningEffect : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField] GameObject hitObject;
    [SerializeField] GameObject projectile;
    [SerializeField] private BoxCollider boxCollider; // 이 게임 오브젝트의 BoxCollider


    void Start()
    {
        boxCollider = GetComponent<BoxCollider>();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Ground"))
        {
            Instantiate(hitObject);
            CheckPlayerInAttackRange();
        }
    }

    void CheckPlayerInAttackRange()
    {
        Vector3 boxCenter = transform.TransformPoint(boxCollider.center); // 월드 좌표에서 박스 콜라이더의 중심 계산
        Collider[] hitColliders = Physics.OverlapBox(boxCenter, boxCollider.size / 2, transform.rotation, LayerMask.GetMask("Player")); // 박스 형태로 플레이어 감지

        foreach (Collider hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Player"))
            {
                B_Player targetPlayer = GameManager.Instance.Player;
                if (targetPlayer != null)
                {
                    Debug.Log("Hit");
                    //targetPlayer.ApplyDamage(attackDamage); // 플레이어에게 피해 적용
                    // 선택적으로 사운드 효과 재생 가능
                    // SoundManager.Instance.PlaySFX(SoundManager.Instance.audioClip[8]);
                }
            }
        }
    }
}
