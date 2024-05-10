using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorAttack : MonoBehaviour
{
    [SerializeField] private float attackDamage; // 공격으로 인한 피해량
    [SerializeField] private float time; // 객체가 자체를 체크하거나 파괴하는 시간
    [SerializeField] private bool isTick = false; // 체크 모드를 결정하는 플래그
    [SerializeField] private float tickTime; // 주기적 체크의 시간 간격

    private float nextTickTime = 0f; // 다음 틱 시간을 추적하는 타이머
    private BoxCollider boxCollider; // 이 게임 오브젝트의 BoxCollider
    [SerializeField] private GameObject particleObject;

    void Start()
    {
        boxCollider = GetComponent<BoxCollider>(); // BoxCollider 컴포넌트를 가져옴
        if (boxCollider == null)
        {
            Debug.LogError("BoxCollider component is missing on the object!");
            return;
        }

        if (isTick)
        {
            StartCoroutine(TickCheck()); // isTick이 true면 주기적으로 체크하는 코루틴 시작
        }
        else
        {
            //StartCoroutine(DelayedCheck()); // isTick이 false면 지연된 한 번의 체크를 실행하는 코루틴 시작
        }
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Projectile"))
        {
            GameObject particle = Instantiate(particleObject,new Vector3(transform.position.x,0, transform.position.z),Quaternion.identity);
            CheckPlayerInAttackRange();
            Destroy(gameObject);
            Destroy(particle, 1f);
        }
    }

    IEnumerator TickCheck()
    {
        while (true)
        {
            yield return new WaitForSeconds(tickTime); // tickTime만큼 대기
            CheckPlayerInAttackRange(); // 공격 범위 내에 플레이어가 있는지 체크
        }
        Destroy(gameObject);
    }

    IEnumerator DelayedCheck()
    {
        yield return new WaitForSeconds(time); // time만큼 대기
        CheckPlayerInAttackRange(); // 시간이 지난 후 플레이어 체크
        Destroy(gameObject); // 체크 후 객체 파괴
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

    private void OnDestroy()
    {
        StopAllCoroutines(); // 객체가 파괴될 때 모든 코루틴을 중지
    }
}
