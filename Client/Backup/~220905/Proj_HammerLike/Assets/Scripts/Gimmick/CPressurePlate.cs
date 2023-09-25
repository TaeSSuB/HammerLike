using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CPressurePlate : MonoBehaviour
{
    public int pressCount = 0;
    public bool isOneTime = false;
    public bool isPressed = false;
    public GameObject[] ActiveArray;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") || collision.CompareTag("Golem"))
        {
            pressCount++;
            if (pressCount > 0)
            {
                isPressed = true;
                for(int i = 0; i < ActiveArray.Length; i++)
                {
                    ActiveArray[i].SetActive(false);
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") || collision.CompareTag("Golem"))
        {
            if (!isOneTime)
            {
                pressCount--;
                if (pressCount <= 0)
                {
                    isPressed = false;
                    for (int i = 0; i < ActiveArray.Length; i++)
                    {
                        ActiveArray[i].SetActive(true);
                    }
                }
            }
        }
    }
}
