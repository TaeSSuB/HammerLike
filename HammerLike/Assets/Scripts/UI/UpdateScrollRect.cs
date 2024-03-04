using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpdateScrollRect : MonoBehaviour
{
    // Start is called before the first frame update
    private ScrollRect scrollRect;

    public GameObject gamePlayPanel;
    public GameObject soundPanel;
    public GameObject graphicPanel;

    public RectTransform gamePlayRect;
    public RectTransform soundPlayRect;
    public RectTransform graphicRect;
    void Start()
    {
        scrollRect = GetComponent<ScrollRect>();
    }

    // Update is called once per frame
    void Update()
    {
        if(gamePlayPanel.activeSelf)
        {
           
            scrollRect.content= gamePlayRect;
        }
        else if(soundPanel.activeSelf)
        {
            scrollRect.content = soundPlayRect;
        }
        else if(graphicPanel.activeSelf)
        {
            scrollRect.content = graphicRect;
        }
    }
}
