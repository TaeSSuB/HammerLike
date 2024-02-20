using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class FixRotation : MonoBehaviour
{
    void Update()
    {
        transform.rotation = Quaternion.Euler(0f,0f,0f);
    }
}
