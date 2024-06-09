using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Temp_ApplySettingRigid : MonoBehaviour
{
    
    // Start is called before the first frame update
    void Start()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.inertiaTensor = Vector3.one;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
