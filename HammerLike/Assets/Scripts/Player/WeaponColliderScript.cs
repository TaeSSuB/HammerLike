<<<<<<< HEAD
using UnityEngine;

public class WeaponColliderScript : MonoBehaviour
{
    public PlayerAtk playerAtk; // PlayerAtk ½ºÅ©¸³Æ®ÀÇ ÂüÁ¶

    private void Awake()
    {
        // playerAtk ÄÄÆ÷³ÍÆ®¸¦ ÀÚµ¿À¸·Î Ã£¾Æ¼­ ÇÒ´çÇÒ ¼öµµ ÀÖ½À´Ï´Ù.
        // ¿¹: playerAtk = FindObjectOfType<PlayerAtk>();
=======
ï»¿using UnityEngine;

public class WeaponColliderScript : MonoBehaviour
{
    public PlayerAtk playerAtk; // PlayerAtk ìŠ¤í¬ë¦½íŠ¸ì˜ ì°¸ì¡°

    private void Awake()
    {
        // playerAtk ì»´í¬ë„ŒíŠ¸ë¥¼ ìë™ìœ¼ë¡œ ì°¾ì•„ì„œ í• ë‹¹í•  ìˆ˜ë„ ìˆìŠµë‹ˆë‹¤.
        // í•˜ì§€ë§Œ FindObjectOfTypeì€ ë„ˆë¬´ ë¹„íš¨ìœ¨ì ì´ê¸´ í•˜ë‹¤
        //playerAtk = FindObjectOfType<PlayerAtk>();
>>>>>>> 490b48c1d07f9272897a1d5bb968027958be33a4
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Monster"))
        {
            Rigidbody enemyRb = other.GetComponent<Rigidbody>();
            if (enemyRb != null)
            {
<<<<<<< HEAD
                // Player°¡ ¹Ù¶óº¸´Â ¹æÇâÀ¸·Î ÈûÀ» °¡ÇÔ
=======
                // Playerê°€ ë°”ë¼ë³´ëŠ” ë°©í–¥ìœ¼ë¡œ í˜ì„ ê°€í•¨
>>>>>>> 490b48c1d07f9272897a1d5bb968027958be33a4
                Vector3 forceDirection = playerAtk.player.transform.forward;
                enemyRb.AddForce(forceDirection.normalized * playerAtk.forceMagnitude, ForceMode.Impulse);
            }
        }
    }
}
