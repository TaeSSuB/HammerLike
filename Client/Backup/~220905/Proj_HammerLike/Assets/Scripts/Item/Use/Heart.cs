using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heart : CItem
{
    public int healAmount = 10;

    private void OnTriggerEnter2D(Collider2D col2d)
    {
        if(col2d.CompareTag("Player"))
        {
            var player = col2d.GetComponent<CPlayer>();
            if (player.maxHp > player.currentHp)
            {
                player.UnitHealed(healAmount);
                Destroy(gameObject);
            }
        }
    }
}
