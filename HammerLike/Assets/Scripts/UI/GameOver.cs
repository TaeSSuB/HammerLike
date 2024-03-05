using UnityEngine;
using UnityEngine.UI; // UI 컴포넌트 사용을 위해 추가
using UnityEngine.SceneManagement; // 씬 관리를 위해 추가

public class GameOver : MonoBehaviour
{
    public GameObject gameOverPanel; // 인스펙터에서 할당할 게임 오버 패널
    public Button restartButton; // 인스펙터에서 할당할 재시작 버튼
    public Button quitButton; // 인스펙터에서 할당할 종료 버튼

    void Start()
    {
        // 버튼에 리스너 추가
        restartButton.onClick.AddListener(RestartGame);
        quitButton.onClick.AddListener(QuitGame);

        // 게임 오버 패널을 비활성화
        gameOverPanel.SetActive(false);
    }

    // 게임 오버 시 호출될 메서드
    public void OnGameOver()
    {
        // 모든 오브젝트의 움직임을 정지
        Time.timeScale = 0;

        // 게임 오버 패널 활성화
        gameOverPanel.SetActive(true);
    }

    void RestartGame()
    {
        // 게임 재시작 시 모든 오브젝트의 움직임 재개
        Time.timeScale = 1;

        // 메인 씬으로 로드
        SceneManager.LoadScene("UI");
    }

    void QuitGame()
    {
        // 게임 종료
        Application.Quit();

        // 유니티 에디터에서 작동하는 경우
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
