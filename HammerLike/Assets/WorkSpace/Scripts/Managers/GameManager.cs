using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using NuelLib;

// singleton pattern
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

    protected override void Awake()
    {
        base.Awake();

        devModeTextObj = Instantiate(textPrefab, textTR);
        devModeTextObj.GetComponent<TextMeshProUGUI>().text = $"{(isDevMode ? "Dev" : "Play")} Mode - " + devModeKey.ToString();
        resetTextObj = Instantiate(textPrefab, textTR);
        resetTextObj.GetComponent<TextMeshProUGUI>().text = $"{(isDevMode ? "Reset - " : "Destroy - ")}" + resetKey.ToString();
        ResetTester();
    }

    private void Update()
    {
        if(Input.GetKeyDown(resetKey))
        {
            ResetTester();
        }
        if(Input.GetKeyDown(devModeKey))
        {
            isDevMode = !isDevMode;
            devModeTextObj.GetComponent<TextMeshProUGUI>().text = $"{(isDevMode ? "Dev" : "Play")} Mode - " + devModeKey.ToString();
            resetTextObj.GetComponent<TextMeshProUGUI>().text = $"{(isDevMode ? "Reset - " : "Destroy - " )}" + resetKey.ToString();

        }
    }

    #region Coordinate Scale
    public Vector3 ApplyCoordScale(Vector3 inVector)
    {
        //return new Vector3(
        //    inVector.x * CoordScale.x,
        //    inVector.y * CoordScale.y,
        //    inVector.z * CoordScale.z
        //    );
        inVector.y = 0;

        var coordVector = inVector * CalcCoordScale(inVector);

        return coordVector;
    }
    public Vector3 ApplyCoordScaleNormalize(Vector3 inVector)
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
    #endregion
}


