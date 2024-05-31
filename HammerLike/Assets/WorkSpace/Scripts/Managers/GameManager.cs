using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using NuelLib;

/// <summary>
/// GameManager : 게임 매니저 클래스 (Singleton MonoBehavior)
/// - 플레이어 정보, 시스템 설정 정보 관리
/// - 개발 모드 설정 및 임시 테스트 그룹 생성 스크립트 포함
/// - 좌표 스케일링 및 시간 스케일링 기능 포함
/// </summary>
public class GameManager : SingletonMonoBehaviour<GameManager>
{
    [Header("Player")]
    [SerializeField] private B_Player player;

    // player Status SO
    [SerializeField] private SO_PlayerStatus playerStatus;

    // system settings SO
    [Header("System Settings")]
    [SerializeField] private SO_SystemSettings systemSettings;

    [Header("Dev")]
    [SerializeField] private bool isDevMode;
    [SerializeField] private bool isLimitFrame;
    [SerializeField] private GameObject devCanvas;

    [SerializeField] private GameObject tempCombatTestGroupPrefab;
    private GameObject currentTempCombatTestGroup;

    [SerializeField] private GameObject textPrefab;
    [SerializeField] private Transform textTR;
    public KeyCode resetKey = KeyCode.F5;
    public KeyCode devModeKey = KeyCode.F2;
    private GameObject devModeTextObj;
    private GameObject resetTextObj;

    public B_Player Player { get => player; }
    public void SetPlayer(B_Player inPlayer)
    {
        player = inPlayer;
    }
    public SO_PlayerStatus PlayerStatus { get => playerStatus; }
    public SO_SystemSettings SystemSettings { get => systemSettings; }

    public Vector3 CoordScale { get => systemSettings.CoordinateScale; }
    public float TimeScale { get => systemSettings.TimeScale; }
    

    #region Unity Callbacks
    protected override void Awake()
    {
        //base.Awake();

        devModeTextObj = Instantiate(textPrefab, textTR);
        devModeTextObj.GetComponent<TextMeshProUGUI>().text = $"{(isDevMode ? "Dev" : "Play")} Mode - " + devModeKey.ToString();
        resetTextObj = Instantiate(textPrefab, textTR);
        resetTextObj.GetComponent<TextMeshProUGUI>().text = $"{(isDevMode ? "Reset - " : "Destroy - ")}" + resetKey.ToString();
        ResetTester();

        #if UNITY_EDITOR || DEVELOPMENT_BUILD
        SetDevMode(true);
        #else
        SetDevMode(false);
        #endif

        //StartCoroutine(GetFPSRoutine());
    }

    private void Update()
    {
        if(Input.GetKeyDown(resetKey))
        {
            ResetTester();
        }
        if(Input.GetKeyDown(devModeKey))
        {
            #if UNITY_EDITOR || DEVELOPMENT_BUILD
            SetDevMode(!isDevMode);
            #endif
        }

        if(isDevMode && Input.GetKeyDown(KeyCode.F1))
        {
            var targetFrameArray = new int[] { 30, 60, 120};

            var currentFrame = (int)Application.targetFrameRate;

            switch(currentFrame)
            {
                case 30:
                    SetFrameRate(targetFrameArray[1]);
                    break;
                case 60:
                    SetFrameRate(targetFrameArray[2]);
                    break;
                case 120:
                    SetFrameRate(targetFrameArray[0]);
                    break;
                // case 240:
                //     SetFrameRate(targetFrameArray[0]);
                //     break;
                default:
                    SetFrameRate(targetFrameArray[1]);
                    break;
            }
        }
    }
    #endregion

    #region Set System Settings

    public float SetFrameRate(int inFrameRate)
    {
        Debug.Log($"Set Frame Rate to {inFrameRate}");
        
        QualitySettings.vSyncCount = 0;
        
        inFrameRate = Mathf.Clamp(inFrameRate, 1, 120);
        Application.targetFrameRate = inFrameRate;
        isLimitFrame = !isLimitFrame;
        
        return Application.targetFrameRate;
    }

    /// <summary>
    /// SetTimeScale : TimeScale 설정
    /// </summary>
    /// <param name="inTimeScale"></param>
    /// <returns></returns>
    public float SetTimeScale(float inTimeScale = 1f)
    {
        // 0 ~ 2
        inTimeScale = Mathf.Clamp(inTimeScale, 0, 2);
        
        if(SystemSettings != null)
            SystemSettings.SetTimeScale = inTimeScale;
        
        Time.timeScale = inTimeScale;
        
        return SystemSettings.TimeScale;
    }

    public void SlowMotion(float inTimeScale, float inDuration)
    {
        StartCoroutine(CoSlowMotion(inTimeScale, inDuration));
    }

    IEnumerator CoSlowMotion(float inTimeScale, float inDuration)
    {
        float duration = inDuration;
        float originTimeScale = 1f;

        SetTimeScale(inTimeScale);

        // while (duration > 0f)
        // {
        //     duration -= Time.unscaledDeltaTime;

        //     yield return null;
        // }
        yield return new WaitForSecondsRealtime(inDuration);

        SetTimeScale(originTimeScale);
    }

    #endregion

    #region Coordinate Scale
    
    /// <summary>
    /// ApplyCoordScale : 벡터에 대한 xz 좌표 상의 스케일링 적용
    /// </summary>
    /// <param name="inVector"></param>
    /// <returns></returns>
    public Vector3 ApplyCoordScale(Vector3 inVector)
    {
        inVector.y = 0;

        var coordVector = inVector * CalcCoordScale(inVector);

        return coordVector;
    }

    public Vector3 ApplyCoordDivide(Vector3 inVector)
    {
        inVector.y = 0;

        var coordVector = inVector / CalcCoordScale(inVector);

        return coordVector;
    }

    /// <summary>
    /// ApplyCoordScaleAfterNormalize : 정규화된 벡터에 대한 xz 좌표 상의 스케일링 
    /// </summary>
    /// <param name="inVector"></param>
    /// <returns></returns>
    public Vector3 ApplyCoordScaleAfterNormalize(Vector3 inVector)
    {
        inVector.y = 0;
        inVector.Normalize();

        //var coordVector = inVector * CalcCoordScale(inVector);

        var coordVector = new Vector3(
            inVector.x * CoordScale.x,
            inVector.y * CoordScale.y,
            inVector.z * CoordScale.z
            );

        return coordVector;
    }

    /// <summary>
    /// ApplyCoordDivideAfterNormalize : 정규화된 벡터에 대한 xz 좌표 상의 스케일링 해제
    /// - 주로 이동 벡터에 대한 스케일링 해제에 활용
    /// - ex. 스케일링으로 인한 방향 벡터 오차 해결
    /// </summary>
    /// <param name="inVector"></param>
    /// <returns></returns>
    public Vector3 ApplyCoordDivideAfterNormalize(Vector3 inVector)
    {
        inVector.y = 0;
        inVector.Normalize();

        //var coordVector = inVector * CalcCoordScale(inVector);

        var coordVector = new Vector3(
            inVector.x / CoordScale.x,
            inVector.y / CoordScale.y,
            inVector.z / CoordScale.z
            );

        return coordVector;
    }

    /// <summary>
    /// CalcCoordScale : xz 좌표 상의 좌표계 상수 계산
    /// - inVector의 xz 좌표 정규화 및 x,z 축에 대한 지향도 계산
    /// - 계산된 좌표계 상수 출력
    /// - 스케일링에 활용
    /// </summary>
    /// <param name="inVector"></param>
    /// <returns></returns>
    public float CalcCoordScale(Vector3 inVector)
    {
        inVector.y = 0;
        inVector.Normalize();

        var coordVecMag = new Vector3(
            inVector.x * CoordScale.x,
            inVector.y * CoordScale.y,
            inVector.z * CoordScale.z
            ).magnitude;

        return coordVecMag;
    }

    #endregion

    #region Dev Mode

    public void SetDevMode(bool inIsDevMode)
    {
        isDevMode = inIsDevMode;

        if(devCanvas != null)
            devCanvas?.SetActive(isDevMode);

        devModeTextObj.GetComponent<TextMeshProUGUI>().text = $"{(isDevMode ? "Dev" : "Play")} Mode - " + devModeKey.ToString();
        resetTextObj.GetComponent<TextMeshProUGUI>().text = $"{(isDevMode ? "Reset - " : "Destroy - " )}" + resetKey.ToString();

    }

    public void ResetTester()
    {
        if (currentTempCombatTestGroup != null)
            Destroy(currentTempCombatTestGroup);

        //isDevMode = inIsTester;
        if (isDevMode)
        {
            currentTempCombatTestGroup = Instantiate(tempCombatTestGroupPrefab);
        }
    }

    // public void GetFPS()
    // {
    //     StartCoroutine(GetFPSRoutine());
    // }

    // private IEnumerator GetFPSRoutine()
    // {
    //     while (true&&fpsText)
    //     {
    //         yield return new WaitForSeconds(fpsInterval);
    //         fpsText.text = $"FPS : {1f / Time.deltaTime}";
    //     }
    // }

    #endregion

}