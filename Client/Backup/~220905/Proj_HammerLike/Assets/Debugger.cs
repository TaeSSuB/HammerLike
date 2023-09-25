using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Debugger : MonoBehaviour
{

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.LeftArrow) && Input.GetKeyDown(KeyCode.RightArrow))
        {
            var dm = DataManager.GetDataInstance();
            dm.UseGold(-9999);
        }
    }
}
