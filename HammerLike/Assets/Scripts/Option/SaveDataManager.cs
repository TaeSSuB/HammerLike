using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SaveDataManager : MonoBehaviour
{
    public GameObject save1FileExistsUI; // 세이브 파일이 존재하는 경우 활성화할 UI 오브젝트
    public GameObject noSave1FileUI; // 세이브 파일이 존재하지 않는 경우 활성화할 UI 오브젝트
    public TextMeshProUGUI save1Text;


    public GameObject save2FileExistsUI; // 세이브 파일이 존재하는 경우 활성화할 UI 오브젝트
    public GameObject noSave2FileUI; // 세이브 파일이 존재하지 않는 경우 활성화할 UI 오브젝트
    public TextMeshProUGUI save2Text;


    public GameObject save3FileExistsUI; // 세이브 파일이 존재하는 경우 활성화할 UI 오브젝트
    public GameObject noSave3FileUI; // 세이브 파일이 존재하지 않는 경우 활성화할 UI 오브젝트
    public TextMeshProUGUI save3Text;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        CheckSaveFiles();
    }

    public void SavePlayerData(PlayerStat stat, int saveFileIndex)
    {
        DateTime now = DateTime.Now;
        string timestamp = now.ToString("yyyyMMddHHmm");
        string fileName = "save" + saveFileIndex + "_" + timestamp + ".es3";
        string path = "E:/HammerLike_Git/HammerLike/HammerLike/" + fileName;

        ES3.Save("playerStat", stat, path);
        Debug.Log(fileName + "에 성공적으로 저장됨");
        CheckSaveFiles();
    }
    /*public void OverwriteSaveData(PlayerStat stat, int saveFileIndex)
    {
        string directoryPath = "D:/HammerLike_Git/HammerLike/HammerLike/";
        string searchPattern = "save" + saveFileIndex + "_*.es3";
        string[] files = Directory.GetFiles(directoryPath, searchPattern);

        // 기존 파일이 있으면 삭제합니다.
        foreach (string file in files)
        {
            File.Delete(file);
        }

        // 새로운 파일 이름을 생성하고 저장합니다.
        DateTime now = DateTime.Now;
        string timestamp = now.ToString("yyyyMMddHHmm");
        string newFileName = "save" + saveFileIndex + "_" + timestamp + ".es3";
        string newPath = directoryPath + newFileName;

        ES3.Save("playerStat", stat, newPath);
        Debug.Log(newFileName + "으로 데이터가 덮어쓰기 됨");

        // 세이브 파일 상태를 다시 체크합니다.
        CheckSaveFiles();
    }*/

    public void DeleteSaveFile(int saveFileIndex)
    {
        string directoryPath = "E:/HammerLike_Git/HammerLike/HammerLike/";
        string searchPattern = "save" + saveFileIndex + "_*.es3";
        string[] files = Directory.GetFiles(directoryPath, searchPattern);

        foreach (string file in files)
        {
            File.Delete(file);
            Debug.Log(file + " 파일이 삭제됨");
        }

        // 세이브 파일 상태를 다시 체크합니다.
        CheckSaveFiles();
    }
    private void CheckSaveFiles()
    {
        string directoryPath = "E:/HammerLike_Git/HammerLike/HammerLike/";

        // 각 세이브 파일에 대해 검사하고 해당하는 입력 필드에 정보를 업데이트
        CheckSaveFile(1, directoryPath, save1FileExistsUI, noSave1FileUI, save1Text);
        CheckSaveFile(2, directoryPath, save2FileExistsUI, noSave2FileUI, save2Text);
        CheckSaveFile(3, directoryPath, save3FileExistsUI, noSave3FileUI, save3Text);
    }

    private void CheckSaveFile(int saveFileIndex, string directoryPath, GameObject fileExistsUI, GameObject noFileUI, TextMeshProUGUI textComponent)
    {
        string searchPattern = "save" + saveFileIndex + "_*.es3";
        string[] files = System.IO.Directory.GetFiles(directoryPath, searchPattern);

        if (files.Length > 0)
        {
            // 파일이 존재하면, 날짜와 시간을 추출합니다.
            string fileName = System.IO.Path.GetFileNameWithoutExtension(files[0]);
            string dateTimePart = fileName.Split('_')[1];

            // 문자열을 DateTime 객체로 파싱합니다.
            if (DateTime.TryParseExact(dateTimePart, "yyyyMMddHHmm", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out DateTime dateTime))
            {
                // 원하는 형식으로 문자열을 변환합니다.
                string formattedDate = dateTime.ToString("yyyy/MM/dd HH:mm");
                textComponent.text = formattedDate; // 텍스트 컴포넌트에 날짜와 시간을 설정합니다.
            }

            fileExistsUI.SetActive(true);
            noFileUI.SetActive(false);
        }
        else
        {
            fileExistsUI.SetActive(false);
            noFileUI.SetActive(true);
            textComponent.text = ""; // 텍스트 컴포넌트를 비웁니다.
        }
    }
}
