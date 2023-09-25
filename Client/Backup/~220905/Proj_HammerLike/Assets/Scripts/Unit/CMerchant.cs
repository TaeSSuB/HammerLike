using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CMerchant : CNPC
{
    public GameObject test;

    public override void Chat(string script)
    {
        base.Chat(script);
        test.SetActive(true);
    }

    public override void Exitplayer()
    {
        base.Exitplayer();
        test.SetActive(false);
    }

    public override void ChatEnd()
    {
        base.ChatEnd();
        test.SetActive(false);
    }
}
