using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookCam : MonoBehaviour
{
    public enum ProjectionType
    {
        Perspective,
        Orthographic
    }
    

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Camera.main != null)
        {
            switch(Camera.main.orthographic)
            {
                case true:
                    //transform.LookAt(Camera.main.transform);
                    transform.rotation = Quaternion.Euler(30, 0, 0);
                    break;
                case false:
                    transform.LookAt(Camera.main.transform);
                    break;
            }
        }
    }
}
