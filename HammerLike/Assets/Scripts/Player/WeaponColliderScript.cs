using UnityEngine;

public class WeaponColliderScript : MonoBehaviour
{
    public PlayerAtk playerAtk; // PlayerAtk 스크립트의 참조

    private void Awake()
    {
        // playerAtk 컴포넌트를 자동으로 찾아서 할당할 수도 있습니다.
        // 하지만 FindObjectOfType은 너무 비효율적이긴 하다
        //playerAtk = FindObjectOfType<PlayerAtk>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Monster"))
        {
            Rigidbody enemyRb = other.GetComponent<Rigidbody>();
            if (enemyRb != null)
            {
                // Player가 바라보는 방향으로 힘을 가함
                Vector3 forceDirection = playerAtk.player.transform.forward;
                enemyRb.AddForce(forceDirection.normalized * playerAtk.forceMagnitude, ForceMode.Impulse);
            }
        }
    }
}
