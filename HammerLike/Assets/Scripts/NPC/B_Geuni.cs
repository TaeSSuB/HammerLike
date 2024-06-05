using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class B_Geuni : B_NPC
{

    /* void Start()
     {

     }*/


    protected override void Update()
    {
        base.Update();
        
    }

    void StartInteraction()
    {
        base.StartInteraction();
        Debug.Log("Geuni Interaction Start");
    }

    void EndInteraction()
    {
        Debug.Log("Geuni Interaction End");
    }
}
