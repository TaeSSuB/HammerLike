using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : CItem
{
    public int goldAmount = 10;

    private void OnTriggerEnter2D(Collider2D col2d)
    {
        if(col2d.CompareTag("Player"))
        {
            //var player = col2d.GetComponent<CPlayer>();
            var dm = DataManager.GetDataInstance();

            dm.UseGold(-goldAmount);

            Destroy(gameObject);
        }
    }
}
