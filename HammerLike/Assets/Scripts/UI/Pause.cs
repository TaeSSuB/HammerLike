using UnityEngine;
using UnityEngine.UI;

public class Pause : MonoBehaviour
{
    [Header("UI 요소")]
    public GameObject pausePopup; // Pause 팝업 창
    public Button continueButton; // 계속 버튼

    private bool isPaused = false;

    void Start()
    {
        // 일시 중지 팝업 창을 비활성화합니다.
        if(pausePopup.activeSelf)
        pausePopup.SetActive(false);

        // 계속 버튼에 대한 리스너를 추가합니다.
        continueButton.onClick.AddListener(ResumeGame);
    }

    void Update()
    {
        // ESC 키를 누르면 일시 중지 또는 재개합니다.
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    void PauseGame()
    {
        isPaused = true;
        pausePopup.SetActive(true);
        Time.timeScale = 0f; // 게임을 일시 중지합니다.
    }

    void ResumeGame()
    {
        isPaused = false;
        pausePopup.SetActive(false);
        Time.timeScale = 1f; // 게임을 재개합니다.
    }
}
