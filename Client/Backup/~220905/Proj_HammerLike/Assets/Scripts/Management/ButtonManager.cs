using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonManager : MonoBehaviour
{
    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }

    public void SceneChange(int idx)
    {
        Debug.Log("SceneChange : " + idx);
        SceneManager.LoadScene(idx);
    }

    public void NewGame(int idx = 1)
    {
        var dm = DataManager.GetDataInstance();
        dm.clearLevelData = idx;
        dm.SaveGame();
        dm.saveWeapons.Clear();
        SceneChange(idx);
    }

    public void LoadGame()
    {
        var dm = DataManager.GetDataInstance();
        dm.LoadGame();
        SceneChange(dm.clearLevelData);
    }
}
