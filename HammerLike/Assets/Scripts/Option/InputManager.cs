using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager instance;

    private Dictionary<string, KeyCode> keyMappings;
    private const string JUMP_KEY = "JumpKey";

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeKeyMappings();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeKeyMappings()
    {
        keyMappings = new Dictionary<string, KeyCode>();

        keyMappings[JUMP_KEY] = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString(JUMP_KEY, KeyCode.Space.ToString()));
    }

    public void SetKeyMapping(string action, KeyCode key)
    {
        if (keyMappings.ContainsKey(action))
        {
            keyMappings[action] = key;
            PlayerPrefs.SetString(action, key.ToString());
            PlayerPrefs.Save();
        }
    }

    public bool GetKeyDown(string action)
    {
        if (keyMappings.ContainsKey(action))
        {
            return Input.GetKeyDown(keyMappings[action]);
        }
        return false;
    }

    public string GetKeyMapping(string action)
    {
        if (InputManager.instance.keyMappings.ContainsKey(action))
        {
            return InputManager.instance.keyMappings[action].ToString();
        }
        return "";
    }
}
