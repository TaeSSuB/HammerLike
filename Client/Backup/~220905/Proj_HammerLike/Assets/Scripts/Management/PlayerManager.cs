using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerManager : MonoBehaviour
{
    public TextMeshProUGUI TMPscriptCaption;
    public string[] scripts;

    void Start()
    {
        Talk("");
        //InvokeRepeating("BlaBla", 3f, 10f);
        StartCoroutine(CoBlaBla(3f, 10f));
    }

    void BlaBla()
    {
        int rnd = Random.Range(0, scripts.Length);
        Talk(scripts[rnd]);
    }

    void Talk(string strScript)
    {
        TMPscriptCaption.text = strScript;
    }

    IEnumerator CoBlaBla(float duration, float repeatRate)
    {
        while (true)
        {
            Talk("");
            yield return new WaitForSeconds(repeatRate);
            BlaBla();
            yield return new WaitForSeconds(duration);
        }
    }
}
