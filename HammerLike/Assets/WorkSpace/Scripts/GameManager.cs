using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// singleton pattern
public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    // player Status SO
    [SerializeField] private SO_PlayerStatus playerStatus;

    public SO_PlayerStatus PlayerStatus { get => playerStatus; }

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
}


