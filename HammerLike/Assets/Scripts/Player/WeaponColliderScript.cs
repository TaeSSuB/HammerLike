using UnityEngine;

public class WeaponColliderScript : MonoBehaviour
{
    public PlayerAtk playerAtk; // PlayerAtk ?¤í¬ë¦½íŠ¸??ì°¸ì¡°

    private void Awake()
    {
        // playerAtk ì»´í¬?ŒíŠ¸ë¥??ë™?¼ë¡œ ì°¾ì•„??? ë‹¹???˜ë„ ?ˆìŠµ?ˆë‹¤.
        // ?˜ì?ë§?FindObjectOfType?€ ?ˆë¬´ ë¹„íš¨?¨ì ?´ê¸´ ?˜ë‹¤
        //playerAtk = FindObjectOfType<PlayerAtk>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Monster"))
        {
            Rigidbody enemyRb = other.GetComponent<Rigidbody>();
            if (enemyRb != null)
            {
                //Vector3 forceDirection = playerAtk.player.transform.forward;
                //enemyRb.AddForce(forceDirection.normalized * playerAtk.forceMagnitude, ForceMode.Impulse);
            }
        }
    }
}
