using UnityEngine;
using TMPro; // TextMeshPro를 사용하기 위해 필요

public class CheatConsole : MonoBehaviour
{
    public TMP_InputField inputField; // Inspector에서 할당
    public TMP_Text commandHistoryText; // 이전 명령어들을 표시할 TextMeshPro Text 컴포넌트, Inspector에서 할당

    private bool isInputFieldShown = false; // 입력 필드의 표시 상태를 추적
    private string commandHistory = ""; // 입력된 명령어들의 기록

    public SceneLoader sceneLoader;
    public bool isHpMax = false;

    private void Start()
    {
        // 이 GameObject를 씬 전환 시에도 파괴되지 않도록 설정
        DontDestroyOnLoad(gameObject);
    }


    void Update()
    {
        // F3 키로 입력 필드 토글
        if (Input.GetKeyDown(KeyCode.F3))
        {
            isInputFieldShown = !isInputFieldShown;
            inputField.gameObject.SetActive(isInputFieldShown);
            commandHistoryText.gameObject.SetActive(isInputFieldShown); // 명령어 히스토리 텍스트도 표시 상태를 토글

            if (isInputFieldShown)
            {
                inputField.ActivateInputField(); // 입력 필드 활성화 및 포커스
            }
        }

        // 입력 필드가 활성화되어 있고, 엔터키가 눌린 경우
        if (isInputFieldShown && (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)))
        {
            string command = inputField.text;
            ProcessCommand(command); // 입력된 텍스트 처리
            UpdateCommandHistory(command); // 명령어 히스토리 업데이트
            inputField.text = ""; // 입력 필드 초기화
            inputField.ActivateInputField(); // 입력 필드 재활성화
        }
    }

    void ProcessCommand(string command)
    {
        // 커맨드 처리 로직
        Debug.Log($"Command: {command}");
        // 여기에 커맨드에 따른 로직 추가

        if(command =="/admin kmj")
        {
            sceneLoader.ChangeScene("Test");
        }

        if(command =="/cheat HpMax")
        {
            isHpMax = true;
        }
    }

    void UpdateCommandHistory(string command)
    {
        // 명령어를 기록에 추가하고, 텍스트 컴포넌트에 표시
        commandHistory += command + "\n"; // 새 줄에 명령어 추가
        commandHistoryText.text = commandHistory; // 텍스트 컴포넌트 업데이트
    }
}
