using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // TextMeshPro 네임스페이스
using System.IO; // 파일 입출력을 위한 네임스페이스

public class SaveData : MonoBehaviour
{
    public TMP_InputField inputField; // 사용자 입력을 받을 TMP_InputField
    public TextMeshProUGUI buttonText; // 버튼의 TextMeshPro 텍스트

    private string csvFilePath = "Assets/SavedData.csv"; // CSV 파일 경로

    public void SaveGameSettings()
    {
        // 사용자가 입력한 텍스트로 버튼의 텍스트를 변경합니다.
        string userInput = inputField.text;
        buttonText.text = string.IsNullOrEmpty(userInput) ? "세이브 파일1" : userInput;

        // 저장된 키 이름을 PlayerPrefs에서 가져옵니다.
        string horizontalAltNegativeButton = PlayerPrefs.GetString("HorizontalAltNegativeButton", "default_value");
        string horizontalAltPositiveButton = PlayerPrefs.GetString("HorizontalAltPositiveButton", "default_value");
        string verticalAltNegativeButton = PlayerPrefs.GetString("VerticalAltNegativeButton", "default_value");
        string verticalAltPositiveButton = PlayerPrefs.GetString("VerticalAltPositiveButton", "default_value");

        // CSV 파일로 저장합니다.
        SaveToCSV(userInput, horizontalAltNegativeButton, horizontalAltPositiveButton, verticalAltNegativeButton, verticalAltPositiveButton);
    }


    private void SaveToCSV(string buttonName, string horizontalNeg, string horizontalPos, string verticalNeg, string verticalPos)
    {
        string csvContent = $"{buttonName},{horizontalNeg},{horizontalPos},{verticalNeg},{verticalPos}\n";

        // 파일이 존재하지 않으면 새로 생성, 존재하면 기존 내용에 추가
        if (!File.Exists(csvFilePath))
        {
            File.WriteAllText(csvFilePath, "Button Name,Horizontal Alt Negative Button,Horizontal Alt Positive Button,Vertical Alt Negative Button,Vertical Alt Positive Button\n");
        }

        File.AppendAllText(csvFilePath, csvContent);
    }
}
