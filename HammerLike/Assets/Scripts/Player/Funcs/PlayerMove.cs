using System.Collections;
using System.Collections.Generic;

using UnityEngine;

//이동, 회피 등

public class PlayerMove : MonoBehaviour
{

    public void Move(float moveSpd)
    {
        float horizon = Input.GetAxis("Horizontal");
        float vert = Input.GetAxis("Vertical");

        Vector3 dir = new Vector3(-horizon, 0f, vert).normalized;

        transform.Translate(dir * moveSpd * Time.deltaTime);
    }

    public void Envasion(EnvasionStat stat)
    { 
        
    }

}
