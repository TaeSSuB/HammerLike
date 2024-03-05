using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    // 이 public 변수는 인스펙터에서 설정할 수 있습니다.
    // 여기에 이동하고 싶은 씬의 이름을 입력하세요.
    public string sceneToLoad;

    // 이 public 함수는 버튼 클릭과 같은 이벤트에 의해 호출될 수 있습니다.
    public void ChangeScene()
    {
        // sceneToLoad에 설정된 씬으로 이동합니다.
        LoadingSceneController.LoadScene(sceneToLoad);
    }

    public void Exit()
    {
        Application.Quit();
    }
}
