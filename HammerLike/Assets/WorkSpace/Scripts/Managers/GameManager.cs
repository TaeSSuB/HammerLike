using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// singleton pattern
public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    // player Status SO
    [SerializeField] private SO_PlayerStatus playerStatus;

    // system settings SO
    [SerializeField] private SO_SystemSettings systemSettings;

    public SO_PlayerStatus PlayerStatus { get => playerStatus; }
    public SO_SystemSettings SystemSettings { get => systemSettings; }

    public Vector3 CoordScale { get => systemSettings.CoordinateScale; }
    public float TimeScale { get => systemSettings.TimeScale; }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public Vector3 ApplyCoordScale(Vector3 inVector)
    {
        return new Vector3(
            inVector.x * CoordScale.x,
            inVector.y * CoordScale.y,
            inVector.z * CoordScale.z
            );
    }
}


