using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamCtrl : MonoBehaviour
{

    public Camera followCam;  //For RenderTex
    public PixelPerfectCamera zoomCam; //Only control zoom using Height vari, For Screen Render

    [Space(10f)]
    public Transform followObj;

	private void Awake()
	{
		
	}

	void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
