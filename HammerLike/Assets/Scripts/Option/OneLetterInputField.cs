using UnityEngine;
using TMPro;

public class OneLetterInputField : MonoBehaviour
{
    public TMP_InputField inputField;

    void Start()
    {
        if (inputField != null)
        {
            inputField.onValueChanged.AddListener(HandleInputChanged);
        }
    }

    private void HandleInputChanged(string input)
    {
        // 일반 문자 입력에 대한 처리: 마지막 글자만 남김
        if (input.Length > 1 && !IsSpecialKeyInput(input))
        {
            inputField.text = input[input.Length - 1].ToString();
        }
    }

    void Update()
    {
        CheckForSpecialKeyInput();
    }

    private void CheckForSpecialKeyInput()
    {
        if (Input.GetKeyDown(KeyCode.CapsLock)) inputField.text = "CapsLock";
        else if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift)) inputField.text = "Shift";
        else if (Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.RightControl)) inputField.text = "Ctrl";
        else if (Input.GetKeyDown(KeyCode.Tab)) inputField.text = "Tab";
        else if (Input.GetKeyDown(KeyCode.LeftArrow)) inputField.text = "Left";
        else if (Input.GetKeyDown(KeyCode.RightArrow)) inputField.text = "Right";
        else if (Input.GetKeyDown(KeyCode.UpArrow)) inputField.text = "Up";
        else if (Input.GetKeyDown(KeyCode.DownArrow)) inputField.text = "Down";
    }

    private bool IsSpecialKeyInput(string input)
    {
        // 특수 키 이름 목록
        string[] specialKeys = { "CapsLock", "Shift", "Ctrl", "Tab", "Left", "Right", "Up", "Down" };
        foreach (string key in specialKeys)
        {
            if (input == key) return true;
        }
        return false;
    }
}
