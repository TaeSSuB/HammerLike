using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class GraphicsOption : MonoBehaviour
{

    ///
    /// 관련 문서
    /// https://docs.unity3d.com/kr/2018.4/Manual/class-PlayerSettingsStandalone.html 
    ///
    
    // 해상도
    public TMP_Text resolutionLabel; // 해상도를 표시할 TextMeshProUGUI 레퍼런스
    private int currentResolutionIndex = 0; // 현재 해상도 인덱스
    private List<Vector2Int> resolutions = new List<Vector2Int>
    {
        new Vector2Int(1280, 720),
        new Vector2Int(1600, 900),
        new Vector2Int(1920, 1080)
    };

    // 창모드
    public TMP_Text modeLabel; // 모드를 표시할 TextMeshProUGUI 레퍼런스
    private int currentModeIndex = 0; // 현재 모드 인덱스
    private FullScreenMode[] modes = { FullScreenMode.ExclusiveFullScreen, FullScreenMode.Windowed};

    // 디스플레이
    public TMP_Text displayLabel; // 디스플레이를 표시할 TextMeshProUGUI 레퍼런스
    private int currentDisplayIndex = 0; // 현재 디스플레이 인덱스
    void Start()
    {
        LoadSettings();
        UpdateResolutionLabel();
        UpdateModeLabel();
        UpdateDisplayLabel();
    }


    void Update()
    {
        
    }

    public void ChangeResolution(bool increase)
    {
        if (increase)
        {
            currentResolutionIndex++;
            if (currentResolutionIndex >= resolutions.Count)
            {
                currentResolutionIndex = 0; // 목록의 끝에 도달하면 처음으로 돌아갑니다.
            }
        }
        else
        {
            currentResolutionIndex--;
            if (currentResolutionIndex < 0)
            {
                currentResolutionIndex = resolutions.Count - 1; // 목록의 시작에 도달하면 마지막으로 돌아갑니다.
            }
        }

        UpdateResolutionLabel(); // 변경 후 해상도 라벨 업데이트
    }

    private void UpdateResolutionLabel()
    {
        Vector2Int currentResolution = resolutions[currentResolutionIndex];
        resolutionLabel.text = $"{currentResolution.x} x {currentResolution.y}";
    }

    public void ChangeMode(bool increase)
    {
        currentModeIndex += increase ? 1 : -1;

        if (currentModeIndex >= modes.Length) // 순환 로직
        {
            currentModeIndex = 0;
        }
        else if (currentModeIndex < 0) // 역순 순환 로직
        {
            currentModeIndex = modes.Length - 1;
        }

        // 모드 설정 적용
        //ApplyModeSetting(modes[currentModeIndex]);

        // 모드 라벨 업데이트
        UpdateModeLabel();
    }



    private void UpdateModeLabel()
    {
        switch (modes[currentModeIndex])
        {
            case FullScreenMode.ExclusiveFullScreen:
                modeLabel.text = "전체 화면";
                break;
            case FullScreenMode.Windowed:
                modeLabel.text = "창 모드";
                break;
            /*case FullScreenMode.FullScreenWindow:
                modeLabel.text = "테두리 없음";
                break;*/
        }
    }

    public void ChangeDisplay(bool increase)
    {
        if (increase)
        {
            currentDisplayIndex++;
            if (currentDisplayIndex >= Display.displays.Length)
            {
                currentDisplayIndex = 0; // 순환
            }
        }
        else
        {
            currentDisplayIndex--;
            if (currentDisplayIndex < 0)
            {
                currentDisplayIndex = Display.displays.Length - 1; // 역순 순환
            }
        }

        UpdateDisplayLabel();
    }

    private void UpdateDisplayLabel()
    {
        displayLabel.text = $"디스플레이 {currentDisplayIndex + 1}";
    }

    public void ApplySettings()
    {
        // 현재 선택된 해상도로 설정 적용
        Vector2Int resolution = resolutions[currentResolutionIndex];
        SetResolution(resolution.x, resolution.y);

        // 현재 선택된 모드로 설정 적용
        SetWindowMode(modes[currentModeIndex]);

        // 현재 선택된 디스플레이로 설정 적용
        ActivateDisplay(currentDisplayIndex);

        // 설정 저장
        PlayerPrefs.SetInt("ResolutionIndex", currentResolutionIndex);
        PlayerPrefs.SetInt("ModeIndex", currentModeIndex);
        PlayerPrefs.SetInt("DisplayIndex", currentDisplayIndex);
        PlayerPrefs.Save(); // 변경사항 저장
    }

    void LoadSettings()
    {
        // 설정 로드
        if (PlayerPrefs.HasKey("ResolutionIndex"))
        {
            currentResolutionIndex = PlayerPrefs.GetInt("ResolutionIndex");
        }
        if (PlayerPrefs.HasKey("ModeIndex"))
        {
            currentModeIndex = PlayerPrefs.GetInt("ModeIndex");
        }
        if (PlayerPrefs.HasKey("DisplayIndex"))
        {
            currentDisplayIndex = PlayerPrefs.GetInt("DisplayIndex");
            // 저장된 디스플레이 인덱스를 기반으로 디스플레이를 활성화
            ActivateDisplay(currentDisplayIndex);
        }

        // 로드된 설정 적용
        ApplySettings();
    }


    public void SetResolution(int width, int height)
    {
        bool isFullScreen = Screen.fullScreen; // 현재 전체 화면 모드 상태 유지
        Screen.SetResolution(width, height, isFullScreen);
    }


    public void SetFullScreen(bool isFullScreen)
    {
        Screen.fullScreen = isFullScreen;
    }

    public void SetWindowMode(FullScreenMode mode)
    {
        // 전체 화면 모드와 테두리 없는 전체 화면 모드 설정
        if (mode == FullScreenMode.ExclusiveFullScreen || mode == FullScreenMode.FullScreenWindow)
        {
            Screen.fullScreenMode = mode;
            Screen.fullScreen = true; // 전체 화면 모드 활성화
        }
        // 창 모드(테두리 있음) 설정
        else if (mode == FullScreenMode.Windowed)
        {
            Screen.fullScreen = false; // 창 모드로 전환
        }
    }


    public void ActivateDisplay(int displayIndex)
    {
        if (displayIndex >= 0 && displayIndex < Display.displays.Length)
        {
            Display.displays[displayIndex].Activate();
        }
    }
}
