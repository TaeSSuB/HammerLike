using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamCtrl : MonoBehaviour
{
    //팔로우 오브젝트 따라 댕기는데

    
    //1. 스테이지 바깥을 넘어가지 않을 것.
    //2. 약간 느리게 따라 갈 것. 
    //3. 


    public Camera mainCam;  //For RenderTex = mainCam
    public PixelPerfectCamera zoomCam; //Only control zoom using Height vari, For Screen Render

    [Space(10f)]
    public Transform followObjTr;

    public void CameraCallibration()
    { 
        //씬 초기화, 스테이지 초기화등 플레이어를 중앙에 두어야 할 때.
        //호출 한번씩 해주쇼잉
    
    }



	private void Awake()
	{
        if (!mainCam)
        {
            mainCam = Camera.main;
        }


        if (!zoomCam)
        { 
            for (int i = 0; i < transform.childCount; ++i)
            {
                GameObject child = transform.GetChild(i).gameObject;

                if (child.name.Equals("Zoom Camera"))
                {
                    //어차피 씬 초기화 단계에서 찾는거기에 크게 신경 안쓰는 편.
                    //string형으로 오브젝트 찾는거 도저히 못참을 경우 없애도 무방.
                    zoomCam = child.GetComponent<PixelPerfectCamera>();
                }

            }
        }
	}

	void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
