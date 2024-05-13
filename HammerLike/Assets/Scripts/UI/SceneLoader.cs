using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{

    // public string sceneToLoad;


    /*public void ChangeScene()
    {
        // sceneToLoad에 설정된 씬으로 이동합니다.
        LoadingSceneController.LoadScene(sceneToLoad);
    }*/

    public void ChangeScene(string sceneName)
    {
        // sceneToLoad에 설정된 씬으로 이동합니다.
        LoadingSceneController.LoadScene(sceneName);
    }

    public void Exit()
    {
        Application.Quit();
    }
}
