using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CKevlarVest : CTresure
{
    public float cooltime = 60f;

    Coroutine coroutine;

    private void OnEnable()
    {
        if (!isActive)
        {
            isActive = true;
            StartCoroutine(CoShield(cooltime));
        }
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    void GetShield()
    {
        if (player != null && !player.isShield)
        {
            player.isShield = true;
            player.shieldSprite.SetActive(true);
            Debug.Log("Get Shield");
        }
    }

    IEnumerator CoShield(float _coolTime)
    {
        while (true)
        {
            if (player != null && !player.isShield)
            {
                player.isShield = true;
                player.shieldSprite.SetActive(true);
                Debug.Log("Get Shield");
                yield return new WaitForSeconds(_coolTime);
            }
            yield return new WaitForSeconds(1f);
        }
    }
}
