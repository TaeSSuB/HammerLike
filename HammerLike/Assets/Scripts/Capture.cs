using System;
using System.Collections;
using UnityEngine;

public class Capture : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            StartCoroutine(Rendering());
        }
    }

    IEnumerator Rendering()
    {
        yield return new WaitForEndOfFrame();

        // 현재 시간을 "년월일_시분초" 형식으로 변환하여 파일 이름에 사용
        string dateTime = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        string fileName = "ScreenShot_" + dateTime + ".png"; // 예: ScreenShot_20240303_123456.png

        // 사용자의 바탕화면에 저장하기 위한 경로 설정
        string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        string folderPath = System.IO.Path.Combine(desktopPath, "ScreenShot");

        // ScreenShot 폴더가 존재하지 않으면 생성
        if (!System.IO.Directory.Exists(folderPath))
        {
            System.IO.Directory.CreateDirectory(folderPath);
        }

        string path = System.IO.Path.Combine(folderPath, fileName);

        byte[] imgBytes;
        Texture2D texture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        texture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0, false);
        texture.Apply();
        imgBytes = texture.EncodeToPNG();
        System.IO.File.WriteAllBytes(path, imgBytes);
    }
}
