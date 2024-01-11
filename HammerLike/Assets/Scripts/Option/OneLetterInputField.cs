using UnityEngine;
using TMPro;

public class OneLetterInputField : MonoBehaviour
{
    public TMP_InputField inputField; // TMP_InputField 컴포넌트를 할당

    void Start()
    {
        if (inputField != null)
        {
            // onValueChanged 이벤트에 메소드 할당
            inputField.onValueChanged.AddListener(HandleInputChanged);
        }
    }

    private void HandleInputChanged(string input)
    {
        if (input.Length > 1)
        {
            // 텍스트가 한 글자를 초과하면 마지막 글자만 남김
            inputField.text = input[input.Length - 1].ToString();
        }
    }
}
