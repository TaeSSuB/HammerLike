using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class B_Jade : B_NPC
{
    // Start is called before the first frame update
    /*void Start()
    {
        
    }*/


    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

    }


    void StartInteraction()
    {
        base.StartInteraction();
        Debug.Log("Jade Interaction Start");
    }

    void EndInteraction()
    {
        Debug.Log("Jade Interaction End");
    }

}
