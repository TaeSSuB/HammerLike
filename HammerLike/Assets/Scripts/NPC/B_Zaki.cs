using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class B_Zaki : B_NPC
{
    // Start is called before the first frame update
    /*void Start()
    {
        base.Start();
    }*/




    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

    }

    void StartInteraction()
    {
        base.StartInteraction();
        Debug.Log("Zaki Interaction Start");
    }

    void EndInteraction()
    {
        base.EndInteraction();
        Debug.Log("Zaki Interaction End");
    }
}
