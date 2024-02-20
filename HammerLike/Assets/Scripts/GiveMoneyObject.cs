using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GiveMoneyObject : MonoBehaviour
{
    public ItemManager item;
    public MeshDestroy mesh;
    private bool isGiven = false;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(!isGiven&&mesh.curHp<=0)
        {
            item.DropItem(0, transform.position);
            isGiven = true;
        }
    }
}
