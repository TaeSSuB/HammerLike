using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CStair : MonoBehaviour
{
    public ButtonManager buttonManager;
    public DataManager dm;
    public int sceneIdx = 0;
    public GameObject[] keyArray;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            for (int i = 0; i < keyArray.Length; i++)
            {
                if (keyArray[i].activeSelf)
                {
                    Debug.Log("Get more Keys");
                    return;
                }
            }

            if (dm == null)
                dm = DataManager.GetDataInstance();

            dm.clearLevelData = sceneIdx;
            dm.SaveGame();
            buttonManager.SceneChange(sceneIdx);
        }
    }

}
