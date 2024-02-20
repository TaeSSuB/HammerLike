using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterAtk : MonoBehaviour
{
    public Monster monster; // Monster 컴포넌트에 대한 참조

    void Awake()
    {
        //monster = GetComponentInParent<Monster>(); // Monster 컴포넌트를 찾아 할당
        if (monster == null)
        {
            Debug.LogError("Monster component not found in parent");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (monster == null)
        {
            Debug.Log("monster is null");
            return; // monster가 null이면 함수 종료
        }

        if (other.gameObject.CompareTag("Player") && monster.attackCollider.enabled)
        {
            Player player = monster.Player; // getter를 사용하여 player에 접근
            if (player != null&&!player.isEvading)
            {
                player.TakeDamage(monster.stat.attackPoint);
                if(monster.monsterType==MonsterType.Melee)
                {
                    SoundManager soundManager = SoundManager.Instance;
                    soundManager.PlaySFX(soundManager.audioClip[7]);
                }
               
            }
        }
    }
}

